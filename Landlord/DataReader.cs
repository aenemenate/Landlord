using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    static class DataReader
    {
        private static List<int> readTips = new List<int>();


        // FUNCTIONS //

        public static string GetNextTip()
        {
            string tip = "";

            XElement root = XElement.Load("res/data/GameTipList.xml");
            IEnumerable<XElement> tips =
                from el in root.Elements("tip")
                select el;

            List<XElement> tipList = new List<XElement>();

            foreach (XElement el in tips)
                tipList.Add(el);

            if (tipList.Count == 0)
                throw new System.Exception();

            int index = Program.RNG.Next(0, tipList.Count);
            if (readTips.Count == tipList.Count)
                readTips = new List<int>();
            while (readTips.Contains(index))
            {
                index = Program.RNG.Next(0, tipList.Count);
            }
            tip = ReadAttribute(tipList[index].Attribute("text"));
            readTips.Add(index);

            return tip;
        }
        public static List<string> GetOverworldCreatureList()
        {
            List<string> stringifiedCreatures = new List<string>();
            // load
            XElement root = XElement.Load("res/data/OverworldCreatureTypes.xml");
            IEnumerable<XElement> creatures =
                from el in root.Elements("creature")
                select el;
            //convert
            foreach (XElement c in creatures)
                stringifiedCreatures.Add(System.Convert.ToString(ReadAttribute(c.Attribute("name"))));
            return stringifiedCreatures;
        }
        public static List<string> GetPlantList()
        {
            List<string> stringifiedPlants = new List<string>();
            // load
            XElement root = XElement.Load("res/data/PlantTypes.xml");
            IEnumerable<XElement> plants =
                from el in root.Elements("plant")
                select el;
            // convert
            foreach (XElement p in plants)
                stringifiedPlants.Add(System.Convert.ToString(ReadAttribute(p.Attribute("name"))));
            return stringifiedPlants;
        }
        public static Dungeon GetNextDungeon(int preferredLvl)
        {
            Dungeon dungeon = null;
            XElement dungeonData = null;

            // load all the dungeons
            XElement root = XElement.Load("res/data/DungeonTypes.xml");
            IEnumerable<XElement> dungeons =
                from el in root.Elements("dungeon")
                select el;

            // get the correct dungeon based on lvl (need to expand this to accomodate multiple dungeons)
            foreach (XElement dun in dungeons) {
                int minLvl;
                int maxLvl;
                XElement lvl_range = dun.Element("level_range");
                minLvl = System.Convert.ToInt32(ReadAttribute(lvl_range.Attribute("min_lvl")));
                maxLvl = System.Convert.ToInt32(ReadAttribute(lvl_range.Attribute("max_lvl")));

                if (preferredLvl >= minLvl && preferredLvl <= maxLvl) {
                    dungeonData = dun;
                    break;
                }
            }

            // convert the dungeon's data into a class
            string name = "";
            int floors;
            List<string> monsters = new List<string>();

            List<string> names = new List<string>();
            foreach (XElement el in dungeonData.Element("names").Elements("name"))
                names.Add(ReadElement(el));
            name = names[Program.RNG.Next(0, names.Count)];

            Enum.TryParse(ReadAttribute(dungeonData.Element("algorithms").Element("algorithm").Attribute("name")), out DungeonType dungeonType);

            floors = System.Convert.ToInt32(ReadAttribute(dungeonData.Element("floors").FirstAttribute));

            // get the monsters
            // get permanent monster types
            string monsterNames = ReadAttribute(dungeonData.Element("monster_types").Attribute("persistent"));
            int i = 0;
            monsters.Add("");
            foreach (char c in monsterNames) {
                if (c == ' ') { i++; monsters.Add(""); }
                else
                    monsters[i] = monsters[i] + c;
            }

            // pick random unique monster types
            List<string> uniqueMonsters = new List<string>();
            int uniqueCount = System.Convert.ToInt32(ReadAttribute(dungeonData.Element("monster_types").Attribute("unique_count")));
            monsterNames = ReadAttribute(dungeonData.Element("monster_types").Attribute("unique"));
            i = 0;
            uniqueMonsters.Add("");
            foreach (char c in monsterNames) {
                if (c == ' ') { i++; uniqueMonsters.Add(""); }
                else
                    uniqueMonsters[i] = uniqueMonsters[i] + c;
            }

            int max = monsters.Count + uniqueCount;
            while (monsters.Count < max) {
                int rand = Program.RNG.Next(0, uniqueMonsters.Count);
                monsters.Add(uniqueMonsters[rand]);
                uniqueMonsters.RemoveAt(rand);
            }

            dungeon = new Dungeon(name, dungeonType, monsters, floors);

            return dungeon;
        }
        public static Monster GetMonster(string name, Block[] map, Point position, Point worldIndex, int currentFloor)
        {
            XElement monsterData = null;

            // load all the effects
            XElement root = XElement.Load( "res/data/MonsterTypes.xml" );
            IEnumerable<XElement> monsters =
                from el in root.Elements( "monster" )
                select el;

            // choose the right effect
            foreach ( XElement m in monsters )
                if ( ReadAttribute( m.FirstAttribute ) == name )
                    monsterData = m;

            if ( monsterData == null )
                return null;

            byte graphic = System.Convert.ToByte( ReadAttribute( monsterData.Attribute( "ascii_char" ) ) );
            string faction = System.Convert.ToString( ReadAttribute( monsterData.Attribute( "faction" ) ) );
            int sightDist = System.Convert.ToInt32( ReadAttribute( monsterData.Attribute( "sight_dist" ) ) );

            byte r = System.Convert.ToByte( ReadAttribute( monsterData.Element( "color" ).Attribute( "r" ) ) ),
                 g = System.Convert.ToByte( ReadAttribute( monsterData.Element( "color" ).Attribute( "g" ) ) ),
                 b = System.Convert.ToByte( ReadAttribute( monsterData.Element( "color" ).Attribute( "b" ) ) );
            Color? color = new Color( r, g, b );

            Enum.TryParse(ReadElement(monsterData.Element("diet")), out DietType diet);

            IEnumerable<XElement> majorAttData = from el in monsterData.Element( "class" ).Element( "major_attributes" ).Elements( "attribute" )
                                                 select el;
            List<Attribute> majorAtt = new List<Attribute>();
            foreach (XElement attE in majorAttData) {
                Enum.TryParse( ReadElement( attE ), out Attribute att );
                majorAtt.Add( att );
            } // translate IEnumerable to List

            IEnumerable<XElement> majorSkillsData = from el in monsterData.Element( "class" ).Element( "major_skills" ).Elements( "skill" )
                                                    select el;
            List<Skill> majorSkills = new List<Skill>();
            foreach ( XElement skillE in majorSkillsData ) {
                Enum.TryParse( ReadElement( skillE ), out Skill skill );
                majorSkills.Add( skill );
            } // translate IEnumerable to List

            IEnumerable<XElement> minorSkillsData = from el in monsterData.Element( "class" ).Element( "minor_skills" ).Elements( "skill" )
                                                    select el;
            List<Skill> minorSkills = new List<Skill>();
            foreach (XElement skillE in minorSkillsData) {
                Enum.TryParse( ReadElement( skillE ), out Skill skill );
                minorSkills.Add( skill );
            } // translate IEnumerable to List

            Class uClass = new Class( majorAtt, majorSkills, minorSkills );
            
            IEnumerable<XElement> baseDesiresData = from el in monsterData.Element( "base_desires" ).Elements( "desire_type" )
                                                    select el;
            Dictionary<DesireType, int> baseDesires = new Dictionary<DesireType, int>();
            foreach (XElement desireTypeE in baseDesiresData) {
                Enum.TryParse( ReadAttribute( desireTypeE.FirstAttribute ), out DesireType desireType );
                baseDesires.Add( desireType, System.Convert.ToInt32( ReadAttribute( desireTypeE.LastAttribute ) ) );
            } // translate IEnumerable to List

            return new Monster( map, position, worldIndex, currentFloor, color, sightDist, 3, baseDesires, uClass, name, "male", diet, faction, graphic );
        }
        public static Animal GetAnimal(string name, Block[] map, Point position, Point worldIndex, int currentFloor)
        {
            XElement creatureData = null;

            // load all the creatures
            XElement root = XElement.Load("res/data/OverworldCreatureTypes.xml");
            IEnumerable<XElement> creatures =
                from el in root.Elements("creature")
                select el;

            // choose the right creature
            foreach (XElement c in creatures)
                if (ReadAttribute(c.FirstAttribute).Equals(name))
                    creatureData = c;

            if (creatureData == null)
                return null;

            byte graphic = System.Convert.ToByte(ReadAttribute(creatureData.Attribute("ascii_char")));
            string faction = System.Convert.ToString(ReadAttribute(creatureData.Attribute("faction")));
            int sightDist = System.Convert.ToInt32(ReadAttribute(creatureData.Attribute("sight_dist")));

            byte r = System.Convert.ToByte(ReadAttribute(creatureData.Element("color").Attribute("r"))),
                 g = System.Convert.ToByte(ReadAttribute(creatureData.Element("color").Attribute("g"))),
                 b = System.Convert.ToByte(ReadAttribute(creatureData.Element("color").Attribute("b")));
            Color? color = new Color(r, g, b);

            Enum.TryParse(ReadElement(creatureData.Element("diet")), out DietType diet);

            IEnumerable<XElement> majorAttData = from el in creatureData.Element("class").Element("major_attributes").Elements("attribute")
                                                 select el;
            List<Attribute> majorAtt = new List<Attribute>();
            foreach (XElement attE in majorAttData) {
                Enum.TryParse(ReadElement(attE), out Attribute att);
                majorAtt.Add(att);
            }

            IEnumerable<XElement> majorSkillsData = from el in creatureData.Element("class").Element("major_skills").Elements("skill")
                                                    select el;
            List<Skill> majorSkills = new List<Skill>();
            foreach (XElement skillE in majorSkillsData) {
                Enum.TryParse(ReadElement(skillE), out Skill skill);
                majorSkills.Add(skill);
            }

            IEnumerable<XElement> minorSkillsData = from el in creatureData.Element("class").Element("minor_skills").Elements("skill")
                                                    select el;
            List<Skill> minorSkills = new List<Skill>();
            foreach (XElement skillE in minorSkillsData) {
                Enum.TryParse(ReadElement(skillE), out Skill skill);
                minorSkills.Add(skill);
            }

            Class uClass = new Class(majorAtt, majorSkills, minorSkills);

            IEnumerable<XElement> baseDesiresData = from el in creatureData.Element("base_desires").Elements("desire_type")
                                                    select el;
            Dictionary<DesireType, int> baseDesires = new Dictionary<DesireType, int>();
            foreach (XElement desireTypeE in baseDesiresData) {
                Enum.TryParse(ReadAttribute(desireTypeE.FirstAttribute), out DesireType desireType);
                baseDesires.Add(desireType, System.Convert.ToInt32(ReadAttribute(desireTypeE.LastAttribute)));
            }

            return new Animal(map, position, worldIndex, currentFloor, color, sightDist, 3, baseDesires, uClass, name, "male", diet, faction, graphic);
        }
        public static Plant GetPlant(string name)
        {
            XElement plantData = null;

            // load all the plants
            XElement root = XElement.Load("res/data/PlantTypes.xml");
            IEnumerable<XElement> plants =
                from el in root.Elements("plant")
                select el;

            // choose the right creature
            foreach (XElement p in plants)
                if (ReadAttribute(p.FirstAttribute).Equals(name))
                    plantData = p;

            if (plantData == null)
                return null;

            bool edible = System.Convert.ToBoolean(ReadAttribute(plantData.Element("edible").Attribute("bool")));
            int growthInterval = System.Convert.ToInt32(ReadAttribute(plantData.Attribute("growth_interval")));
            int seedRadius = System.Convert.ToInt32(ReadAttribute(plantData.Attribute("seed_radius")));

            byte r = System.Convert.ToByte(ReadAttribute(plantData.Element("fore_color").Attribute("r"))),
                 g = System.Convert.ToByte(ReadAttribute(plantData.Element("fore_color").Attribute("g"))),
                 b = System.Convert.ToByte(ReadAttribute(plantData.Element("fore_color").Attribute("b")));
            Color? foreColor = new Color(r, g, b);

            IEnumerable<XElement> growthStagesTemp =
                from el in plantData.Element("growth_stages").Elements("growth_stage")
                select el;
            List<byte> growthStages = new List<byte>();
            foreach (XElement growthStage in growthStagesTemp)
                growthStages.Add(System.Convert.ToByte(ReadAttribute(growthStage.Attribute("graphic"))));

            string requirement;
            if (ReadAttribute(plantData.Element("requirement").FirstAttribute).Equals(""))
                requirement = "";
            else {
                requirement =
                    ReadAttribute(plantData.Element("requirement").FirstAttribute)
                    + ';'
                    + ReadAttribute(plantData.Element("requirement").Attribute("type"))
                    + ';'
                    + ReadAttribute(plantData.Element("requirement").Attribute("dist"));
            }

            return new Plant(growthStages[0], name, growthInterval, seedRadius, growthStages, requirement, edible, foreColor);
        }
        public static WeaponEnchantment GetNextWeaponEnchantment()
        {
            // load all the enchantments
            XElement root = XElement.Load("res/data/WeaponEnchantTypes.xml");
            IEnumerable<XElement> enchants =
                from el in root.Elements("enchantment")
                select el;

            List<XAttribute> names = new List<XAttribute>();

            foreach (XElement e in enchants)
                names.Add(e.Attribute("name"));

            return GetWeaponEnchantment(ReadAttribute(names[Program.RNG.Next(0, names.Count)]));
        }
        public static WeaponEnchantment GetWeaponEnchantment( string enchantName )
        {
            WeaponEnchantment wEnchant = null;
            XElement wEnchantData = null;

            // load all the enchantments
            XElement root = XElement.Load("res/data/WeaponEnchantTypes.xml");
            IEnumerable<XElement> enchants =
                from el in root.Elements("enchantment")
                select el;

            // choose the right enchantment
            foreach (XElement e in enchants)
                if (ReadAttribute(e.Attribute("name")).Equals(enchantName))
                    wEnchantData = e;

            if (wEnchantData == null)
                return null;

            string name = enchantName;

            List<XElement> partNames = wEnchantData.Element("part_names").Elements().ToList();
            string partName = ReadAttribute(partNames[Program.RNG.Next(0, partNames.Count)].FirstAttribute);

            Enum.TryParse(ReadAttribute(wEnchantData.Attribute("damage_type")), out DamageType damageType);
            Effect appliedEffect = GetEffect(ReadAttribute(wEnchantData.Attribute("applied_effect")));
            int victimDamage = Program.RNG.Next(System.Convert.ToInt32(ReadAttribute(wEnchantData.Element("victim_damage").FirstAttribute)), System.Convert.ToInt32(ReadAttribute(wEnchantData.Element("victim_damage").LastAttribute)));

            wEnchant = new WeaponEnchantment(name, partName, damageType, victimDamage, appliedEffect);

            return wEnchant;
        }
        public static Effect GetNextEffect()
        {
            // load all the effects
            XElement root = XElement.Load("res/data/EffectTypes.xml");
            IEnumerable<XElement> effects =
                from el in root.Elements("effect")
                select el;

            List<XAttribute> names = new List<XAttribute>();

            foreach (XElement e in effects)
                names.Add(e.Attribute("name"));

            return GetEffect(ReadAttribute(names[Program.RNG.Next(0, names.Count)]));
        }
        public static Effect GetEffect( string effectName )
        {
            Effect effect = null;
            XElement effectData = null;

            // load all the effects
            XElement root = XElement.Load("res/data/EffectTypes.xml");
            IEnumerable<XElement> effects =
                from el in root.Elements("effect")
                select el;

            // choose the right effect
            foreach (XElement e in effects)
                if (ReadAttribute(e.FirstAttribute) == effectName)
                    effectData = e;

            if (effectData == null)
                return null;

            // load the victim damage parameters
            int victimDmgMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_damage").FirstAttribute)),
                victimDmgMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_damage").LastAttribute)),
                victimMagickaDrainMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_magicka_drain").FirstAttribute)),
                victimMagickaDrainMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_magicka_drain").LastAttribute)),
                victimStaminaDrainMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_stamina_drain").FirstAttribute)),
                victimStaminaDrainMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_stamina_drain").LastAttribute));

            int[] victimResourceDamage = new int[3];
            victimResourceDamage[0] = Program.RNG.Next(victimDmgMin, victimDmgMax + 1);
            victimResourceDamage[1] = Program.RNG.Next(victimMagickaDrainMin, victimMagickaDrainMax + 1);
            victimResourceDamage[2] = Program.RNG.Next(victimStaminaDrainMin, victimStaminaDrainMax + 1);
            
            int victimHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_healing").FirstAttribute)),
                victimHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_healing").LastAttribute)),
                victimMagickaHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_magicka_healing").FirstAttribute)),
                victimMagickaHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_magicka_healing").LastAttribute)),
                victimStaminaHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_stamina_healing").FirstAttribute)),
                victimStaminaHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("victim_stamina_healing").LastAttribute));

            int[] victimResourceHealing = new int[3];
            victimResourceHealing[0] = Program.RNG.Next(victimHealingMin, victimHealingMax + 1);
            victimResourceHealing[1] = Program.RNG.Next(victimMagickaHealingMin, victimMagickaHealingMax + 1);
            victimResourceHealing[2] = Program.RNG.Next(victimStaminaHealingMin, victimStaminaHealingMax + 1);
            
            int userHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_healing").FirstAttribute)),
                userHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_healing").LastAttribute)),
                userMagickaHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_magicka_healing").FirstAttribute)),
                userMagickaHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_magicka_healing").LastAttribute)),
                userStaminaHealingMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_stamina_healing").FirstAttribute)),
                userStaminaHealingMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("user_stamina_healing").LastAttribute));

            int[] userResourceHealing = new int[3];
            userResourceHealing[0] = Program.RNG.Next(userHealingMin, userHealingMax + 1);
            userResourceHealing[1] = Program.RNG.Next(userMagickaHealingMin, userMagickaHealingMax + 1);
            userResourceHealing[2] = Program.RNG.Next(userStaminaHealingMin, userStaminaHealingMax + 1);

            // load the damage type
            Enum.TryParse(ReadElement(effectData.Element("victim_damage_type")), out DamageType victimDamageType);

            // load the status effect
            Enum.TryParse(ReadAttribute(effectData.Element("victim_status_effect").FirstAttribute), out StatusEffect statusEffect);
            
            // load the victim healing parameters

            // load the user healing parameters

            int turnsMin = System.Convert.ToInt32(ReadAttribute(effectData.Element("turns").FirstAttribute)), 
                turnsMax = victimDmgMax = System.Convert.ToInt32(ReadAttribute(effectData.Element("turns").LastAttribute));

            int chance = System.Convert.ToInt32(ReadAttribute(effectData.Element("inflict_chance").FirstAttribute));

            int turns = Program.RNG.Next(turnsMin, turnsMax + 1);

            effect = new Effect(effectName, victimResourceDamage, victimDamageType, statusEffect, victimResourceHealing, userResourceHealing, chance, turns);

            return effect;
        }


        public static string ReadAttribute( XAttribute attribute )
        {
            string att = attribute.ToString();
            string text = "";

            bool reading = false;
            foreach (char c in att)
                if (reading == true && c != '"')
                    text += c;
                else if (c == '"')
                    reading = true;

            return text;
        }
        public static string ReadElement( XElement element )
        {
            string el = element.ToString();
            string text = "";

            bool reading = false;
            foreach (char c in el)
                if (reading == true) {
                    if (c != '<')
                        text += c;
                    else
                        reading = false;
                }
                else if (c == '>')
                    reading = true;

            return text;
        }
    }
}
