using System;
using SadConsole;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Media;
using Microsoft.Xna.Framework.Audio;

namespace Landlord
{
    static class Menus
    {
        static private bool clickedDialog;
        static private Point startPoint;
        static private Element[,] worldView;
        static private int granularity;
        static private GameState prevGameState = new MainMenu();

        static public class MenuScreens
        {
            static private List<string> title = new List<string>()
            {
                @"___/\\\__________________________________________________/\\\__                  ",
                @" __\/\\\_________________________________________________\/\\\__                 ",
                @"  __\/\\\_________________________________________________\/\\\__                ",
                @"   __\/\\\______________/\\\\\\\\\_____/\\/\\\\\\__________\/\\\__               ",
                @"    __\/\\\_____________\////////\\\___\/\\\////\\\____/\\\\\\\\\__              ",
                @"     __\/\\\_______________/\\\\\\\\\\__\/\\\__\//\\\__/\\\////\\\__             ",
                @"      __\/\\\______________/\\\/////\\\__\/\\\___\/\\\_\/\\\__\/\\\__            ",
                @"       __\/\\\\\\\\\\\\\\\_\//\\\\\\\\/\\_\/\\\___\/\\\_\//\\\\\\\/\\_           ",
                @"        __\///////////////___\////////\//__\///____\///___\///////\//__          "
            };
            static private List<string> titlePart2 = new List<string>()
            {
                @"         _______________________________________________________________         ",
                @"          __/\\\_________________________________________________/\\\____        ",
                @"           _\/\\\________________________________________________\/\\\____       ",
                @"            _\/\\\________________________________________________\/\\\____      ",
                @"             _\/\\\_________________/\\\\\_____/\\/\\\\\\\_________\/\\\____     ",
                @"              _\/\\\_______________/\\\///\\\__\/\\\/////\\\___/\\\\\\\\\____    ",
                @"               _\/\\\______________/\\\__\//\\\_\/\\\___\///___/\\\////\\\____   ",
                @"                _\/\\\_____________\//\\\__/\\\__\/\\\_________\/\\\__\/\\\____  ",
                @"                 _\/\\\\\\\\\\\\\\\__\///\\\\\/___\/\\\_________\//\\\\\\\/\\___ ",
                @"                  _\///////////////_____\/////_____\///___________\///////\//____",
            };

            static public void MainMenu()
            {
                prevGameState = new MainMenu();

                int[] red = new int[3] { 150, 40, 40 };
                int[] green = new int[3] { 0, 128, 0 };
                Color color;

                int startX = (Program.Console.Width / 2) - (title[0].Length / 2);
                int startY = (Program.Console.Height / 6);
                int buttonY = 2;


                bool IsDirectoryEmpty(string path)
                {
                    return !Directory.EnumerateFileSystemEntries(path).Any();
                }

                void AddContinueButton()
                {
                    if (!IsDirectoryEmpty(@"saves\data\"))
                    {
                        var continueButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width)
                        {
                            Position = new Microsoft.Xna.Framework.Point(0, buttonY),
                            Text = "Continue"
                        };
                        continueButton.Click += (btn, args) =>
                        {
                            SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                            Program.CurrentState = new Loading();
                            clickedDialog = true;
                        };
                        Program.ControlsConsole.Add(continueButton);
                        buttonY += 2;
                    }
                }

                void AddNewGameButton()
                {
                    var newGameButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width)
                    {
                        Position = new Microsoft.Xna.Framework.Point(0, buttonY),
                        Text = "New Game"
                    };

                    newGameButton.Click += (btn, args) =>
                    {
                        SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                        if (!IsDirectoryEmpty(@"saves\data\"))
                            ConfirmNewGame();
                        else
                        {
                            Program.Console.Clear();
                            Program.CurrentState = new CreateCharacter();
                        }
                        clickedDialog = true;
                    };

                    buttonY += 2;

                    Program.ControlsConsole.Add(newGameButton);
                }

                void AddQuitButton()
                {
                    var quitButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width)
                    {
                        Position = new Microsoft.Xna.Framework.Point(0, buttonY),
                        Text = "Quit"
                    };

                    quitButton.Click += (btn, args) =>
                    {
                        SadConsole.Game.Instance.Exit();
                    };

                    Program.ControlsConsole.Add(quitButton);
                }

                void PrintTitle()
                {
                    foreach (string line in title)
                    {
                        int count = 0;
                        foreach (char c in line)
                        {
                            color = c == '_' ? new Color(red[0] + (startY - Program.Console.Height / 6) * 10, red[1], red[2])
                                : color = new Color(green[0], green[1] + (startY - Program.Console.Height / 6) * 5, green[2]);

                            Program.Console.SetGlyph(startX + count, startY, c, color);

                            count++;
                        }
                        startY++;
                    }
                    foreach (string line in titlePart2)
                    {
                        int count = 0;
                        foreach (char c in line)
                        {
                            color = c == '_' ? new Color(red[0] + (startY - Program.Console.Height / 6) * 10, red[1], red[2])
                                : color = new Color(green[0], green[1] + (startY - Program.Console.Height / 6) * 5, green[2]);

                            Program.Console.SetGlyph(startX + count, startY, c, color);

                            count++;
                        }
                        startY++;
                    }
                }

                PrintTitle();
                

                Program.ControlsConsole = new ControlsConsole(Program.Console.Width / 8, Program.Console.Height / 4)
                {
                    Position = new Microsoft.Xna.Framework.Point((Program.Console.Width / 16) * 7, startY)
                };

                AddContinueButton();

                AddNewGameButton();

                AddQuitButton();

                SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);

                Program.CurrentState = new DialogWindow();
            }

            static public void ConfirmNewGame()
            {
                prevGameState = new MainMenu();
                Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 3, Program.Console.Height / 4)
                {
                    Position = new Microsoft.Xna.Framework.Point((Program.Console.Width / 6) * 2, (Program.Console.Height / 8) * 3)
                };

                var label = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width - 1, 3)
                {
                    Position = new Microsoft.Xna.Framework.Point(1, 1)
                };
                var cursor = new SadConsole.Cursor(label);
                cursor.Print($"Are you sure? Starting a new game will permanently delete your save.");

                var yesButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1)
                {
                    Position = new Microsoft.Xna.Framework.Point(1, (Program.ControlsConsole.Height / 4) * 3),
                    Text = "Yes"
                };
                yesButton.Click += (btn, args) =>
                {
                    clickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.Console.Clear();
                    Program.CurrentState = new CreateCharacter();
                    ReadWrite.DeleteSave();
                };

                var noButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1);
                noButton.Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width - noButton.Width - 1, (Program.ControlsConsole.Height / 4) * 3);
                noButton.Text = "No";
                noButton.Click += (btn, args) =>
                {
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = prevGameState;
                };

                Program.ControlsConsole.Add(label);
                Program.ControlsConsole.Add(noButton);
                Program.ControlsConsole.Add(yesButton);

                SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
                Program.CurrentState = new DialogWindow();
            }

        }

        static public class WorldMap
        {
            static public void GenerateWorldView()
            {
                /* This function fills the array of the world map with their appropriate graphics
                 */

                // "Global" variables

                int hectareWidth = Program.WorldMap.HeightMap.GetLength(0) / 4;
                int hectareHeight = Program.WorldMap.HeightMap.GetLength(1) / 4;

                List<Point> mountainSpots = new List<Point>();
                List<Point> dungeonSpots = new List<Point>();

                // Functions //
                bool TileNextToExploredArea(Point point)
                {
                    List<Point> adjacentPoints = new List<Point>();
                    List<Point> blackListedPoints = new List<Point>();
                    adjacentPoints.Add(new Point(point.X - 1, point.Y));
                    adjacentPoints.Add(new Point(point.X + 1, point.Y));
                    adjacentPoints.Add(new Point(point.X, point.Y - 1));
                    adjacentPoints.Add(new Point(point.X, point.Y + 1));
                    adjacentPoints.Add(new Point(point.X - 1, point.Y + 1));
                    adjacentPoints.Add(new Point(point.X - 1, point.Y - 1));
                    adjacentPoints.Add(new Point(point.X + 1, point.Y + 1));
                    adjacentPoints.Add(new Point(point.X + 1, point.Y - 1));
                    foreach (Point adjacentPoint in adjacentPoints)
                        if (adjacentPoint.X < 0 || adjacentPoint.X >= worldView.GetLength(0) || adjacentPoint.Y < 0 || adjacentPoint.Y >= worldView.GetLength(1))
                            blackListedPoints.Add(adjacentPoint);
                    foreach (Point blackListedPoint in blackListedPoints)
                        adjacentPoints.Remove(blackListedPoint);
                    int plotSize = (worldView.GetLength(0) / 5);
                    foreach (Point adjacentPoint in adjacentPoints)
                    {
                        //bool adjacentPointInSameTile = adjacentPoint.X / plotSize == point.X / plotSize && adjacentPoint.Y / plotSize == point.Y / plotSize;
                        if (worldView[adjacentPoint.X, adjacentPoint.Y] is DirtFloor || worldView[adjacentPoint.X, adjacentPoint.Y] is Water || worldView[adjacentPoint.X, adjacentPoint.Y] is Grass || worldView[adjacentPoint.X, adjacentPoint.Y] is PlayerWorld)
                        {
                            //if (adjacentPointInSameTile || explored == true)
                            return true;
                        }
                    }
                    return false;
                }

                void DetermineGranularity()
                {
                    int weight = 6;
                    if (Program.Console.Height <= weight)
                        weight = Program.Console.Height - 1;

                    granularity = Program.WorldMap.HeightMap.GetLength(1) / (Program.Console.Height - weight);
                    while (Program.WorldMap.HeightMap.GetLength(1) % granularity != 0)
                        granularity += 1;
                }

                void DetermineTile(ref List<Point> mountainPoints, int x, int y)
                {
                    Point worldIndex;

                    int plains = 0, trees = 0, dirt = 0, mountains = 0, lakes = 0, unexplored = 0, explored = 0;
                    int iLimit = (x + 1) * granularity;
                    int jLimit = (y + 1) * granularity;
                    for (int i = x * granularity; i < iLimit; i++) {
                        for (int j = y * granularity; j < jLimit; j++) {
                            worldIndex = new Point(i / hectareWidth, j / hectareHeight);
                            Point localPos = new Point(i - (worldIndex.X * hectareWidth), j - (worldIndex.Y * hectareHeight));

                            switch (Program.WorldMap[worldIndex.X, worldIndex.Y][localPos.X, localPos.Y].Explored) {
                                case (true):
                                    explored++;
                                    break;
                                case (false):
                                    unexplored++;
                                    break;
                            }
                            if (Program.WorldMap[worldIndex.X, worldIndex.Y][localPos.X, localPos.Y].Type == BlockType.Wall)
                                mountains++;
                            else if (Program.WorldMap[worldIndex.X, worldIndex.Y][localPos.X, localPos.Y].Type == BlockType.Plant)
                                plains++;
                            else if (Program.WorldMap[worldIndex.X, worldIndex.Y][localPos.X, localPos.Y] is Tree)
                                trees++;
                            else if (Program.WorldMap[worldIndex.X, worldIndex.Y].Floor[localPos.X * Program.WorldMap.TileWidth + localPos.Y] is DirtFloor)
                                dirt++;
                            else if (Program.WorldMap[worldIndex.X, worldIndex.Y].Floor[localPos.X * Program.WorldMap.TileWidth + localPos.Y] is Water)
                                lakes++;
                        }
                    }
                    bool mountainous = false;
                    if (lakes >= plains && lakes >= mountains && lakes >= dirt)
                        worldView[x, y] = new Water();
                    else if (plains >= lakes && plains >= mountains && plains >= dirt)
                        worldView[x, y] = new Grass();
                    else if (mountains >= plains && mountains >= lakes && mountains >= dirt) {
                        mountainous = true;
                        mountainPoints.Add(new Point(x, y));
                        worldView[x, y] = new Wall(Material.Stone);
                    }
                    else if (dirt >= plains && dirt >= lakes && dirt >= mountains)
                        worldView[x, y] = new DirtFloor();
                    if (trees > 3)
                        worldView[x, y] = new ForestTile(worldView[x, y].BackColor);
                    if (unexplored > explored && !mountainous)
                        worldView[x, y] = new Unexplored();
                }


                DetermineGranularity();

                // set map array
                worldView = new Element[Program.WorldMap.HeightMap.GetLength(0) / granularity, Program.WorldMap.HeightMap.GetLength(1) / granularity];

                int xLimit = worldView.GetLength(0);
                int yLimit = worldView.GetLength(1);

                // set the terrain of the world map
                for (int x = 0; x < xLimit; x++)
                    for (int y = 0; y < yLimit; y++)
                        DetermineTile(ref mountainSpots, x, y);

                // set dungeon entrances
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                        if (Program.WorldMap[i, j].DungeonEntrance != null) {
                            Point dungEntrance = new Point(Program.WorldMap[i, j].DungeonEntrance.X + (i * hectareWidth), Program.WorldMap[i, j].DungeonEntrance.Y + (j * hectareHeight));
                            bool tileVisible = TileNextToExploredArea(new Point(dungEntrance.X / granularity, dungEntrance.Y / granularity));
                            if (tileVisible)
                                worldView[(Program.WorldMap[i, j].DungeonEntrance.X + (i * hectareWidth)) / granularity,
                                    (Program.WorldMap[i, j].DungeonEntrance.Y + (j * hectareHeight)) / granularity] = new DownStair(Material.Stone);
                        }

                // set visibility of mountainous terrain
                foreach (Point point in mountainSpots)
                    if (!TileNextToExploredArea(point))
                        worldView[point.X, point.Y] = new Unexplored();

                // set the player
                if (Program.Player.CurrentFloor == -1) // a player that is in a dungeon will be rendered over the dungeon spot
                    worldView[(Program.Player.Position.X + (Program.Player.WorldIndex.X * hectareWidth)) / granularity,
                        (Program.Player.Position.Y + (Program.Player.WorldIndex.Y * hectareHeight)) / granularity] = new PlayerWorld();
                else
                    worldView[(Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].DungeonEntrance.X + (Program.Player.WorldIndex.X * hectareWidth)) / granularity,
                        (Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].DungeonEntrance.Y + (Program.Player.WorldIndex.Y * hectareHeight)) / granularity] = new PlayerWorld();

            }
            static public void View()
            {
                void DrawTooltip(int exploredTiles, int unexploredTiles, Point mWorldIndex, Point mPos) {
                    if (exploredTiles < (unexploredTiles / 2))
                        return;
                    MapTile currentMap = Program.WorldMap[mWorldIndex.X, mWorldIndex.Y];

                    string cost = $"{ currentMap.Cost } gold";
                    if (currentMap.Owned)
                        cost = "owned";

                    int tooltipX = mPos.X + 2, tooltipY = mPos.Y;
                    string tooltipText = $"cost: {cost}";
                    if (tooltipX + tooltipText.Length >= Program.Window.Width - StatusPanel.Width)
                        tooltipX = Program.Window.Width - StatusPanel.Width - tooltipText.Length;

                    Program.Console.Print(tooltipX, tooltipY, tooltipText, Color.Black, Color.AntiqueWhite);
                    if (SadConsole.Global.MouseState.LeftClicked && currentMap.Owned == false)
                        BuyMap(currentMap);
                }

                if (clickedDialog == true)
                {
                    clickedDialog = false;
                    return;
                }

                Play.RenderMap(Program.Player, Program.Console, Program.Window);
                Program.MsgConsole.Render();

                Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                    SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
                int worldViewHeight = worldView.GetLength(1);
                startPoint = new Point( InventoryPanel.Width + ( GUI.MapWidth - InventoryPanel.Width) / 2 - WorldView.GetLength(0) / 2, Program.Console.Height / 2 - worldViewHeight / 2);
                if (Program.Console.Height <= 3)
                    return;
                Point mouseWorldPos = new Point(mousePos.X - startPoint.X, mousePos.Y - startPoint.Y);
                Point mouseWorldIndex = new Point((mouseWorldPos.X * Granularity) / Program.WorldMap.TileHeight, (mouseWorldPos.Y * Granularity) / Program.WorldMap.TileWidth);

                if (mousePos.X < startPoint.X)
                    mouseWorldIndex.X = -1;
                if (mousePos.Y < startPoint.Y)
                    mouseWorldIndex.Y = -1;

                int unexplored = 0, explored = 0;
                for (int i = startPoint.X - 1; i <= startPoint.X + WorldView.GetLength(0); i++) {
                    for (int j = startPoint.Y - 1; j <= startPoint.Y + WorldView.GetLength(1); j++)
                    {
                        Point worldPos = new Point(i - startPoint.X, j - startPoint.Y);
                        if (worldPos.X < 0 || worldPos.Y < 0 || i == startPoint.X + WorldView.GetLength(0) || j == startPoint.Y + WorldView.GetLength(1))
                            Program.Console.SetGlyph(i, j, 178, Color.Gold, Color.Black);
                        else
                        {
                            Point currentIndex = new Point((worldPos.X * Granularity) / Program.WorldMap.TileHeight, (worldPos.Y * Granularity) / Program.WorldMap.TileWidth);
                            MapTile currentMap = Program.WorldMap[currentIndex.X, currentIndex.Y];

                            bool mouseHovering = currentIndex.Equals(mouseWorldIndex);

                            Color foreColor = WorldView[worldPos.X, worldPos.Y].ForeColor;
                            Color backColor = WorldView[worldPos.X, worldPos.Y].BackColor;
                            if (currentMap.Owned == false && !(WorldView[worldPos.X, worldPos.Y] is PlayerWorld) && !mouseHovering)
                            {
                                backColor *= 0.95F;
                                foreColor *= 0.95F;
                            }
                            else if (mouseHovering)
                            {
                                if (WorldView[worldPos.X, worldPos.Y] is Unexplored)
                                    unexplored += 1;
                                else explored += 1;
                                backColor *= 1.2F;
                                foreColor *= 1.2F;
                            }
                            Program.Console.SetGlyph(i, j, WorldView[worldPos.X, worldPos.Y].Graphic,
                                foreColor, backColor);
                        }
                    }
                }

                Program.Console.Print(startPoint.X + WorldView.GetLength( 0 ) / 2 - 4, startPoint.Y - 1, "TOWN MAP", Color.Gold, Color.Black);

                if (mouseWorldIndex.X >= 0 && mouseWorldIndex.X < 4 && mouseWorldIndex.Y >= 0 && mouseWorldIndex.Y < 4)
                    DrawTooltip(explored, unexplored, mouseWorldIndex, mousePos);
                else if (SadConsole.Global.MouseState.LeftClicked)
                {
                    Program.CurrentState = new Play();
                    ClickedDialog = true;
                }
            }
            static public void BuyMap(MapTile map)
            {
                Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
                {
                    Position = new Microsoft.Xna.Framework.Point((Program.Console.Width / 8) * 3, (Program.Console.Height / 8) * 3)
                };

                var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 3);
                var cursor = new SadConsole.Cursor(label);
                label.Position = new Microsoft.Xna.Framework.Point(1, 1);
                cursor.Print($"Would you like to buy this hectare for {map.Cost} gold?");

                var yesButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1)
                {
                    Position = new Microsoft.Xna.Framework.Point(1, (Program.ControlsConsole.Height / 4) * 3),
                    Text = "Yes"
                };
                yesButton.Click += (btn, args) =>
                {
                    clickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    if (map.Cost > Program.Player.Gold)
                        DisplayNotEnoughGold();
                    else
                    {
                        Program.Player.Gold -= map.Cost;
                        map.Owned = true;
                        Program.CurrentState = new ViewWorld();
                    }
                };

                var noButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1);
                noButton.Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width - noButton.Width - 1, (Program.ControlsConsole.Height / 4) * 3);
                noButton.Text = "No";
                noButton.Click += (btn, args) =>
                {
                    clickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = new ViewWorld();
                };

                Program.ControlsConsole.Add(label);
                Program.ControlsConsole.Add(noButton);
                Program.ControlsConsole.Add(yesButton);

                SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
                Program.CurrentState = new DialogWindow();
            }
            static private void DisplayNotEnoughGold()
            {
                Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
                {
                    Position = new Microsoft.Xna.Framework.Point(Program.Console.Width / 8 * 3, Program.Console.Height / 8 * 3)
                };

                var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 3);
                var cursor = new SadConsole.Cursor(label);
                label.Position = new Microsoft.Xna.Framework.Point(1, 1);
                cursor.Print("You don't have enough gold.");

                var okButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
                {
                    Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height / 4 * 3),
                    Text = "Woops!"
                };
                okButton.Click += (btn, args) =>
                {
                    clickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = new ViewWorld();
                };

                Program.ControlsConsole.Add(label);
                Program.ControlsConsole.Add(okButton);

                SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
                Program.CurrentState = new DialogWindow();
            }
            static public void DisplayNotOwned(string action)
            {
                prevGameState = Program.CurrentState;
                Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
                {
                    Position = new Microsoft.Xna.Framework.Point(Program.Console.Width / 8 * 3, Program.Console.Height / 8 * 3)
                };

                var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 5);
                var cursor = new SadConsole.Cursor(label);
                label.Position = new Microsoft.Xna.Framework.Point(1, 1);
                cursor.Print($"You need to buy this hectare to {action}.");

                var okButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
                {
                    Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height / 4 * 3),
                    Text = "Woops!"
                };
                okButton.Click += (btn, args) =>
                {
                    clickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = prevGameState;
                };

                Program.ControlsConsole.Add(label);
                Program.ControlsConsole.Add(okButton);

                SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
                Program.CurrentState = new DialogWindow();
            }
        }

        static public class LoadSave
        {
            static private bool loading = false;
            static private DateTime startLoad = DateTime.Now;
            static private System.Threading.Thread loadThread;

            // FUNCTIONS //
            static public void SaveScreen()
            {
                if (ClickedDialog)
                    ClickedDialog = !ClickedDialog;
                if (!loading) {
                    Program.Console.Clear();
                    loading = true;
                    ReadWrite.LoadObjHoldersFromProgram();
                    loadThread =
                        new System.Threading.Thread( new System.Threading.ThreadStart( ReadWrite.SaveGame ) );
                    loadThread.IsBackground = true;
                    loadThread.Start();
                    Program.Animations = new List<Animation>() { new LoadingAnim( "Saving" ) };
                    ReadWrite.OnFinishedLoading += CloseSaveScreen;
                }
            }
            static public void CloseSaveScreen( object sender, EventArgs e )
            {
                Program.Console.Clear();
                if (Program.Animations.Count > 0)
                    Program.FinishedAnims.Add( Program.Animations[0] );
                Program.CurrentState = new MainMenu();
                loading = false;
                System.GC.Collect();
            }
            static public void LoadScreen()
            {
                if (ClickedDialog)
                    ClickedDialog = !ClickedDialog;
                if (!loading) {
                    Program.Console.Clear();
                    loading = true;
                    loadThread =
                        new System.Threading.Thread( new System.Threading.ThreadStart( ReadWrite.LoadGame ) );
                    loadThread.IsBackground = true;
                    loadThread.Start();
                    Program.Animations = new List<Animation>() { new LoadingAnim( "Loading" ) };
                    DisplayTipMenu();

                    ReadWrite.OnFinishedLoading += CloseLoadScreen;
                }
            }
            static public void CloseLoadScreen( object sender, EventArgs e )
            {
                SadConsole.Global.CurrentScreen.Children.Remove( Program.ControlsConsole );
                Program.Console.Clear();
                if (Program.Animations.Count > 0)
                    Program.FinishedAnims.Add(Program.Animations[0]);
                Program.CurrentState = new Play();
                ReadWrite.SetObjHoldersToProgram();
                loading = false;
                System.GC.Collect();
            }
            static public void DisplayTipMenu()
            {
                Program.ControlsConsole = new BorderedConsole( Program.Console.Width / 2, Program.Console.Height / 4 ) {
                    Position = new Microsoft.Xna.Framework.Point( Program.Console.Width / 4, Program.Console.Height / 8 * 3 )
                };

                string tip = DataReader.GetNextTip();

                var label = new SadConsole.Controls.DrawingSurface( Program.ControlsConsole.Width - 2, 5 );
                var cursor = new Cursor( label );
                label.Position = new Microsoft.Xna.Framework.Point( 1, 1 );
                cursor.Print( tip );

                var okButton = new SadConsole.Controls.Button( Program.ControlsConsole.Width / 4 ) {
                    Position = new Microsoft.Xna.Framework.Point( Program.ControlsConsole.Width / 8 * 3, Program.ControlsConsole.Height / 4 * 3 ),
                    Text = "Next Tip"
                };
                okButton.Click += ( btn, args ) => {
                    SadConsole.Global.CurrentScreen.Children.Remove( Program.ControlsConsole );
                    DisplayTipMenu();
                };

                Program.ControlsConsole.Add( label );
                Program.ControlsConsole.Add( okButton );

                SadConsole.Global.CurrentScreen.Children.Add( Program.ControlsConsole );

            }
            static public void GenerateDungeonScreen()
            {
                bool loadingFailed = DateTime.Now - startLoad > new TimeSpan(TimeSpan.TicksPerSecond * 5);
                if (loadingFailed)
                    loadThread.Abort();
                if (ClickedDialog)
                    ClickedDialog = !ClickedDialog;
                if (!loading || !loadThread.IsAlive) {
                    startLoad = DateTime.Now;
                    Program.Console.Clear();
                    loading = true;
                    Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].Dungeon = DataReader.GetNextDungeon(Program.Player.Stats.Level.Lvl);
                    Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].Dungeon.OnFinishedGenerating += CloseGenerateDungeonScreen;
                    loadThread =
                        new System.Threading.Thread( new System.Threading.ThreadStart( Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].Dungeon.Init ) );
                    loadThread.IsBackground = true;
                    loadThread.Start();
                    Program.Animations = new List<Animation>() { new LoadingAnim( "Generating Dungeon" ) };
                }
            }
            static public void CloseGenerateDungeonScreen( object sender, EventArgs e )
            {
                Program.Console.Clear();
                if (Program.Animations.Count > 0)
                    Program.FinishedAnims.Add(Program.Animations[0]);
                Program.Player.TakeStairsDown();
                Program.CurrentState = new Play();
                loading = false;
            }
            static public void GenerateWorldMapScreen()
            {
                bool loadingFailed = DateTime.Now - startLoad > new TimeSpan(TimeSpan.TicksPerMinute * 2);
                if (loadingFailed) loadThread.Abort();
                if (ClickedDialog) ClickedDialog = !ClickedDialog;
                if (!loading || !loadThread.IsAlive) {
                    startLoad = DateTime.Now;
                    Program.Console.Clear();
                    loading = true;
                    Program.WorldMap.OnFinishedGenerating += CloseGenerateWorldMapScreen;
                    loadThread =
                        new System.Threading.Thread(new System.Threading.ThreadStart(Program.WorldMap.GenerateWorldMap));
                    loadThread.IsBackground = true;
                    loadThread.Start();
                    Program.Animations = new List<Animation>() { new LoadingAnim("Generating World") };
                }
            }
            static public void CloseGenerateWorldMapScreen(object sender, EventArgs e)
            {
                Program.Console.Clear();
                if (Program.Animations.Count > 0)
                    Program.FinishedAnims.Add(Program.Animations[0]);
                GeneratingWorldMap state = (GeneratingWorldMap)Program.CurrentState;
                CreaturePlacementHelper.PlacePlayer(state.UClass, state.Gender, state.Name);
                Program.CurrentState = new Play();
                loading = false;
            }
        }

        // FUNCTIONS //
        static public void PauseMenu()
        {
            prevGameState = Program.CurrentState;
            Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.Console.Width / 8) * 3, (Program.Console.Height / 8) * 3)
            };
            var returnButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width)
            {
                Position = new Microsoft.Xna.Framework.Point(0, 1),
                Text = "Return to Game"
            };
            returnButton.Click += (btn, args) =>
            {
                Program.Console.Clear();
                clickedDialog = true;
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = prevGameState;
            };
            var saveButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width)
            {
                Position = new Microsoft.Xna.Framework.Point(0, 3),
                Text = "Save & Exit"
            };
            saveButton.Click += (btn, args) =>
            {
                clickedDialog = true;
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = new Saving();
            };

            Program.ControlsConsole.Add(returnButton);
            Program.ControlsConsole.Add(saveButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }
        static public void DeathNotification()
        {

            prevGameState = Program.CurrentState;
            Play.RenderMap(Program.Player, Program.Console, Program.Window);
            Program.MsgConsole.Render();

            Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.Console.Width / 8 * 3, Program.Console.Height / 8 * 3)
            };

            var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 5);
            var cursor = new Cursor(label);
            label.Position = new Microsoft.Xna.Framework.Point(1, 1);
            cursor.Print("You died! Try again, noob!");

            var okButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width - 2)
            {
                Position = new Microsoft.Xna.Framework.Point(1, Program.ControlsConsole.Height / 4 * 3),
                Text = "Return to Main Menu"
            };
            okButton.Click += (btn, args) =>
            {
                clickedDialog = true;
                Program.Console.Clear();
                Program.MsgConsole.Console.Clear();
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = new MainMenu();
                ReadWrite.DeleteSave();
            };

            Program.ControlsConsole.Add(label);
            Program.ControlsConsole.Add(okButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }
        static public void DisplayIncorrectUsage(string text)
        {
            prevGameState = Program.CurrentState;
            Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.Console.Width / 8 * 3, Program.Console.Height / 8 * 3)
            };

            var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 5);
            var cursor = new SadConsole.Cursor(label);
            label.Position = new Microsoft.Xna.Framework.Point(1, 1);
            cursor.Print(text);

            var okButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height / 4 * 3),
                Text = "Woops!"
            };
            okButton.Click += (btn, args) =>
            {
                clickedDialog = true;
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = prevGameState;
            };

            Program.ControlsConsole.Add(label);
            Program.ControlsConsole.Add(okButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }
        static public void DisplayIdentified(string text)
        {
            prevGameState = Program.CurrentState;
            Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.Console.Width / 8 * 3, Program.Console.Height / 8 * 3)
            };

            var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 5);
            var cursor = new SadConsole.Cursor(label);
            label.Position = new Microsoft.Xna.Framework.Point(1, 1);
            cursor.Print(text);

            var okButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height / 4 * 3),
                Text = "Okay"
            };
            okButton.Click += (btn, args) =>
            {
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = prevGameState;
            };

            Program.ControlsConsole.Add(label);
            Program.ControlsConsole.Add(okButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }
        static public void SelectWieldingHand(Item currentlyViewedItem)
        {
            prevGameState = Program.CurrentState;
            
            Program.ControlsConsole = new BorderedConsole(Program.Console.Width / 4, Program.Console.Height / 4)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.Console.Width / 8) * 3, (Program.Console.Height / 8) * 3)
            };

            var label = new SadConsole.Controls.DrawingSurface(Program.Console.Width / 4 - 2, 3);
            var cursor = new SadConsole.Cursor(label);
            label.Position = new Microsoft.Xna.Framework.Point(1, 2);
            cursor.Print($"Would you like to wield the {currentlyViewedItem.Name} in your main hand or your off hand?");

            var leftButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1)
            {
                Position = new Microsoft.Xna.Framework.Point(1, (Program.ControlsConsole.Height / 4) * 3),
                Text = "Main"
            };
            leftButton.Click += (btn, args) =>
            {
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.Player.Wield(Program.Player.Inventory.FindIndex(i => i.Name == currentlyViewedItem.Name), true);
                currentlyViewedItem = null;
                Program.CurrentState = prevGameState;
            };

            var rightButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3 - 1);
            rightButton.Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width - rightButton.Width - 1, (Program.ControlsConsole.Height / 4) * 3);
            rightButton.Text = "Off";
            rightButton.Click += (btn, args) =>
            {
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.Player.Wield(Program.Player.Inventory.FindIndex(i => i.Name == currentlyViewedItem.Name), false);
                currentlyViewedItem = null;
                Program.CurrentState = prevGameState;
            };

            Program.ControlsConsole.Add(label);
            Program.ControlsConsole.Add(leftButton);
            Program.ControlsConsole.Add(rightButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }

        // PROPERTIES //
        static public bool ClickedDialog {
            get { return clickedDialog; }
            set { clickedDialog = value; }
        }
        static public GameState PrevGameState {
            get { return prevGameState; }
            set { prevGameState = value; }
        }
        static public Element[,] WorldView {
            get { return worldView; }
            set { worldView = value; }
        }
        static public int Granularity {
            get { return granularity; }
            set { granularity = value; }
        }
    }

    static class CharacterCreation
    {
        static private string name = "";
        static private Class uclass = new Class();
        static int numOfMajorAttr;
        static int numOfMajorSkills;
        static int numOfMinorSkills;


        // FUNCTIONS //

        static public void CharacterCreationScreen()
        {
            Menus.PrevGameState = Program.CurrentState;
            Program.ControlsConsole = new ControlsConsole(Program.Console.Width, Program.Console.Height)
            {
                Position = new Microsoft.Xna.Framework.Point(0, 0)
            };

            //"CREATE YOUR CHARACTER" LABEL
            int y = 2;
            var label = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 2, 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            label.Print((label.Width / 2) - 10, 0, $"CREATE YOUR CHARACTER");

            //NAME FIELD
            y += 4;
            var nameLabel = new SadConsole.Controls.DrawingSurface(6, 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            nameLabel.Print(0, 0, $"Name:");
            var nameField = new SadConsole.Controls.InputBox(Program.ControlsConsole.Width / 2 - 6)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4 + 6, y),
                Text = $"{name}"
            };

            //SELECT MAJOR ATTRIBUTES
            y += 4;
            var descriptionLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 2, 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            descriptionLabel.Print((label.Width / 2) - 15, 0, $"Select three major attributes:");

            //ENDURANCE
            y +=4;
            var enduranceLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            enduranceLabel.Print(0, 0, $"Endurance:    {Class.AttributeModifiers[Attribute.Endurance] + 40}", Color.AntiqueWhite);
            var enduranceButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            enduranceButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Endurance] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Endurance] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Endurance] = 0;
                    numOfMajorAttr--;
                }
                enduranceLabel.Print(0, 0, $"Endurance:    {Class.AttributeModifiers[Attribute.Endurance] + 40}", Color.AntiqueWhite);
            };

            //STRENGTH
            y += 2;
            var strengthLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            strengthLabel.Print(0, 0, $"Strength:     {Class.AttributeModifiers[Attribute.Strength] + 40}", Color.AntiqueWhite * 0.95F);
            var strengthButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            strengthButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Strength] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Strength] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Strength] = 0;
                    numOfMajorAttr--;
                }
                strengthLabel.Print(0, 0, $"Strength:     {Class.AttributeModifiers[Attribute.Strength] + 40}", Color.AntiqueWhite * 0.95F);
            };

            //AGILITY
            y += 2;
            var agilityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            agilityLabel.Print(0, 0, $"Agility:      {Class.AttributeModifiers[Attribute.Agility] + 40}", Color.AntiqueWhite);
            var agilityButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            agilityButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Agility] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Agility] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Agility] = 0;
                    numOfMajorAttr--;
                }
                agilityLabel.Print(0, 0, $"Agility:      {Class.AttributeModifiers[Attribute.Agility] + 40}", Color.AntiqueWhite);
            };

            //DEXTERITY
            y += 2;
            var dexterityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            dexterityLabel.Print(0, 0, $"Dexterity:    {Class.AttributeModifiers[Attribute.Dexterity] + 40}", Color.AntiqueWhite * 0.95F);
            var dexterityButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            dexterityButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Dexterity] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Dexterity] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Dexterity] = 0;
                    numOfMajorAttr--;
                }
                dexterityLabel.Print(0, 0, $"Dexterity:    {Class.AttributeModifiers[Attribute.Dexterity] + 40}", Color.AntiqueWhite * 0.95F);
            };

            //INTELLIGENCE
            y += 2;
            var intelligenceLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            intelligenceLabel.Print(0, 0, $"Intelligence: {Class.AttributeModifiers[Attribute.Intelligence] + 40}", Color.AntiqueWhite);
            var intelligenceButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            intelligenceButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Intelligence] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Intelligence] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Intelligence] = 0;
                    numOfMajorAttr--;
                }
                intelligenceLabel.Print(0, 0, $"Intelligence: {Class.AttributeModifiers[Attribute.Intelligence] + 40}", Color.AntiqueWhite);
            };

            //WILLPOWER
            y += 2;
            var willpowerLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            willpowerLabel.Print(0, 0, $"Willpower:    {Class.AttributeModifiers[Attribute.Willpower] + 40}", Color.AntiqueWhite * 0.95F);
            var willpowerButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            willpowerButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Willpower] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Willpower] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Willpower] = 0;
                    numOfMajorAttr--;
                }
                willpowerLabel.Print(0, 0, $"Willpower:    {Class.AttributeModifiers[Attribute.Willpower] + 40}", Color.AntiqueWhite * 0.95F);
            };

            //PERSONALITY
            y += 2;
            var personalityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            personalityLabel.Print(0, 0, $"Personality:  {Class.AttributeModifiers[Attribute.Personality] + 40}", Color.AntiqueWhite);
            var personalityButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            personalityButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Personality] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Personality] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Personality] = 0;
                    numOfMajorAttr--;
                }
                personalityLabel.Print(0, 0, $"Personality:  {Class.AttributeModifiers[Attribute.Personality] + 40}", Color.AntiqueWhite);
            };

            //LUCK
            y += 2;
            var luckLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 4, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            luckLabel.Print(0, 0, $"Luck:         {Class.AttributeModifiers[Attribute.Luck] + 40}", Color.AntiqueWhite * 0.95F);
            var luckButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 4)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 2, y),
                Text = "Select"
            };
            luckButton.Click += (btn, args) =>
            {
                if (Class.AttributeModifiers[Attribute.Luck] == 0)
                {
                    if (numOfMajorAttr < 3)
                    {
                        Class.AttributeModifiers[Attribute.Luck] = 10;
                        numOfMajorAttr++;
                    }
                }
                else
                {
                    Class.AttributeModifiers[Attribute.Luck] = 0;
                    numOfMajorAttr--;
                }
                luckLabel.Print(0, 0, $"Luck:         {Class.AttributeModifiers[Attribute.Luck] + 40}", Color.AntiqueWhite * 0.95F);
            };

            //CONTINUE TO SKILL SELECTION
            var continueButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height - 4),
                Text = "Continue"
            };
            continueButton.Click += (btn, args) =>
            {
                if (nameField.Text != "" && numOfMajorAttr == 3)
                {
                    Menus.ClickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = new ChooseSkills();
                    name = nameField.Text;
                }
            };

            //RETURN TO MAIN MENU
            var backButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height - 2),
                Text = "Back"
            };
            backButton.Click += (btn, args) =>
            {
                Menus.ClickedDialog = true;
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = new MainMenu();
            };

            Program.ControlsConsole.Add(label);
            Program.ControlsConsole.Add(nameLabel);
            Program.ControlsConsole.Add(nameField);
            Program.ControlsConsole.Add(descriptionLabel);

            Program.ControlsConsole.Add(enduranceLabel);
            Program.ControlsConsole.Add(enduranceButton);
            Program.ControlsConsole.Add(strengthLabel);
            Program.ControlsConsole.Add(strengthButton);
            Program.ControlsConsole.Add(agilityLabel);
            Program.ControlsConsole.Add(agilityButton);
            Program.ControlsConsole.Add(dexterityLabel);
            Program.ControlsConsole.Add(dexterityButton);
            Program.ControlsConsole.Add(intelligenceLabel);
            Program.ControlsConsole.Add(intelligenceButton);
            Program.ControlsConsole.Add(willpowerLabel);
            Program.ControlsConsole.Add(willpowerButton);
            Program.ControlsConsole.Add(personalityLabel);
            Program.ControlsConsole.Add(personalityButton);
            Program.ControlsConsole.Add(luckLabel);
            Program.ControlsConsole.Add(luckButton);

            Program.ControlsConsole.Add(continueButton);
            Program.ControlsConsole.Add(backButton);

            Program.ControlsConsole.MouseMove += (mouse, args) =>
            {
                name = nameField.Text;
            };

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            SadConsole.Global.FocusedConsoles.Set(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }

        static public void SkillSelectionScreen()
        {
            int maxSkills = 4;

            Menus.PrevGameState = Program.CurrentState;
            Program.ControlsConsole = new ControlsConsole(Program.Console.Width - 1, Program.Console.Height) {
                Position = new Microsoft.Xna.Framework.Point(1, 0)
            };

            //CHOOSE FOUR MAJOR AND MINOR SKILLS
            int y = 2;
            var label = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 2, 3) {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 4, y)
            };
            label.Print((label.Width / 2) - 17, 0, $"CHOOSE FOUR [c:r f:RoyalBlue:5]MAJOR AND [c:r f:ForestGreen:5]MINOR SKILLS");

            //ENDURANCE
            y = Program.ControlsConsole.Height / 2 - 8;
            var enduranceLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1) {
                Position = new Microsoft.Xna.Framework.Point(0, y)
            };
            enduranceLabel.Print(0, 0, "Endurance:");
            //HEAVY ARMOR
            y += 4;
            var heavyArmor = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(0, y), "Hvy Armor", Skill.HeavyArmor);
            //SPEAR
            y += 4;
            var spear = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point( 0, y ), "Spear", Skill.Spear);
            //BLOCK
            y += 4;
            var block = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point( 0, y ), "Blocking", Skill.Block);
            //SWIMMING
            y += 4;
            var swimming = SkillButton( Program.ControlsConsole.Width / 7 - 1, new Point( 0, y ), "Swimming", Skill.Swimming );

            //STRENGTH
            y = Program.ControlsConsole.Height / 2 - 8;
            var strengthLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 7, y)
            };
            strengthLabel.Print(0, 0, "Strength:");

            //ACROBATICS
            y += 4;
            var acrobatics = SkillButton( Program.ControlsConsole.Width / 7 - 1, new Point( Program.ControlsConsole.Width / 7, y ), "Acrobatics", Skill.Acrobatics );
            //BRAWLING
            y += 4;
            var brawling = SkillButton( Program.ControlsConsole.Width / 7 - 1, new Point( Program.ControlsConsole.Width / 7, y ), "Brawling", Skill.Brawling );
            //FORGING
            y += 4;
            var forging = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7, y), "Forging", Skill.Forging);
            //HEAVY WEAPONS
            y += 4;
            var heavyWeapons = SkillButton( Program.ControlsConsole.Width / 7 - 1, new Point( Program.ControlsConsole.Width / 7, y ), "Hvy Weapons", Skill.HeavyWeapons );

            //AGILITY
            y = Program.ControlsConsole.Height / 2 - 8;
            var agilityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.ControlsConsole.Width / 7) * 2, y)
            };
            agilityLabel.Print(0, 0, "Agility:");

            //LIGHT ARMOR
            y += 4;
            var lightArmor = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 2, y), "Lgt Armor", Skill.LightArmor);
            //UNARMORED
            y += 4;
            var unarmored = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 2, y), "Unarmored", Skill.Unarmored);
            //SHORT BLADE
            y += 4;
            var shortBlade = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 2, y), "Short Bld", Skill.ShortBlade);
            //SNEAK
            y += 4;
            var stealth = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 2, y), "Stealth", Skill.Sneaking);
            
            //DEXTERITY
            y = Program.ControlsConsole.Height / 2 - 8;
            var dexterityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1) {
                Position = new Microsoft.Xna.Framework.Point((Program.ControlsConsole.Width / 7) * 3, y)
            };
            dexterityLabel.Print(0, 0, "Dexterity:");

            //MARKSMAN
            y += 4;
            var marksman = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 3, y), "Marksman", Skill.Marksmanship);
            //LOCKPICK
            y += 4;
            var lockpick = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 3, y), "Lockpick", Skill.Lockpick);
            //LONG BLADE
            y += 4;
            var longBlade = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 3, y), "Long Bld", Skill.LongBlades);
            //CRAFTING
            y += 4;
            var crafting = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 3, y), "Crafting", Skill.Crafting);
            crafting.IsEnabled = false;

            //INTELLIGENCE
            y = Program.ControlsConsole.Height / 2 - 8;
            var intelligenceLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.ControlsConsole.Width / 7) * 4, y)
            };
            intelligenceLabel.Print(0, 0, "Intelligence:");

            //ILLUSION
            y += 4;
            var illusion = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 4, y), "Illusion", Skill.Illusion);
            //ALCHEMY
            y += 4;
            var alchemy = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 4, y), "Alchemy", Skill.Alchemy);
            //ENCHANT
            y += 4;
            var enchant = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 4, y), "Enchantment", Skill.Enchant);

            //WILLPOWER
            y = Program.ControlsConsole.Height / 2 - 8;
            var willpowerLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.ControlsConsole.Width / 7) * 5, y)
            };
            willpowerLabel.Print(0, 0, "Willpower:");

            //CONJURATION
            y += 4;
            var necromancy = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 5, y), "Necromancy", Skill.Conjuration);
            //RESTORATION
            y += 4;
            var restoration = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 5, y), "Restoration", Skill.Restoration);
            //DESTRUCTION
            y += 4;
            var destruction = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 5, y), "Destruction", Skill.Destruction);

            //PERSONALITY
            y = Program.ControlsConsole.Height / 2 - 8;
            var personalityLabel = new SadConsole.Controls.DrawingSurface(Program.ControlsConsole.Width / 7 - 1, 1)
            {
                Position = new Microsoft.Xna.Framework.Point((Program.ControlsConsole.Width / 7) * 6, y)
            };
            personalityLabel.Print(0, 0, "Personality:");

            //MERCANTILE
            y += 4;
            var mercantile = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 6, y), "Mercantile", Skill.Mercantile);
            //SPEECHCRAFT
            y += 4;
            var speechcraft = SkillButton(Program.ControlsConsole.Width / 7 - 1, new Point(Program.ControlsConsole.Width / 7 * 6, y), "Speech", Skill.Speech);

            //PLAY GAME
            var playButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3) {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height - 4),
                Text = "Play!"
            };
            playButton.Click += (btn, args) => {
                if (numOfMajorSkills == maxSkills && numOfMinorSkills == maxSkills) {
                    numOfMajorAttr = 0;
                    numOfMajorSkills = 0;
                    numOfMinorSkills = 0;
                    Menus.ClickedDialog = true;
                    SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                    Program.CurrentState = new GeneratingWorldMap(uclass, "male", name);

                    Program.WorldMap = new WorldMap(100, 100, "Test");
                    Program.Identification = new Identification(true);
                    Program.TimeHandler = new TimeHandler(8, 0, 0, 1);
                    Program.Factions = new List<Faction>();
                    name = "";
                    uclass = new Class();
                }
            };

            //BACK TO CHARACTER CREATION
            var backButton = new SadConsole.Controls.Button(Program.ControlsConsole.Width / 3) {
                Position = new Microsoft.Xna.Framework.Point(Program.ControlsConsole.Width / 3, Program.ControlsConsole.Height - 2),
                Text = "Back"
            };
            backButton.Click += (btn, args) => {
                Menus.ClickedDialog = true;
                SadConsole.Global.CurrentScreen.Children.Remove(Program.ControlsConsole);
                Program.CurrentState = new CreateCharacter();
            };
            Program.ControlsConsole.Add(label);

            Program.ControlsConsole.Add(enduranceLabel);
            spear.IsEnabled = false;
            Program.ControlsConsole.Add(heavyArmor);
            Program.ControlsConsole.Add(spear);
            Program.ControlsConsole.Add(block);
            Program.ControlsConsole.Add( swimming );

            Program.ControlsConsole.Add( strengthLabel );
            forging.IsEnabled = false;
            Program.ControlsConsole.Add( acrobatics );
            Program.ControlsConsole.Add( brawling );
            Program.ControlsConsole.Add( forging );
            Program.ControlsConsole.Add( heavyWeapons );

            Program.ControlsConsole.Add(agilityLabel);
            stealth.IsEnabled = false;
            Program.ControlsConsole.Add(lightArmor);
            Program.ControlsConsole.Add(unarmored);
            Program.ControlsConsole.Add(shortBlade);
            Program.ControlsConsole.Add(stealth);

            Program.ControlsConsole.Add(dexterityLabel);
            Program.ControlsConsole.Add(marksman);
            Program.ControlsConsole.Add(lockpick);
            Program.ControlsConsole.Add(longBlade);
            Program.ControlsConsole.Add(crafting);

            Program.ControlsConsole.Add(intelligenceLabel);
            illusion.IsEnabled = false;
            alchemy.IsEnabled = false;
            enchant.IsEnabled = false;
            Program.ControlsConsole.Add(illusion);
            Program.ControlsConsole.Add(alchemy);
            Program.ControlsConsole.Add(enchant);

            Program.ControlsConsole.Add(willpowerLabel);
            necromancy.IsEnabled = false;
            restoration.IsEnabled = false;
            destruction.IsEnabled = false;
            Program.ControlsConsole.Add(necromancy);
            Program.ControlsConsole.Add(restoration);
            Program.ControlsConsole.Add(destruction);

            Program.ControlsConsole.Add(personalityLabel);
            speechcraft.IsEnabled = false;
            Program.ControlsConsole.Add(mercantile);
            Program.ControlsConsole.Add(speechcraft);

            Program.ControlsConsole.Add(backButton);
            Program.ControlsConsole.Add(playButton);

            SadConsole.Global.CurrentScreen.Children.Add(Program.ControlsConsole);
            Program.CurrentState = new DialogWindow();
        }

        static private SadConsole.Controls.Button SkillButton(int width, Point position, string text, Skill skill, bool disabled = false)
        {
            int maxSkills = 4;
            SadConsole.Themes.ButtonTheme majorSkillTheme = new SadConsole.Themes.ButtonTheme()
            {
                Focused = new SadConsole.Cell(Color.AntiqueWhite, Color.RoyalBlue),
                Normal = new SadConsole.Cell(Color.AntiqueWhite, Color.RoyalBlue),
                MouseOver = new SadConsole.Cell(Color.AntiqueWhite, Color.RoyalBlue * 1.2F),
                MouseClicking = new SadConsole.Cell(Color.RoyalBlue * 1.2F, Color.AntiqueWhite),
                Disabled = new SadConsole.Cell(Color.AntiqueWhite, Color.RoyalBlue * 0.8F)
            };
            SadConsole.Themes.ButtonTheme minorSkillTheme = new SadConsole.Themes.ButtonTheme() {
                Focused = new SadConsole.Cell(Color.AntiqueWhite, Color.ForestGreen),
                Normal = new SadConsole.Cell(Color.AntiqueWhite, Color.ForestGreen),
                MouseOver = new SadConsole.Cell(Color.AntiqueWhite, Color.ForestGreen * 1.2F),
                MouseClicking = new SadConsole.Cell(Color.ForestGreen * 1.2F, Color.AntiqueWhite),
                Disabled = new SadConsole.Cell( Color.AntiqueWhite, Color.ForestGreen * 0.8F )
            };

            var temp = new SadConsole.Controls.Button(width, 1)
            {
                Position = new Microsoft.Xna.Framework.Point(position.X, position.Y),
                Text = text
            };

            if (Class.MajorSkills.Contains(skill))
            {
                temp.Theme = majorSkillTheme;
            }
            else if (Class.MinorSkills.Contains(skill))
            {
                temp.Theme = minorSkillTheme;
            }
            temp.DetermineAppearance();
            temp.Compose(true);

            temp.Click += (btn, args) =>
            {
                if (!Class.MajorSkills.Contains(skill) && !Class.MinorSkills.Contains(skill) && numOfMajorSkills < maxSkills)
                {
                    Class.MajorSkills.Add(skill);
                    temp.Theme = majorSkillTheme;
                    temp.Compose(true);
                    numOfMajorSkills++;
                }
                else if (!Class.MinorSkills.Contains(skill) && numOfMinorSkills < maxSkills)
                {
                    if (Class.MajorSkills.Contains(skill))
                    {
                        Class.MajorSkills.Remove(skill);
                        numOfMajorSkills--;
                    }
                    Class.MinorSkills.Add(skill);
                    temp.Theme = minorSkillTheme;
                    temp.Compose(true);
                    numOfMinorSkills++;
                }
                else
                {
                    if (Class.MajorSkills.Contains(skill))
                    {
                        Class.MajorSkills.Remove(skill);
                        numOfMajorSkills--;
                    }
                    else if (Class.MinorSkills.Contains(skill))
                    {
                        Class.MinorSkills.Remove(skill);
                        numOfMinorSkills--;
                    }
                    temp.Theme = null;
                    temp.Compose(true);
                }
            };

            return temp;
        }

        // PROPERTIES //
        static public string Name {
            get { return name; }
            set { name = value; }
        }
        static public Class Class {
            get { return uclass; }
            set { uclass = value; }
        }
    }

    class BorderedConsole : SadConsole.ControlsConsole
    {
        private SadConsole.Surfaces.BasicSurface borderSurface;

        public BorderedConsole(int width, int height) : base(width, height)
        {
            borderSurface = new SadConsole.Surfaces.BasicSurface(width + 2, height + 2, base.textSurface.Font);

            var editor = new SadConsole.Surfaces.SurfaceEditor(borderSurface);

            SadConsole.Shapes.Box box = SadConsole.Shapes.Box.Thick();
            box.Width = borderSurface.Width;
            box.Height = borderSurface.Height;
            box.Draw(editor);

            base.Renderer.Render(borderSurface);
        }

        public override void Draw(TimeSpan delta)
        {
            // Draw our border to the screen
            Global.DrawCalls.Add(new DrawCallSurface(borderSurface, this.Position - new Microsoft.Xna.Framework.Point(1), UsePixelPositioning));

            // Draw the main console to the screen
            base.Draw(delta);
        }
    }
}
