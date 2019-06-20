using System;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Window
    {
        private string fontFile;
        private int width;
        private int height;
        private int oldWindowPixelsWidth;
        private int oldWindowPixelsHeight;
        private Microsoft.Xna.Framework.Point minSize;
        

        // CONSTRUCTOR //

        public Window (string fontFile, int width, int height)
        {
            this.fontFile = fontFile;
            this.width = width;
            this.height = height;
            minSize = new Microsoft.Xna.Framework.Point(100 * 12, 50 * 12);
            Init(fontFile, width, height);
        }


        // FUNCTIONS //

        public void Init(string fontFile, int width, int height)
        {
            Settings.ResizeMode = SadConsole.Settings.WindowResizeOptions.Stretch;
            
            SadConsole.Game.Create(fontFile, width, height);
            oldWindowPixelsHeight = 0;
            oldWindowPixelsWidth = 0;
        }

        public void ClientSizeChanged(object sender, EventArgs e)
        {
            // Get the size of the window.
            int windowPixelsWidth = SadConsole.Game.Instance.Window.ClientBounds.Width;
            int windowPixelsHeight = SadConsole.Game.Instance.Window.ClientBounds.Height;
            
            // If this is getting called because of the ApplyChanges, exit.
            if (windowPixelsWidth == oldWindowPixelsWidth && windowPixelsHeight == oldWindowPixelsHeight)
                return;

            // Store for later
            oldWindowPixelsWidth = windowPixelsWidth;
            oldWindowPixelsHeight = windowPixelsHeight;

            // Get the exact pixels we can fit in that window based on a font.
            int fontPixelsWidth = (windowPixelsWidth / SadConsole.Global.FontDefault.Size.X) * SadConsole.Global.FontDefault.Size.X;
            int fontPixelsHeight = (windowPixelsHeight / SadConsole.Global.FontDefault.Size.Y) * SadConsole.Global.FontDefault.Size.Y;

            // Resize the monogame rendering to match
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferWidth = windowPixelsWidth;
            SadConsole.Global.GraphicsDeviceManager.PreferredBackBufferHeight = windowPixelsHeight;
            SadConsole.Global.GraphicsDeviceManager.ApplyChanges();

            // Tell sadconsole how much to render to the screen.
            Global.RenderWidth = fontPixelsWidth;
            Global.RenderHeight = fontPixelsHeight;
            Global.ResetRendering();

            // Get the total cells you can fit
            int totalCellsX = fontPixelsWidth / SadConsole.Global.FontDefault.Size.X;
            int totalCellsY = fontPixelsHeight / SadConsole.Global.FontDefault.Size.Y;
            width = totalCellsX;
            height = totalCellsY;

            // Resize your console based on totalCellsX/Y
            Program.Console = new Console(totalCellsX, totalCellsY);
            SadConsole.Global.CurrentScreen = Program.Console;

            if (Program.CurrentState is DialogWindow)
                Program.CurrentState = Menus.PrevGameState;
            else if (Program.CurrentState is ViewItem       || Program.CurrentState is ViewLoot 
                  || Program.CurrentState is ViewEquipment  || Program.CurrentState is CraftMenu)
            {
                Play.RenderMap();
                Program.MsgConsole.Render();
            }

            Program.CurrentState.ClientSizeChanged();
            GUI.Console = new SadConsole.Console( width, height );
        }

        public Point CalculateMapStartPoint()
        {
            int startX = 0, startY = 0;
            bool viewPastLeftMostPoint = Program.Player.Position.X - (width - (StatusPanel.Width)) / 2 < 0;
            bool viewPastRightMostPoint = Program.Player.Position.X + (width - (StatusPanel.Width)) / 2 >= Program.WorldMap.TileWidth;
            if (!viewPastLeftMostPoint && !viewPastRightMostPoint)
                startX = Program.Player.Position.X - (GUI.MapWidth / 2);
            else if (viewPastLeftMostPoint)
                startX = 0;
            else if (viewPastRightMostPoint)
                startX = Program.WorldMap.TileWidth - GUI.MapWidth;

            bool viewPastTopMostPoint = Program.Player.Position.Y - height / 2 < 0;
            bool viewPastBottomMostPoint = Program.Player.Position.Y + height / 2 >= Program.WorldMap.TileHeight;
            if (!viewPastTopMostPoint && !viewPastBottomMostPoint)
                startY = Program.Player.Position.Y - (height) / 2;
            else if (viewPastTopMostPoint)
                startY = 0;
            else if (viewPastBottomMostPoint)
                startY = Program.WorldMap.TileHeight - height;
            return new Point(startX, startY);
        }

        public void Print(int x, int y, string str)
        {
            Print(x, y, str, this.Width - x - 1);
        }

        public void Print(int x, int y, string str, int length)
        {
            Print(x, y, str, length, Color.AntiqueWhite);
        }

        public void Print(int x, int y, string str, int length, Color foreColor)
        {
            if (y > Program.Window.Height)
                return;

            str = str + ' ';
            int endIndex = str.LastIndexOf(' ');
            string nextLine = str.Substring(0, endIndex + 1);
            while (true)
            {
                if (nextLine.Length > length + 1)
                {
                    if (nextLine.Contains(" "))
                        endIndex = nextLine.Substring(0, endIndex).LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                }
                else
                {
                    Program.Console.Print(x, y, nextLine, foreColor);
                    y++;
                    str = str.Substring(endIndex + 1, str.Length - nextLine.Length);
                    endIndex = str.LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                    if (str == "")
                        return;
                }
            }
        }

        public int Print(int x, int y, string str, int length, Color foreColor, Color backColor)
        {
            str = str + ' ';
            int lines = 0;
            int endIndex = str.LastIndexOf(' ');
            string nextLine = str.Substring(0, endIndex + 1);
            while (true)
            {
                if (nextLine.Length > length + 1)
                {
                    if (nextLine.Contains(" "))
                        endIndex = nextLine.Substring(0, endIndex).LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                }
                else
                {
                    Program.Console.Print(x, y, nextLine, foreColor, backColor);
                    y++;
                    lines++;
                    str = str.Substring(endIndex + 1, str.Length - nextLine.Length);
                    endIndex = str.LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                    if (str == "")
                        return lines;
                }
            }
        }

        public void Print( Console console, int x, int y, string str, int length, Color foreColor )
        {
            str = str + ' ';
            int endIndex = str.LastIndexOf( ' ' );
            string nextLine = str.Substring( 0, endIndex + 1 );
            while (true)
            {
                if (nextLine.Length > length + 1)
                {
                    if (nextLine.Contains( " " ))
                        endIndex = nextLine.Substring( 0, endIndex ).LastIndexOf( ' ' );
                    nextLine = str.Substring( 0, endIndex + 1 );
                } else
                {

                    if (y > Program.Window.Height)
                        return;
                    console.Print( x, y, nextLine, foreColor );
                    y++;
                    str = str.Substring( endIndex + 1, str.Length - nextLine.Length );
                    endIndex = str.LastIndexOf( ' ' );
                    nextLine = str.Substring( 0, endIndex + 1 );
                    if (str == "")
                        return;
                }
            }
        }
        
        public void Print( Console console, int x, int y, string str, int length, Color foreColor, Color backColor )
        {
            str = str + ' ';
            int endIndex = str.LastIndexOf( ' ' );
            string nextLine = str.Substring( 0, endIndex + 1 );
            while (true)
            {
                if (nextLine.Length > length + 1)
                {
                    if (nextLine.Contains( " " ))
                        endIndex = nextLine.Substring( 0, endIndex ).LastIndexOf( ' ' );
                    nextLine = str.Substring( 0, endIndex + 1 );
                } else
                {
                    console.Print( x, y, nextLine, foreColor, backColor );
                    y++;
                    str = str.Substring( endIndex + 1, str.Length - nextLine.Length );
                    endIndex = str.LastIndexOf( ' ' );
                    nextLine = str.Substring( 0, endIndex + 1 );
                    if (str == "")
                        return;
                }
            }
        }

        public void PrintMessage(int y, string str, int length, Color foreColor)
        {
            str = str + ' ';
            int endIndex = str.LastIndexOf(' ');
            string nextLine = str.Substring(0, endIndex + 1);
            while (true)
            {
                if (nextLine.Length > length + 1)
                {
                    if (nextLine.Contains(" "))
                        endIndex = nextLine.Substring(0, endIndex).LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                }
                else
                {
                    Program.MsgConsole.Console.Print(InventoryPanel.Width, y, nextLine, new Color(foreColor, 0.97F), new Color(Color.DarkSlateGray, 0.7F));
                    y++;
                    str = str.Substring(endIndex + 1, str.Length - nextLine.Length);
                    endIndex = str.LastIndexOf(' ');
                    nextLine = str.Substring(0, endIndex + 1);
                    if (str == "")
                        return;
                }
            }
        }

        static public Tuple<string, string> SplitNameIfGreaterThanLength(string str, string pt2, int length)
        {
            int endIndex;
            if (str.Length > length)
            {
                if (str.Contains(" "))
                {
                    endIndex = str.LastIndexOf(' ');
                    while (str.Substring(0, endIndex).Length > length)
                        endIndex = str.Substring(0, endIndex).LastIndexOf(' ');
                }
                else
                    endIndex = length - 1;
            }
            else
                endIndex = str.Length - 1;

            if (endIndex != str.Length - 1 && str[endIndex] == ' ')
                pt2 += str.Substring(endIndex + 1, (str.Length - endIndex) - 1);
            else if (endIndex != str.Length - 1 && str[endIndex] != ' ')
                pt2 += str.Substring(endIndex, (str.Length - endIndex) - 1);

            str = str.Substring(0, endIndex + 1);

            if (pt2.Length > length)
                pt2 = pt2.Substring(0, length - 3) + "...";

            return Tuple.Create(str, pt2);
        }


        // PROPERTIES //

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public string FontFile
        {
            get { return fontFile; }
            set { fontFile = value; }
        }

        public Microsoft.Xna.Framework.Point MinSize
        {
            get { return minSize; }
            set { minSize = value; }
        }

        public Point MousePos
        {
            get {
                return new Point(Global.MouseState.ScreenPosition.X / Global.FontDefault.Size.X,
                  Global.MouseState.ScreenPosition.Y / Global.FontDefault.Size.Y);
            }
        }
    }
}
