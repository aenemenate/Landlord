using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ViewLoot : GameState
    {
        private GUI.LootMenu lootMenu;

        public ViewLoot(List<Item> inventory, string containerName) : base()
        {
            lootMenu = new GUI.LootMenu(inventory, containerName);
        }

        public override void Update()
        {
            StatusPanel.HandleStatus();
            InventoryPanel.HandleInventory();
            lootMenu.HandleLootMenu();
        }

        public override void Render()
        {
            lootMenu.RenderLootMenu();
        }

        public override void ClientSizeChanged()
        {
            lootMenu.iItemList.StartPos = new Point(GUI.LootMenu.StartX + 1, GUI.LootMenu.StartY + 3);
            lootMenu.iItemList.Width = GUI.LootMenu.Width - 2;
            lootMenu.iItemList.Height = GUI.LootMenu.Height - 6;
            lootMenu.iItemList.RButtonPos = new Point(GUI.LootMenu.StartX + GUI.LootMenu.Width - 2, GUI.LootMenu.StartY + 1);
            lootMenu.iItemList.LButtonPos = new Point(GUI.LootMenu.StartX + 1, GUI.LootMenu.StartY + 1);
        }

        // PROPERTIES //

        public GUI.LootMenu LootMenu
        {
            get { return lootMenu; }
        }
    }
}
