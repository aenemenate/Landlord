using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Chest : Block
    {
        private List<Item> inventory = new List<Item>();

        // CONSTRUCTORS //
        public Chest(Material material, byte graphic = 228, BlockType type = BlockType.Chest, string name = "chest", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Chest() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate(Creature user)
        {
            if (user is Player)
            {
                Program.Animations.Add(new OpenLootView());
                Program.CurrentState = new ViewLoot(inventory, 100, Name);
            }
        }

        // PROPERTIES //
        public List<Item> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
    }

}
