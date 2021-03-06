﻿using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.Parsing {
    public class DictMapper<TEnv> {

        private ParamParser<TEnv> RestParser = null;
        private bool AcceptRest => RestParser != null;
        private readonly Dictionary<string, ParamParser<TEnv>> Parsers = new Dictionary<string, ParamParser<TEnv>>();
        //private readonly Dictionary<string, object> markedValues = new Dictionary<string, object>();

        //public DictMapper Then(string key, ParamParser parser, object markedValue) {
        //    parsers[key] = parser;
        //    markedValues[key] = markedValue;
        //    return this;
        //}

        public DictMapper<TEnv> Then(string key, ParamParser<TEnv> parser) {
            Parsers[key] = parser;
            return this;
        }

        public DictMapper<TEnv> Rest(ParamParser<TEnv> parser) {
            RestParser = parser;
            return this;
        }

        //public DictMapper Mark(string key, object markedValue) {
        //    markedValues[key] = markedValue;
        //    return this;
        //}

        public bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, Args dict, out string err) {
            while (SkipSep(reader)) {
                if (!ParseNext(dispatcher, env, reader, dict, out err)) {
                    return false;
                }
            }
            SkipSep(reader);
            err = null;
            return true;
        }

        // 识别映射参数的分隔符，并确保在读取后依然有内容
        private bool SkipSep(StringReader reader) {
            bool hasSep = false;
            while (!ArgUtil.IsCommandEnd(reader) && Config.DictSeps.Contains(reader.Peek())) {
                reader.Skip(Config.DictSeps);
                hasSep = true;
            }
            return hasSep && !ArgUtil.IsCommandEnd(reader);
        }

        public bool ParseNext(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, Args dict, out string err) {
            // 识别映射参数名
            string key = reader.Read(ArgUtil.IsNameChar);
            if (string.IsNullOrEmpty(key)) {
                err = "未解析到参数名";
                return false;
            }
            // 获取值解析器
            if (!Parsers.TryGetValue(key, out ParamParser<TEnv> parser)) {
                if (AcceptRest) {
                    parser = RestParser;
                } else {
                    err = $"该指令不接收参数[{key}]";
                    return false;
                }
            }

            // 识别映射参数名与值的连接符，并确保在读取后依然有内容
            if (!(reader.Skip(Config.ListSeps) &&
                Config.Linkers.Contains(reader.Peek()) &&
                reader.Skip(Config.Linkers) &&
                reader.Skip(Config.ListSeps))
            ) {
                //// 如果没有检测到连接符，则按照标记参数处理
                //if (markedValues.TryGetValue(key, out object value)) {
                //    dict[key] = value;
                //    err = null;
                //    return true;
                //}
                // 生成错误信息
                err = $"未接收到参数[{key}]值的内容";
                return false;
            }


            if (parser.TryParse(dispatcher, env, reader, out object result)) {
                dict[key] = result;
            } else {
                err = $"映射参数[{key}]应为{parser.Tip}，却得到：{reader.ReadToEndOrMaxOrEmpty(Config.MaxCut, Config.EmptyStrTip)}";
                return false;
            }

            err = null;
            return true;
        }
    }
}
