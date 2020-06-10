using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.UI.Model {
    public class Group {

        public long Id { get; }
        public string Name { get; }
        public string SceName { get; }

        public Group(long id, string name, string sceName) {
            Id = id;
            Name = name;
            SceName = sceName;
        }

        public override string ToString() => $"{Name} ({SceName} - {Id})";
    }
}
