using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class CharacterSheet : GameState
    {
        private Creature character;

        public CharacterSheet( Creature character ) : base()
        {
            this.character = character;
        }

        public override void Update()
        {
            StatusPanel.HandleStatus();

            if (Program.Animations.Count == 0)
            {
                Play.RenderMap(Program.Player, Program.Console, Program.Window);
                Program.MsgConsole.Render();
            }

            GUI.CharacterSheet.HandleCharacterSheet();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
            GUI.CharacterSheet.RenderCharacterSheet(character);
        }

        public override void ClientSizeChanged()
        {
            GUI.Console.Clear();
        }

        public Creature Character
        {
            get { return character; }
            set { character = value; }
        }
    }
}
