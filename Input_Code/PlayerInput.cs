using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class PlayerInput
    {
        private DateTime lastMovement = DateTime.Now;
        private bool selectingActDir = false; // whether or not the player is currently choosing the direction to act
        private bool cancelMove = false;

        // INPUT HANDLING

        public bool HandleInput(bool allowMouseInput, bool allowKeysInput)
        {
            if (allowKeysInput)
                HandleKeys();
            if (!selectingActDir && allowMouseInput)
                HandleMouse();
            return selectingActDir;
        }

        public void HandleKeys()
        {
            bool upPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad8);
            bool upRightPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad9);
            bool rightPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad6);
            bool downRightPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad3);
            bool downPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad2);
            bool downLeftPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad1);
            bool leftPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad4);
            bool upLeftPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad7);

            bool waitPressed = SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.NumPad5) || SadConsole.Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.W);
            bool actionPressed = SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter) || SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space);

            Point nextPos = new Point();

            List<Point> interactiveSpots = GetNearbyInteractiveBlocks(Program.Player.Position);



            if (actionPressed && !selectingActDir) // if you pressed action
            {
                bool multipleNearbyObjs = interactiveSpots.Count > 1;
                bool noNearbyObj = interactiveSpots.Count == 0;

                // first check if the current block is enterable (you cannot pick up items the player is standing on)
                if (Program.Player.CurrentBlock.Enterable == true)
                    switch (noNearbyObj)
                    {
                        case true: { Program.Player.CurrentBlock.Activate(Program.Player); } break;
                        case false: { multipleNearbyObjs = true; } break;
                    }


                // if there's only one nearby object
                if (!multipleNearbyObjs && !noNearbyObj)
                {
                    if (!(Program.WorldMap.LocalTile[interactiveSpots[0].X, interactiveSpots[0].Y] is Item))
                        Program.WorldMap.LocalTile[interactiveSpots[0].X, interactiveSpots[0].Y].Activate(Program.Player);
                    else
                        Program.Player.GetItem(interactiveSpots[0]);
                }
                else if (multipleNearbyObjs)
                {
                    Program.MsgConsole.Clear();
                    Program.MsgConsole.WriteLine("Select a direction (5 for current position)");
                    selectingActDir = true;
                }
            }

            // select the direction to act in. This will be handled differently depending on what the player is currently doing.
            if (upPressed)
                nextPos = new Point(Program.Player.Position.X, Program.Player.Position.Y - 1);
            else if (upRightPressed)
                nextPos = new Point(Program.Player.Position.X + 1, Program.Player.Position.Y - 1);
            else if (rightPressed)
                nextPos = new Point(Program.Player.Position.X + 1, Program.Player.Position.Y);
            else if (downRightPressed)
                nextPos = new Point(Program.Player.Position.X + 1, Program.Player.Position.Y + 1);
            else if (downPressed)
                nextPos = new Point(Program.Player.Position.X, Program.Player.Position.Y + 1);
            else if (downLeftPressed)
                nextPos = new Point(Program.Player.Position.X - 1, Program.Player.Position.Y + 1);
            else if (leftPressed)
                nextPos = new Point(Program.Player.Position.X - 1, Program.Player.Position.Y);
            else if (upLeftPressed)
                nextPos = new Point(Program.Player.Position.X - 1, Program.Player.Position.Y - 1);
            else if (waitPressed)
            {
                if (!selectingActDir)
                    Program.Player.Wait();
                else
                    nextPos = new Point(Program.Player.Position.X, Program.Player.Position.Y);
            }


            // Choose how to handle the next position input, if there is one
            if (!nextPos.Equals(new Point()))
            {
                bool movementCooldownReached; // the cooldown is basically a time buffer in between input handles.

                if (SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                {
                    movementCooldownReached = DateTime.Now - lastMovement > new TimeSpan(TimeSpan.TicksPerSecond / 24);
                }
                else
                    movementCooldownReached = DateTime.Now - lastMovement > new TimeSpan(TimeSpan.TicksPerSecond / 6);


                if (selectingActDir)
                {
                    if (nextPos.Equals(Program.Player.Position) && Program.Player.CurrentBlock.Enterable == true)
                        Program.Player.CurrentBlock.Activate(Program.Player);
                    else if (interactiveSpots.Contains(nextPos))
                    {
                        if (!(Program.WorldMap.LocalTile[nextPos.X, nextPos.Y] is Item))
                            Program.WorldMap.LocalTile[nextPos.X, nextPos.Y].Activate(Program.Player);
                        else
                            Program.Player.GetItem(nextPos);
                    }
                    selectingActDir = false;
                    lastMovement = DateTime.Now;
                }
                else if (movementCooldownReached)
                {
                    if (nextPos.X == Program.WorldMap.LocalTile.Width || nextPos.X == -1 || nextPos.Y == Program.WorldMap.LocalTile.Height || nextPos.Y == -1)
                        Engine.HandleMapSwitching(Program.Player);
                    else if (Program.WorldMap.LocalTile[nextPos.X, nextPos.Y] is Creature && ((Creature)Program.WorldMap.LocalTile[nextPos.X, nextPos.Y]).Alive)
                        Program.WorldMap.LocalTile.GetCreatureAtPosition(nextPos).Activate(Program.Player);
                    else
                        Program.Player.Move(nextPos, Program.WorldMap.LocalTile);
                    lastMovement = DateTime.Now;
                }
            }
        }

        public void HandleMouse()
        {
            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point mapPos = Program.WorldMap.LocalTile.GetMousePos(mousePos);
            bool mouseIsOnMap = !(mousePos.X < InventoryPanel.Width || mousePos.X >= Program.Console.Width - StatusPanel.Width);

            if (!SadConsole.Global.MouseState.IsOnScreen || !(DateTime.Now - lastMovement > new TimeSpan( TimeSpan.TicksPerSecond / 6 )))
                return;

            if (SadConsole.Global.MouseState.LeftButtonDown)
                HandleLeftClicked(mapPos, mouseIsOnMap);
            else if (SadConsole.Global.MouseState.RightClicked)
                HandleRightClicked(mapPos, mouseIsOnMap);
        }

        private void HandleLeftClicked(Point mapPos, bool mouseIsOnMap)
        {
            if (!cancelMove && mouseIsOnMap)
            {
                if (Menus.ClickedDialog)
                    Menus.ClickedDialog = false;
                else if (SadConsole.Global.MouseState.RightButtonDown)
                    cancelMove = true;
                else if (Program.Player.Path != null)
                    Program.Player.Path = null;
                else
                {
                    if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is Player)
                    {
                        bool movedMaps = Engine.HandleMapSwitching(Program.Player);
                        if (movedMaps == false && Program.Player.CurrentBlock.Enterable)
                            Program.Player.CurrentBlock.Activate(Program.Player);
                    }
                    else if (Program.Player.PointNextToSelf(mapPos) && Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Interactive)
                    {
                        if (!(Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is Item )) {
                            if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is Chest || Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is CraftingTable)
                                GUI.LootMenu.ClickedContainer = true;
                            Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Activate( Program.Player );
                        }
                        else
                            Program.Player.GetItem(mapPos);
                    }
                }
                lastMovement = DateTime.Now;
            }
        }

        private void HandleRightClicked(Point mapPos, bool mouseIsOnMap)
        {
            if (cancelMove)
                cancelMove = false;
            else if (mouseIsOnMap)
                if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Explored && Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Solid == false && mapPos.Y < 100)
                    Program.Player.SetPath(mapPos);
        }


        // CHECKS FOR FUNCTIONS
        
        public List<Point> GetNearbyInteractiveBlocks(Point position)
        {
            List<Point> interactivePoints = new List<Point>();
            for (int i = Math.Max(0, position.X - 1); i <= Math.Min(Program.WorldMap.LocalTile.Width - 1, position.X + 1); i++)
                for (int j = Math.Max(0, position.Y - 1); j <= Math.Min(Program.WorldMap.LocalTile.Height - 1, position.Y + 1); j++)
                    if (Program.WorldMap.LocalTile[i, j].Interactive && !position.Equals(new Point(i, j)))
                        interactivePoints.Add(new Point(i, j));
            return interactivePoints;
        }


        public bool SelectingActionDir
        {
            get { return selectingActDir; }
            set { selectingActDir = value; }
        }

        public bool CancelMove
        {
            get { return cancelMove; }
        }

    }
}
