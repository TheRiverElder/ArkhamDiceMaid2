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

        string CreateItem(DMEnv env, string name, Args dict) {
            Investigator inv = env.Inv;
            if (inv.Inventory.Has(name)) {
                return $"{inv.Name}的物品栏中已经存在{name}，请重命名";
            }
            Item item = new Item(name);
            if (dict.Count > 0) {
                item.Weapon = new WeaponInfo(
                    dict.TryGet("技能名", out string sn) ? sn : "肉搏",
                    dict.TryGet("技能值", out int sv) ? sv : 25,
                    dict.TryGet("伤害", out string d) ? d : "1d1+DB",
                    dict.TryGet("穿刺", out bool i) ? i : false,
                    dict.TryGet("连发数", out int mc) ? mc : 1,
                    dict.TryGet("弹匣", out int c) ? c : 1,
                    dict.TryGet("故障", out int m) ? m : 100,
                    dict.TryGet("弹药", out int cl) ? cl : 0,
                    dict.TryGet("消耗", out int co) ? co : 0
                );
            }
            inv.Inventory.Put(item);
            env.Save();
            return $"{inv.Name}创造了物品：{name}";
        }

        string DestoryItem(DMEnv env, string name) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.Has(name)) {
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
            if (sce.desk.ContainsKey(newName)) {
                return $"桌子上已有物品：{newName}，请重命名";
            }
            if (!inv.Inventory.TryGet(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            inv.Inventory.Remove(name);
            item.Name = newName;
            sce.desk[item.Name] = item;
            env.Save();
            return $"{inv.Name}丢弃了{name}" + (name == newName ? "" : "，并重命名为：" + newName);
        }

        string PickItem(DMEnv env, string name, string newName) {
            if (newName == null) {
                newName = name;
            }
            env.TryGetInv(out Scenario sce, out Investigator inv);
            if (!sce.desk.TryGetValue(name, out Item item)) {
                return $"桌子上没有物品：{name}";
            }
            if (inv.Inventory.Has(newName)) {
                return $"{inv.Name}的物品栏中已经存在{newName}，请重命名";
            }
            item.Name = newName;
            inv.Inventory.Put(item);
            sce.desk.Remove(name);
            env.Save();
            return $"{inv.Name}拾取了：{name}";
        }

        string EditItem(DMEnv env, string name, string newName, Args dict) {
            if (newName == null) {
                newName = name;
            }
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGet(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            if (dict.Count > 0) {
                if (!item.IsWeapon) {
                    item.Weapon = new WeaponInfo();
                }
                WeaponInfo w = item.Weapon;
                if (dict.TryGet("技能名", out string sn)) w.SkillName = sn;
                if (dict.TryGet("技能值", out int sv)) w.SkillValue = sv;
                if (dict.TryGet("伤害", out string d)) w.Damage = d;
                if (dict.TryGet("穿刺", out bool i)) w.Impale = i;
                if (dict.TryGet("连发数", out int mc)) w.MaxCount = mc;
                if (dict.TryGet("弹匣", out int c)) w.Capacity = c;
                if (dict.TryGet("故障", out int m)) w.Mulfunction = m;
                if (dict.TryGet("弹药", out int cl)) w.CurrentLoad = cl;
                if (dict.TryGet("消耗", out int co)) w.Cost = co;
            }
            if (!string.Equals(name, newName)) {
                inv.Inventory.Remove(name);
                item.Name = newName;
                inv.Inventory.Put(item);
                env.Save();
                return $"{inv.Name}重命名了物品：{name} => {newName}";
            } else {
                env.Save();
                return $"{inv.Name}编辑了物品：{name}";
            }
        }

        string LoadBullets(DMEnv env, string name, Dice amount) {
            Investigator inv = env.Inv;
            if (!inv.Inventory.TryGet(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            if (!item.IsWeapon) {
                return $"{item.Name}不是武器";
            }
            WeaponInfo w = item.Weapon;
            int left = w.Capacity - w.CurrentLoad;
            if (left <= 0) {
                return "弹匣已经装满，无需装弹";
            }
            int loaded = Math.Min(left, amount.Roll());
            w.CurrentLoad += loaded;
            env.Save();
            return new StringBuilder().AppendLine($"{inv.Name}为{name}装弹{loaded}")
                .Append($"弹药：{w.CurrentLoad}/{w.Capacity}").ToString();
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper mapper = new DictMapper()
                .Then("技能名", new StringParser())
                .Then("技能值", new IntParser())
                .Then("伤害", new StringParser())
                .Then("穿刺", new BoolParser("是", "否"), true)
                .Then("连发数", new IntParser())
                .Then("弹匣", new IntParser())
                .Then("故障", new IntParser())
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
                Literal<DMEnv>("捡起")
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
                        Dice<DMEnv>("装弹量")
                        .Executes((env, args, dict) => LoadBullets(env, args.GetStr("武器名"), args.GetDice("装弹量")))
                    )
                )
            );
        }
    }
}
