using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Bow : RangedWeapon
    {
        public Bow(bool instantiating, byte graphic = 40, double volume = 0.003, bool hollow = false, DamageType damageType = DamageType.Blunt)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Bow() : base()
        {
        }


        // FUNCTIONS

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "bow",
                "war bow",
                "short bow"
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
