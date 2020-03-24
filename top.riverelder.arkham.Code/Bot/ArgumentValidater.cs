using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot
{
    public class ArgumentValidater
    {
        public static string Number = @"[-+]?\d+"; 
        public static string Value = @"\d+(/\d+)?"; 
        public static string Any = @"\S+"; 
        public static string Dice = @"(\d+([dD]\d+)?)([+-]\d+([dD]\d+)?)*"; 


        public static ArgumentValidater Empty { get { return new ArgumentValidater(); } }



        public static int Unlimited = -1;


        private int ListArgCountMin = 0;
        private int ListArgCountMax = 0;
        private IList<string> ListArgRegExps = new List<string>();
        private string RestListArgRegExp = null;
        private IDictionary<string, string> DictArgRegExps = new Dictionary<string, string>();
        private string RestDictArgRegExp = null;

        private ISet<string> NecerssaryDictArgs = new HashSet<string>();

        public string ListArgCountString
        {
            get {
                if (ListArgCountMax == Unlimited)
                {
                    return $">={ListArgCountMin}";
                }
                return $"{ListArgCountMin}~{ListArgCountMax}";
            }
        }

        public ArgumentValidater SetListArgCount(int limit)
        {
            this.ListArgCountMin = limit;
            this.ListArgCountMax = limit;
            return this;
        }

        public ArgumentValidater SetListArgCountMin(int min)
        {
            this.ListArgCountMin = min;
            return this;
        }

        public ArgumentValidater SetListArgCountMax(int max)
        {
            this.ListArgCountMax = max;
            return this;
        }

        public ArgumentValidater AddListArg(string regExp)
        {
            ListArgRegExps.Add(regExp);
            return this;
        }

        public ArgumentValidater AddDictArg(string key, string regExp, bool necessary)
        {
            DictArgRegExps[key] = regExp;
            if (necessary)
            {
                NecerssaryDictArgs.Add(key);
            }
            return this;
        }

        public ArgumentValidater SetRestListArg(string regExp)
        {
            RestListArgRegExp = regExp;
            return this;
        }

        public ArgumentValidater SetRestDictArg(string regExp)
        {
            RestDictArgRegExp = regExp;
            return this;
        }

        public bool Match(string[] listArgs, IDictionary<string, string> dictArgs, out string err)
        {
            StringBuilder builder = new StringBuilder();
            // 判断线性参数的数量
            if (listArgs.Length < ListArgCountMin || (ListArgCountMax != Unlimited && listArgs.Length > ListArgCountMax))
            {
                builder.AppendLine($"线性参数数量错误，需要{ListArgCountString}，得到{listArgs.Length}");
            }
            else // 如果没有缺失线性参数，则检查线性参数的合理性
            {
                // 错误的线性参数名
                ISet<int> incorrectListArgs = new HashSet<int>();
                for (int i = 0; i < listArgs.Length; i++)
                {
                    string regExp = (i < ListArgRegExps.Count) ? ListArgRegExps[i] : RestListArgRegExp;
                    if (regExp != null && !Regex.IsMatch(listArgs[i], regExp))
                    {
                        incorrectListArgs.Add(i);
                    }
                }
                if (incorrectListArgs.Count > 0)
                {
                    builder.AppendLine($"错误的线性参数：{string.Join("、", incorrectListArgs.ToArray())}");
                }
            }

            // 缺失的必要映射参数名
            ISet<string> missingDictArg = new HashSet<string>();
            // 判断缺失的线性的参数
            foreach (string key in NecerssaryDictArgs)
            {
                if (!dictArgs.ContainsKey(key))
                {
                    missingDictArg.Add(key);
                }
            }
            // 如果有缺失元素则增加错误提示
            if (missingDictArg.Count > 0)
            {
                builder.AppendLine($"缺少映射参数：{string.Join("、", missingDictArg.ToArray())}");
            }

            // 错误的线性参数名
            ISet<string> incorrectDictArgs = new HashSet<string>();
            // 检查映射参数的合理性
            foreach (string key in dictArgs.Keys)
            {
                string regExp = DictArgRegExps.ContainsKey(key) ? DictArgRegExps[key] : RestDictArgRegExp;
                if (regExp != null && !Regex.IsMatch(dictArgs[key], regExp))
                {
                    incorrectDictArgs.Add(key);
                }
            }
            if (incorrectDictArgs.Count > 0)
            {
                builder.AppendLine($"错误的映射参数：{string.Join("、", incorrectDictArgs.ToArray())}");
            }

            // 返回结果
            if (builder.Length == 0)
            {
                err = null;
                return true;
            }
            err = builder.ToString();
            return false;
        }
    }
}
