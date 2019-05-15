using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ViewEquipment : GameState
    {
        public ViewEquipment() : base()
        {
        }

        public override void Update()
        {
            bool mouseOnItem = InventoryPanel.HandleInventory();
            StatusPanel.HandleStatus();
            InventoryPanel.HandleEquipmentView(mouseOnItem);
        }

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {
        }
    }
}
