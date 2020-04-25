using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Inv : DiceCmdEntry {

        public string Usage => "人物卡 新建 <姓名> [描述]; {属性名:属性值}";


        public static string CreateInv(DMEnv env, string name, string desc, Args dict) {
            Scenario sce = env.Sce;
            Investigator inv = new Investigator(name, desc);

            inv.Values.FillWith(Global.DefaultValues);

            foreach (string key in dict.Keys) {
                if (dict.TryGet(key, out Value value)) {
                    inv.Values.Put(key, value);
                }
            }
            inv.Calc(out string err);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("名字：{0}", name).AppendLine();
            if (!string.IsNullOrEmpty(desc)) {
                builder.AppendFormat("描述：{0}", desc).AppendLine();
            }
            builder
                .Append("体格：").Append(inv.Build)
                .Append("伤害加值：").AppendLine(inv.DamageBonus);

            foreach (string key in inv.Values.Names) {
                if (inv.Values.TryWidelyGet(key, out Value value)) {
                    builder.AppendFormat("{0}:{1} ", key, value);
                }
            }
            sce.PutInvestigator(inv);
            sce.Control(env.SelfId, inv.Name);
            env.Save();
            return builder.ToString();
        }


        public static string DestoryInv(DMEnv env, string name) {
            Scenario sce = env.Sce;

            if (!sce.AdminList.Contains(env.SelfId)) {
                return "你不是管理员！";
            } else if (!sce.ExistInvestigator(name)) {
                return "不存在调查员：" + name;
            }
            sce.PlayerNames.Remove(env.SelfId);
            sce.Investigators.Remove(name);
            
            env.Save();
            return "成功销毁：" + name + "，TA永远地消失了……";
        }

        public static string ReCalc(DMEnv env, Investigator inv) {
            inv.Calc(out string err);
            env.Save();

            return new StringBuilder()
                .Append(inv.Name).AppendLine("的数据：")
                .Append("体格：").Append(inv.Build).Append('，')
                .Append("伤害加值：").Append(inv.DamageBonus)
                .ToString();
        }

        public static string SetDB(DMEnv env, Investigator inv, Dice db) {
            inv.DamageBonus = db.ToString();
            env.Save();

            return new StringBuilder()
                .Append(inv.Name).AppendLine("的数据：")
                .Append("体格：").Append(inv.Build).Append('，')
                .Append("伤害加值：").Append(inv.DamageBonus)
                .ToString();
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper mapper = new DictMapper();
            mapper.Rest(new ValueParser());

            dispatcher.Register("人物卡")
            .Handles(Extensions.ExistSce())
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
                .Handles(Extensions.ExistSelfInv())
                .Executes((env, args, dict) => ReCalc(env, env.Inv))
                .Then(
                    PresetNodes.String<DMEnv>("名称")
                    .Handles(Extensions.ExistInv())
                    .Executes((env, args, dict) => ReCalc(env, args.GetInv("名称")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("伤害加值")
                .Handles(Extensions.ExistSelfInv())
                .Then(
                    Extensions.Dice("数值")
                    .Executes((env, args, dict) => SetDB(env, env.Inv, args.GetDice("数值")))
                )
            );

            dispatcher.SetAlias("车卡", "人物卡 新建");
            dispatcher.SetAlias("撕卡", "人物卡 销毁");
            dispatcher.SetAlias("重算", "人物卡 重算");
        }
    }
}
