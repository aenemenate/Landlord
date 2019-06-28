using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Saving : GameState
    {
        public Saving() : base()
        {
        }

        public override void Update()
        {
            Menus.LoadSave.SaveScreen();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
