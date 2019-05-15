using Microsoft.Xna.Framework;

namespace Landlord
{
    class Plank : Item
    {
        public Plank(bool instantiating, byte graphic = 61, ItemType type = ItemType.Plank, double volume = 0.96, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public Plank() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A plank which was cut from a log.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wood plank";
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
