

namespace top.riverelder.RiverCommand {
    public interface ICmdEntry<TEnv> {

        void OnRegister(CmdDispatcher<TEnv> dispatcher);

    }
}
