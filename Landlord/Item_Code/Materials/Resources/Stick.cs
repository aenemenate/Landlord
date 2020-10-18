using Microsoft.Xna.Framework;
namespace Landlord
{
    class Stick : Item
    {
        public Stick(bool instantiating, byte graphic = 92, ItemType type = ItemType.Stick, double volume = 0.05, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public Stick() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A stick. What else do you expect?";
        }

        public override string DetermineName(bool identifying)
        {
            return "stick";
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
