﻿using Native.Tool.IniConfig.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Utils {
    class SaveUtil {
        public static bool TryLoad(string scenarioName, out Scenario scenario) {
            string path = Path.Combine(Global.DataDir, scenarioName + ".json");
            if (!File.Exists(path)) {
                scenario = null;
                return false;
            }

            string json = File.ReadAllText(path);
            Global.Scenarios[scenarioName] = scenario = JsonConvert.DeserializeObject<Scenario>(json, options);
            return true;
        }

        private static readonly JsonSerializerSettings options = new JsonSerializerSettings();
        static SaveUtil() {
            options.Converters.Add(new ValueConverter());
        }

        public static HashSet<char> InvalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
        public static bool Save(Scenario scenario) {
            if (!Directory.Exists(Global.DataDir)) {
                Directory.CreateDirectory(Global.DataDir);
            }
            string fileName = scenario.Name;
            foreach (char c in InvalidFileNameChars) {
                fileName = fileName.Replace(c, '-');
            }
            string json = JsonConvert.SerializeObject(scenario, Formatting.Indented, options);
            File.WriteAllText(Path.Combine(Global.DataDir, fileName + ".json"), json);
            return true;
        }

        public static bool LoadGlobal() {
            if (!File.Exists(Global.ConfFile)) {
                return false;
            }

            try {
                IniObject global = IniObject.Load(Global.ConfFile, Encoding.UTF8);

                IniSection conf = global["Config"];
                if (conf.TryGetValue(nameof(Global.Sleep), out IniValue sleep)) Global.Sleep = sleep.ToBoolean();
                if (conf.TryGetValue(nameof(Global.Prefix), out IniValue prefix)) Global.Prefix = prefix.ToString();
                if (conf.TryGetValue(nameof(Global.DoAt), out IniValue doAt)) Global.DoAt = doAt.ToBoolean();
                if (conf.TryGetValue(nameof(Global.AutoLoad), out IniValue autoLoad)) Global.DoAt = doAt.ToBoolean();
                if (conf.TryGetValue(nameof(Global.GreatSuccess), out IniValue gs)) Global.GreatSuccess = gs.ToInt32();
                if (conf.TryGetValue(nameof(Global.GreatFailure), out IniValue gf)) Global.GreatFailure = gf.ToInt32();
                if (conf.TryGetValue(nameof(Global.AllowedLead), out IniValue al)) Global.AllowedLead = al.ToBoolean();
                if (conf.TryGetValue(nameof(Global.Lead), out IniValue lead)) Global.Lead = lead.ToInt32();

                IniSection defaultValues = global["DefaultValues"];
                Global.DefaultValues.Clear();
                foreach (string key in defaultValues.Keys) {
                    string val = defaultValues[key].ToString();
                    Value value = Value.Of(val);
                    if (value != null) {
                        Global.DefaultValues.Put(key, value);
                    }
                }

                IniSection aliases = global["Aliases"];
                foreach (string key in aliases.Keys) {
                    Global.DefaultValues.SetAlias(key, aliases[key].ToString());
                }

                IniSection groups = global["Groups"];
                Global.Groups.Clear();
                foreach (string key in groups.Keys) {
                    if (long.TryParse(key, out long g)) {
                        Global.Groups[g] = groups[key].ToString();
                    }
                }
                
                IniSection users = global["Users"];
                Global.Users.Clear();
                foreach (string key in users.Keys) {
                    if (long.TryParse(key, out long u)) {
                        Global.Users[u] = users[key].ToInt64();
                    }
                }
            } catch (Exception e) {
                Global.Log(e.Message);
                return false;
            }
            return true;
        }

        public static bool SaveGlobal() {
            IniSection conf = new IniSection("Config") {
                [nameof(Global.Sleep)] = new IniValue(Global.Sleep),
                [nameof(Global.Prefix)] = new IniValue(Global.Prefix),
                [nameof(Global.AutoLoad)] = new IniValue(Global.AutoLoad),
                [nameof(Global.DoAt)] = new IniValue(Global.DoAt),
                [nameof(Global.GreatSuccess)] = new IniValue(Global.GreatSuccess),
                [nameof(Global.GreatFailure)] = new IniValue(Global.GreatFailure),
                [nameof(Global.AllowedLead)] = new IniValue(Global.AllowedLead),
                [nameof(Global.Lead)] = new IniValue(Global.Lead),
            };

            IniSection defaultValues = new IniSection("DefaultValues");
            foreach (string name in Global.DefaultValues.Names) {
                defaultValues[name] = new IniValue(Global.DefaultValues[name].ToString());
            }

            IniSection aliases = new IniSection("Aliases");
            foreach (string name in Global.DefaultValues.aliases.Keys) {
                aliases[name] = new IniValue(Global.DefaultValues.aliases[name]);
            }

            IniSection groups = new IniSection("Groups");
            foreach (long g in Global.Groups.Keys) {
                groups[g.ToString()] = new IniValue(Global.Groups[g]);
            }

            IniSection users = new IniSection("Users");
            foreach (long u in Global.Users.Keys) {
                users[u.ToString()] = new IniValue(Global.Users[u]);
            }

            IniObject global = new IniObject
            {
                conf,
                defaultValues,
                aliases,
                groups,
                users,
            };
            global.Encoding = Encoding.UTF8;

            global.Save(Global.ConfFile);
            return File.Exists(Global.ConfFile);
        }
    }
}
