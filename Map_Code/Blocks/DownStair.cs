
namespace Landlord
{
    class DownStair : Block
    {
        public DownStair(Material material, byte graphic = 62, BlockType type = BlockType.DownStair, string name = "down stairs", bool solid = false, bool opaque = true, bool interactive = false, bool enterable = true)
               : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public DownStair() : base()
        {
        }

        public override void Activate(Creature user)
        {
            user.TakeStairsDown();
        }
    }

}
