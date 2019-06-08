﻿
namespace Landlord
{
    class Door : Block
    {
        public Door(Material material, byte graphic = 43, BlockType type = BlockType.Door, string name = "door", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Door() : base()
        {
        }

        public override void Activate(Creature user)
        {
            user.OpenDoor(this);
        }
    }

}
