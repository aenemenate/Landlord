using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Shield : MeleeWeapon
    {
        public Shield(bool instantiating, byte graphic = 9, double volume = 0.125, bool hollow = true, DamageType damageType = DamageType.Blunt)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Shield(Material material, string enchantment, byte graphic = 9,
            double volume = 0.125, bool hollow = false)
            : base(material, enchantment, graphic, volume, hollow)
        {

        }

        public Shield() : base()
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
