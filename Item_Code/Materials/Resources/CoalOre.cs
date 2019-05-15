using Microsoft.Xna.Framework;

namespace Landlord
{
    class CoalOre : Item
    {
        public CoalOre( bool instantiating, byte graphic = 45, ItemType type = ItemType.Coal, double volume = 2, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Physics.MaterialColors[Material.Coal];
            Identify();
        }

        public CoalOre() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A boulder of coal.";
        }

        public override string DetermineName(bool identifying)
        {
            return "coal";
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
