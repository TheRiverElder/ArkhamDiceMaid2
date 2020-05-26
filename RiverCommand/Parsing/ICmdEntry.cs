

namespace top.riverelder.RiverCommand.Parsing {
    public interface ICmdEntry<TEnv> {

        void OnRegister(CmdDispatcher<TEnv> dispatcher);

    }
}
