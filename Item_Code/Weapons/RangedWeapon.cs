using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    abstract class RangedWeapon : Item
    {
        public int maxEnchantments;
        private string weaponName;
        public List<WeaponEnchantment> enchantments;

        // CONSTRUCTORS

        public RangedWeapon(byte graphic, double volume, bool hollow, DamageType damageType, ItemType type = ItemType.MeleeWeapon)
                : base(graphic, type, volume, hollow, damageType)
        {
            enchantments = new List<WeaponEnchantment>();
            maxEnchantments = 0;
            DetermineEnchantments();
            Rarity = DetermineRarity();
            weaponName = DetermineWeaponName();

            ForeColor = Physics.MaterialColors[Material]; // Color

            Identify(); // this is only to be called while weapon identification hasn't been implemented
        }

        public RangedWeapon() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            return;
        }

        public abstract string DetermineWeaponName();

        public override Material DetermineMaterial()
        {
        #pragma warning disable CS0219 // Variable is assigned but its value is never used
            int woodChance = 200, copperChance = 50, brassChance = 25, bronzeChance = 20, ironChance = 10, steelChance = 5, platinumChance = 2;
        #pragma warning restore CS0219 // Variable is assigned but its value is never used
            int rand = Program.RNG.Next(0, woodChance + copperChance + brassChance + bronzeChance + ironChance + steelChance + platinumChance + 1);


            int chance = woodChance;
            if (rand <= chance)
                return Material.Wood;
            chance += copperChance;
            if (rand <= chance)
                return Material.Copper;
            chance += brassChance;
            if (rand <= chance)
                return Material.Brass;
            chance += bronzeChance;
            if (rand <= chance)
                return Material.Bronze;
            chance += ironChance;
            if (rand <= chance)
                return Material.Iron;
            chance += steelChance;
            if (rand <= chance)
                return Material.Steel;
            return Material.Platinum;
        }

        public override Rarity DetermineRarity()
        {
            if (enchantments == null || enchantments.Count == 0)
                return Rarity.Common;
            if (enchantments.Count == 1)
                return Rarity.Uncommon;
            else if (enchantments.Count == 2 || enchantments.Count == 3)
                return Rarity.Rare;
            else if (enchantments.Count == 4)
                return Rarity.Legendary;
            return Rarity.Common;
        }

        public override string DetermineDescription()
        {
            string description = "";

            if (enchantments == null || enchantments.Count == 0)
                return $"An unenchanted {weaponName}.";
            else
            {
                description = $"An enchanted {weaponName} which ";

                foreach (WeaponEnchantment enchant in enchantments)
                    if (enchant.AppliedEffect != null)
                        description += $"has a {enchant.AppliedEffect.Chance}% chance to inflict {enchant.AppliedEffect.Name}, ";

                foreach (WeaponEnchantment enchant in enchantments)
                    description += $"deals {enchant.VictimDamage} {enchant.DamageType.ToString().ToLower()} damage on hit, ";

                description = description.Substring(0, description.LastIndexOf(',')) + '.';

                if (description.Count(c => c == ',') > 0)
                {
                    int commaIndex = description.LastIndexOf(',');
                    description = description.Substring(0, commaIndex) + " and " + description.Substring(commaIndex + 2, description.Length - (commaIndex + 2));
                }

                return description;
            }


        }

        public override string DetermineName(bool identifying)
        {
            if (enchantments != null && enchantments.Count > 0)
            {
                return enchantments[0].PartName + ' ' + Physics.MaterialNames[Material] + ' ' + weaponName;
            }
            return Physics.MaterialNames[Material] + ' ' + weaponName;
        }

        public void DetermineEnchantments()
        {
            int rand = Program.RNG.Next(0, 101);
            if (rand <= 66)         // 66% chance
                maxEnchantments = 0;
            else if (rand <= 81)    // 15% chance
                maxEnchantments = 1;
            else if (rand <= 90)    //  9% chance
                maxEnchantments = 2;
            else if (rand <= 96)    //  6% chance
                maxEnchantments = 3;
            else                    //  4% chance
                maxEnchantments = 4;

            int enchantCount = 0;

            rand = Program.RNG.Next(0, 101);
            if (rand <= 78 && maxEnchantments >= 0)      // 78% chance
                enchantCount = 0;
            else if (rand <= 90 && maxEnchantments >= 1) // 12% chance
                enchantCount = 1;
            else if (rand <= 96 && maxEnchantments >= 2) //  6% chance
                enchantCount = 2;
            else if (rand <= 99 && maxEnchantments >= 3) //  3% chance
                enchantCount = 3;
            else                                         //  1% chance
                enchantCount = maxEnchantments;

            int i = 0;
            while (i < enchantCount && i < maxEnchantments) {
                WeaponEnchantment enchantment = DataReader.GetNextWeaponEnchantment();
                if (enchantments.Find(e => e.Name == enchantment.Name) == null) {
                    enchantments.Add(enchantment);
                    i++;
                }
            }
        }

        public Skill GetWeaponSkill()
        {
                return Skill.Marksmanship;
        }


        // PROPERTIES

        public List<WeaponEnchantment> Enchantments
        {
            get { return enchantments; }
            set { enchantments = value; }
        }

        public string WeaponName
        {
            get { return weaponName; }
            set { weaponName = value; }
        }
    }
}
