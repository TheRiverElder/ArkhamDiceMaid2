using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Bot
{
    public class CmdEnv
    {
        public long User { get; }
        public long Group { get; }
        public string ScenarioName { get; }

        public Scenario Scenario
        {
            get => Global.Scenarios[ScenarioName];
            set => Global.Scenarios[ScenarioName] = value;
        }

        public bool ScenarioExist => Global.Scenarios.ContainsKey(ScenarioName);

        public bool InvestigatorExist => ScenarioExist && Scenario.ExistInvestigator(User);

        public Investigator Investigator
        {
            get => Scenario.GetInvestigator(User);
            set
            {
                Scenario.PutInvestigator(value);
                Scenario.Control(User, value.Name);
            }
        }

        public CmdEnv(long user, long group, string scenarioName)
        {
            User = user;
            Group = group;
            ScenarioName = scenarioName;
        }
    }
}
