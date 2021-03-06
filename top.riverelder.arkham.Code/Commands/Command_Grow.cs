﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Grow : DiceCmdEntry {

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("成长").Then(
                Extensions.Dice("增量").Rest(
                    PresetNodes.String<DMEnv>("技能名")
                    .Handles(PreProcesses.ConvertObjectArrayToStringArray)
                    .Executes((env, args, dict) => Grow(env, env.Inv, args.Get<string[]>("技能名"), args.GetDice("增量")))
                )
            ).Rest(
                PresetNodes.String<DMEnv>("技能名")
                .Handles(PreProcesses.ConvertObjectArrayToStringArray)
                .Executes((env, args, dict) => Grow(env, env.Inv, args.Get<string[]>("技能名"), Dice.Of("1d10")))
            );
        }

        public static void Grow(DMEnv env, Investigator inv, string[] skillNames, Dice dice) {
            env.Append(inv.Name + "的成长：");
            foreach (string skillName in skillNames) {
                env.LineAppend(Grow(inv, skillName, dice));
            }
            env.Save();
        }

        public static string Grow(Investigator inv, string skillName, Dice dice) {
            if (!inv.Values.TryWidelyGet(skillName, out Value skill)) {
                return "❓未找到" + skillName;
            }
            if (!skill.Check().succeed) {
                int prev = skill.Val;
                int delta = dice.Roll();
                skill.Add(delta);
                return $"✔️{skillName}：{prev} + {delta} => {skill.Val}";
            } else {
                return "❌" + skillName + "：成长失败";
            }
        }
    }
}
