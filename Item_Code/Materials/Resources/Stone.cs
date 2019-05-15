using Microsoft.Xna.Framework;

namespace Landlord
{
    class Stone : Item
    {
        public Stone( bool instantiating, byte graphic = 45, ItemType type = ItemType.Stone, double volume = 2.5, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Physics.MaterialColors[Material.Stone];
            Identify();
        }

        public Stone() : base()
        {
        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A boulder of workable stone.";
        }

        public override string DetermineName(bool identifying)
        {
            return "stone";
        }

        public override Material DetermineMaterial()
        {
            return Material.Stone;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }
    }
}
