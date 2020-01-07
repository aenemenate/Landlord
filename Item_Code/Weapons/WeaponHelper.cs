using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class WeaponHelper
    {
        public static int GetWeaponCost(this Item weapon, Creature c)
        {
            int returnvalue;
            int multiplier = 1;
            if (weapon is Weapon w && w.TwoHanded) multiplier = 2;
            if (weapon != null) {
                if (weapon is MeleeWeapon) {
                    if (weapon is Sword)
                        returnvalue = 8 - (c.GetWeaponSkill(weapon) / 25);
                    else if (weapon is Dagger)
                        returnvalue = 6 - (c.GetWeaponSkill(weapon) / 33);
                    else if (weapon is Mace)
                        returnvalue = 16 - (c.GetWeaponSkill(weapon) / 12);
                    else if (weapon is Axe)
                        returnvalue = 16 - (c.GetWeaponSkill(weapon) / 12);
                    else if (weapon is Spear)
                        returnvalue = 8 - (c.GetWeaponSkill(weapon) / 25);
                    else returnvalue = -1;
                }
                else
                    returnvalue = 8 - (c.GetWeaponSkill(weapon) / 25);
            }
            else
                returnvalue = 4 - (c.Stats.Skills[Skill.Brawling] / 25);
            return returnvalue * multiplier;
        }

        public static int GetWepDmg(this Item weapon, Creature c)
        {
            int damage;
            // determine the damage value
            if (weapon != null) {
                // minimum skill deals half dmg, max skill deals full damage
                damage = (int)(weapon.Damage * (.5 + (double)c.GetWeaponSkill(weapon) / 200));
            }
            else // the damage type is bone, for your fist
                damage = (int)(Physics.ShearYields[Material.Bone] * (.5 + (double)c.Stats.Skills[Skill.Brawling] / 200));
            return damage;

        }

        public static DamageType GetWepDmgType(this Item weapon)
        {
            DamageType dmgType = DamageType.Blunt;
            if (weapon != null)
                dmgType = weapon.DamageType;

            return dmgType;
        }
    }
}
