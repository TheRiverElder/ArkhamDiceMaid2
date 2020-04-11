﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Spell : DiceCmdEntry {

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper mapper = new DictMapper();
            mapper.Rest(new DiceParser());

            dispatcher.Register("法术")
            .Handles(Extensions.ExistSce())
            .Then(
                Literal<DMEnv>("创造").Then(
                    String<DMEnv>("法术名")
                    .MapDict(mapper)
                    .Executes((env, args, dict) => CreateSpell(env, args.GetStr("法术名"), dict))
                )
            ).Then(
                Literal<DMEnv>("销毁").Then(
                    String<DMEnv>("法术名")
                    .Executes((env, args, dict) => DestorySpell(env, args.GetStr("法术名")))
                )
            ).Then(
                Literal<DMEnv>("学习")
                .Handles(Extensions.ExistSelfInv())
                .Then(
                    String<DMEnv>("法术名")
                    .Executes((env, args, dict) => LearnSpell(env, env.Inv, args.GetStr("法术名")))
                )
            ).Then(
                Literal<DMEnv>("忘记")
                .Handles(Extensions.ExistSelfInv())
                .Then(
                    String<DMEnv>("法术名")
                    .Executes((env, args, dict) => ForgetSpell(env, env.Inv, args.GetStr("法术名")))
                )
            ).Then(
                Literal<DMEnv>("使用")
                .Handles(Extensions.ExistSelfInv())
                .Then(
                    String<DMEnv>("法术名")
                    .Executes((env, args, dict) => UseSpell(env, env.Inv, args.GetStr("法术名")))
                )
            );

            dispatcher.SetAlias("学习", "法术 学习");
            dispatcher.SetAlias("忘记", "法术 忘记");
            dispatcher.SetAlias("施法", "法术 使用");
        }

        public static string CreateSpell(DMEnv env, string name, Args cost) {
            Scenario sce = env.Sce;
            if (sce.Spells.ContainsKey(name)) {
                return "已经存在法术：" + name;
            }

            Dictionary<string, string> costStr = new Dictionary<string, string>();
            foreach (string valueName in cost.Keys) {
                if (cost.TryGet(valueName, out Dice dice)) {
                    costStr[valueName] = dice.ToString();
                }
            }
            Spell spell = new Spell(name, costStr);
            sce.Spells[name] = spell;
            env.Save();
            return "成功创造法术：" + name;
        }

        public static string DestorySpell(DMEnv env, string name) {
            Scenario sce = env.Sce;
            if (!sce.Spells.ContainsKey(name)) {
                return "不存在法术：" + name;
            }
            sce.Spells.Remove(name);
            env.Save();
            return "成功销毁法术：" + name;
        }

        public static string LearnSpell(DMEnv env, Investigator inv, string name) {
            Scenario sce = env.Sce;
            if (!sce.Spells.ContainsKey(name)) {
                return "不存在法术：" + name;
            } else if (inv.Spells.Contains(name)) {
                return inv.Name + "已经学会了" + name;
            }
            inv.Spells.Add(name);
            env.Save();
            return inv.Name + "学习了法术：" + name;
        }

        public static string ForgetSpell(DMEnv env, Investigator inv, string name) {
            Scenario sce = env.Sce;
            if (!inv.Spells.Contains(name)) {
                return inv.Name + "还没学会" + name;
            }
            inv.Spells.Remove(name);
            env.Save();
            return inv.Name + "忘记了法术：" + name;
        }

        public static string UseSpell(DMEnv env, Investigator inv, string name) {
            Scenario sce = env.Sce;
            if (!inv.Spells.Contains(name)) {
                return inv.Name + "还没学会" + name;
            } else if (!sce.Spells.TryGetValue(name, out Spell spell)) {
                return "不存在法术：" + name;
            } else if (spell.Use(inv, out string reply)) {
                return "施法失败\n" + reply;
            } else { 
                env.Save();
                return inv.Name + "使用了" + name;
            }
        }

    }
}
