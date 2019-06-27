using System.Collections.Generic;

namespace Landlord
{

    class Mace : MeleeWeapon
    {
        public Mace(bool instantiating, byte graphic = 47, double volume = 0.01, bool hollow = false, DamageType damageType = DamageType.Blunt)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Mace() : base()
        {
        }


        // FUNCTIONS

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "shovel",
                "club",
                "mace",
                "pernach",
                "maul",
                "bludgeon",
                "morning star"
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