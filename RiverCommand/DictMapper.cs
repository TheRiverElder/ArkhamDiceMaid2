using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {
    public class DictMapper {

        public static readonly string LinkChs = ":：=";
        public static readonly string DictSep = ";；\n";

        private ParamParser restParser = null;
        private bool acceptRest = true;
        private readonly Dictionary<string, ParamParser> parsers = new Dictionary<string, ParamParser>();
        private readonly Dictionary<string, object> markedValues = new Dictionary<string, object>();

        public DictMapper Then(string key, ParamParser parser, object markedValue) {
            parsers[key] = parser;
            markedValues[key] = markedValue;
            return this;
        }

        public DictMapper Then(string key, ParamParser parser) {
            parsers[key] = parser;
            return this;
        }

        public DictMapper Rest(ParamParser parser) {
            restParser = parser;
            return this;
        }

        public DictMapper SkipRest() {
            acceptRest = false;
            return this;
        }

        public DictMapper Mark(string key, object markedValue) {
            markedValues[key] = markedValue;
            return this;
        }

        public void Parse(StringReader reader, Args dict, out string err) {
            IList<string> errors = new List<string>();
            while (reader.HasNext) {
                if (!ParseNext(reader, dict, out string error)) {
                    errors.Add(error);
                } else {
                    break;
                }
            }
            err = string.Join("，", errors);
        }

        public bool ParseNext(StringReader reader, Args dict, out string err) {
            if (!reader.HasNext) {
                err = null;
                return true;
            }
            reader.SkipWhiteSpaceAnd(LinkChs + DictSep);
            string key = reader.ReadToWhiteSpaceOr(LinkChs + DictSep);
            if (string.IsNullOrEmpty(key)) {
                err = null;
                return true;
            }
            reader.SkipWhiteSpace();
            if (!reader.HasNext) {
                err = null;
                return true;
            }
            if (LinkChs.IndexOf(reader.Peek()) < 0) {
                if (markedValues.TryGetValue(key, out object markedValue)) {
                    dict[key] = markedValue;
                } else {
                    dict[key] = key;
                }
                err = null;
                return true;
            } else {
                reader.SkipWhiteSpaceAnd(LinkChs);
                if (parsers.TryGetValue(key, out ParamParser parser) || (parser = restParser) != null) {
                    if (parser.TryParse(reader, out object result)) {
                        dict[key] = result;
                    } else {
                        err = $"{key}:{parser.Tip}";
                        return false;
                    }
                } else {
                    string value = reader.ReadToWhiteSpaceOr(DictSep);
                    if (acceptRest) {
                        dict[key] = value;
                    }
                }
            }
            err = null;
            return true;
        }
    }
}
