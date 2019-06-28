using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum ItemType
    {
        Bottle,
        Potion,
        MeleeWeapon,
        RangedWeapon,
        BlueprintPouch,
        RecipePouch,
        Armor,
        Food,
        Log, 
        Stone,
        Coal,
        Plank,
        Wheel,
        Arrow,
        Bolt,
    }
    
    public enum Rarity
    {
        Common = 1,
        Uncommon = 3,
        Rare = 5,
        Legendary = 8
    }

    public enum DamageType
    {
        Blunt,
        Shear,
        Magic,
        Burn,
        Frost,
        Shock,
        Poison,
        None
    }
    
    abstract class Item : Block
    {
        private ItemType itemType;            // VAR itemType denotes which category of items this item belongs to.
        private Rarity rarity;              // VAR rarity indicates how valuable an item is
        private double volume;            // VAR volume stores the size of an item. It's measured in cubic feet
        private DamageType damageType;  // VAR damageType determines what kind of damage this item deals in combat (bulky items deal blunt damage and sharp ones deal shear damage).
        private bool hollow;          // VAR hollow is used to help determine the weight of an item. Hollow items wiegh less than solid ones, after all.
        private Block blockPlacedOn;

        // CONSRUCTORS
        
        public Item(byte graphic, ItemType itemType, double volume, bool hollow, DamageType damageType, BlockType type = BlockType.Item, bool solid = false, 
            bool opaque = false, bool interactive = true, bool enterable = false, string name = "")
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            ForeColor = Color.White;
            BackColor = Color.Pink;
            this.itemType = itemType;
            this.volume = volume;
            this.hollow = hollow;
            this.damageType = damageType;
            Material = DetermineMaterial();
            rarity = DetermineRarity();

            blockPlacedOn = null;
        }

        public Item() : base()
        {
            Type = BlockType.Item;
        }


        // FUNCTIONS

        public abstract string DetermineName(bool identifying);

        public abstract string DetermineDescription();

        public abstract Material DetermineMaterial();

        public abstract Rarity DetermineRarity();


        public void Identify()
        {
            Program.Identification.IdentifyItem(Name);
        }


        public string ReturnRarityString()
        {
            switch(rarity)
            {
                case (Rarity.Common):
                    return "common";
                case (Rarity.Uncommon):
                    return "uncommon";
                case (Rarity.Rare):
                    return "rare";
                case (Rarity.Legendary):
                    return "legendary";
            }
            return "";
        }

        public string ReturnIdentifiedString()
        {
            if (!Identified)
                return "n unidentified";
            else
                return "";
        }

        public Color ReturnRarityColor()
        {
            switch (rarity)
            {
                case (Rarity.Common):
                    return Color.AntiqueWhite;
                case (Rarity.Uncommon):
                    return Color.SpringGreen;
                case (Rarity.Rare):
                    return Color.DodgerBlue;
                case (Rarity.Legendary):
                    return Color.Orange;
            }
            return Color.AntiqueWhite;
        }
        
        public DamageType ReturnRandomDamageType()
        {
            int maxValue = 0;
            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
                if (damageType != DamageType.None)
                    maxValue++;

            int rand = Program.RNG.Next(0, maxValue);

            return (DamageType)rand;
        }
        

        public Tuple<byte, Color> GetComparisonArrow(Item otherItem)
        {
            const byte betterArrow = 24, worseArrow = 25, sameArrow = 45;

            if (Object.ReferenceEquals(this.GetType(), otherItem.GetType()) == false)
                return Tuple.Create((byte)0, Color.Black);
            int otherVal = 0, thisVal = 0;
            if (otherItem is Armor otherA && this is Armor selfA)
            {
                bool thisIsLightArmor = Physics.GetArmorSkillMaterials(Skill.LightArmor).Contains(selfA.Material), thisIsHeavyArmor = Physics.GetArmorSkillMaterials(Skill.HeavyArmor).Contains(selfA.Material);
                bool otherIsLightArmor = Physics.GetArmorSkillMaterials(Skill.LightArmor).Contains(otherA.Material), otherIsHeavyArmor = Physics.GetArmorSkillMaterials(Skill.HeavyArmor).Contains(otherA.Material);
                if (thisIsLightArmor && otherIsHeavyArmor || thisIsHeavyArmor && otherIsLightArmor)
                    return Tuple.Create((byte)0, Color.Black);

                otherVal = (int)(Physics.ImpactYields[otherA.Material] + Physics.ShearYields[otherA.Material]);
                thisVal = (int)(Physics.ImpactYields[selfA.Material] + Physics.ShearYields[selfA.Material]);

                foreach (ArmorEnchantment enchant in otherA.Enchantments)
                    otherVal += enchant.Effect;
                foreach (ArmorEnchantment enchant in selfA.Enchantments)
                    thisVal += enchant.Effect;
            }
            else
            {
                otherVal = (int)otherItem.Damage;
                thisVal = (int)this.Damage;

                if (otherItem is MeleeWeapon otherW)
                    foreach (WeaponEnchantment enchant in otherW.Enchantments)
                        otherVal += enchant.VictimDamage;
                if (this is MeleeWeapon selfW)
                    foreach (WeaponEnchantment enchant in selfW.Enchantments)
                        thisVal += enchant.VictimDamage;
            }
            
            return (thisVal > otherVal) ? Tuple.Create(betterArrow, Color.Green) : ((thisVal < otherVal) ? Tuple.Create(worseArrow, Color.Red) : Tuple.Create(sameArrow, Color.Gray));
        }

        // PARAMETERS

        public ItemType ItemType {
            get { return itemType; }
            set { itemType = value; }
        }

        public Rarity Rarity {
            get { return rarity; }
            set { rarity = value; }
        }

        public DamageType DamageType {
            get { return damageType; }
            set { damageType = value; }
        }

        public double Damage {
            get { return damageType == DamageType.Blunt ? Physics.ImpactYields[Material] : Physics.ShearYields[Material]; }
        }

        public string Description {
            get { return DetermineDescription(); }
        }

        new public string Name {
            get { return DetermineName(false); }
        }

        public string IDName {
            get { return DetermineName(true); }
        }

        public double Weight {
            get { return Convert.GetWeightOfItem(this); }
        }

        public double Volume {
            get { return volume; }
            set { volume = value; }
        }

        public bool Hollow {
            get { return hollow; }
            set { hollow = value; }
        }

        public bool Identified{
            get { return Program.Identification.IsThisIdentified(IDName); }
        }

        public Block BlockPlacedOn {
            get { return blockPlacedOn; }
            set { blockPlacedOn = value; }
        }
    }

}
