using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    public class DMEnv {

        public string InvName;
        public Scenario Sce { get; }

        public Investigator Inv => Sce.GetInvestigator(InvName);

        public bool ExistSce => Sce != null;
        public bool ExistInv => ExistSce && Sce.ExistInvestigator(InvName);

        public DMEnv(string invName, Scenario sce) {
            InvName = invName;
            Sce = sce;
        }
    }
}
