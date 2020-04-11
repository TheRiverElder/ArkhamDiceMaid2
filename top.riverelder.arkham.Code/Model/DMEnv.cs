using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    public class DMEnv {

        public long SelfId;
        public long GroupId;

        public bool TryGetSce(out Scenario sce) {
            if (!Global.Groups.TryGetValue(GroupId, out string sceName) 
                || !Global.Scenarios.TryGetValue(sceName, out sce)) {
                sce = null;
                return false;
            }
            return true;
        }

        public bool TryGetInv(out Scenario sce, out Investigator inv) {
            if (!Global.Groups.TryGetValue(GroupId, out string sceName) 
                || !Global.Scenarios.TryGetValue(sceName, out sce) 
                || !sce.PlayerNames.TryGetValue(SelfId, out string invName) 
                || !sce.TryGetInvestigator(invName, out inv)) {
                sce = null;
                inv = null;
                return false;
            }
            return true;
        }

        public void Save() {
            if (TryGetSce(out Scenario sce)) {
                SaveUtil.Save(sce);
            }
        }

        public Scenario Sce => TryGetSce(out Scenario sce) ? sce : null;

        public Investigator Inv => TryGetInv(out Scenario sce, out Investigator inv) ? inv : null;

        public DMEnv(long selfId, long groupId) {
            SelfId = selfId;
            GroupId = groupId;
        }
    }
}
