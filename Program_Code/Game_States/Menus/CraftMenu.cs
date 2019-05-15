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

            if (Program.Animations.Count == 0)
            {
                Play.RenderMap();
                Program.MsgConsole.Render();
            }
            
            GUI.CraftMenu.HandleCraftMenu();
        }

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {
        }
    }
}
