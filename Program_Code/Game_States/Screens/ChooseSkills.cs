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

        public override void Render()
        {
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
