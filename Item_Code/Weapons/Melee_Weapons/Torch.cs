using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Torch : MeleeWeapon
    {
        public Torch(bool twoHanded, byte graphic = 47, double volume = 0.005, bool hollow = false, DamageType damageType = DamageType.Burn)
                : base(graphic, volume, hollow, twoHanded, damageType)
        {
            if (twoHanded) Volume = 0.009;
            this.Material = Material.Wood;
        }

        public Torch() : base()
        {
        }


        // FUNCTIONS

        public override string DetermineWeaponName()
        {
            List<string> names = new List<string>()
            {
                "torch"
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
