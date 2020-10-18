using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    abstract class Weapon : Item
    {
        private string weaponName;
        private bool twoHanded;
        private int maxEnchantments;
        private List<WeaponEnchantment> enchantments;

        // CONSTRUCTORS
        public Weapon(byte graphic, double volume, bool hollow, bool twoHanded, DamageType damageType, ItemType type)
                : base(graphic, type, volume, hollow, damageType)
        {
            Rarity = DetermineRarity();
            ForeColor = Physics.MaterialColors[Material];
            Identify(); // this is only to be called while weapon identification hasn't been implemented

            enchantments = new List<WeaponEnchantment>();
            maxEnchantments = 1;
            DetermineEnchantments();
            weaponName = DetermineWeaponName();
            this.twoHanded = twoHanded;
        }
        public Weapon() : base()
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
            int rand = Program.RNG.Next(0, 401);

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            int copperChance = 180, brassChance = 75, bronzeChance = 65, ironChance = 40, steelChance = 25, platinumChance = 15;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            int chance = copperChance;
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
            // if (rand <= platinumChance)
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

            string particle = "A";
            switch (weaponName[0]) {
                case ('a'): case ('e'):
                case ('i'): case ('o'):
                case ('u'):
                case ('A'): case ('E'):
                case ('I'): case ('O'):
                case ('U'):
                    particle = "An";
                    break;
            }
            if (enchantments == null || enchantments.Count == 0)
                return $"{particle} {weaponName}.";
            else
            {
                description = $"An enchanted {weaponName} which ";

                foreach (WeaponEnchantment enchant in enchantments)
                    if (enchant.AppliedEffect != null)
                        description += $"has a {enchant.AppliedEffect.Chance}% chance to inflict {enchant.AppliedEffect.Name}, ";

                foreach (WeaponEnchantment enchant in enchantments)
                    if (enchant.VictimDamage != 0)
                        description += $"deals {enchant.VictimDamage} {enchant.DamageType.ToString().ToLower()} dmg, ";

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
            string adjectives = "";
            if (twoHanded) adjectives += "two-handed ";
            if (enchantments != null && enchantments.Count > 0)
                return enchantments[0].PartName + ' ' + Physics.MaterialNames[Material] + ' ' + weaponName;
            return adjectives + Physics.MaterialNames[Material] + ' ' + weaponName;
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
            while (i < enchantCount && i < maxEnchantments)
            {
                WeaponEnchantment enchantment = DataReader.GetNextWeaponEnchantment();
                if (enchantments.Find(e => e.Name == enchantment.Name) == null)
                {
                    enchantments.Add(enchantment);
                    i++;
                }
            }
        }

        // PROPERTIES
        public int MaxEnchantments
        {
            get { return maxEnchantments; }
            set { maxEnchantments = value; }
        }
        public bool TwoHanded
        {
            get { return twoHanded; }
            set { twoHanded = value; }
        }
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
