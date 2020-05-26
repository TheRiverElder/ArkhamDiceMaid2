using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Utils {
    public class AdvancedStringBuilder {

        private StringBuilder Builder = new StringBuilder();

        /// <summary>
        /// 向缓冲中加入信息
        /// </summary>
        /// <param name="messages">信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Append(params string[] messages) {
            foreach (string elem in messages) {
                Builder.Append(elem);
            }
            return this;
        }

        /// <summary>
        /// 向缓冲中加入信息
        /// </summary>
        /// <param name="messages">信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Append(string message) {
            Builder.Append(message);
            return this;
        }

        /// <summary>
        /// 向缓冲中加入信息
        /// </summary>
        /// <param name="messages">信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Append(object message) {
            Builder.Append(message);
            return this;
        }

        /// <summary>
        /// 向缓冲中加入多行信息，先加入信息，后换行
        /// </summary>
        /// <param name="messages">多行信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder AppendLines(params string[] messages) {
            foreach (string elem in messages) {
                Builder.AppendLine(elem.ToString());
            }
            return this;
        }

        /// <summary>
        /// 向缓冲中加入单行信息，先加入信息，后换行
        /// </summary>
        /// <param name="messages">信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder AppendLine(string message) {
            Builder.AppendLine(message);
            return this;
        }

        /// <summary>
        /// 向缓冲中加入多行信息，先换行，后加入信息
        /// </summary>
        /// <param name="messages">多行信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder LinesAppend(params string[] messages) {
            foreach (string elem in messages) {
                Builder.AppendLine().Append(elem);
            }
            return this;
        }

        /// <summary>
        /// 向缓冲中加入单行信息，先换行，后加入信息
        /// </summary>
        /// <param name="messages">信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder LineAppend(string message) {
            Builder.AppendLine().Append(message);
            return this;
        }

        /// <summary>
        /// 向缓冲中加入多个信息，并用连接符链接
        /// </summary>
        /// <param name="joiner">连接符</param>
        /// <param name="messages">多个信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Append(string joiner, params string[] messages) {
            Builder.Append(string.Join(joiner, messages));
            return this;
        }

        /// <summary>
        /// 向缓冲中加入多个信息，并用连接符链接
        /// </summary>
        /// <param name="joiner">连接符</param>
        /// <param name="messages">多个信息</param>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Append<T>(string joiner, IEnumerable<T> messages) {
            Builder.Append(string.Join(joiner, messages));
            return this;
        }

        /// <summary>
        /// 添加换行符
        /// </summary>
        /// <returns>返回自身以便链式调用</returns>
        public AdvancedStringBuilder Line() {
            Builder.AppendLine();
            return this;
        }

        /// <summary>
        /// 提供了以Setter的方法添加内容的方式
        /// </summary>
        public string Next {
            set {
                Append(value);
            }
        }

        /// <summary>
        /// 将其转化为字符串返回
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString() {
            return Builder.ToString();
        }

    }
}
