using Microsoft.Xna.Framework;

namespace Landlord
{
    class Leaf : Item
    {
        string leafName;
        // CONSTRUCTORS
        public Leaf(bool instantiating, string leafName, Color foreColor, byte graphic = 45, ItemType type = ItemType.Leaf, double volume = 0.0002, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            this.leafName = leafName;
            ForeColor = foreColor;
            Identify();
        }

        public Leaf() : base()
        {

        }

        // FUNCTIONS
        public override void Activate(Creature user)
        {
        }
        public override string DetermineDescription()
        {
            return "A " + leafName + ".";
        }
        public override string DetermineName(bool identifying)
        {
            return leafName + " leaf";
        }
        public override Material DetermineMaterial()
        {
            return Material.Plant;
        }
        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }

        public string LeafName
        {
            get { return leafName; }
            set { leafName = value; }
        }
    }
}
