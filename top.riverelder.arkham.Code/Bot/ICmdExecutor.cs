using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot
{
    public interface ICmdExecutor
    {
        string Execute(Dictionary<string, object> args, Dictionary<string, string> dict, CmdEnv env);
    }
}
