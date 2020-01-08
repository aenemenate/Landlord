using Microsoft.Xna.Framework;

namespace Landlord
{
    class WoodWheel : Item
    {
        public WoodWheel( bool instantiating, byte graphic = 9, ItemType type = ItemType.Wheel, double volume = 3.14, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public WoodWheel() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A wheel made from glued logs.";
        }
        public override string DetermineName(bool identifying)
        {
            return "wood wheel";
        }

        public override Material DetermineMaterial()
        {
            return Material.Wood;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
