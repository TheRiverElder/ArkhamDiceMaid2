using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    public class Scenario
    {
        public List<Horse> Horses = new List<Horse>();

        public string Name { get; set; } = "未命名模组";

        public HashSet<long> AdminList = new HashSet<long>();

        public Dictionary<long, string> PlayerNames = new Dictionary<long, string>();

        public Dictionary<string, Investigator> Investigators = new Dictionary<string, Investigator>();

        public Dictionary<string, Item> Desk = new Dictionary<string, Item>();

        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();
        

        public Scenario(string name)
        {
            Name = name;
        }

        #region 卡操作

        #region 以名字为键


        /// <summary>
        /// 通过调查员名字获取调查员
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <param name="investigator">调查员</param>
        /// <returns>是否成功</returns>
        public bool TryGetInvestigator(string name, out Investigator investigator)
        {
            return Investigators.TryGetValue(name, out investigator);
        }
        /// <summary>
        /// 查看的成员是否存在
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>是否存在</returns>
        public bool ExistInvestigator(string name)
        {
            return Investigators.ContainsKey(name);
        }

        /// <summary>
        /// 通过调查员名字获取调查员
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>调查员，若没找到则返回null</returns>
        public Investigator GetInvestigator(string name)
        {
            if (Investigators.TryGetValue(name, out Investigator inv))
            {
                return inv;
            }
            return null;
        }
        #endregion

        #region 以user为键


        /// <summary>
        /// 查看的成员是否存在
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>是否存在</returns>
        public bool ExistInvestigator(long user)
        {
            return PlayerNames.TryGetValue(user, out string name) && Investigators.ContainsKey(name);
        }

        /// <summary>
        /// 通过调查员名字获取调查员
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>调查员，若没找到则返回null</returns>
        public Investigator GetInvestigator(long user)
        {
            if (PlayerNames.TryGetValue(user, out string name) && Investigators.TryGetValue(name, out Investigator inv))
            {
                return inv;
            }
            return null;
        }

        /// <summary>
        /// 通过玩家ID获取调查员
        /// </summary>
        /// <param name="qq">玩家id</param>
        /// <param name="investigator">调查员</param>
        /// <returns>是否成功</returns>
        public bool TryGetInvestigator(long user, out Investigator investigator)
        {
            if (!PlayerNames.TryGetValue(user, out string name))
            {
                investigator = null;
                return false;
            }
            return Investigators.TryGetValue(name, out investigator);
        }

        #endregion

        /// <summary>
        /// 加入新的调查员
        /// </summary>
        /// <param name="inv">调查员</param>
        public void PutInvestigator(Investigator inv)
        {
            Investigators[inv.Name] = inv;
        }

        /// <summary>
        /// 让玩家获取对调查员的控制权
        /// </summary>
        /// <param name="qq">玩家id</param>
        /// <param name="name">调查员名</param>
        public void Control(long qq, string name)
        {
            PlayerNames[qq] = name;
        }

        #endregion
        
    }
}
