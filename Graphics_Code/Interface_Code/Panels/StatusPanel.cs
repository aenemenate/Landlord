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

        static private int previousHP = Program.Player.Stats.Resources[Resource.HP], previousMP = Program.Player.Stats.Resources[Resource.MP], previousSP = Program.Player.Stats.Resources[Resource.SP];
        static private Color statusColorLighter = new Color( 161, 97, 102 ), statusColor = new Color( 136, 71, 76 ), statusColorDarker = new Color( 122, 45, 51 );

        // FUNCTIONS

        public static bool HandleStatus()
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            bool DetermineCurrentItem( bool itemSelected )
            {
                // determine if a creature, status effect, or quest is selected
                return false;
            }

            // item selection logic
            bool itemHoveredOver = RenderStatus();
            bool leftClicked = SadConsole.Global.MouseState.LeftClicked;
            bool shiftPressed = leftClicked && Global.KeyboardState.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftShift );

            if (leftClicked && Program.CurrentState is Play)
            {
                bool determined = DetermineCurrentItem( itemHoveredOver );
                if (determined)
                {
                } else
                {
                    bool mouseOverSkillsButton = mousePos.X >= StartX + 1 && mousePos.X <= StartX + 8 && mousePos.Y == 11;
                    bool mouseOverStatsButton = mousePos.Y == 11 && mousePos.X >= Program.Console.Width - 1 - "[STATS]".Length && mousePos.X <= Program.Console.Width - 2;
                    bool mouseOverTabs = mousePos.Y >= tabStartY - 3 && mousePos.Y <= tabStartY - 1;
                    if (mouseOverSkillsButton)
                    {
                        Program.Animations.Add( new OpenStatusView( statusColor ) );
                        Program.CurrentState = new ViewSkills();
                    } else if (mouseOverStatsButton)
                    {
                        Program.Animations.Add( new OpenStatusView( statusColor ) );
                        Program.CurrentState = new ViewAttributes();
                    } else if (mouseOverTabs)
                    {
                        if (mousePos.X > StartX && mousePos.X < StartX + 7)
                            selectedTab = 0;
                        else if (mousePos.X > StartX + 7 && mousePos.X < StartX + 14)
                            selectedTab = 1;
                        else if (mousePos.X > StartX + 14 && mousePos.X < Program.Window.Width - 1)
                            selectedTab = 2;
                    }
                }
            }

            return itemHoveredOver;
        }

        public static bool RenderStatus()
        {
            Color textColor = Color.AntiqueWhite, highlightedColor = Color.OrangeRed, bgColor = statusColorDarker;

            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            // FUNCTIONS //

            void PrintLookFunc()
            {
                Point mapPos = Program.WorldMap.LocalTile.GetMousePos( mousePos );

                bool mouseIsOnMap = !( mousePos.X < 0 || mousePos.X >= Program.Console.Width - StatusPanel.Width );

                Program.Window.Print( StartX + 1, Program.Window.Height - 3, "                  ", 18 );

                if (Program.CurrentState is Play && Program.WorldMap.LocalTile.PointWithinBounds( mapPos ) && mouseIsOnMap && Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Explored)
                {
                    if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Type != BlockType.Empty)
                    {
                        if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is Item item)
                        {
                            Program.Window.Print( StartX + 1, Program.Window.Height - 3, item.Name, 20 );
                            Tuple<byte, Color> comparisonArrow = GUI.GetItemArrow( item );
                            int arrowX = StartX + 1 + ( item.Name.Length > 20 ? item.Name.Length % 20 : item.Name.Length ) + 1,
                                arrowY = Program.Window.Height - ( 3 - item.Name.Length / 20 );
                            if (comparisonArrow.Item1 != 0)
                                Program.Console.SetGlyph( arrowX, arrowY, comparisonArrow.Item1, comparisonArrow.Item2 );
                        } else if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y] is Creature creature)
                        {
                            if (creature is Player == false)
                                Program.Window.Print( StartX + 1, Program.Window.Height - 3, creature.Name
                                    + $" ({creature.Stats.Resources[Resource.HP]}/{creature.Stats.Resources[Resource.MaxHP]} hp)", 20 );
                            else
                                Program.Window.Print( StartX + 1, Program.Window.Height - 3, $"You, on {Program.Player.CurrentBlock.Name}.", 20 );
                        } else
                            Program.Window.Print( StartX + 1, Program.Window.Height - 3, Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Name, 20 );
                    } else
                        Program.Window.Print( StartX + 1, Program.Window.Height - 3, Program.WorldMap.LocalTile.Floor[mapPos.X * Program.WorldMap.LocalTile.Width + mapPos.Y].Name, 20 );
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

            //void PrintResourceBars()
            //{
            //    if (Program.Player.Stats.Resources[Resource.HP] != previousHP || Program.Console.GetGlyph( StartX + 1, 4 ) != 'H')
            //    {
            //        int hp = Program.Player.Stats.Resources[Resource.HP], maxhp = Program.Player.Stats.Resources[Resource.MaxHP];
            //        int hpBarOffset = 5;
            //        int hpBarWidth = width - ( hpBarOffset + 1 );

            //        Program.Console.Print( StartX + 1, 4, "HP: ", textColor );

            //        for (int i = 0; i < hpBarWidth; i++)
            //        {
            //            if (( ( i * maxhp ) / hpBarWidth ) + 1 <= hp && hp != 0)
            //                Program.Console.SetGlyph( StartX + hpBarOffset + i, 4, 178, Color.Red, Color.Red );
            //            else
            //                Program.Console.Print( StartX + hpBarOffset + i, 4, " ", statusColor, statusColor );
            //        }

            //        string healthNum = $"{hp}/{maxhp}";

            //        int numStartX = StartX + hpBarOffset + hpBarWidth / 2 - healthNum.Length / 2;

            //        Program.Console.Print( numStartX, 4, healthNum, textColor );

            //        previousHP = hp;
            //    } else if (Program.Player.Stats.Resources[Resource.MP] != previousMP || Program.Console.GetGlyph( StartX + 1, 6 ) != 'M')
            //    {
            //        int mp = Program.Player.Stats.Resources[Resource.MP], maxmp = Program.Player.Stats.Resources[Resource.MaxMP];
            //        int mpBarOffset = 5;
            //        int mpBarWidth = width - ( mpBarOffset + 1 );

            //        Program.Console.Print( StartX + 1, 6, "MP: ", textColor );

            //        for (int i = 0; i < mpBarWidth; i++)
            //        {
            //            if (( ( i * mp ) / mpBarWidth ) + 1 <= mp && mp != 0)
            //                Program.Console.SetGlyph( StartX + mpBarOffset + i, 6, 178, Color.RoyalBlue, Color.RoyalBlue );
            //            else
            //                Program.Console.Print( StartX + mpBarOffset + i, 6, " ", statusColor, statusColor );
            //        }

            //        string manaNum = $"{mp}/{maxmp}";

            //        int numStartX = StartX + mpBarOffset + mpBarWidth / 2 - manaNum.Length / 2;

            //        Program.Console.Print( numStartX, 6, manaNum, textColor );

            //        previousMP = mp;
            //    } else if (Program.Player.Stats.Resources[Resource.SP] != previousSP || Program.Console.GetGlyph( StartX + 1, 8 ) != 'S')
            //    {
            //        int sp = Program.Player.Stats.Resources[Resource.SP], maxsp = Program.Player.Stats.Resources[Resource.MaxSP];
            //        int spBarOffset = 5;
            //        int spBarWidth = width - ( spBarOffset + 1 );

            //        Program.Console.Print( StartX + 1, 8, "SP: ", textColor );

            //        for (int i = 0; i < spBarWidth; i++)
            //        {
            //            if (( ( i * maxsp ) / spBarWidth ) + 1 <= sp && sp != 0)
            //                Program.Console.SetGlyph( StartX + spBarOffset + i, 8, 178, Color.YellowGreen, Color.YellowGreen );
            //            else
            //                Program.Console.Print( StartX + spBarOffset + i, 8, " ", statusColor, statusColor );
            //        }

            //        string staminaNum = $"{sp}/{maxsp}";

            //        int numStartX = StartX + spBarOffset + spBarWidth / 2 - staminaNum.Length / 2;

            //        Program.Console.Print( numStartX, 8, staminaNum, textColor );

            //        previousSP = sp;
            //    }
            //}

            //void PrintSkillsStats()
            //{
            //    bool highlighting = false;

            //    int y = 11;

            //    if (mousePos.Y == y && mousePos.X >= StartX + 1 && mousePos.X <= StartX + 8)
            //        highlighting = true;

            //    if (!highlighting)
            //        Program.Console.Print( StartX + 1, y, "[SKILLS]", textColor, statusColor );
            //    else
            //        Program.Console.Print( StartX + 1, y, "[SKILLS]", highlightedColor, statusColor );

            //    highlighting = false;

            //    if (mousePos.Y == y && mousePos.X >= Program.Console.Width - 1 - "[STATS]".Length && mousePos.X <= Program.Console.Width - 2)
            //        highlighting = true;

            //    if (!highlighting)
            //        Program.Console.Print( Program.Console.Width - 1 - "[STATS]".Length, y, "[STATS]", textColor, statusColor );
            //    else
            //        Program.Console.Print( Program.Console.Width - 1 - "[STATS]".Length, y, "[STATS]", highlightedColor, statusColor );
            //}

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

            bool PrintTabs()
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

                switch (selectedTab)
                {
                    case 0: // locals
                        return PrintLocals();
                    case 1: // skills
                        return PrintSkills();
                    case 2: // quests
                        return PrintQuests();
                }
                return false;
            }

            bool PrintLocals()
            {
                List<Creature> locals = Program.Player.LocalCreatures;
                locals.Sort( ( x, y ) => x.Position.DistFrom(Program.Player.Position).CompareTo( y.Position.DistFrom( Program.Player.Position ) ) );
                int heightofDisplay = Program.Console.Height - 3 - tabStartY;

                float bgAlpha = 0.99F, fgAlpha = 0.99F;
                
                bool PrintCreatures ()
                {
                    int padding = 0;
                    for (int i = 1, index = 0; i < heightofDisplay && index < locals.Count; i += padding, index++)
                    {
                        int curY = i + tabStartY;
                        Creature creature = locals[index];
                        // draw creature name
                        curY += Program.Window.Print( StartX + 1, curY, creature.Name + " / LVL " + creature.Stats.Level.Lvl, 20, textColor, bgColor * 0.98F );
                        // draw creature hp
                        DrawResourceBar(curY, creature.Stats.Resources[Resource.HP], creature.Stats.Resources[Resource.MaxHP], 20, new Color(45, 51, 122));
                        curY++;
                        // for each stat, draw if appropriate
                        if (creature is Player)
                        {
                            DrawResourceBar( curY, creature.Stats.Resources[Resource.MP], creature.Stats.Resources[Resource.MaxMP], 20, new Color( 45, 122, 116 ) );
                            curY++;
                            DrawResourceBar( curY, creature.Stats.Resources[Resource.SP], creature.Stats.Resources[Resource.MaxSP], 20, new Color( 122, 116, 45 ) );
                            curY++;
                        }
                        // draw status effects
                        foreach (Effect effect in creature.Effects)
                        {
                            DrawResourceBar( curY, effect.Turns, effect.TotalTurns, 20, new Color( 122, 45, 90 ), effect.Name );
                            curY++;
                        }
                        // draw creature goal, if appropriate

                        padding = 1 + curY - i - tabStartY;
                    }

                    return false;
                }

                return PrintCreatures();
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
            bool trueIfMouseOverItem = PrintTabs();
            PrintLookFunc();
            return trueIfMouseOverItem;
        }


        public static void HandleSkillsView()
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                 SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );
            int viewStartX = StartX - 20;

            if (Program.Animations.Count == 0)
                RenderSkillsView();

            bool clickedOutsideOfItemWindow = ( mousePos.X > viewStartX + 20 || mousePos.X < viewStartX );

            if (SadConsole.Global.MouseState.LeftClicked && ( clickedOutsideOfItemWindow ))
                Program.Animations.Add( new CloseStatusView() );
        }

        public static void RenderSkillsView()
        {
            int viewStartX = StartX - 20;

            Color bgColor = new Color( statusColor, 0.99F );
            Color textColor = new Color( Color.AntiqueWhite, 0.99F );

            // clear the page
            for (int j = 0; j < Program.Window.Height; j++)
                GUI.Console.Print( viewStartX, j, "                    ", textColor, bgColor );

            List<string> sortedSkillList = Enum.GetNames( typeof( Skill ) ).ToList();
            sortedSkillList.Sort();

            int y = 0;
            // print major/minor skills
            for (int i = 0; i < sortedSkillList.Count; i++)
            {
                Color skillColor = textColor;
                string skillName = sortedSkillList[i];
                Enum.TryParse( skillName, out Skill currentSkill );

                if (!( Program.Player.Class.MajorSkills.Contains( currentSkill ) || Program.Player.Class.MinorSkills.Contains( currentSkill ) ))
                    continue;
                if (Program.Player.Class.MajorSkills.Contains( currentSkill ))
                    skillColor = new Color( Color.RoyalBlue, 0.99F );
                else if (Program.Player.Class.MinorSkills.Contains( currentSkill ))
                    skillColor = new Color( Color.LimeGreen, 0.99F );

                Program.Window.Print( GUI.Console, viewStartX + 1, 1 + y * 2, skillName, 18, skillColor );
                Program.Window.Print( GUI.Console, viewStartX + 17, 1 + y * 2, $"{Program.Player.Stats.Skills[currentSkill]}", 2, textColor );
                y++;
            }
            // print misc. skills
            for (int i = 0; i < sortedSkillList.Count; i++)
            {
                string skillName = sortedSkillList[i];
                Enum.TryParse( skillName, out Skill currentSkill );
                if (Program.Player.Class.MajorSkills.Contains( currentSkill ) || Program.Player.Class.MinorSkills.Contains( currentSkill ))
                    continue;
                Program.Window.Print( GUI.Console, viewStartX + 1, 1 + y * 2, skillName, 18, textColor );
                Program.Window.Print( GUI.Console, viewStartX + 17, 1 + y * 2, $"{Program.Player.Stats.Skills[currentSkill]}", 2, textColor );
                y++;
            }
        }


        public static void HandleAttributesView()
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                 SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );
            int viewStartX = StartX - 20;

            if (Program.Animations.Count == 0)
                RenderAttributesView();

            bool clickedOutsideOfItemWindow = ( mousePos.X > viewStartX + 20 || mousePos.X < viewStartX );

            if (SadConsole.Global.MouseState.LeftClicked && ( clickedOutsideOfItemWindow ))
                Program.Animations.Add( new CloseStatusView() );
        }

        public static void RenderAttributesView()
        {
            int viewStartX = StartX - 20;
            Color textColor = new Color( Color.AntiqueWhite, 0.99F );

            // clear the page
            for (int j = 0; j < Program.Window.Height; j++)
                GUI.Console.Print( viewStartX, j, "                    ", Color.White, new Color( statusColor, 0.99F ) );

            for (int index = 0; index < Enum.GetNames( typeof( Attribute ) ).Length; index++)
            {
                Program.Window.Print( GUI.Console, viewStartX + 1, 1 + index * 2, Enum.GetName( typeof( Attribute ), index ), 18, textColor );
                Program.Window.Print( GUI.Console, viewStartX + 17, 1 + index * 2, $"{Program.Player.Stats.Attributes[(Attribute)index]}", 2, textColor );
            }
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
