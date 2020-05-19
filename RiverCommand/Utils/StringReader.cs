using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Utils {
    public class StringReader {

        //private HashSet<char> SepSet;
        //private HashSet<char> EofSet;

        public int Cursor { get; set; } = 0;

        public string Data { get; }

        //public StringReader(IEnumerable<char> sepSet, IEnumerable<char> eofSet, string data) {
        //    SepSet = new HashSet<char>(sepSet);
        //    EofSet = new HashSet<char>(eofSet);
        //    Data = data;
        //}

        public StringReader(string data, int cursor) {
            Cursor = cursor;
            Data = data;
        }

        public StringReader(string data) {
            Data = data;
        }

        public StringReader(StringReader reader) {
            Data = reader.Data;
            Cursor = reader.Cursor;
        }

        public bool HasNext => Cursor < Data.Length;

        public char Read() => Data.ElementAt(Cursor++);

        public string PeekRest() => Data.Substring(Cursor);

        public string ReadRest() {
            string s = Data.Substring(Cursor);
            Cursor = Data.Length;
            return s;
        }

        public char Peek() => Data.ElementAt(Cursor);

        public bool Read(string s) {
            int start = Cursor;
            for (int i = 0; i < s.Length; i++) {
                if (!HasNext || Data[Cursor] != s[i]) {
                    Cursor = start;
                    return false;
                }
                Cursor++;
            }
            return true;
        }

        public void Skip() => Cursor++;

        public void SkipWhiteSpace() {
            while (HasNext && char.IsWhiteSpace(Data[Cursor])) {
                Cursor++;
            }
        }

        public void SkipWhiteSpaceExcept(string chs) {
            while (HasNext && chs.IndexOf(Data[Cursor]) < 0 && char.IsWhiteSpace(Data[Cursor])) {
                Cursor++;
            }
        }

        public void SkipWhiteSpaceAnd(IEnumerable<char> chs) {
            while (HasNext && (char.IsWhiteSpace(Data[Cursor]) || chs.Contains(Data[Cursor]))) {
                Cursor++;
            }
        }

        public void Skip(string chs) {
            while (HasNext && chs.IndexOf(Data[Cursor]) >= 0) {
                Cursor++;
            }
        }

        public bool Skip(IEnumerable<char> chs) {
            while (HasNext && chs.Contains(Data[Cursor])) {
                Cursor++;
            }
            return HasNext;
        }

        public bool SkipTo(IEnumerable<char> chs) {
            while (HasNext && !chs.Contains(Data[Cursor])) {
                Cursor++;
            }
            return HasNext;
        }

        public string ReadToWhiteSpaceOr(string chs) {
            int start = Cursor;
            while (HasNext && !char.IsWhiteSpace(Data[Cursor]) && chs.IndexOf(Data[Cursor]) < 0) {
                Cursor++;
            }
            return Data.Substring(start, Cursor - start);
        }

        public string Read(IEnumerable<char> chs) {
            int start = Cursor;
            while (HasNext && chs.Contains(Data[Cursor])) {
                Cursor++;
            }
            return Data.Substring(start, Cursor - start);
        }

        public string Read(Func<char, bool> pred) {
            int start = Cursor;
            while (HasNext && pred(Data[Cursor])) {
                Cursor++;
            }
            return Data.Substring(start, Cursor - start);
        }

        public string ReadTo(IEnumerable<char> chs) {
            int start = Cursor;
            while (HasNext && !chs.Contains(Data[Cursor])) {
                Cursor++;
            }
            return Data.Substring(start, Cursor - start);
        }

        public string ReadToWhiteSpace() {
            int start = Cursor;
            while (HasNext && !char.IsWhiteSpace(Data[Cursor])) {
                Cursor++;
            }
            return Data.Substring(start, Cursor - start);
        }

        public void SkipTo(string chs) {
            while (HasNext && chs.IndexOf(Data[Cursor]) < 0) {
                Cursor++;
            }
        }

        public string ReadToEnd() {
            int start = Cursor;
            Cursor = Data.Length;
            return Data.Substring(start);
        }

        public string ReadToEndOrMaxOrEmpty(int max, string empty) {
            int len = Math.Min(Data.Length - Cursor, max);
            return len == 0 ? empty : Data.Substring(Cursor, len);
        }

        public string Slice(int index) {
            return Data.Substring(index < Cursor ? index : Cursor, Math.Abs(Cursor - index));
        }
    }
}
