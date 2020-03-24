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

        public Dictionary<long, string> player2investigatorMap = new Dictionary<long, string>();

        public Dictionary<string, Investigator> investigators = new Dictionary<string, Investigator>();

        public Dictionary<string, Item> desk = new Dictionary<string, Item>();

        public Queue<FightEvent> queue = new Queue<FightEvent>();

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
            return investigators.TryGetValue(name, out investigator);
        }
        /// <summary>
        /// 查看的成员是否存在
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>是否存在</returns>
        public bool ExistInvestigator(string name)
        {
            return investigators.ContainsKey(name);
        }

        /// <summary>
        /// 通过调查员名字获取调查员
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>调查员，若没找到则返回null</returns>
        public Investigator GetInvestigator(string name)
        {
            if (investigators.TryGetValue(name, out Investigator inv))
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
            return player2investigatorMap.TryGetValue(user, out string name) && investigators.ContainsKey(name);
        }

        /// <summary>
        /// 通过调查员名字获取调查员
        /// </summary>
        /// <param name="name">调查员名</param>
        /// <returns>调查员，若没找到则返回null</returns>
        public Investigator GetInvestigator(long user)
        {
            if (player2investigatorMap.TryGetValue(user, out string name) && investigators.TryGetValue(name, out Investigator inv))
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
            if (!player2investigatorMap.TryGetValue(user, out string name))
            {
                investigator = null;
                return false;
            }
            return investigators.TryGetValue(name, out investigator);
        }

        #endregion

        /// <summary>
        /// 加入新的调查员
        /// </summary>
        /// <param name="inv">调查员</param>
        public void PutInvestigator(Investigator inv)
        {
            investigators[inv.Name] = inv;
        }

        /// <summary>
        /// 让玩家获取对调查员的控制权
        /// </summary>
        /// <param name="qq">玩家id</param>
        /// <param name="name">调查员名</param>
        public void Control(long qq, string name)
        {
            player2investigatorMap[qq] = name;
        }

        #endregion

        public void Emit(FightEvent e) {
            queue.Enqueue(e);
        }

        public bool Handle(string targetName, out FightEvent e) {
            FightEvent[] es = queue.Where(evt => evt.TargetName == targetName).ToArray();
            e = es.Length == 0 ? null : es[0];
            return es.Length != 0;
        }
    }
}
