
namespace Landlord
{
    class Wall : Block
    {
        private int hp;
        public Wall(Material material, byte graphic = 178, BlockType type = BlockType.Wall, string name = "wall", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false)
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
            if (material == Material.Stone || material == Material.Coal)
                hp = (int)Physics.ImpactFractures[Material];
            else
                hp = 1000;
        }

        public Wall() : base()
        {
        }
        public override void Activate(Creature user)
        {
            user.PickWall(this);
        }

        public int HP {
            get { return hp; }
            set { hp = value; }
        }
    }

}
