using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Item : ICommand {

        public static readonly string SkillRegExp = @"([^ \t\r\n:：0-9]+)\s*[:：]?\s*(\d+)";

        public string Name => "物品";

        public string Usage => "物品 <创造|丢弃|拾取|销毁|编辑> <物品名> [重命名]";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMin(2)
            .SetListArgCountMax(3)
            .AddListArg("创造|丢弃|拾取|销毁|编辑")
            .AddListArg(ArgumentValidater.Any)
            .AddListArg(ArgumentValidater.Any)
            .AddDictArg("技能", @"\D+\d+", false)
            .AddDictArg("伤害", ArgumentValidater.Any, false)
            .AddDictArg("穿刺", "是|否", false)
            .AddDictArg("次数", ArgumentValidater.Number, false)
            .AddDictArg("弹匣", ArgumentValidater.Number, false)
            .AddDictArg("故障", ArgumentValidater.Number, false)
            .AddDictArg("弹药", ArgumentValidater.Number, false);

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            string opt = listArgs[0];
            string name = listArgs[1];
            string newName = listArgs.Length > 2 ? listArgs[2] : name;

            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }

            Scenario scenario = env.Scenario;

            Dictionary<string, object> dict = ParseWeapon(dictArgs);


            string ret = "未知错误";
            switch (opt) {
                case "创造": ret = CreateItem(inv, name, dict); break;
                case "丢弃": {
                        if (!scenario.desk.ContainsKey(newName)) {
                            if (!inv.Inventory.TryGet(name, out Item item)) {
                                return $"{inv.Name}的物品栏中不存在{newName}";
                            }
                            inv.Inventory.Remove(name);
                            item.Name = newName;
                            scenario.desk[item.Name] = item;
                            return $"{inv.Name}丢弃了{name}";
                        }
                        ret = $"桌子上已有物品：{newName}，请重命名";
                    } break;
                case "拾取": {
                        if (!scenario.desk.TryGetValue(name, out Item item)) {
                            return $"桌子上没有物品：{name}";
                        }
                        if (inv.Inventory.Has(newName)) {
                            return $"{inv.Name}的物品栏中已经存在{newName}，请重命名";
                        }
                        item.Name = newName;
                        inv.Inventory.Put(item);
                        scenario.desk.Remove(name);
                        ret = $"{inv.Name}拾取了{item.Name}";
                    } break;
                case "销毁": {
                        if (!inv.Inventory.Has(newName)) {
                            return $"{inv.Name}物品栏没有物品：{name}";
                        }
                        inv.Inventory.Remove(name);
                        ret = $"{inv.Name}销毁了{name}";
                    } break;
                case "编辑": ret = EditItem(inv, name, newName, dict); break;
            }
            SaveUtil.Save(scenario);
            return ret;
        }

        Dictionary<string, object> ParseWeapon(IDictionary<string, string> rd) {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (rd.TryGetValue("技能", out string s)) {
                Match match = Regex.Match(s, SkillRegExp);
                dict["技能"] = Value.Of(match.Groups[1].Value, match.Groups[2].Value);
            }
            if (rd.TryGetValue("伤害", out string d)) dict["伤害"] = d;
            if (rd.TryGetValue("穿刺", out string i)) dict["穿刺"] = "是".Equals(i);
            if (rd.TryGetValue("次数", out string mc)) dict["次数"] = int.Parse(mc);
            if (rd.TryGetValue("弹匣", out string c)) dict["弹匣"] = int.Parse(c);
            if (rd.TryGetValue("故障", out string m)) dict["故障"] = int.Parse(m);
            if (rd.TryGetValue("弹药", out string cl)) dict["弹药"] = int.Parse(cl);
            return dict;
        }

        string CreateItem(Investigator inv, string name, Dictionary<string, object> dict) {
            if (inv.Inventory.Has(name)) {
                return $"{inv.Name}的物品栏中已经存在{name}，请重命名";
            }
            Item item = new Item(name);
            if (dict.Count > 0) {
                item.Weapon = new WeaponInfo(
                    dict.TryGetValue("技能", out object s) ? (Value)s : new Value("肉搏", 25),
                    dict.TryGetValue("伤害", out object d) ? (string)d : "1d1",
                    dict.TryGetValue("穿刺", out object i) ? (bool)i : false,
                    dict.TryGetValue("次数", out object mc) ? (int)mc : 1,
                    dict.TryGetValue("弹匣", out object c) ? (int)c : 1,
                    dict.TryGetValue("故障", out object m) ? (int)m : 100,
                    dict.TryGetValue("弹药", out object cl) ? (int)cl : 0
                );
            }
            inv.Inventory.Put(item);
            return $"{inv.Name}创造了物品：{name}";
        }

        string EditItem(Investigator inv, string name, string newName, Dictionary<string, object> dict) {
            if (!inv.Inventory.TryGet(name, out Item item)) {
                return $"{inv.Name}的物品栏中不存在{name}";
            }
            if (dict.Count > 0) {
                if (!item.IsWeapon) {
                    item.Weapon = new WeaponInfo();
                }
                WeaponInfo w = item.Weapon;
                if (dict.TryGetValue("技能", out object s)) w.Skill = (Value)s;
                if (dict.TryGetValue("伤害", out object d)) w.Damage = (string)d;
                if (dict.TryGetValue("穿刺", out object i)) w.Impale = (bool)i;
                if (dict.TryGetValue("次数", out object mc)) w.MaxCount = (int)mc;
                if (dict.TryGetValue("弹匣", out object c)) w.Capacity = (int)c;
                if (dict.TryGetValue("故障", out object m)) w.Mulfunction = (int)m;
                if (dict.TryGetValue("弹药", out object cl)) w.CurrentLoad = (int)cl;
            }
            if (!string.Equals(name, newName)) {
                inv.Inventory.Remove(name);
                item.Name = newName;
                inv.Inventory.Put(item);
                return $"{inv.Name}重命名了物品：{name}=>{newName}";
            }
            return $"{inv.Name}编辑了物品：{name}";
        }
    }
}
