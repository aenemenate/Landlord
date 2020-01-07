using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Shield : MeleeWeapon
    {
        public Shield(bool twoHanded, byte graphic = 9, double volume = 0.1, bool hollow = true, DamageType damageType = DamageType.Blunt)
                : base(graphic, volume, hollow, twoHanded, damageType)
        {
            if (twoHanded) Volume = 0.02;
        }

        public Shield() : base()
        {
        }


        // FUNCTIONS

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "shield",
                "targa",
                "buckler",
                "parma"
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
