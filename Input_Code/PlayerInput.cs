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
        private bool aimingMode = false; // whether or not the player is currently aiming

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
            bool firePressed = SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F);

            Point nextPos = new Point();

            List<Point> interactiveSpots = GetNearbyInteractiveBlocks(Program.Player.Position);

            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            if (firePressed && Program.Player.Body.MainHand is RangedWeapon)
                aimingMode = !aimingMode;

            if (aimingMode)
                return;

            if (actionPressed && !selectingActDir) // if you pressed action
            {
                bool multipleNearbyObjs = interactiveSpots.Count > 1;
                bool noNearbyObj = interactiveSpots.Count == 0;

                // first check if the current block is enterable (you cannot pick up items the player is standing on)
                if (Program.Player.CurrentBlock.Enterable == true)
                    switch (noNearbyObj) {
                        case true: { Program.Player.CurrentBlock.Activate(Program.Player); } break;
                        case false: { multipleNearbyObjs = true; } break;
                    }


                // if there's only one nearby object
                if (!multipleNearbyObjs && !noNearbyObj) {
                    if (!(blocks[interactiveSpots[0].X * width + interactiveSpots[0].Y] is Item)) {
                        Block block = blocks[interactiveSpots[0].X * width + interactiveSpots[0].Y];
                        if (block is Tree)
                            Program.Player.ChopTree(interactiveSpots[0]);
                        else if (block is Plant)
                            Program.Player.HarvestPlant(interactiveSpots[0]);
                        else
                            block.Activate(Program.Player);
                    }
                    else
                        Program.Player.GetItem(interactiveSpots[0]);
                }
                else if (multipleNearbyObjs) {
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
            else if (waitPressed) {
                if (!selectingActDir)
                    Program.Player.Wait();
                else
                    nextPos = new Point(Program.Player.Position.X, Program.Player.Position.Y);
            }


            // Choose how to handle the next position input, if there is one
            if (!nextPos.Equals(new Point())) {
                bool movementCooldownReached; // the cooldown is basically a time buffer in between input handles.

                if (SadConsole.Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                    movementCooldownReached = DateTime.Now - lastMovement > new TimeSpan(TimeSpan.TicksPerSecond / 24);
                else
                    movementCooldownReached = DateTime.Now - lastMovement > new TimeSpan(TimeSpan.TicksPerSecond / 6);


                if (selectingActDir) {
                    if (nextPos.Equals(Program.Player.Position) && Program.Player.CurrentBlock.Enterable == true)
                        Program.Player.CurrentBlock.Activate(Program.Player);
                    else if (interactiveSpots.Contains(nextPos)) {
                        if (!(blocks[nextPos.X * width + nextPos.Y] is Item)) {
                            Block block = blocks[nextPos.X * width + nextPos.Y];
                            if (block is Tree)
                                Program.Player.ChopTree(nextPos);
                            else if (block is Plant)
                                Program.Player.HarvestPlant(nextPos);
                            else
                                block.Activate(Program.Player);
                        }
                        else
                            Program.Player.GetItem(nextPos);
                    }
                    selectingActDir = false;
                    lastMovement = DateTime.Now;
                }
                else if (movementCooldownReached) {
                    if (nextPos.X == width || nextPos.X == -1 || nextPos.Y == height || nextPos.Y == -1)
                        CreaturePlacementHelper.HandleMapSwitching(Program.Player);
                    else if (blocks[nextPos.X * width + nextPos.Y] is Creature creature && creature.Alive)
                        creatures.GetCreatureAtPosition(nextPos).Activate(Program.Player);
                    else
                        Program.Player.Move(nextPos);
                    lastMovement = DateTime.Now;
                }
            }
        }

        public void HandleMouse()
        {
            Point worldIndex = Program.Player.WorldIndex;
            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point mapPos = Program.WorldMap[worldIndex.X, worldIndex.Y].GetMousePos(mousePos);
            bool mouseIsOnMap = !(mousePos.X < InventoryPanel.Width || mousePos.X >= Program.Console.Width - StatusPanel.Width);

            if (!SadConsole.Global.MouseState.IsOnScreen || !(DateTime.Now - lastMovement > new TimeSpan( TimeSpan.TicksPerSecond / 10 )))
                return;

            if (SadConsole.Global.MouseState.LeftButtonDown)
                HandleLeftClicked(mapPos, mouseIsOnMap);
        }

        private void HandleLeftClicked(Point mapPos, bool mouseIsOnMap)
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;
            if (Program.Player.Path != null)
                Program.Player.Path = null;

            if (mouseIsOnMap)
            {
                aimingMode = false;
                if (Menus.ClickedDialog)
                    Menus.ClickedDialog = false;
                else if (aimingMode) {
                    Program.Player.Shoot(mapPos);
                    aimingMode = false;
                }
                else {
                    if (blocks[mapPos.X * width + mapPos.Y] is Player) {
                        bool movedMaps = CreaturePlacementHelper.HandleMapSwitching(Program.Player);
                        if (movedMaps == false && Program.Player.CurrentBlock.Enterable)
                            Program.Player.CurrentBlock.Activate(Program.Player);
                    }
                    else if (Program.Player.Position.NextToPoint(mapPos) && blocks[mapPos.X * width + mapPos.Y].Interactive) {
                        if (!(blocks[mapPos.X * width + mapPos.Y] is Item )) {
                            Block block = blocks[mapPos.X * width + mapPos.Y];
                            if (block is Chest || block is CraftingTable || block is StoneMill)
                                GUI.LootMenu.ClickedContainer = true;
                            if (block is Tree)
                                Program.Player.ChopTree(mapPos);
                            else if (block is Plant)
                                Program.Player.HarvestPlant(mapPos);
                            else
                                block.Activate(Program.Player);
                        }
                        else
                            Program.Player.GetItem(mapPos);
                    }
                }
                lastMovement = DateTime.Now;
            }
        }

        public void HandleRightClicked()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point mapPos = Program.WorldMap[worldIndex.X, worldIndex.Y].GetMousePos(mousePos);
            bool mouseIsOnMap = !(mousePos.X < InventoryPanel.Width || mousePos.X >= Program.Console.Width - StatusPanel.Width);

            aimingMode = false;

            if (mouseIsOnMap)
                if (blocks[mapPos.X * width + mapPos.Y].Explored && blocks[mapPos.X * width + mapPos.Y].Solid == false && mapPos.Y < 100)
                    Program.Player.SetPath(mapPos);
        }

        public List<Point> GetNearbyInteractiveBlocks(Point position)
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            List<Point> interactivePoints = new List<Point>();
            for (int i = Math.Max(0, position.X - 1); i <= Math.Min(width - 1, position.X + 1); i++)
                for (int j = Math.Max(0, position.Y - 1); j <= Math.Min(height - 1, position.Y + 1); j++)
                    if (blocks[i * width + j].Interactive && !position.Equals(new Point(i, j)))
                        interactivePoints.Add(new Point(i, j));
            return interactivePoints;
        }

        public bool AimingMode
        {
            get { return aimingMode; }
            set { aimingMode = value; }
        }
    }
}
