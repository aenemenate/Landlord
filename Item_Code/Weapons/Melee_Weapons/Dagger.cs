using System.Collections.Generic;

namespace Landlord
{

    class Dagger : MeleeWeapon
    {
        public Dagger(bool instantiating, byte graphic = 45, double volume = 0.0026, bool hollow = false, DamageType damageType = DamageType.Shear)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Dagger() : base()
        {
        }


        // FUNCTIONS

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "dagger",
                "kukri",
                "kujang",
                "knife",
                "stiletto"
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