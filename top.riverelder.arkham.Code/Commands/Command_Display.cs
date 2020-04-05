//using System.Collections.Generic;
//using System.Text;

//using top.riverelder.arkham.Code.Model;
//using top.riverelder.arkham.Code.Utils;

//namespace top.riverelder.arkham.Code.Commands
//{
//    class Command_Display : DiceCmdEntry {
//        public string Name => "显示";

//        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
//                .SetListArgCountMin(1)
//                .SetListArgCountMax(2)
//                .AddListArg(@"数值|信息|物品|战斗")
//                .AddListArg(ArgumentValidater.Any);

//        public string Usage => "显示 <属性|信息|物品|战斗>";
        

//        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
//        {
//            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
//                return err;
//            }
//            string target = listArgs[0];
//            string valName = listArgs.Length > 1 ? listArgs[1] : null;

//            switch (target) {
//                case "信息": return $"姓名：{inv.Name}，描述：{inv.Desc}，体格：{inv.Build}，DB：{inv.DamageBonus}";
//                case "物品": {
//                        if (inv.Inventory.Count == 0) {
//                            return $"{inv.Name}没有物品";
//                        }
//                        if (!string.IsNullOrEmpty(valName) && inv.Inventory.TryGet(valName, out Item it))
//                        {
//                            StringBuilder b = new StringBuilder().AppendLine($"{inv.Name}的 {valName}：");
//                            if (it.IsWeapon)
//                            {
//                                WeaponInfo w = it.Weapon;
//                                b
//                                    .Append("技能名：").AppendLine(w.SkillName)
//                                    .Append("技能值：").AppendLine(w.SkillValue.ToString())
//                                    .Append("伤害：").AppendLine(w.Damage)
//                                    .Append("贯穿：").AppendLine(w.Impale ? "是" : "否")
//                                    .Append("次数：").AppendLine(w.MaxCount.ToString())
//                                    .Append("弹匣：").AppendLine(w.Capacity.ToString())
//                                    .Append("故障：").AppendLine(w.Mulfunction.ToString())
//                                    .Append("弹药：").Append(w.CurrentLoad.ToString());
//                            }
//                            return b.ToString();
//                        }
//                        StringBuilder sb = new StringBuilder().Append($"{inv.Name}的物品：");
//                        foreach (Item item in inv.Inventory.Items)
//                        {
//                            sb.AppendLine().Append(item.Name);
//                        }
//                        return sb.ToString();
//                    } 
//                case "战斗": {
//                        if (inv.Fights.Count == 0) {
//                            return $"{inv.Name}没有战斗事件";
//                        }
//                        StringBuilder sb = new StringBuilder().Append($"{inv.Name}的战斗事件：");
//                        foreach (FightEvent fight in inv.Fights) {
//                            sb.AppendLine().Append($"来自{fight.SourceName}使用{fight.WeaponName}的攻击");
//                        }
//                        return sb.ToString();
//                    } 
//            }
//            // 由于限定了参数，所有现在target必定是"属性"
//            if (!string.IsNullOrEmpty(valName) && inv.Values.TryGet(valName, out Value value))
//            {
//                return $"{inv.Name}的{valName}：普通={value.Val}，困难={value.HardVal}，极难={value.ExtremeVal}";
//            }
//            StringBuilder builder = new StringBuilder();
//            builder.Append($"{inv.Name}的属性：");

//            IList<string> names = new List<string>(inv.Values.Names);
//            for (int i = 0; i < names.Count; i++)
//            {
//                string name = names[i];
//                Value v = inv.Values[name];
//                builder.AppendLine().Append($"{name}:{v.Val}");
//            }
//            return builder.ToString();
//        }

//        string DisplayInfo(Investigator inv) {
//            return $"姓名：{inv.Name}，描述：{inv.Desc}，体格：{inv.Build}，DB：{inv.DamageBonus}";
//        }

//        string DisplayInventory(Investigator inv) {
//            return $"姓名：{inv.Name}，描述：{inv.Desc}，体格：{inv.Build}，DB：{inv.DamageBonus}";
//        }
//    }
//}
