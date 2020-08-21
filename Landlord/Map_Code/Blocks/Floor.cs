
namespace Landlord
{
    class Floor : Block
    {
        public Floor(Material material, byte graphic = 46, BlockType type = BlockType.Floor, string name = "floor", bool solid = false, bool opaque = false, bool interactive = false, bool enterable = false)
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Floor() : base()
        {
        }
        public override void Activate(Creature user)
        {
        }
    }

}
