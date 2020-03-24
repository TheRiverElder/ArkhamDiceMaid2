using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    public class FightTask
    {
        public IList<string> investigatorNames = new List<string>();

        private int currentIndex = 0;


        public string NextInvName()
        {
            if (currentIndex >= investigatorNames.Count)
            {
                currentIndex = 0;
            }
            return investigatorNames[currentIndex++];
        }

        public bool Clear()
        {
            return true;
        }

        public bool CanStop => investigatorNames.Count == 0;
    }
}
