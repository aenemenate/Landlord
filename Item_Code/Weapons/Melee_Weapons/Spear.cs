using System.Collections.Generic;

namespace Landlord
{

    class Spear : MeleeWeapon
    {
        public Spear(bool instantiating, byte graphic = 47, double volume = 0.015, bool hollow = false, DamageType damageType = DamageType.Shear)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Spear(Material material, string enchantment, byte graphic = 47,
            double volume = 0.15, bool hollow = false)
            : base(material, enchantment, graphic, volume, hollow)
        {

        }

        public Spear() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            //user.Wield(this);
            return;
        }

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "swordstaff",
                "pike",
                "spear",
                "halberd",
                "glaive",
                "lance"
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