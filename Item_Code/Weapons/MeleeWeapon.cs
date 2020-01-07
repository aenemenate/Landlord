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

        public override Skill GetWeaponSkill()
        {
            if (this is Axe || this is Mace)
                return Skill.HeavyWeapons;
            else if (this is Dagger)
                return Skill.ShortBlade;
            else if (this is Shield)
                return Skill.Block;
            else if (this is Spear)
                return Skill.Spear;
            else if (this is Sword)
                return Skill.LongBlades;
            else
                return Skill.Brawling;
        }
    }
}
