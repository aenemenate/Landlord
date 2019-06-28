using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class CraftMenu : GameState
    {
        public CraftMenu() : base()
        {
            Program.Animations.Add(new OpenCraftMenu());
            GUI.CraftMenu.CurrentlyViewedRecipe = null;
        }

        public override void Update()
        {
            StatusPanel.HandleStatus();
            InventoryPanel.HandleInventory();

            GUI.CraftMenu.HandleCraftMenu();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            if (Program.Animations.Count == 0) {
                Play.RenderMap(Program.Player, Program.Console, Program.Window);
                Program.MsgConsole.Render();
            }
        }

        public override void ClientSizeChanged()
        {
        }
    }
}
