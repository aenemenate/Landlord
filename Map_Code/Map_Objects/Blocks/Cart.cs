using Microsoft.Xna.Framework;

namespace Landlord
{
    class Cart : Chest
    {
        // CONSTRUCTORS //
        public Cart(Material material, byte graphic = 15 + 16 * 13 + 9, BlockType type = BlockType.Cart, string name = "cart", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false)
                  : base(material, graphic, type, name, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Cart() : base()
        {
        }
    }

}
