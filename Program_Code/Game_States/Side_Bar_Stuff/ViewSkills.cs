using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ViewSkills : GameState
    {
        public ViewSkills() : base()
        {
        }

        public override void Update()
        {
            InventoryPanel.HandleInventory();
            StatusPanel.HandleStatus();
            StatusPanel.HandleSkillsView();
        }

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {
        }
    }
}
