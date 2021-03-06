﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ViewItem : GameState
    {
        public ViewItem() : base()
        {
        }

        public override void Update()
        {
            StatusPanel.HandleStatus();
            bool mouseOnItem = InventoryPanel.HandleInventory();
            InventoryPanel.HandleItemView(mouseOnItem);
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {
        }
    }

}
