using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Faction
    {
        private string name;
        private Dictionary<string, int> relations;
        private List<string> ideologies;

        public Faction (string name, Dictionary<string, int> relations, List<string> ideologies) {
            this.name = name;
            this.relations = relations;
            this.ideologies = ideologies;
        }

        public string Name {
            get { return name; }
            set { name = value; }
        }
        public Dictionary<string, int> Relations {
            get { return relations; }
            set { relations = value; }
        }
        public List<string> Ideologies {
            get { return ideologies; }
            set { ideologies = value; }
        }
    }
}
