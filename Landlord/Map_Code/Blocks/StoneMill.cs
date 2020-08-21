using Microsoft.Xna.Framework;

namespace Landlord
{
    class StoneMill : Block
    {

        // CONSTRUCTORS //
        public StoneMill(Material material, byte graphic = (15 + 16 * 8), BlockType type = BlockType.StoneMill, string name = "stone mill", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public StoneMill() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate(Creature user)
        {
            try {
                user.Wield(user.Inventory.FindIndex(i => i.Name == "recipe pouch"), true);
            }
            catch {
                Program.MsgConsole.WriteLine("You don't have a recipe pouch!");
            }
        }

    }

}
