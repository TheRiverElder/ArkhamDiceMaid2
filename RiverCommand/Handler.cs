﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand {
    public delegate object CmdExecutor<TEnv>(TEnv env, Args args, Args dict, out string reply);
    public delegate bool PreHandler<TEnv>(TEnv env, Args args, object ori, out object arg, out string err);
}
