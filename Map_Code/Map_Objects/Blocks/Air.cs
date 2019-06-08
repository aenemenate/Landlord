using Microsoft.Xna.Framework;

namespace Landlord
{
    class Air : Block
    {
        public Air() : base()
        {
            this.Graphic = 1;
            this.Name = "air";
            this.Type = BlockType.Empty;
            this.ForeColor = new Color(0, 0, 0, 0);
            this.BackColor = new Color(0, 0, 0, 0);
            this.Solid = false;
            this.Opaque = false;
            Interactive = false;
            Enterable = false;
        }
        public override void Activate(Creature user)
        {
        }
    }

}
