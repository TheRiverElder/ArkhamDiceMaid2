using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Inv : DiceCmdEntry {

        public string Usage => "人物卡 新建 <姓名> [描述]; {属性名:属性值}";


        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper<DMEnv> mapper = new DictMapper<DMEnv>();
            mapper.Rest(new ValueParser());

            dispatcher.Register("人物卡")
            .Then(
                PresetNodes.Literal<DMEnv>("新建").Then(
                    PresetNodes.String<DMEnv>("名称")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => CreateInv(env, args.GetStr("名称"), "", dict))
                    .Then(
                        PresetNodes.String<DMEnv>("描述")
                        .MapDict(mapper)
                        .Executes((env, args, dict) => CreateInv(env, args.GetStr("名称"), args.GetStr("描述"), dict))
                    )
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("重命名").Then(
                    PresetNodes.String<DMEnv>("新名称")
                    .Executes((env, args, dict) => Rename(env, args.GetStr("新名称")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("销毁").Then(
                    PresetNodes.String<DMEnv>("名称")
                    .Executes((env, args, dict) => "销毁人物卡需要操作者是管理员，并且需要加上“强制”参数！")
                    .Then(
                        PresetNodes.Literal<DMEnv>("强制")
                        .Executes((env, args, dict) => DestoryInv(env, args.GetStr("名称")))
                    )
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("重算")
                .Executes((env, args, dict) => ReCalc(env, env.Inv))
                .Then(
                    PresetNodes.String<DMEnv>("名称")
                    .Handles(Extensions.ExistInv)
                    .Executes((env, args, dict) => ReCalc(env, args.GetInv("名称")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("检查")
                .Executes((env, args, dict) => Check(env, env.Inv))
            ).Then(
                PresetNodes.Literal<DMEnv>("伤害加值")
                .Then(
                    Extensions.Dice("数值")
                    .Executes((env, args, dict) => SetDB(env, args.GetDice("数值")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("标记")
                .Then(
                    PresetNodes.Literal<DMEnv>("添加")
                    .Rest(
                        PresetNodes.String<DMEnv>("标签")
                        .Handles(PreProcesses.ConvertObjectArrayToStringArray)
                        .Executes((env, args, dict) => AddTags(env, env.Inv, args.Get<string[]>("标签")))
                    )
                ).Then(
                    PresetNodes.Literal<DMEnv>("移除")
                    .Rest(
                        PresetNodes.String<DMEnv>("标签")
                        .Handles(PreProcesses.ConvertObjectArrayToStringArray)
                        .Executes((env, args, dict) => RemoveTags(env, env.Inv, args.Get<string[]>("标签")))
                    )
                )
            );

            dispatcher.SetAlias("车卡", "人物卡 新建");
            dispatcher.SetAlias("撕卡", "人物卡 销毁");
            dispatcher.SetAlias("重算", "人物卡 重算");
            dispatcher.SetAlias("标记", "人物卡 标记 添加");
            dispatcher.SetAlias("反标", "人物卡 标记 移除");
        }

        public static HashSet<char> HardInputChars = new HashSet<char>("·、…—");
        public static int HardInputCharCountLimit = 4;
        public static bool CheckNameInputHardness(string name, out string hint) {
            HashSet<char> appearedHardChars = new HashSet<char>();
            foreach (char ch in name) {
                if (HardInputChars.Contains(ch)) {
                    appearedHardChars.Add(ch);
                }
            }
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            if (name.Length > HardInputCharCountLimit) {
                builder.Append($"名字过长，不宜超过{HardInputCharCountLimit}个字");
                flag = true;
            }
            if (appearedHardChars.Count > 0) {
                if (flag) {
                    builder.Append("且");
                }
                builder.Append("包含难打的字符：").Append(string.Join("", appearedHardChars));
                flag = true;
            }
            hint = "❗" + builder.ToString();
            return flag;
        }

        public static bool Rename(DMEnv env, string name) {
            Investigator inv = env.Inv;
            string oldName = inv.Name;
            if (oldName == name) {
                env.Next = "新旧名字相同，无需更改";
                return false;
            }
            inv.Name = name;
            
            env.Sce.Investigators.Remove(oldName);
            env.Sce.PutInvestigator(inv);
            env.Sce.Control(env.SelfId, inv.Name);
            env.Save();
            env.Next = $"{oldName}被重命名为：{name}" + (CheckNameInputHardness(name, out string hint) ? "\n" + hint : "");
            return true;
        }

        public static bool CreateInv(DMEnv env, string name, string desc, Args dict) {
            Scenario sce = env.Sce;
            Investigator inv = new Investigator(name, desc);

            inv.Values.FillWith(Global.DefaultValues);

            foreach (string key in dict.Keys) {
                if (dict.TryGet(key, out Value value)) {
                    inv.Values.Put(key, value);
                }
            }
            inv.Calc(out string err);

            env.Append("名字：", name).Line();
            if (!string.IsNullOrEmpty(desc)) {
                env.Append("描述：", desc).Line();
            }
            if (CheckNameInputHardness(name, out string hint)) {
                env.AppendLine(hint).Line();
            }
            env
                .Append("体格：").Append(inv.Build)
                .Append("伤害加值：").AppendLine(inv.DamageBonus);

            foreach (string key in inv.Values.Names) {
                if (inv.Values.TryWidelyGet(key, out Value value)) {
                    env.Append($"{key}:{value} ");
                }
            }
            sce.PutInvestigator(inv);
            sce.Control(env.SelfId, inv.Name);
            env.Save();
            return true;
        }

        public static bool DestoryInv(DMEnv env, string name) {
            Scenario sce = env.Sce;

            if (env.IsAdmin) {
                env.Next = "你不是管理员！";
                return false;
            } else if (!sce.ExistInvestigator(name)) {
                env.Next = "不存在调查员：" + name;
                return false;
            }
            sce.PlayerNames.Remove(env.SelfId);
            sce.Investigators.Remove(name);
            
            env.Save();
            env.Next = "成功销毁：" + name + "，TA永远地消失了……";
            return true;
        }

        public static void ReCalc(DMEnv env, Investigator inv) {
            inv.Calc(out string err);
            env.Save();

            env
                .Append(inv.Name).AppendLine("的数据：")
                .Append("体格：").Append(inv.Build).Append('，')
                .Append("伤害加值：").Append(inv.DamageBonus);
        }

        private static string[] BasicNine = new string[] {
            "力量", "体型", "体质", "智力", "教育", "敏捷", "意志", "外貌", "幸运",
        };

        public static bool Check(DMEnv env, Investigator inv) {
            HashSet<string> missingValueNames = new HashSet<string>();
            ValueSet values = inv.Values;
            foreach (string name in BasicNine) {
                if (!values.Has(name)) {
                    missingValueNames.Add(name);
                }
            }
            int maxHealth, maxMagic, calcDodge;
            List<string> warnings = new List<string>();
            // 通过体质与体型检查体力数值
            if (!values.TryGet("体力", out Value health)) {
                missingValueNames.Add("体力");
            } else if (health.Max <= 0) {
                warnings.Add("【体力】未设上限");
            } else if (
                !values.TryGet("体型", out Value siz) &&
                !values.TryGet("体质", out Value con) &&
                health.Max != (maxHealth = (int)((siz.Val + con.Val) / 10.0))) {
                warnings.Add($"【体力】上限不等于[体型]与[体质]之和的十分之一({maxHealth})");
            }
            // 通过意志检查魔法数值
            if (!values.TryGet("魔法", out Value magic)) {
                missingValueNames.Add("魔法");
            } else if (magic.Max <= 0) {
                warnings.Add("【魔法】未设上限");
            } else if (
                !values.TryGet("意志", out Value pow) &&
                magic.Max != (maxMagic = (int)(pow.Val / 5.0))) {
                warnings.Add($"【魔法】上限不等于[意志]的五分之一({maxMagic})");
            }
            // 通过意志检查理智数值
            if (!values.TryGet("理智", out Value sannity)) {
                missingValueNames.Add("理智");
            } else if (
                !values.TryGet("意志", out Value pow) &&
                magic.Max != pow.Val) {
                warnings.Add($"【理智】不等于[意志]的值({pow.Val})");
            }
            // 通过敏捷检查闪避数值
            if (!values.TryGet("闪避", out Value dodge)) {
                missingValueNames.Add("闪避");
            } else if (
                !values.TryGet("敏捷", out Value dex) &&
                dodge.Max != (calcDodge = (int)(dex.Val / 2.0))) {
                warnings.Add($"【闪避】不等于[敏捷]的一半({calcDodge})");
            }

            if (missingValueNames.Count == 0 && warnings.Count == 0) {
                env.Next = "未发现数据错误（不包括体格、伤害加值）";
                return true;
            }
            
            if (missingValueNames.Count > 0) {
                env.Append("缺失数据：" + string.Join("、", missingValueNames));
                if (warnings.Count > 0) {
                    env.Line();
                }
            }
            if (warnings.Count > 0) {
                env.Append(string.Join("\n", warnings));
            }

            return false;
        }

        public static void AddTags(DMEnv env, Investigator inv, string[] tags) {
            foreach (string tag in tags) {
                inv.Tags.Add(tag.ToUpper());
            }
            env.Save();

            env.Next = inv.Name + "添加了标签：" + string.Join("、", tags);
        }

        public static void RemoveTags(DMEnv env, Investigator inv, string[] tags) {
            foreach (string tag in tags) {
                inv.Tags.Remove(tag.ToUpper());
            }
            env.Save();

            env.Next = inv.Name + "移除了标签：" + string.Join("、", tags);
        }

        public static void SetDB(DMEnv env, Dice db) {
            env.Inv.DamageBonus = db.ToString();
            env.Save();

            env.Append(env.Inv.Name).AppendLine("的数据：")
                .Append("体格：").Append(env.Inv.Build).Append('，')
                .Append("伤害加值：").Append(env.Inv.DamageBonus);
        }
    }
}
