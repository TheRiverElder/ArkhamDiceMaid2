using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.RiverCommand {

    /// <summary>
    /// 当命令被解析后的执行器
    /// </summary>
    /// <typeparam name="TEnv">环境类型</typeparam>
    /// <param name="env">环境</param>
    /// <param name="args">线性参数</param>
    /// <param name="dict">映射参数</param>
    /// <returns>执行的结果，不一定是字符串</returns>
    public delegate object CmdExecutor<TEnv>(TEnv env = default(TEnv), Args args = null, Args dict = null);
    public delegate void VoidCmdExecutor<TEnv>(TEnv env = default(TEnv), Args args = null, Args dict = null);

    /// <summary>
    /// 对初步获得的参数进行预处理
    /// </summary>
    /// <typeparam name="TEnv">环境类型</typeparam>
    /// <param name="env">环境</param>
    /// <param name="args">之前的线性参数</param>
    /// <param name="ori">初步解析到的参数</param>
    /// <param name="arg">处理后得到的参数</param>
    /// <param name="err">错误信息</param>
    /// <returns>是否能通过该处理</returns>
    public delegate bool PreHandler<TEnv>(TEnv env, Args args, object ori, out object arg, out string err);
    
}
