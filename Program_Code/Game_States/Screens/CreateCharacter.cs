using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class CreateCharacter : GameState
    {
        public CreateCharacter() : base()
        {
        }

        public override void Update()
        {
            CharacterCreation.CharacterCreationScreen();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
