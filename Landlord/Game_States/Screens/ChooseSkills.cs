using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class ChooseSkills : GameState
    {
        public ChooseSkills()
        {

        }

        public override void Update()
        {
            CharacterCreation.SkillSelectionScreen();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
