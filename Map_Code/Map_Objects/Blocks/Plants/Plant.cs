using Microsoft.Xna.Framework;

namespace Landlord
{
    class Plant : Block
    {
        public Plant(byte graphic, string name, bool solid = false, bool opaque = false, BlockType type = BlockType.Plant, bool interactive = true, bool enterable = false) : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.BackColor = Color.Pink;
        }
        public Plant() : base()
        {

        }
        public void Grow()
        {

        }
        public override void Activate(Creature user) {

        }
    }
}
