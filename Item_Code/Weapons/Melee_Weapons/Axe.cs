using System.Collections.Generic;

namespace Landlord
{
    class Axe : MeleeWeapon
    {
        public Axe(bool instantiating, byte graphic = 47, double volume = 0.0125, bool hollow = false, DamageType damageType = DamageType.Shear)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Axe(Material material, string enchantment, byte graphic = 47,
            double volume = 0.0125, bool hollow = false)
            : base(material, enchantment, graphic, volume, hollow)
        {
        }

        public Axe() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            return;
        }

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "axe",
                "waraxe",
                "woodcutting axe",
                "pickaxe",
                "battle-axe"
            };
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
}