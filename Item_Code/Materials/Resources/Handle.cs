using Microsoft.Xna.Framework;

namespace Landlord
{
    class Handle : Item
    {
        public Handle(bool instantiating, Material material, byte graphic = 92, ItemType type = ItemType.Stick, double volume = 0.05, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            this.Material = material;
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public Handle() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A " + Physics.MaterialNames[Material] + " handle.";
        }

        public override string DetermineName(bool identifying)
        {
            return Physics.MaterialNames[Material] + " handle";
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
