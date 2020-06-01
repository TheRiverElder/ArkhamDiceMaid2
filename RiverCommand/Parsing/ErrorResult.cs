using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Parsing {
    public class ErrorResult : ICmdResult {

        public int MatchedLength { get; }

        public int ArgLength { get; }

        public bool IsError => true;

        public string Error { get; }

        public string[] Errors => SubError == null ? 
            new string[] { Error } : 
            (new string[] { Error }).Concat(SubError.Errors).ToArray();

        public ErrorResult SubError { get; }

        public ErrorResult(int matchedLength, int argLength, string error) {
            MatchedLength = matchedLength;
            ArgLength = argLength;
            Error = error;
        }

        public ErrorResult(int matchedLength, int argLength, string error, ErrorResult subError) : this(matchedLength, argLength, error) {
            SubError = subError;
        }

        public object Execute() => throw new NotSupportedException("命令调度的错误信息不可被执行");


        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"第{ArgLength}个参数:")
                .Append(Error);
            ErrorResult err = this;
            while (err.SubError != null) {
                err = err.SubError;
                sb.AppendLine($"第{err.ArgLength}个参数:")
                    .Append(err.Error);
            }
            return sb.ToString();
        }
    }
}
