using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Class
    {
        private Dictionary<Attribute, int> attributeModifiers;
        private List<Skill> majorSkills;
        private List<Skill> minorSkills;


        // CONSTRUCTOR

        public Class()
        {
            Init();
        }

        public Class(List<Attribute> majorAtt, List<Skill> majorSkills, List<Skill> minorSkills)
        {
            Init();
            foreach (Attribute a in majorAtt)
                attributeModifiers[a] = 10;
            this.majorSkills = majorSkills;
            this.minorSkills = minorSkills;
        }

        // FUNCTIONS

        public void Init()
        {
            AttributeModifiers = new Dictionary<Attribute, int>
            {
                { Attribute.Endurance, 0},
                { Attribute.Strength, 0 },
                { Attribute.Agility, 0 },
                { Attribute.Dexterity, 0 },
                { Attribute.Personality, 0 },
                { Attribute.Intelligence, 0 },
                { Attribute.Willpower, 0 },
                { Attribute.Luck, 0 }
            };
            majorSkills = new List<Skill>();
            minorSkills = new List<Skill>();
            minorSkills.Add( Skill.Crafting );
        }


        // PARAMETERS

        public Dictionary<Attribute, int> AttributeModifiers
        {
            get { return attributeModifiers; }
            set { attributeModifiers = value; }
        }

        public List<Skill> MajorSkills
        {
            get { return majorSkills; }
            set { majorSkills = value; }
        }

        public List<Skill> MinorSkills
        {
            get { return minorSkills; }
            set { minorSkills = value; }
        }
    }
}
