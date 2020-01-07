using System.Collections.Generic;

namespace Landlord
{

    class Spear : MeleeWeapon
    {
        public Spear(bool twoHanded, byte graphic = 47, double volume = 0.015, bool hollow = false, DamageType damageType = DamageType.Shear)
                : base(graphic, volume, hollow, twoHanded, damageType)
        {
        }

        public Spear() : base()
        {
        }


        // FUNCTIONS

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