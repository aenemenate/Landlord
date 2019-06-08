using Microsoft.Xna.Framework;

namespace Landlord
{
    class CraftingTable : Block
    {

        // CONSTRUCTORS //
        public CraftingTable(Material material, byte graphic = (16 * 6 + 14), BlockType type = BlockType.CraftingTable, string name = "crafting table", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public CraftingTable() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate(Creature user)
        {
            try
            {
                user.Wield(user.Inventory.FindIndex(i => i.Name == "recipe pouch"), true);
            }
            catch
            {
                Program.MsgConsole.WriteLine("You don't have a recipe pouch!");
            }
        }

    }

}
