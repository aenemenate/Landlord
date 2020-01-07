using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    abstract class RangedWeapon : Weapon
    {
        public RangedWeapon(byte graphic, double volume, bool hollow, DamageType damageType, ItemType type = ItemType.MeleeWeapon)
                : base(graphic, volume, hollow, damageType, type)
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
