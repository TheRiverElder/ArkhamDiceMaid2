﻿using System;
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
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Item : DiceCmdEntry {

        public static string Usage => "物品 <创造|丢弃|拾取|销毁|编辑|装弹> <物品名> [重命名|弹药数]";

        public static string FillWeaponInfo(Item item, Args dict) {
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

        public static bool CreateItem(DMEnv env, string name, Args dict) {
            Investigator inv = env.Inv;
            if (inv.Inventory.ContainsKey(name)) {
                env.Next = $"{inv.Name}的物品栏中已经存在{name}，请重命名";
                return false;
            }
            Item item = new Item(name);
            env.Append($"{inv.Name}创造了物品：{name}" + FillWeaponInfo(item, dict));

            inv.Inventory[name] = item;
            env.Save();
            return true;
        }

        public static bool DestoryItem(DMEnv env, string name) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.ContainsKey(name)) {
                env.Append($"未找到{inv.Name}的{name}");
                return false;
            }
            inv.Inventory.Remove(name);
            env.Save();
            env.Append($"{inv.Name}销毁了了物品：{name}");
            return true;
        }

        public static bool ThrowItem(DMEnv env, string name, string newName) {
            if (newName == null) {
                newName = name;
            }
            Scenario sce = env.Sce;
            Investigator inv = env.Inv;
            if (sce.Desk.ContainsKey(newName)) {
                env.Append($"桌子上已有物品：{newName}，请重命名");
                return false;
            }
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                env.Append($"{inv.Name}的物品栏中不存在{name}");
                return false;
            }
            inv.Inventory.Remove(name);
            item.Name = newName;
            sce.Desk[item.Name] = item;
            env.Save();
            env.Append($"{inv.Name}丢弃了{name}" + (name == newName ? "" : "，并重命名为：" + newName));
            return true;
        }

        public static bool PickItem(DMEnv env, string name, string newName) {
            if (newName == null) {
                newName = name;
            }
            Scenario sce = env.Sce;
            Investigator inv = env.Inv;
            if (!sce.Desk.TryGetValue(name, out Item item)) {
                env.Append($"桌子上没有物品：{name}");
                return false;
            }
            if (inv.Inventory.ContainsKey(newName)) {
                env.Append($"{inv.Name}的物品栏中已经存在{newName}，请重命名");
                return false;
            }
            item.Name = newName;
            inv.Inventory[item.Name] = item;
            sce.Desk.Remove(name);
            env.Save();
            env.Append($"{inv.Name}拾取了：{name}");
            return true;
        }

        public static bool PassItem(DMEnv env, string name, Investigator targetInv, string newName) {
            if (newName == null) {
                newName = name;
            }
            if (!env.Inv.Inventory.TryGetValue(name, out Item item)) {
                env.Append($"{env.Inv.Name}的物品栏中不存在{name}");
                return false;
            }
            if (targetInv.Inventory.ContainsKey(newName)) {
                env.Append($"{targetInv.Name}的物品栏中已经存在{newName}");
                return false;
            }
            env.Inv.Inventory.Remove(name);
            item.Name = newName;
            targetInv.Inventory[item.Name] = item;
            env.Save();
            env.Append($"{env.Inv.Name}把{item.Name}给了：{targetInv.Name}");
            return true;
        }

        public static bool EditItem(DMEnv env, string name, string newName, Args dict) {
            if (newName == null) {
                newName = name;
            }
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                env.Append($"{inv.Name}的物品栏中不存在{name}");
                return false;
            }
            string wiChanges = FillWeaponInfo(item, dict);
            if (!string.Equals(name, newName)) {
                inv.Inventory.Remove(name);
                item.Name = newName;
                inv.Inventory[item.Name] = (item);
                env.Save();
                env.Append($"{inv.Name}重命名了物品：{name} => {newName}");
                return true;
            } else {
                env.Save();
                env.Append($"{inv.Name}编辑了物品：{name}" + wiChanges);
                return true;
            }
        }

        public static bool LoadBullets(DMEnv env, string name, Dice amount) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGetValue(name, out Item item)) {
                env.Append($"{inv.Name}的物品栏中不存在{name}");
                return false;
            }
            int left = item.Capacity - item.CurrentLoad;
            if (left <= 0) {
                env.Append("弹匣已经装满，无需装弹");
                return false;
            }
            int loaded = Math.Min(left, amount.Roll());
            item.CurrentLoad += loaded;
            env.Save();
            env.AppendLine($"{inv.Name}为{name}装弹{loaded}")
                .Append($"弹药：{item.CurrentLoad}/{item.Capacity}");
            return false;
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper<DMEnv> mapper = new DictMapper<DMEnv>()
                .Then("技能名", new StringParser<DMEnv>())
                .Then("类型", new OrParser<DMEnv>(new string[] { "肉搏", "投掷", "射击" }))
                .Then("伤害", new DiceParser())
                .Then("贯穿", new BoolParser<DMEnv>("是", "否"))
                .Then("连发数", new IntParser<DMEnv>())
                .Then("弹匣", new IntParser<DMEnv>())
                .Then("故障值", new IntParser<DMEnv>())
                .Then("弹药", new IntParser<DMEnv>())
                .Then("消耗", new IntParser<DMEnv>());

            dispatcher.Register("物品").Then(
                Literal<DMEnv>("创造")
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
                        String<DMEnv>("目标名")
                        .Handles(ExistInv)
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
