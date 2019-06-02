using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Level
    {
        private int lvl;
        private Dictionary<Attribute, int> lvlProgress; // this is bumped every time a skill is leveled up
        private int maxLvlProgress;                     // The governing attribute is added and/or its value is incremented
        private int unspentAttrPoints;
        private int unspentSkillPoints;
        private int skillExpIncrement;
        private Dictionary<Skill, int> skillEXP;
        private Dictionary<Skill, int> maxSkillEXP;


        // CONSTRUCTORS //

        public Level (Dictionary<Skill, int> baseSkillLvls)
        {
            lvl = 1;
            lvlProgress = new Dictionary<Attribute, int>();
            maxLvlProgress = 10;
            unspentAttrPoints = 0;
            unspentSkillPoints = 0;
            skillExpIncrement = 10;
            InitSkillEXP(baseSkillLvls);
        }

        public Level ()
        {

        }


        // FUNCTIONS //

        public void SpendSkillPoint()
        {
            unspentSkillPoints--;
        }

        public void SpendAttrPoints()
        {
            unspentAttrPoints -= 3;
        }

        public bool LvlSkill(Skill skill, int amount, Creature c)
        {
            if (c.Class.MajorSkills.Contains( skill ))
                amount = (int)( amount * 1.5 );
            if (c.Class.MinorSkills.Contains( skill ))
                amount = (int)( amount * 1.2 );
            
            skillEXP[skill] += amount;
            if (skillEXP[skill] >= maxSkillEXP[skill])
            {
                skillEXP[skill] -= maxSkillEXP[skill];
                maxSkillEXP[skill] += skillExpIncrement;
                if (c.Class.MinorSkills.Contains( skill ) || c.Class.MajorSkills.Contains( skill ))
                {
                    Attribute governingAtt = Stats.ReturnGoverningAttribute( skill );
                    if (!lvlProgress.ContainsKey( governingAtt ))
                        lvlProgress.Add( governingAtt, 1 );
                    else
                        lvlProgress[governingAtt]++;
                }
                CheckLvlProgress( c );
                string skillName = Enum.GetName(typeof(Skill), skill);
                Program.MsgConsole.WriteLine($"{c.Name} increased their proficiency with {skillName}!");
                return true;
            }
            return false;
        }

        public void LvlUp( Creature c )
        {
            lvl++;
            unspentSkillPoints++;
            unspentAttrPoints += 3;
            Program.MsgConsole.WriteLine("You leveled up!");
            Program.CurrentState = new LevelUp( c, lvlProgress);
            lvlProgress = new Dictionary<Attribute, int>();
        }

        private void InitSkillEXP(Dictionary<Skill, int> baseSkillLvls)
        {
            skillEXP = new Dictionary<Skill, int>
            {
                { Skill.HeavyArmor, 0 },
                { Skill.Spear, 0 },
                { Skill.Forging, 0 },
                { Skill.HeavyWeapons, 0 },
                { Skill.LongBlades, 0 },
                { Skill.Block, 0 },
                { Skill.LightArmor, 0 },
                { Skill.Marksmanship, 0 },
                { Skill.Sneaking, 0 },
                { Skill.Swimming, 0 },
                { Skill.Acrobatics, 0 },
                { Skill.ShortBlade, 0 },
                { Skill.Unarmored, 0 },
                { Skill.Illusion, 0 },
                { Skill.Mercantile, 0 },
                { Skill.Speech, 0 },
                { Skill.Alchemy, 0 },
                { Skill.Conjuration, 0 },
                { Skill.Enchant, 0 },
                { Skill.Lockpick, 0 },
                { Skill.Destruction, 0 },
                { Skill.Restoration, 0 },
                { Skill.Crafting, 0 }
            };

            maxSkillEXP = new Dictionary<Skill, int>
            {
                { Skill.HeavyArmor, baseSkillLvls[Skill.HeavyArmor] * skillExpIncrement },
                { Skill.Spear, baseSkillLvls[Skill.Spear] * skillExpIncrement },
                { Skill.Forging, baseSkillLvls[Skill.Forging] * skillExpIncrement },
                { Skill.HeavyWeapons, baseSkillLvls[Skill.HeavyWeapons] * skillExpIncrement },
                { Skill.LongBlades, baseSkillLvls[Skill.LongBlades] * skillExpIncrement },
                { Skill.Block, baseSkillLvls[Skill.Block] * skillExpIncrement },
                { Skill.LightArmor, baseSkillLvls[Skill.LightArmor] * skillExpIncrement },
                { Skill.Marksmanship, baseSkillLvls[Skill.Marksmanship] * skillExpIncrement },
                { Skill.Sneaking, baseSkillLvls[Skill.Sneaking] * skillExpIncrement },
                { Skill.Swimming, baseSkillLvls[Skill.Swimming] * skillExpIncrement },
                { Skill.Acrobatics, baseSkillLvls[Skill.Acrobatics] * skillExpIncrement },
                { Skill.ShortBlade, baseSkillLvls[Skill.ShortBlade] * skillExpIncrement },
                { Skill.Unarmored, baseSkillLvls[Skill.Unarmored] * skillExpIncrement },
                { Skill.Illusion, baseSkillLvls[Skill.Illusion] * skillExpIncrement },
                { Skill.Mercantile, baseSkillLvls[Skill.Mercantile] * skillExpIncrement },
                { Skill.Speech, baseSkillLvls[Skill.Speech] * skillExpIncrement },
                { Skill.Alchemy, baseSkillLvls[Skill.Alchemy] * skillExpIncrement },
                { Skill.Conjuration, baseSkillLvls[Skill.Conjuration] * skillExpIncrement },
                { Skill.Enchant, baseSkillLvls[Skill.Enchant] * skillExpIncrement },
                { Skill.Lockpick, baseSkillLvls[Skill.Lockpick] * skillExpIncrement },
                { Skill.Destruction, baseSkillLvls[Skill.Destruction] * skillExpIncrement },
                { Skill.Restoration, baseSkillLvls[Skill.Restoration] * skillExpIncrement },
                { Skill.Crafting, baseSkillLvls[Skill.Crafting] * skillExpIncrement }
            };
        }

        private void CheckLvlProgress( Creature c )
        {
            int totalLvlProgress = 0;
            foreach (KeyValuePair<Attribute, int> valueP in lvlProgress)
                totalLvlProgress += valueP.Value;
            if (totalLvlProgress >= maxLvlProgress)
                LvlUp( c );
        }


        // PROPERTIES //

        public int Lvl
        {
            get { return lvl; }
            set { lvl = value; }
        }

        public Dictionary<Attribute, int> LvlProgress
        {
            get { return lvlProgress; }
            set { lvlProgress = value; }
        }

        public int MaxLvlProgress
        {
            get { return maxLvlProgress; }
            set { maxLvlProgress = value; }
        }

        public int UnspentAttrPoints
        {
            get { return unspentAttrPoints; }
        }

        public int UnspentSkillPoints
        {
            get { return unspentSkillPoints; }
        }

        public int SkillExpIncrement
        {
            get { return skillExpIncrement; }
            set { skillExpIncrement = value; }
        }

        public Dictionary<Skill, int> SkillEXP
        {
            get { return skillEXP; }
            set { skillEXP = value; }
        }

        public Dictionary<Skill, int> MaxSkillEXP
        {
            get { return maxSkillEXP; }
            set { maxSkillEXP = value; }
        }
    }
}
