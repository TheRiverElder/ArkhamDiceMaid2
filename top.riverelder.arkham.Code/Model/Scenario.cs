using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    public class Scenario {
        public List<Horse> Horses = new List<Horse>();

        public string Name { get; set; } = "δ����ģ��";

        public Dictionary<long, string> PlayerNames = new Dictionary<long, string>();

        public Dictionary<string, Investigator> Investigators = new Dictionary<string, Investigator>();

        public Dictionary<string, Item> Desk = new Dictionary<string, Item>();

        public Dictionary<string, Spell> Spells = new Dictionary<string, Spell>();


        public Scenario(string name) {
            Name = name;
        }

        #region ������

        #region ������Ϊ��


        /// <summary>
        /// ͨ������Ա���ֻ�ȡ����Ա
        /// </summary>
        /// <param name="name">����Ա��</param>
        /// <param name="investigator">����Ա</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool TryGetInvestigator(string name, out Investigator investigator) {
            return Investigators.TryGetValue(name, out investigator);
        }
        /// <summary>
        /// �鿴�ĳ�Ա�Ƿ����
        /// </summary>
        /// <param name="name">����Ա��</param>
        /// <returns>�Ƿ����</returns>
        public bool ExistInvestigator(string name) {
            return Investigators.ContainsKey(name);
        }

        /// <summary>
        /// ͨ������Ա���ֻ�ȡ����Ա
        /// </summary>
        /// <param name="name">����Ա��</param>
        /// <returns>����Ա����û�ҵ��򷵻�null</returns>
        public Investigator GetInvestigator(string name) {
            if (Investigators.TryGetValue(name, out Investigator inv)) {
                return inv;
            }
            return null;
        }
        #endregion

        #region ��userΪ��


        /// <summary>
        /// �鿴�ĳ�Ա�Ƿ����
        /// </summary>
        /// <param name="name">����Ա��</param>
        /// <returns>�Ƿ����</returns>
        public bool ExistInvestigator(long user) {
            return PlayerNames.TryGetValue(user, out string name) && Investigators.ContainsKey(name);
        }

        /// <summary>
        /// ͨ������Ա���ֻ�ȡ����Ա
        /// </summary>
        /// <param name="name">����Ա��</param>
        /// <returns>����Ա����û�ҵ��򷵻�null</returns>
        public Investigator GetInvestigator(long user) {
            if (PlayerNames.TryGetValue(user, out string name) && Investigators.TryGetValue(name, out Investigator inv)) {
                return inv;
            }
            return null;
        }

        /// <summary>
        /// ͨ�����ID��ȡ����Ա
        /// </summary>
        /// <param name="qq">���id</param>
        /// <param name="investigator">����Ա</param>
        /// <returns>�Ƿ�ɹ�</returns>
        public bool TryGetInvestigator(long user, out Investigator investigator) {
            if (!PlayerNames.TryGetValue(user, out string name)) {
                investigator = null;
                return false;
            }
            return Investigators.TryGetValue(name, out investigator);
        }

        #endregion

        /// <summary>
        /// �����µĵ���Ա
        /// </summary>
        /// <param name="inv">����Ա</param>
        public void PutInvestigator(Investigator inv) {
            Investigators[inv.Name] = inv;
        }

        /// <summary>
        /// ����һ�ȡ�Ե���Ա�Ŀ���Ȩ
        /// </summary>
        /// <param name="qq">���id</param>
        /// <param name="name">����Ա��</param>
        public void Control(long qq, string name) {
            PlayerNames[qq] = name;
        }

        #endregion

    }
}