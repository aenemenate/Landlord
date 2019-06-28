using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord.Item_Code.Projectiles
{
    class Arrow : Item
    {
        public Arrow(bool instantiating, byte graphic = 94, ItemType type = ItemType.Arrow, double volume = 0.0005, bool hollow = true, DamageType damageType = DamageType.Shear)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.AntiqueWhite;
            Identify();
        }

        public Arrow() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return $"An arrow with a tip fashioned of {Material}.";
        }

        public override string DetermineName(bool identifying)
        {
            return Physics.MaterialNames[Material] + " arrow";
        }

        public override Material DetermineMaterial()
        {
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            int copperChance = 200, brassChance = 50, bronzeChance = 40, ironChance = 25, steelChance = 15, platinumChance = 5;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            int rand = Program.RNG.Next(0, copperChance + brassChance + bronzeChance + ironChance + steelChance + platinumChance);

            int chance = copperChance;
            if (rand <= chance)
                return Material.Copper;
            chance += brassChance;
            if (rand <= chance)
                return Material.Brass;
            chance += bronzeChance;
            if (rand <= chance)
                return Material.Bronze;
            chance += ironChance;
            if (rand <= chance)
                return Material.Iron;
            chance += steelChance;
            if (rand <= chance)
                return Material.Steel;
            return Material.Platinum;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }
    }
}
