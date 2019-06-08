using static SadConsole.Global;
using Microsoft.Xna.Framework;

namespace Landlord
{
    static class UserInterfaceInput
    {
        // ui shortcuts
        private static Microsoft.Xna.Framework.Input.Keys cycleDisplay = Microsoft.Xna.Framework.Input.Keys.Tab;
        private static Microsoft.Xna.Framework.Input.Keys openMap = Microsoft.Xna.Framework.Input.Keys.M;
        private static Microsoft.Xna.Framework.Input.Keys openCraftMenu = Microsoft.Xna.Framework.Input.Keys.R;
        private static Microsoft.Xna.Framework.Input.Keys enterBuildMode = Microsoft.Xna.Framework.Input.Keys.B;
        private static Microsoft.Xna.Framework.Input.Keys openCharacterSheet = Microsoft.Xna.Framework.Input.Keys.C;
        private static Microsoft.Xna.Framework.Input.Keys openJournal = Microsoft.Xna.Framework.Input.Keys.J;

        private static Microsoft.Xna.Framework.Input.Keys esc = Microsoft.Xna.Framework.Input.Keys.Escape;
        private static Microsoft.Xna.Framework.Input.Keys devFunc1 = Microsoft.Xna.Framework.Input.Keys.F1;

        public static void HandleKeys()
        {
            if (KeyboardState.IsKeyReleased(openMap)) {
                if (Program.CurrentState is Play play && play.PlayMode == PlayMode.Roguelike)
                    Program.CurrentState = new ViewWorld(true);
                else if (Program.CurrentState is ViewWorld) {
                    Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["closeMap"]);
                    Program.CurrentState = new Play();
                }
            }
            else if (KeyboardState.IsKeyReleased(cycleDisplay) && Program.CurrentState is Play)
                InventoryPanel.Displaying = !InventoryPanel.Displaying;
            else if (KeyboardState.IsKeyReleased(openCraftMenu))
            {
                if (Program.CurrentState is Play play && play.PlayMode == PlayMode.Roguelike && Program.Player.Inventory.Exists(i => i.Name == "recipe pouch"))
                    Program.Player.Wield(Program.Player.Inventory.FindIndex(i => i.Name == "recipe pouch"), true);
            }
            else if (KeyboardState.IsKeyReleased(enterBuildMode)) {
                if ((Program.CurrentState is Play play && play.PlayMode == PlayMode.Roguelike) && Program.Player.Inventory.Exists(i => i.Name == "blueprint pouch"))
                    Program.Player.Wield(Program.Player.Inventory.FindIndex(i => i.Name == "blueprint pouch"), true);
            }
            else if (KeyboardState.IsKeyReleased( openCharacterSheet )) {
                if (( Program.CurrentState is Play play && play.PlayMode == PlayMode.Roguelike ))
                    Program.CurrentState = new CharacterSheet( Program.Player );
                else if (Program.CurrentState is CharacterSheet cs)
                    Program.CurrentState = new Play();
            }
            else if (KeyboardState.IsKeyReleased(devFunc1)) {
                Item item = Program.Player.Inventory.Find( i => i.Name == "recipe pouch" );
                Program.Player.Inventory.Remove( item );
                Program.Player.Inventory.Add( new RecipePouch(false) );

                item = Program.Player.Inventory.Find( i => i.Name == "blueprint pouch" );
                Program.Player.Inventory.Remove( item );
                Program.Player.Inventory.Add( new BlueprintPouch( false ) );
            }
            else if (KeyboardState.IsKeyReleased(esc)) {
                if (Program.CurrentState is ViewWorld) {
                    Program.AudioEngine.PlaySound( Program.AudioEngine.CachedSoundFX["closeMap"] );
                    Program.CurrentState = new Play();
                } else if (Program.CurrentState is ViewLoot)
                    Program.Animations.Add( new CloseLootView() );
                else if (Program.CurrentState is CraftMenu)
                    Program.Animations.Add( new CloseCraftMenu() );
                else if (Program.CurrentState is DialogWindow) {
                    CurrentScreen.Children.Remove( Program.ControlsConsole );
                    Program.CurrentState = Menus.PrevGameState;
                } else if (Program.CurrentState is CharacterSheet)
                    Program.CurrentState = new Play();
                else if (Program.CurrentState is Play play) {
                    if (play.PlayMode == PlayMode.Roguelike)
                        Menus.PauseMenu();
                    else if (play.PlayMode == PlayMode.BuildMode) {
                        Program.Player.Unequip( Program.Player.Body.MainHand );
                        play.PlayMode = PlayMode.Roguelike;
                    }
                }
            }
        }
    }
}
