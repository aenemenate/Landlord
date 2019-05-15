using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ViewAttributes : GameState
    {
        public ViewAttributes() : base()
        {
        }

        public override void Update()
        {
            InventoryPanel.HandleInventory();
            StatusPanel.HandleStatus();
            StatusPanel.HandleAttributesView();
        }

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {
        }
    }
}
