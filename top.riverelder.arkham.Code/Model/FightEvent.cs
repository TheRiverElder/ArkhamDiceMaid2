using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    public class FightEvent {
        public string SourceName { get; set; }
        public string TargetName { get; set; }
        public string WeaponName { get; set; }

        public FightEvent(string sourceName, string targetName, string weaponName) {
            SourceName = sourceName;
            TargetName = targetName;
            WeaponName = weaponName;
        }
    }
}
