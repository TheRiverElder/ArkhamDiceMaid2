using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Utils {
    public class StringReader {

        public int Cursor { get; set; } = 0;

        public string Data { get; }

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

        public void SkipWhiteSpaceAnd(string chs) {
            while (HasNext && (char.IsWhiteSpace(Data[Cursor]) || chs.IndexOf(Data[Cursor]) >= 0)) {
                Cursor++;
            }
        }

        public void Skip(string chs) {
            while (HasNext && chs.IndexOf(Data[Cursor]) >= 0) {
                Cursor++;
            }
        }

        public string ReadToWhiteSpaceOr(string chs) {
            int start = Cursor;
            while (HasNext && !char.IsWhiteSpace(Data[Cursor]) && chs.IndexOf(Data[Cursor]) < 0) {
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
    }
}
