using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class CreatureHelper
    {
        private static Random dice = new Random();
        // gets
        public static int GetWeaponSkill(this Creature creature, Item weapon)
        {
            if (weapon != null)
            {
                if (weapon is MeleeWeapon)
                {
                    if (weapon is Sword)
                        return creature.Stats.Skills[Skill.LongBlades];
                    else if (weapon is Dagger)
                        return creature.Stats.Skills[Skill.ShortBlade];
                    else if (weapon is Mace || weapon is Axe)
                        return creature.Stats.Skills[Skill.HeavyWeapons];
                    else if (weapon is Spear)
                        return creature.Stats.Skills[Skill.Spear];
                    else 
                        return creature.Stats.Skills[Skill.Brawling];
                }
                else if (weapon is RangedWeapon)
                    return creature.Stats.Skills[Skill.Marksmanship];
                return 0;
            }
            else
                return creature.Stats.Skills[Skill.Brawling];
        }
        public static Armor GetRandomArmorPiece(this Creature creature)
        {
            int rand = Program.RNG.Next(0, 5);
            switch (rand)
            {
                case (0):
                    return creature.Body.Helmet;
                case (1):
                    return (creature.Body.ChestPiece != null) ? (Armor)creature.Body.ChestPiece : (Armor)creature.Body.Shirt;
                case (2):
                    return creature.Body.Gauntlets;
                case (3):
                    return creature.Body.Leggings;
                case (4):
                    return creature.Body.Boots;
                default:
                    return creature.Body.Shirt;
            }
        }

        // checks
        public static bool CheckAttackLanded(this Creature attacker, Item weapon, Creature defender)
        {
            double attackersHitRate =
            (attacker.GetWeaponSkill(attacker.Body.MainHand) + (attacker.Stats.Attributes[Attribute.Dexterity] / 2.5) + (dice.Next(0, attacker.Stats.Attributes[Attribute.Luck]) / 10))
                * (0.75 + 0.5 * attacker.Stats.Resources[Resource.SP] / attacker.Stats.Resources[Resource.MaxSP]);

            double defendersEvasion =
                ((defender.Stats.Attributes[Attribute.Agility] / 5) + (dice.Next(0, defender.Stats.Attributes[Attribute.Luck]) / 10))
                    * (0.75 + 0.5 * defender.Stats.Resources[Resource.SP] / defender.Stats.Resources[Resource.MaxSP]);

            double maxMissChance = 150;
            double chanceToMiss = maxMissChance - attackersHitRate;
            double chanceToDodge = attackersHitRate - (attackersHitRate - defendersEvasion);

            int diceRoll = Program.RNG.Next(0, (int)maxMissChance);
            if (diceRoll <= chanceToMiss)
            {
                Program.MsgConsole.WriteLine($"{attacker.Name}'s attack missed.");
                attacker.LvlWeaponSkill(weapon, 10);
                return false;
            }

            diceRoll = dice.Next(0, (int)attackersHitRate + 2);
            if (diceRoll <= chanceToDodge)
            {
                Program.MsgConsole.WriteLine($"{defender.Name} evaded {attacker.Name}'s attack.");
                attacker.LvlWeaponSkill(weapon, 20);
                return false;
            }

            attacker.LvlWeaponSkill(weapon, 40);

            return true;
        }
        // ^-> NOTE: has side effects. Don't call unless you understand what they are.
        public static bool CheckCanCarryItem(this Creature creature, Item item)
        {
            double carrying = 0;

            if (creature.Inventory.Count != 0)
                foreach (Item i in creature.Inventory)
                    carrying += i.Weight;

            if (carrying + item.Weight <= creature.Stats.Attributes[Attribute.Strength] * 2)
                return true;
            return false;
        }
    }
}
