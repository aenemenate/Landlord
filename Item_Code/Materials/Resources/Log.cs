using Microsoft.Xna.Framework;

namespace Landlord
{
    class Log : Item
    {
        public Log(bool instantiating, byte graphic = 45, ItemType type = ItemType.Log, double volume = 5.8, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = new Color(205, 133, 63);
            Identify();
        }

        public Log() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return "A small log obtained by chopping down a tree.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wood log";
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
