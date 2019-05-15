using Microsoft.Xna.Framework;

namespace Landlord
{
    class StoneWheel : Item
    {
        public StoneWheel( bool instantiating, byte graphic = 7, ItemType type = ItemType.Wheel, double volume = 3.14, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Physics.MaterialColors[Material.Stone];
            Identify();
        }

        public StoneWheel() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A wheel shaped from stone.";
        }
        public override string DetermineName(bool identifying)
        {
            return "stone wheel";
        }

        public override Material DetermineMaterial()
        {
            return Material.Stone;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
