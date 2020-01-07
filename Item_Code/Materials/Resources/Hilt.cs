using Microsoft.Xna.Framework;

namespace Landlord
{
    class Hilt : Item
    {
        public Hilt(bool instantiating, Material material, byte graphic = 252, ItemType type = ItemType.Hilt, double volume = 0.05, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            Material = material;
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public Hilt() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A " + Physics.MaterialNames[Material] + " hilt.";
        }

        public override string DetermineName(bool identifying)
        {
            return Physics.MaterialNames[Material] + " hilt";
        }

        public override Material DetermineMaterial()
        {
            return Material.Wood;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }
    }
}
