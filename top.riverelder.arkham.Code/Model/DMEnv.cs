﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    public class DMEnv {

        public long SelfId;
        public long GroupId;
        public bool IsAdmin;

        public DMEnv(long selfId, long groupId, bool isAdmin) {
            SelfId = selfId;
            GroupId = groupId;
            IsAdmin = isAdmin;
        }

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
            SaveUtil.Save(Sce);
        }

        private Scenario SceCache;
        private Investigator InvCache;

        public void RefreshCache() {
            SceCache = null;
            InvCache = null;
        }

        public Scenario Sce {
            get {
                if (SceCache != null) {
                    return SceCache;
                }
                if (!Global.Groups.TryGetValue(GroupId, out string sceName)) {
                    throw new Exception("该群还未关联存档！");
                }
                if (!Global.Scenarios.TryGetValue(sceName, out Scenario sce)) {
                    throw new Exception($"该群所关联的存档【{sceName}】还未载入！\n使用“读团”指令或者配置自动载入存档");
                }
                return SceCache = sce;
            }
        }

        public Investigator Inv {
            get {
                if (InvCache != null) {
                    return InvCache;
                }
                Scenario sce = Sce;
                if (!Sce.PlayerNames.TryGetValue(SelfId, out string invName)) {
                    throw new Exception("你还未与人物卡关联");
                }
                if (!sce.Investigators.TryGetValue(invName, out Investigator inv)) {
                    throw new Exception($"不存在名为【{invName}】的人物卡，该卡可能已经被删除！");
                }
                return InvCache = inv;
            }
        }
    }
}
