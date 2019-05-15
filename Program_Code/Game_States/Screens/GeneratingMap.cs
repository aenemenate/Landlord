using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class GeneratingMap : GameState
    {
        public GeneratingMap() : base()
        {
        }

        public override void Update()
        {
            Menus.LoadSave.GenerateScreen();
        }

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
