using Microsoft.Xna.Framework;

namespace Landlord
{
    class EmptyBottle : Item
    {
        public EmptyBottle(bool instantiating, byte graphic = 235, ItemType type = ItemType.Bottle, double volume = 0.016, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.AntiqueWhite;
            Identify();
        }

        public EmptyBottle() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "An empty glass bottle. It may shatter upon impact.";
        }

        public override string DetermineName(bool identifying)
        {
            return "glass bottle";
        }

        public override Material DetermineMaterial()
        {
            return Material.Glass;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }
    }
}
