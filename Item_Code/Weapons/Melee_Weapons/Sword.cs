using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Sword : MeleeWeapon
    {
        public Sword(bool instantiating, byte graphic = 47, double volume = 0.004, bool hollow = false, DamageType damageType = DamageType.Shear)
                : base(graphic, volume, hollow, damageType)
        {
        }

        public Sword(Material material, string enchantment, byte graphic = 47,
            double volume = 0.004, bool hollow = false)
            : base(material, enchantment, graphic, volume, hollow)
        {

        }

        public Sword() : base()
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
                "falchion",
                "katana",
                "rapier",
                "sabre",
                "sword",
                "scimitar"
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
