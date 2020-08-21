using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class GeneratingDungeon : GameState
    {
        public GeneratingDungeon() : base()
        {
        }

        public override void Update()
        {
            Menus.LoadSave.GenerateDungeonScreen();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
