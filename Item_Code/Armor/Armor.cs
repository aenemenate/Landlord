using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    abstract class Armor : Item
    {
        public int maxEnchantments;
        private string armorName;
        public List<ArmorEnchantment> enchantments = new List<ArmorEnchantment>();

        // CONSTRUCTORS

        public Armor(byte graphic, double volume, bool hollow, DamageType damageType = DamageType.None, ItemType type = ItemType.Armor)
                : base(graphic, type, volume, hollow, damageType)
        {
            maxEnchantments = 0;
            DetermineEnchantments();
            Rarity = DetermineRarity();
            armorName = DetermineArmorName();
            ForeColor = Physics.MaterialColors[Material];
            Identify();
        }

        public Armor(Material material, bool enchanted, byte graphic, double volume, bool hollow, DamageType damageType = DamageType.None, ItemType type = ItemType.Armor) 
            : base(graphic, type, volume, hollow, damageType)
        {
            enchantments = new List<ArmorEnchantment>();
            maxEnchantments = 0;
            if (enchanted)
                DetermineEnchantments();
            Rarity = DetermineRarity();
            armorName = DetermineArmorName();
            ForeColor = Physics.MaterialColors[Material];
            Identify();
        }

        public Armor() : base()
        {
        }


        // FUNCTIONS

        public abstract string DetermineArmorName();

        public override Material DetermineMaterial()
        {
            int rand = Program.RNG.Next(0, 1001);
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            int clothChance = 210, leatherChance = 200, boneChance = 190, copperChance = 180, brassChance = 75, bronzeChance = 65, ironChance = 40, steelChance = 25, platinumChance = 15;
#pragma warning restore CS0219 // Variable is assigned but its value is never used
            int chance = clothChance;
            if (rand <= chance)
                return Material.Cloth;
            chance += leatherChance;
            if (rand <= chance)
                return Material.Leather;
            chance += boneChance;
            if (rand <= chance)
                return Material.Bone;
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
            if (enchantments == null || enchantments.Count == 0)
            {
                if (armorName[armorName.Length - 1] == 's' & armorName != "cuirass")
                    return $"A pair of unenchanted {Name}.";
                else
                    return $"An unenchanted {Name}.";
            }
            foreach (ArmorEnchantment enchantment in enchantments)
                description += enchantment.Name + ": " + enchantment.Effect + ' ';
            return description;
        }

        public override string DetermineName(bool identifying)
        {
            if (enchantments != null && enchantments.Count > 0)
            {
                return Physics.MaterialNames[Material] + ' ' + ArmorName + " of " + enchantments[0].Name;
            }
            return Physics.MaterialNames[Material] + ' ' + ArmorName;
        }

        public Skill GetSkill()
        {
            if (Physics.GetArmorSkillMaterials( Skill.LightArmor ).Contains( this.Material ))
                return Skill.LightArmor;
            if (Physics.GetArmorSkillMaterials( Skill.HeavyArmor ).Contains( this.Material ))
                return Skill.HeavyArmor;
            return Skill.Unarmored;
        }


        public void DetermineEnchantments()
        {
            // the amount of enchantments you will find scales with the level you are at in the dungeon
            int maxEnchantments;
            
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

            for (int e = 0; e < enchantCount; e++)
                enchantments.Add(ReturnRandomEnchantment());
        }

        public ArmorEnchantment ReturnRandomEnchantment()
        {
            int armorValue = Math.Max((int)Physics.ImpactYields[Material], (int)Physics.ShearYields[Material]);
            int enchantValue = Program.RNG.Next(1, armorValue);

            DamageType damageType = DamageType.None;
            while (damageType == DamageType.None)
            {
                damageType = ReturnRandomDamageType();
                if (enchantments.Count != 0)
                    foreach (ArmorEnchantment enchant in enchantments)
                        if (damageType == enchant.DamageType)
                            damageType = DamageType.None;
            }

            ArmorEnchantment enchantment = new ArmorEnchantment(damageType, enchantValue);
            return enchantment;
        }


        // PARAMETERS
        
        public List<ArmorEnchantment> Enchantments
        {
            get { return enchantments; }
            set { enchantments = value; }
        }

        public string ArmorName
        {
            get { return armorName; }
            set { armorName = value; }
        }
    }
    
    class Helmet : Armor
    {
        public Helmet(bool instantiating, byte graphic = 252, double volume = 0.07, bool hollow = true)
                : base(graphic, volume, hollow)
        {
        }

        public Helmet(Material material, bool enchanted, byte graphic = 252, 
            double volume = 0.07, bool hollow = true) 
            : base(material, enchanted, graphic, volume, hollow)
        {

        }

        public Helmet() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.ChestPiece);
            //user.Wear(this);
            return;
        }

        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "cap", "cowl", "visage" });
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "helm", "cap", "mempo", "visage" });
            else
                names.AddRange(new List<string>() { "helmet", "helm", "warhelm", "mempo" });
            return names[Program.RNG.Next(0, names.Count)];
        }

        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Helmet.xp");
        //    }
        //}
    }

    class ChestPiece : Armor
    {
        public ChestPiece(bool instantiating, byte graphic = 240, double volume = 0.2, bool hollow = true)
                : base(graphic, volume, hollow)
        {
        }

        public ChestPiece(Material material, bool enchanted, byte graphic = 240,
            double volume = 0.2, bool hollow = true)
            : base(material, enchanted, graphic, volume, hollow)
        {
        }

        public ChestPiece() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.ChestPiece);
            //user.Wear(this);
            return;
        }

        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "tunic", "robe", "cloak" });
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "vest", "jacket", "tunic" });
            else
                names.AddRange(new List<string>() { "cuirass", "corslet" });
            return names[Program.RNG.Next(0, names.Count)];
        }

        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Chestpiece.xp");
        //    }
        //}
    }

    class Shirt : Armor
    {
        public Shirt(bool instantiating, byte graphic = 61, double volume = 0.09, bool hollow = true)
                : base(graphic, volume, hollow)
        {
        }

        public Shirt(Material material, bool enchanted, byte graphic = 61,
            double volume = 0.09, bool hollow = true)
            : base(material, enchanted, graphic, volume, hollow)
        {
        }

        public Shirt() : base()
        {
        }

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.Shirt);
            //user.Wear(this);
            return;
        }
        
        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "shirt", "doublet", "under-tunic" });
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "doublet", "under-tunic", "brigandine" });
            else
                names.AddRange(new List<string>() { "chainmail", "hauberk", "gambeson" });
            return names[Program.RNG.Next(0, names.Count)];
        }

        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Chainmail.xp");
        //    }
        //}
    }

    class Gauntlets : Armor
    {
        public Gauntlets(bool instantiating, byte graphic = 229, double volume = 0.02, bool hollow = true)
                : base(graphic, volume, hollow)
        {
        }

        public Gauntlets(Material material, bool enchanted, byte graphic = 229,
            double volume = 0.02, bool hollow = true)
            : base(material, enchanted, graphic, volume, hollow)
        {
        }

        public Gauntlets() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.ChestPiece);
            //user.Wear(this);
            return;
        }

        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "handwraps", "gloves", "grips" });
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "handguards", "gloves", "bracers" });
            else
                names.AddRange(new List<string>() { "gauntlets", "bracers", "fists" });
            return names[Program.RNG.Next(0, names.Count)];
        }
        
        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Left Gauntlet.xp");
        //    }
        //}
    }

    class Leggings : Armor
    {
        public Leggings(bool instantiating, byte graphic = 239, double volume = 0.12, bool hollow = true)
                : base(graphic, volume, hollow)
        {
        }

        public Leggings(Material material, bool enchanted, 
            byte graphic = 239, double volume = 0.12, bool hollow = true)
            : base(material, enchanted, graphic, volume, hollow)
        {
        }

        public Leggings() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.ChestPiece);
            //user.Wear(this);
            return;
        }

        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "tights", "kilt", "trousers" });
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "pants", "leggings", "trousers" });
            else
                names.AddRange(new List<string>() { "greaves", "tassets", "chausses" });
            return names[Program.RNG.Next(0, names.Count)];
        }

        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Leggings.xp");
        //    }
        //}
    }

    class Boots : Armor
    {
        public Boots(bool instantiating, byte graphic = 217, double volume = 0.07, bool hollow = true, DamageType damageType = DamageType.None)
               : base(graphic, volume, hollow, damageType)
        {
        }

        public Boots(Material material, bool enchanted,
            byte graphic = 217, double volume = 0.07, bool hollow = true)
            : base(material, enchanted, graphic, volume, hollow)
        {
        }

        public Boots() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.TakeOff(ArmorTypes.Boots);
            //user.Wear(this);
            return;
        }

        public override string DetermineArmorName()
        {
            List<string> names = new List<string>();
            if (Material == Material.Cloth)
                names.AddRange(new List<string>() { "shoes", "monks", "moccasins" } );
            else if (Material == Material.Leather)
                names.AddRange(new List<string>() { "carbatines", "galoshes", "boots" } );
            else
                names.AddRange(new List<string>() { "sabatons", "sollerets", "clogs" } );
            return names[Program.RNG.Next(0, names.Count)];
        }

        //public override void SetColorAndSplash()
        //{
        //    splash = new Splash();
        //    if (material == Materials.Copper)
        //    {
        //        color = Colors.Copper;
        //        splash.ReadFromFile("imgs/armor/Copper Boots.xp");
        //    }
        //}
    }
}
