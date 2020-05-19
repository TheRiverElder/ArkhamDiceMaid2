using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    public class Horse {
        public static Random Rand = new Random();

        public static int TotalProgress = 16;
        public static int MaxHealth = 30;
        
        public int Health = MaxHealth;
        public int Stuck = 0;
        public int Ponential = Rand.Next(20, 80);
        public int Progress = 0;
        public Dictionary<string, int> Bets = new Dictionary<string, int>();
        public HashSet<string> Sources = new HashSet<string>();

        public bool Step() {
            Sources.Clear();
            if (Progress >= TotalProgress) {
                return true;
            } else if (!Alive) {
                return false;
            }

            if (Stuck > 0) {
                Stuck--;
                return false;
            }

            if (10 + Rand.Next(10) > Health) {
                Stuck = 1 + Rand.Next(2);
                return false;
            }

            Progress++;
            if (Rand.Next(100) <= Ponential) {
                Progress++;
            }

            return Progress >= TotalProgress;
        }

        public bool Alive => Health > 0;
        public bool HitEnd => Progress >= TotalProgress;

        public string Display() {
            StringBuilder sb = new StringBuilder();
            // 状态
            string status;
            if (!Alive) {
                status = "死亡";
            } else {
                status = Stuck > 0 ? "受阻" : "存活";
            }
            sb.AppendLine($"生命：{Health}/{MaxHealth}，状态:{status}");
            // 生命
            int minlayer = Health / 10;
            int top = Health - minlayer * 10;
            for (int i = 0; i < 10; i++) {
                switch (i < top ? minlayer + 1 : minlayer) {
                    case 0: sb.Append("🖤"); break;
                    case 1: sb.Append("❤️"); break;
                    case 2: sb.Append("💛"); break;
                    case 3: sb.Append("💚"); break;
                    default: sb.Append("💔"); break;
                }
            }
            sb.AppendLine();
            // 跑道
            sb.Append("🚩");
            string icon = Alive ? "🏇" : "💀";
            for (int i = TotalProgress - 1; i >= 0; i--) {
                if (Alive && i == Progress - 1) {
                    sb.Append("💨");
                } else {
                    sb.Append(i == Progress ? icon : "_");
                }
            }
            sb.Append("|");
            return sb.ToString();
        }
    }
}
