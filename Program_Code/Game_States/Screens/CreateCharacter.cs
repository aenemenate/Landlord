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

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
