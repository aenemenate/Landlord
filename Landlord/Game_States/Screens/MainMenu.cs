using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class MainMenu : GameState
    {
        public MainMenu() : base()
        {
        }

        public override void Update()
        {
            Menus.MenuScreens.MainMenu();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
