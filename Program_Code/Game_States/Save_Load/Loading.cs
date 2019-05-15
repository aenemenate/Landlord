using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Loading : GameState
    {
        public Loading() : base()
        {
        }

        public override void Update()
        {
        }

        public override void Render()
        {
            Menus.LoadSave.LoadScreen();

        }

        public override void ClientSizeChanged()
        {
            Menus.LoadSave.DisplayTipMenu();
        }
    }
}
