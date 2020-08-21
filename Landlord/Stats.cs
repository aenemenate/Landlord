using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum Skill
    {
        HeavyArmor,
        Spear,
        Block,
        Brawling,
        Forging,
        HeavyWeapons,
        LongBlades,
        LightArmor,
        Marksmanship,
        Sneaking,
        Acrobatics,
        Swimming,
        ShortBlade,
        Unarmored,
        Illusion,
        Mercantile,
        Speech,
        Alchemy,
        Conjuration,
        Enchant,
        Lockpick,
        Destruction,
        Restoration,
        Crafting
    }

    public enum Attribute
    {
        Endurance,
        Strength,
        Personality,
        Agility,
        Dexterity,
        Intelligence,
        Willpower,
        Luck
    }

    public enum Resource
    {
        HP,
        HV,
        MP,
        SP,
        MaxHP,
        MaxHV,
        MaxMP,
        MaxSP
    }



    class Stats
    {
        private Dictionary<Attribute, int> attributes;
        private Dictionary<Resource, int> resources;
        private Dictionary<Skill, int> skills;
        private Level level;


        // CONSTRUCTORS

        public Stats(Class uclass)
        {
            SetAttributes(uclass);
            SetResources();
            SetSkills(uclass);
            level = new Level(skills);
        }

        public Stats ()
        {

        }


        // FUNCTIONS

        private void SetAttributes(Class uclass)
        {
            attributes = new Dictionary<Attribute, int>
            {
                { Attribute.Endurance, 40 + uclass.AttributeModifiers[Attribute.Endurance]},
                { Attribute.Strength, 40 + uclass.AttributeModifiers[Attribute.Strength] },
                { Attribute.Agility, 40 + uclass.AttributeModifiers[Attribute.Agility] },
                { Attribute.Dexterity, 40 + uclass.AttributeModifiers[Attribute.Dexterity]},
                { Attribute.Intelligence, 40 + uclass.AttributeModifiers[Attribute.Intelligence] },
                { Attribute.Willpower, 40 + uclass.AttributeModifiers[Attribute.Willpower] },
                { Attribute.Personality, 40 + uclass.AttributeModifiers[Attribute.Personality] },
                { Attribute.Luck, 40 + uclass.AttributeModifiers[Attribute.Luck] }
            };
        }

        private void SetResources()
        {
            int hp = (attributes[Attribute.Strength] + attributes[Attribute.Endurance]) / 2;
            int mp = attributes[Attribute.Intelligence];
            int sp = attributes[Attribute.Strength] + attributes[Attribute.Willpower] + attributes[Attribute.Agility] + attributes[Attribute.Endurance];
            resources = new Dictionary<Resource, int>
            {
                { Resource.HP, hp },
                { Resource.MaxHP, hp },
                { Resource.HV, sp + hp },
                { Resource.MaxHV, sp + hp },
                { Resource.MP, mp },
                { Resource.MaxMP, mp },
                { Resource.SP, sp },
                { Resource.MaxSP, sp }
            };
        }

        private void SetSkills(Class uclass)
        {
            skills = new Dictionary<Skill, int>
            {
                { Skill.HeavyArmor, 5 },
                { Skill.Spear, 5 },
                { Skill.Forging, 5 },
                { Skill.HeavyWeapons, 5 },
                { Skill.LongBlades, 5 },
                { Skill.Block, 5 },
                { Skill.LightArmor, 5 },
                { Skill.Marksmanship, 5 },
                { Skill.Sneaking, 5 },
                { Skill.Swimming, 5 },
                { Skill.Acrobatics, 5 },
                { Skill.Brawling, 5 },
                { Skill.ShortBlade, 5 },
                { Skill.Unarmored, 5 },
                { Skill.Illusion, 5 },
                { Skill.Mercantile, 5 },
                { Skill.Speech, 5 },
                { Skill.Alchemy, 5 },
                { Skill.Conjuration, 5 },
                { Skill.Enchant, 5 },
                { Skill.Lockpick, 5 },
                { Skill.Destruction, 5 },
                { Skill.Restoration, 5 },
                { Skill.Crafting, 5 }
            };
            foreach (Skill skill in uclass.MajorSkills)
                skills[skill] += 25;
            foreach (Skill skill in uclass.MinorSkills)
                skills[skill] += 10;
        }

        public void LvlSkill(Skill skill, int amount, Creature c)
        {
            bool lvldSkill = Level.LvlSkill(skill, amount, c);

            if (lvldSkill)
                skills[skill]++;
        }

        public static Attribute ReturnGoverningAttribute(Skill skill)
        {
            switch (skill)
            {
                case (Skill.HeavyArmor):
                case (Skill.Spear):
                case (Skill.Block):
                case ( Skill.Swimming ):
                    return Attribute.Endurance;
                case (Skill.HeavyWeapons):
                case (Skill.Brawling):
                case (Skill.Acrobatics):
                case (Skill.Forging):
                    return Attribute.Strength;
                case (Skill.LightArmor):
                case (Skill.Unarmored):
                case (Skill.ShortBlade):
                case (Skill.Sneaking):
                    return Attribute.Agility;
                case (Skill.Marksmanship):
                case (Skill.Lockpick):
                case (Skill.LongBlades):
                case (Skill.Crafting):
                    return Attribute.Dexterity;
                case (Skill.Illusion):
                case (Skill.Alchemy):
                case (Skill.Enchant):
                    return Attribute.Intelligence;
                case (Skill.Conjuration):
                case (Skill.Restoration):
                case (Skill.Destruction):
                    return Attribute.Willpower;
                case (Skill.Mercantile):
                case (Skill.Speech):
                    return Attribute.Personality;
                default:
                    return Attribute.Luck;
            }
        }

        public Resource ReturnRandomResource()
        {
            Random rng = new Random();
            int rand = rng.Next(0, 101);
            if (rand < 33)
                return Resource.HP;
            if (rand < 66)
                return Resource.MP;
            else
                return Resource.SP;
        }

        public Color ReturnResourceColor(Resource resource)
        {
            switch (resource)
            {
                case (Resource.HP):
                    return Color.Red;
                case (Resource.MP):
                    return Color.RoyalBlue;
                default: // Resource.SP
                    return Color.Yellow;
            }
        }

        public string ReturnResourceName(Resource resource)
        {
            switch (resource)
            {
                case (Resource.HP):
                    return "health";
                case (Resource.MP):
                    return "magicka";
                default: // Resource.SP
                    return "stamina";
            }
        }

        public Resource ReturnResourceMax(Resource resource)
        {
            switch (resource)
            {
                case (Resource.HP):
                    return Resource.MaxHP;
                case (Resource.MP):
                    return Resource.MaxMP;
                case (Resource.HV):
                    return Resource.MaxHV;
                default: // Resource.SP
                    return Resource.MaxSP;
            }
        }


        // PARAMETERS

        public Dictionary<Attribute, int> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public Dictionary<Skill, int> Skills
        {
            get { return skills; }
            set { skills = value; }
        }

        public Dictionary<Resource, int> Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public Level Level
        {
            get { return level; }
            set { level = value; }
        }
    }
}
