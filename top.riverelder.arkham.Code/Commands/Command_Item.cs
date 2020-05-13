using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using static top.riverelder.arkham.Code.Utils.Extensions;
using static top.riverelder.RiverCommand.PresetNodes;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Item : DiceCmdEntry {

        public string Usage => "物品 <创造|丢弃|拾取|销毁|编辑|装弹> <物品名> [重命名|弹药数]";

        static string FillWeaponInfo(Item item, Args dict) {
            StringBuilder sb = new StringBuilder();
            if (dict.TryGet("技能名", out string sn)) sb.AppendLine().Append("技能名：").Append(item.SkillName = sn);
            if (dict.TryGet("类型", out string t)) sb.AppendLine().Append("类型：").Append(item.Type = t);
            if (dict.TryGet("伤害", out Dice d)) sb.AppendLine().Append("伤害：").Append(item.Damage = d.ToString());
            if (dict.TryGet("贯穿", out bool i)) sb.AppendLine().Append("贯穿：").Append((item.Impale = i) ? "是" : "否");
            if (dict.TryGet("连发数", out int mc)) sb.AppendLine().Append("连发数：").Append(item.MaxCount = mc);
            if (dict.TryGet("弹匣", out int c)) sb.AppendLine().Append("弹匣：").Append(item.Capacity = c);
            if (dict.TryGet("故障值", out int m)) sb.AppendLine().Append("故障值：").Append(item.Mulfunction = m);
            if (dict.TryGet("弹药", out int cl)) sb.AppendLine().Append("弹药：").Append(item.CurrentLoad = cl);
            if (dict.TryGet("消耗", out int co)) sb.AppendLine().Append("消耗：").Append(item.Cost = co);
            return sb.ToString();
        }

        string CreateItem(DMEnv env, string name, Args dict) {
            Investigator inv = env.Inv;
            if (inv.Inventory.ContainsKey(name)) {
                return $"{inv.Name}的物品栏中已经存在{name}，请重命名";
            }
            Item item = new Item(name);
            string ret = $"{inv.Name}创造了物品：{name}" + FillWeaponInfo(item, dict);

            inv.Inventory[name] = item;
            env.Save();
            return ret;
        }

        string DestoryItem(DMEnv env, string name) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.ContainsKey(name)) {
                return $"未找到{inv.Name}的{name}";
            }
            inv.Inventory.Remove(name);
            env.Save();
            return $"{inv.Name}销毁了了物品：{name}";
        }

        string ThrowItem(DMEnv env, string name, string newName) {
            if (newName == null) {
                newName = name;
            }
            env.TryGetInv(out Scenario sce, out Investigator inv);
            if (sce.Desk.ContainsKey(newName)) {
                return $"桌子上已有物品：{newName}，请重命名";
            }
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            inv.Inventory.Remove(name);
            item.Name = newName;
            sce.Desk[item.Name] = item;
            env.Save();
            return $"{inv.Name}丢弃了{name}" + (name == newName ? "" : "，并重命名为：" + newName);
        }

        string PickItem(DMEnv env, string name, string newName) {
            if (newName == null) {
                newName = name;
            }
            env.TryGetInv(out Scenario sce, out Investigator inv);
            if (!sce.Desk.TryGetValue(name, out Item item)) {
                return $"桌子上没有物品：{name}";
            }
            if (inv.Inventory.ContainsKey(newName)) {
                return $"{inv.Name}的物品栏中已经存在{newName}，请重命名";
            }
            item.Name = newName;
            inv.Inventory[item.Name] = item;
            sce.Desk.Remove(name);
            env.Save();
            return $"{inv.Name}拾取了：{name}";
        }

        string PassItem(DMEnv env, string name, Investigator targetInv, string newName) {
            if (newName == null) {
                newName = name;
            }
            if (!env.Inv.Inventory.TryGetValue(name, out Item item)) {
                return $"{env.Inv.Name}的物品栏中不存在{name}";
            }
            if (targetInv.Inventory.ContainsKey(newName)) {
                return $"{targetInv.Name}的物品栏中已经存在{newName}";
            }
            env.Inv.Inventory.Remove(name);
            item.Name = newName;
            targetInv.Inventory[item.Name] = item;
            env.Save();
            return $"{env.Inv.Name}把{item.Name}给了：{targetInv.Name}";
        }

        string EditItem(DMEnv env, string name, string newName, Args dict) {
            if (newName == null) {
                newName = name;
            }
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            string wiChanges = FillWeaponInfo(item, dict);
            if (!string.Equals(name, newName)) {
                inv.Inventory.Remove(name);
                item.Name = newName;
                inv.Inventory[item.Name] = (item);
                env.Save();
                return $"{inv.Name}重命名了物品：{name} => {newName}";
            } else {
                env.Save();
                return $"{inv.Name}编辑了物品：{name}" + wiChanges;
            }
        }

        string LoadBullets(DMEnv env, string name, Dice amount) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            int left = item.Capacity - item.CurrentLoad;
            if (left <= 0) {
                return "弹匣已经装满，无需装弹";
            }
            int loaded = Math.Min(left, amount.Roll());
            item.CurrentLoad += loaded;
            env.Save();
            return new StringBuilder().AppendLine($"{inv.Name}为{name}装弹{loaded}")
                .Append($"弹药：{item.CurrentLoad}/{item.Capacity}").ToString();
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper mapper = new DictMapper()
                .Then("技能名", new StringParser())
                .Then("类型", new OrParser(new string[] { "肉搏", "投掷", "射击" }))
                .Then("伤害", new DiceParser())
                .Then("贯穿", new BoolParser("是", "否"), true)
                .Then("连发数", new IntParser())
                .Then("弹匣", new IntParser())
                .Then("故障值", new IntParser())
                .Then("弹药", new IntParser())
                .Then("消耗", new IntParser())
                .SkipRest();

            dispatcher.Register("物品").Handles(ExistSelfInv())
            .Handles(ExistSelfInv())
            .Then(
                Literal<DMEnv>("创造")
                .Handles(ExistSelfInv())
                .Then(
                    String<DMEnv>("物品名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => CreateItem(env, args.GetStr("物品名"), dict))
                )
            ).Then(
                Literal<DMEnv>("销毁")
                .Then(
                    String<DMEnv>("物品名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => DestoryItem(env, args.GetStr("物品名")))
                )
            ).Then(
                Literal<DMEnv>("丢弃")
                .Then(
                    String<DMEnv>("物品名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => ThrowItem(env, args.GetStr("物品名"), null))
                    .Then(
                        String<DMEnv>("新名")
                        .MapDict(mapper)
                        .Executes((env, args, dict) => ThrowItem(env, args.GetStr("物品名"), args.GetStr("新名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("拾取")
                .Then(
                    String<DMEnv>("物品名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => PickItem(env, args.GetStr("物品名"), null))
                    .Then(
                        String<DMEnv>("新名")
                        .MapDict(mapper)
                        .Executes((env, args, dict) => PickItem(env, args.GetStr("物品名"), args.GetStr("新名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("传递")
                .Then(
                    String<DMEnv>("物品名")
                    .Then(
                        Literal<DMEnv>("目标名")
                        .Handles(ExistInv())
                        .Executes((env, args, dict) => PassItem(env, args.GetStr("物品名"), args.GetInv("目标名"), null))
                        .Then(
                            String<DMEnv>("新名")
                            .Executes((env, args, dict) => PassItem(env, args.GetStr("物品名"), args.GetInv("目标名"), args.GetStr("新名")))
                        )
                    )
                )
            ).Then(
                Literal<DMEnv>("编辑")
                .Then(
                    String<DMEnv>("物品名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => EditItem(env, args.GetStr("物品名"), null, dict))
                    .Then(
                        String<DMEnv>("新名")
                        .MapDict(mapper)
                        .Executes((env, args, dict) => EditItem(env, args.GetStr("物品名"), args.GetStr("新名"), dict))
                    )
                )
            ).Then(
                Literal<DMEnv>("装弹")
                .Then(
                    String<DMEnv>("武器名").Then(
                        Dice("装弹量")
                        .Executes((env, args, dict) => LoadBullets(env, args.GetStr("武器名"), args.GetDice("装弹量")))
                    )
                )
            );

            dispatcher.SetAlias("造物", "物品 创造");
            dispatcher.SetAlias("销毁", "物品 销毁");
            dispatcher.SetAlias("丢弃", "物品 丢弃");
            dispatcher.SetAlias("拾取", "物品 拾取");
            dispatcher.SetAlias("传递", "物品 传递");
            dispatcher.SetAlias("装弹", "物品 装弹");


            dispatcher.SetAlias("it", "物品");
            dispatcher.SetAlias("cr", "物品 创造");
            dispatcher.SetAlias("ds", "物品 销毁");
            dispatcher.SetAlias("th", "物品 丢弃");
            dispatcher.SetAlias("pu", "物品 拾取");
            dispatcher.SetAlias("ld", "物品 装弹");
            dispatcher.SetAlias("ps", "物品 传递");
        }
    }
}
