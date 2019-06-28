using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class LevelUp : GameState
    {
        private Creature character;
        public Dictionary<Attribute, int> lvlProgress;
        public Attribute selectedAttribute;

        public LevelUp( Creature character, Dictionary<Attribute, int> lvlProgress  ) : base()
        {
            this.character = character;
            this.lvlProgress = lvlProgress;
            selectedAttribute = Attribute.Luck;
        }

        public override void Update()
        {
            StatusPanel.HandleStatus();

            if (Program.Animations.Count == 0)
            {
                Play.RenderMap(Program.Player, Program.Console, Program.Window);
                Program.MsgConsole.Render();
            }

            GUI.CharacterSheet.HandleLevelUp( selectedAttribute );
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            selectedAttribute = GUI.CharacterSheet.RenderLevelUp( character, lvlProgress );
        }

        public void FinalizeLevelUp( List<Attribute> selectedAttributes)
        {
            foreach (Attribute att in selectedAttributes)
                character.Stats.Attributes[att] += lvlProgress.ContainsKey(att) ? Math.Min( 5, lvlProgress[att] + 1 ) : 1;

            // level up resources
            character.Stats.Resources[Resource.MaxHP] += character.Stats.Attributes[Attribute.Endurance] / 10;
            character.Stats.Resources[Resource.MaxMP] = character.Stats.Attributes[Attribute.Intelligence];
            character.Stats.Resources[Resource.MP] = character.Stats.Attributes[Attribute.Intelligence];

            Program.CurrentState = new Play();
        }

        public override void ClientSizeChanged()
        {

        }
    }
}
