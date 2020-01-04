using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Landlord
{
    class Intro : GameState
    {
        public Intro() : base()
        {
        }

        public override void Update()
        {
            if (SadConsole.Global.KeyboardState.KeysDown.Count > 0)
            {
                Program.CurrentState = new Play();
            }
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            Program.Window.Print(20, 5, "You've struck upon livable land while following a river. Here is where you stake your claim, now strike riches!", Program.Window.Width - 40);
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
