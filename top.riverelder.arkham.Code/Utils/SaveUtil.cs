using Native.Tool.IniConfig.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Utils
{
    class SaveUtil
    {
        public static bool TryLoad(string scenarioName, out Scenario scenario)
        {
            string path = Path.Combine(Global.DataDir, scenarioName + ".json");
            if (!File.Exists(path))
            {
                scenario = null;
                return false;
            }

            string json = File.ReadAllText(path);
            scenario = JsonConvert.DeserializeObject<Scenario>(json);
            return true;
        }


        public static bool Save(Scenario scenario)
        {
            if (!Directory.Exists(Global.DataDir))
            {
                Directory.CreateDirectory(Global.DataDir);
            }
            
            string json = JsonConvert.SerializeObject(scenario, Formatting.Indented);
            File.WriteAllText(Path.Combine(Global.DataDir, scenario.Name + ".json"), json);
            return true;
        }

        public static bool LoadGlobal()
        {
            if (!File.Exists(Global.ConfFile))
            {
                return false;
            }

            try
            {
                IniObject global = IniObject.Load(Global.ConfFile, Encoding.UTF8);

                IniSection conf = global["Config"];
                if (conf.TryGetValue("Prefix", out IniValue prefix)) Global.Prefix = prefix.ToString();
                if (conf.TryGetValue("GreatSuccess", out IniValue gs)) Global.GreatSuccess = gs.ToInt32();
                if (conf.TryGetValue("GreatFailure", out IniValue gf)) Global.GreatFailure = gf.ToInt32();

                IniSection defaultValues = global["DefaultValues"];
                Global.DefaultValues.Clear();
                foreach (string key in defaultValues.Keys)
                {
                    string val = defaultValues[key].ToString();
                    Value value = Value.Of(key, val);
                    if (value != null)
                    {
                        Global.DefaultValues.Put(value);
                    }
                }

                IniSection aliases = global["Aliases"];
                foreach (string key in aliases.Keys)
                {
                    Global.DefaultValues.Set(aliases[key].ToString(), key);
                }

                IniSection groups = global["Groups"];
                Global.Groups.Clear();
                foreach (string key in groups.Keys)
                {
                    if (long.TryParse(key, out long g))
                    {
                        Global.Groups[g] = groups[key].ToString();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool SaveGlobal()
        {
            IniSection conf = new IniSection("Config")
            {
                ["Prefix"] = new IniValue(Global.Prefix),
                ["GreatSuccess"] = new IniValue(Global.GreatSuccess.ToString()),
                ["GreatFailure"] = new IniValue(Global.GreatFailure.ToString())
            };

            IniSection defaultValues = new IniSection("DefaultValues");
            foreach (string name in Global.DefaultValues.Names)
            {
                defaultValues[name] = new IniValue(Global.DefaultValues[name].ToString());
            }

            IniSection aliases = new IniSection("Aliases");
            foreach (string name in Global.DefaultValues.aliases.Keys)
            {
                aliases[name] = new IniValue(Global.DefaultValues.aliases[name]);
            }

            IniSection groups = new IniSection("Groups");
            foreach (long g in Global.Groups.Keys)
            {
                groups[g.ToString()] = new IniValue(Global.Groups[g]);
            }

            IniObject global = new IniObject
            {
                conf,
                defaultValues,
                aliases,
                groups
            };
            global.Encoding = Encoding.UTF8;

            global.Save(Global.ConfFile);
            return File.Exists(Global.ConfFile);
        }
    }
}
