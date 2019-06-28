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

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            Menus.LoadSave.LoadScreen();

        }

        public override void ClientSizeChanged()
        {
            Menus.LoadSave.DisplayTipMenu();
        }
    }
}
