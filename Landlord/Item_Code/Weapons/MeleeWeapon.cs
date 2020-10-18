using System;
using System.Collections.Generic;
using System.Linq;

namespace Landlord
{
    abstract class MeleeWeapon : Weapon
    {
        public MeleeWeapon(byte graphic, double volume, bool hollow, bool twoHanded, DamageType damageType, ItemType type = ItemType.MeleeWeapon)
                : base(graphic, volume, hollow, twoHanded, damageType, type)
        {
        }

        public MeleeWeapon() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            return;
        }
    }
}
