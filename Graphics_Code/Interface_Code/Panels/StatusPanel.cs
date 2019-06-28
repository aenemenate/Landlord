using System;
using System.Collections.Generic;
using SadConsole;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Landlord
{
    static class StatusPanel
    {
        static private int width = 22;

        static private int tabStartY = 6; // tab innards start at statusStartX, 6. They end at statusStartX, console.Height - 3;
        static private int selectedTab = 0; // 0 == locals, 1 == skills, 2 == quests
        static private Creature curCreature;

        static private int previousHP = Program.Player.Stats.Resources[Resource.HP], previousMP = Program.Player.Stats.Resources[Resource.MP], previousSP = Program.Player.Stats.Resources[Resource.SP];
        static private Color statusColorLighter = new Color( 161, 97, 102 ), statusColor = new Color( 136, 71, 76 ), statusColorDarker = new Color( 122, 45, 51 );

        // FUNCTIONS

        public static void HandleStatus()
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            RenderStatus();
            bool leftClicked = SadConsole.Global.MouseState.LeftClicked;
            bool shiftPressed = leftClicked && Global.KeyboardState.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftShift );

            if (leftClicked && Program.CurrentState is Play) {
                if (curCreature != null) {
                    Program.CurrentState = new CharacterSheet(curCreature);
                }
                else {
                    bool mouseOverTabs = mousePos.Y >= tabStartY - 3 && mousePos.Y <= tabStartY - 1 && mousePos.X > StartX;
                    if (mouseOverTabs)
                    {
                        if (mousePos.X < StartX + 7)
                            selectedTab = 0;
                        else if (mousePos.X > StartX + 7 && mousePos.X < StartX + 14)
                            selectedTab = 1;
                        else if (mousePos.X > StartX + 14 && mousePos.X < Program.Window.Width - 1)
                            selectedTab = 2;
                    }
                }
            }
            curCreature = null;
        }
        // returns true if an entry in a displayed list is being highlighted
        public static void RenderStatus()
        {
            Color textColor = Color.AntiqueWhite, highlightedColor = Color.OrangeRed, bgColor = statusColorDarker;

            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            // FUNCTIONS //

            void PrintLookFunc()
            {
                Point worldIndex = Program.Player.WorldIndex;
                int currentFloor = Program.Player.CurrentFloor;
                Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                Tile[] tiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Floor : Program.WorldMap[worldIndex.X, worldIndex.Y].Floor;
                int width = Program.WorldMap[worldIndex.X, worldIndex.Y].Width;
                Point mapPos = Program.WorldMap[worldIndex.X, worldIndex.Y].GetMousePos(mousePos);

                bool mouseIsOnMap = !(mousePos.X < 0 || mousePos.X >= Program.Console.Width - StatusPanel.Width);

                Program.Window.Print(StartX + 1, Program.Window.Height - 3, "                  ", 18);
                Program.Window.Print(StartX + 1, Program.Window.Height - 2, "                  ", 18);

                if (Program.CurrentState is Play && Program.WorldMap[worldIndex.X, worldIndex.Y].PointWithinBounds(mapPos) && mouseIsOnMap && blocks[mapPos.X * width + mapPos.Y].Explored) {
                    if (blocks[mapPos.X * width + mapPos.Y] is Item i) // print item name
                        Program.Window.Print(StartX + 1, Program.Window.Height - 3, i.Name, 18);
                    else if (blocks[mapPos.X * width + mapPos.Y].Type != BlockType.Empty) // print block name
                        Program.Window.Print(StartX + 1, Program.Window.Height - 3, blocks[mapPos.X * width + mapPos.Y].Name, 18);
                    else
                        Program.Window.Print(StartX + 1, Program.Window.Height - 3, tiles[mapPos.X * width + mapPos.Y].Name, 18);
                }
            }

            void PrintBgColor()
            {
                // fill the background
                if (!Program.Console.GetBackground( Program.Window.Width - 1, 0 ).Equals( statusColorDarker ))
                    for (int i = Program.Window.Width - 1; i >= StartX; i--)
                        for (int j = 0; j < Program.Console.Height; j++)
                            Program.Console.SetGlyph( i, j, ' ', statusColorDarker, statusColorDarker );
            }

            void DrawResourceBar(int y, int hp, int maxhp, int barWidth, Color barColor, string text = "")
            {
                for (int i = 0; i < barWidth; i++)
                {
                    if (( ( i * maxhp ) / barWidth ) + 1 <= hp && hp != 0)
                        Program.Console.SetGlyph( StartX + 1 + i, y, 178, barColor, barColor );
                    else
                        Program.Console.Print( StartX + 1 + i, y, " ", bgColor * 0.98F, bgColor * 0.98F );
                }

                string healthNum = text == "" ? $"{hp}/{maxhp}" : text;

                int numStartX = StartX + 1 + barWidth / 2 - healthNum.Length / 2;

                Program.Console.Print( numStartX, y, healthNum, textColor );
            }

            void PrintTime()
            {
                string hour = $"{Program.TimeHandler.CurrentTime.Hour}";
                string minute = $"{Program.TimeHandler.CurrentTime.Minute}";
                string second = $"{Program.TimeHandler.CurrentTime.Second}";

                if (hour.Length == 1)
                    hour = "0" + hour;

                if (minute.Length == 1)
                    minute = "0" + minute;

                if (second.Length == 1)
                    second = "0" + second;

                string time = hour + ":" + minute + ":" + second;

                Program.Console.Print( StartX + ( width / 2 - time.Length / 2 ), tabStartY - 4, time, textColor );
            }

            void PrintTabs()
            {
                Color borderColor = textColor * 0.95F;

                byte topLeft = 218, topRight = 191, bottomLeft = 192, bottomRight = 217, bottomLeftT = 195, bottomRightT = 180, horizontal = 196, vertical = 179, topT = 194, bottomT = 193;

                void PrintTabMarker( int tab )
                {
                    int leftMargin = 1;
                    string tabName = "locals";

                    if (tab != 0)
                    {
                        leftMargin = tab == 1 ? 8 : 15;
                        tabName = tab == 1 ? "skills" : "quests";
                    }
                    Program.Console.Print( StartX + leftMargin, tabStartY - 2, tabName, textColor );
                    if (tab == selectedTab)
                    {
                        Program.Console.Print( StartX + leftMargin, tabStartY - 1, "      " );
                        Program.Console.SetGlyph( StartX + leftMargin - 1, tabStartY - 1, selectedTab != 0 ? bottomRight : vertical, borderColor );
                        Program.Console.SetGlyph( StartX + leftMargin + 6, tabStartY - 1, selectedTab != 2 ? bottomLeft : vertical, borderColor );
                    }
                }

                // print tabs and their borders
                for (int i = StartX; i < Program.Console.Width; i++)
                    for (int j = tabStartY - 3; j < tabStartY; j++)
                    {
                        byte character = horizontal;
                        if (j == tabStartY - 3 || j == tabStartY - 1)
                        {
                            if (i == StartX)
                                character = j == tabStartY - 3 ? topLeft : bottomLeftT;
                            else if (i == Program.Window.Width - 1)
                                character = j == tabStartY - 3 ? topRight : bottomRightT;
                            else if (i == StartX + 7 || i == StartX + 14)
                                character = j == tabStartY - 3 ? topT : bottomT;
                        } else
                            character = vertical;
                        Program.Console.SetGlyph( i, j, character, borderColor );
                    }

                if (Program.Console.GetGlyph( StartX, tabStartY ) != vertical)
                    // print left and right side of tab window border
                    for (int j = tabStartY; j < Program.Window.Height - 1; j++)
                    {
                        Program.Console.SetGlyph( StartX, j, vertical, borderColor );
                        Program.Console.SetGlyph( Program.Window.Width - 1, j, vertical, borderColor );
                    }

                // bottom row
                for (int i = StartX + 1; i < Program.Window.Width - 1; i++)
                    Program.Console.SetGlyph( i, Program.Window.Height - 1, horizontal, borderColor );
                Program.Console.SetGlyph( StartX, Program.Window.Height - 1, bottomLeft, borderColor );
                Program.Console.SetGlyph( Program.Window.Width - 1, Program.Window.Height - 1, bottomRight, borderColor );

                PrintTabMarker( 0 );
                PrintTabMarker( 1 );
                PrintTabMarker( 2 );

                for (int j = tabStartY; j < Program.Window.Height - 1; j++)
                    Program.Console.Print( StartX + 1, j, "                    ", statusColorDarker, statusColorDarker );

                switch (selectedTab) {
                    case 0: // locals
                        PrintLocals();
                        return;
                    case 1: // skills
                        PrintSkills();
                        return;
                    case 2: // quests
                        PrintQuests();
                        return;
                }
            }

            void PrintLocals()
            {
                List<Creature> locals = Program.Player.LocalCreatures;
                locals.Sort( ( x, y ) => x.Position.DistFrom(Program.Player.Position).CompareTo( y.Position.DistFrom( Program.Player.Position ) ) );
                int heightofDisplay = Program.Console.Height - 3 - tabStartY;

                //float bgAlpha = 0.99F, fgAlpha = 0.99F; //might use these at some point
                
                void PrintCreatures ()
                {
                    int padding = 0;
                    for (int i = 1, index = 0; i < heightofDisplay && index < locals.Count; i += padding, index++)
                    {
                        int curY = i + tabStartY;
                        Color tempBG = bgColor;
                        Creature creature = locals[index];
                        if (mousePos.Y >= curY && mousePos.Y <= curY + 1 & mousePos.X > StartX + 1 && mousePos.X < Program.Window.Width - 1) {
                            tempBG = bgColor * 1.3F;
                            curCreature = creature;
                        }
                        // draw creature name
                        curY += Program.Window.Print( StartX + 1, curY, creature.Name + " / LVL " + creature.Stats.Level.Lvl, 20, textColor, tempBG);
                        // draw creature hp
                        DrawResourceBar(curY, creature.Stats.Resources[Resource.HP], creature.Stats.Resources[Resource.MaxHP], 20, new Color(45, 51, 122));
                        curY++;
                        // for each stat, draw if appropriate
                        if (creature is Player) {
                            DrawResourceBar(curY, creature.Stats.Resources[Resource.HV], creature.Stats.Resources[Resource.MaxHV], 20, new Color(45, 122, 51));
                            curY++;
                            DrawResourceBar( curY, creature.Stats.Resources[Resource.MP], creature.Stats.Resources[Resource.MaxMP], 20, new Color( 45, 122, 116 ) );
                            curY++;
                            DrawResourceBar( curY, creature.Stats.Resources[Resource.SP], creature.Stats.Resources[Resource.MaxSP], 20, new Color( 122, 116, 45 ) );
                            curY++;
                        }
                        // draw status effects
                        foreach (Effect effect in creature.Effects) {
                            DrawResourceBar( curY, effect.Turns, effect.TotalTurns, 20, new Color( 122, 45, 90 ), effect.Name );
                            curY++;
                        }
                        // draw creature goal, if appropriate

                        padding = 1 + curY - i - tabStartY;
                    }
                }

                PrintCreatures();
            }

            bool PrintSkills()
            {
                Program.Window.Print( StartX + 1, tabStartY + 1, "You haven't learned any skills.", width - 3, Color.LightGray );
                return false;
            }

            bool PrintQuests()
            {
                Program.Window.Print( StartX + 1, tabStartY + 1, "You haven't embarked upon any quests.", width - 3, Color.LightGray );
                return false;
            }

            // START //
            PrintBgColor();
            PrintTime();
            PrintTabs();
            PrintLookFunc();
        }

        // PROPERTIES //

        static public int Width
        {
            get { return width; }
        }
        static public int StartX
        {
            get { return Program.Window.Width - width; }
        }
    }

}
