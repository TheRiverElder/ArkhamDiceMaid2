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

            inv.Values.Fill(Global.DefaultValues);

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
            );
        }
    }
}
