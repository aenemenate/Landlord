
namespace Landlord
{
    class UpStair : Block
    {
        public UpStair(Material material, byte graphic = 60, BlockType type = BlockType.UpStair, string name = "up stairs", bool solid = false, bool opaque = true, bool interactive = false, bool enterable = true)
               : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }
        public UpStair() : base()
        {
        }
        public override void Activate(Creature user)
        {
            user.TakeStairsUp();
        }
    }

}
