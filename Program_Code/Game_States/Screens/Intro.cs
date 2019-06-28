using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Intro : GameState
    {
        public Intro() : base()
        {
        }

        public override void Update()
        {
            Program.Window.Print(20, 5, "You've arrive in the township of TEST with 5000 gold in hand. Explore this land and buy your first plot to get started " +
                "on the long journey to riches!", Program.Window.Width - 40);
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
