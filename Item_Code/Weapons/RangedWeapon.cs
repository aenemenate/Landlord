using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    abstract class RangedWeapon : Weapon
    {
        public RangedWeapon(byte graphic, double volume, bool hollow, bool twoHanded, DamageType damageType, ItemType type = ItemType.RangedWeapon)
                : base(graphic, volume, hollow, twoHanded, damageType, type)
        {
        }

        public RangedWeapon() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            return;
        }

        public override Skill GetWeaponSkill()
        {
                return Skill.Marksmanship;
        }
    }
}
