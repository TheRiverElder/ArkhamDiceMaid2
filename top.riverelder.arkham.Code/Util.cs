using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code
{
    public static class Util
    {
        private static int uid = DateTime.Now.Millisecond;

        /// <summary>
        /// 获取一个id
        /// </summary>
        public static int NextId => uid++;
    }
}
