using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    abstract class Animation
    {
        private int frames;
        private int curFrame;
        private TimeSpan frameTime;
        private DateTime lastFrameTime;
        private bool finished;
        public Animation(int frames)
        {
            this.frames = frames;
            curFrame = 0;
            finished = false;
            frameTime = new TimeSpan(100000);
            lastFrameTime = DateTime.Now;
        }

        public abstract void Play();
        public void Delete()
        {
            Program.FinishedAnims.Add(this);
        }

        public int Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        public int CurFrame
        {
            get { return curFrame; }
            set { curFrame = value; }
        }
        public bool Finished
        {
            get { return finished; }
            set { finished = value; }
        }
        public TimeSpan FrameTime
        {
            get { return frameTime; }
            set { frameTime = value; }
        }
        public DateTime LastFrameTime
        {
            get { return lastFrameTime; }
            set { lastFrameTime = value; }
        }
    }

    class OpenItemView : Animation
    {
        private Color color;


        public OpenItemView (Color color, int frames = 20) : base (frames)
        {
            this.color = color;
            Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["openMenu"]);
        }

        public override void Play()
        {
            if (CurFrame == Frames)
            {
                Delete();
                return;
            }
            if (DateTime.Now - LastFrameTime >= FrameTime)
            {
                for (int j = 0; j < Program.Console.Height; j++)
                    GUI.Console.SetGlyph( CurFrame + InventoryPanel.Width, j, ' ', new Color( color, 0.97F ), new Color( color, 0.97F ) );
                CurFrame++;
                LastFrameTime = DateTime.Now;
            }
        }
    }

    class CloseItemView : Animation
    {
        public CloseItemView(int frames = 20) : base(frames)
        {
            Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["closeMenu"]);
        }

        public override void Play()
        {
            if (DateTime.Now - LastFrameTime >= FrameTime)
            {
                for ( int j = 0; j < Program.Window.Height; j++ )
                    GUI.Console.Clear( InventoryPanel.Width + (Frames - 1) - CurFrame, j );

                CurFrame++;
                LastFrameTime = DateTime.Now;
            }
            if (CurFrame == Frames)
            {
                if (Program.CurrentState is ViewItem || Program.CurrentState is ViewEquipment)
                    Program.CurrentState = new Play();
                else if (Program.CurrentState is ViewLoot)
                    Program.QueuedAnimations.Add(new OpenLootView());
                Delete();
                return;
            }
        }
    }

    class OpenStatusView : Animation
    {
        private Color color;


        public OpenStatusView(Color color, int frames = 20) : base(frames)
        {
            this.color = color;
            Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["openMenu"]);
        }
        
        public override void Play()
        {
            if (CurFrame == Frames)
            {
                Delete();
                return;
            }
            if (DateTime.Now - LastFrameTime >= FrameTime)
            {
                for (int j = 0; j < Program.Console.Height; j++)
                    GUI.Console.SetGlyph(Program.Window.Width - 1 - StatusPanel.Width - CurFrame, j, ' ', new Color(color, 0.99F), new Color(color, 0.99F));
                CurFrame++;
                LastFrameTime = DateTime.Now;
            }
        }
    }

    class CloseStatusView : Animation
    {
        public CloseStatusView(int frames = 20) : base(frames)
        {
            Program.AudioEngine.PlaySound(Program.AudioEngine.CachedSoundFX["closeMenu"]);
        }

        public override void Play()
        {
            if (DateTime.Now - LastFrameTime >= FrameTime)
            {
                for (int j = 0; j < Program.Window.Height; j++)
                    GUI.Console.Clear((Program.Window.Width - StatusPanel.Width) - Frames + CurFrame, j);
                CurFrame++;
                LastFrameTime = DateTime.Now;
            }
            if (CurFrame == Frames)
            {
                Program.CurrentState = new Play();
                Delete();
                return;
            }
        }
    }
    
    class LoadingAnim : Animation
    {
        private string text;

        public LoadingAnim(string text, int frames = 4) : base(frames)
        {
            this.text = text;
            FrameTime = new TimeSpan(4000000);
            Program.Console.Print(Program.Console.Width - (text.Length + 3 + 1), Program.Console.Height - 2, text);
        }

        public override void Play()
        {
            if (DateTime.Now - LastFrameTime >= FrameTime)
            {
                string periods = "";
                string spaces = "";
                for (int i = 0; i < Frames; i++)
                {
                    if (i < CurFrame)
                        periods += '.';
                    else
                        spaces += ' ';
                }
                Program.Console.Print(Program.Console.Width - (text.Length + 3 + 1), Program.Console.Height - 2, text + periods + spaces);
                if (CurFrame < Frames - 1)
                    CurFrame++;
                else
                    CurFrame = 0;
                LastFrameTime = DateTime.Now;
            }
        }
    }

    class OpenLootView : Animation
    {
        private Point cursor;
        private Point dir = new Point(0, -1); // the current direction
        private int leftBoundary; // the maximal edge of animation
        private int upLimit, rightLimit, leftLimit, downLimit; // the current edges of animation

        public OpenLootView(int frames = 0) : base(frames)
        {
            InventoryPanel.Displaying = true;
            InventoryPanel.DisplayingEquipment = false;
            leftBoundary = GUI.LootMenu.StartX;
            cursor = new Point(leftBoundary + (GUI.LootMenu.Width) / 2, GUI.LootMenu.StartY + (GUI.LootMenu.Height - 1) / 2);
            upLimit = cursor.Y - 1;
            rightLimit = cursor.X + 1;
            downLimit = cursor.Y + 1;
            leftLimit = cursor.X - 1;
        }

        public override void Play()
        {
            while (true) {
                Program.Console.SetGlyph(cursor.X, cursor.Y, ' ', Color.Black, InventoryPanel.darkerColor);
                if (dir.X == -1 && cursor.X == leftLimit) {
                    dir = new Point(0, -1);
                    leftLimit--;
                }
                else if (dir.Y == -1 && cursor.Y == upLimit) {
                    dir = new Point(1, 0);
                    if (upLimit > GUI.LootMenu.StartY)
                        upLimit--;
                }
                else if (dir.X == 1 && cursor.X == rightLimit) {
                    dir = new Point(0, 1);
                    rightLimit++;
                }
                else if (dir.Y == 1 && cursor.Y == downLimit) {
                    dir = new Point(-1, 0);
                    if (downLimit < GUI.LootMenu.StartY + GUI.LootMenu.Height - 1)
                        downLimit++;
                    break;
                }

                cursor = new Point(cursor.X + dir.X, cursor.Y + dir.Y);
                // the animation ends once the cursor is less than the leftBoundary
                if (cursor.X < leftBoundary) {
                    Delete();
                    break;
                }
            }
            
        }
    }

    class CloseLootView : Animation
    {
        private Point cursor;
        private Point dir = new Point(0, 1); // the current direction
        private Point endPoint; // the maximal edge of animation
        private int upLimit, rightLimit, leftLimit, downLimit; // the current edges of animation

        public CloseLootView(int frames = 0) : base(frames)
        {
            upLimit = GUI.LootMenu.StartY;
            rightLimit = GUI.LootMenu.StartX + GUI.LootMenu.Width + 1;
            downLimit = GUI.LootMenu.StartY + GUI.LootMenu.Height;
            leftLimit = GUI.LootMenu.StartX;
            endPoint = endPoint = new Point( GUI.LootMenu.StartX + ( GUI.LootMenu.Width - 1 ) / 2, GUI.LootMenu.StartY + ( GUI.LootMenu.Height - 1 ) / 2 );
            cursor = new Point(leftLimit, upLimit);
        }

        public override void Play()
        {
            while (true)
            {
                Point startPoint = Program.Window.CalculateMapStartPoint();
                Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].DrawCell(cursor.X + startPoint.X, cursor.Y + startPoint.Y, Program.Player, Program.Console, Program.Window);

                if (dir.X == 1 && cursor.X == rightLimit) {
                    dir = new Point(0, 1);
                    rightLimit--;
                }
                else if (dir.Y == 1 && cursor.Y == downLimit) {
                    dir = new Point(-1, 0);
                    if (downLimit > endPoint.Y)
                        downLimit--;
                }
                if (dir.X == -1 && cursor.X == leftLimit) {
                    dir = new Point(0, -1);
                    leftLimit++;
                }
                else if (dir.Y == -1 && cursor.Y == upLimit) {
                    dir = new Point(1, 0);
                    if (upLimit < endPoint.Y)
                        upLimit++;
                    break;
                }

                cursor = new Point(cursor.X + dir.X, cursor.Y + dir.Y);

                // the animation ends once the cursor is less than the leftBoundary
                if (cursor.Equals(endPoint)) {
                    Delete();
                    Program.CurrentState = new Play();
                    InventoryPanel.Displaying = false;
                    break;
                }
            }
        }
    }

    class OpenCraftMenu : Animation
    {
        private Point cursor;
        private Point dir = new Point(0, -1); // the current direction
        private int leftBoundary = GUI.CraftMenu.StartX; // the maximal edge of animation
        private int upLimit, rightLimit, leftLimit, downLimit; // the current edges of animation

        public OpenCraftMenu(int frames = 0) : base(frames)
        {
            cursor = new Point(leftBoundary + (GUI.CraftMenu.Width - 1) / 2, GUI.CraftMenu.StartY + (GUI.CraftMenu.Height - 1) / 2);
            upLimit = cursor.Y - 1;
            rightLimit = cursor.X + 1;
            downLimit = cursor.Y + 1;
            leftLimit = cursor.X - 1;
        }

        public override void Play()
        {
            while (true)
            {
                Program.Console.SetGlyph(cursor.X, cursor.Y, ' ', Color.Black, InventoryPanel.darkerColor);

                if (dir.X == -1 && cursor.X == leftLimit)
                {
                    dir = new Point(0, -1);
                    leftLimit--;
                }
                else if (dir.Y == -1 && cursor.Y == upLimit)
                {
                    dir = new Point(1, 0);
                    if (upLimit > GUI.CraftMenu.StartY)
                        upLimit--;
                }
                else if (dir.X == 1 && cursor.X == rightLimit)
                {
                    dir = new Point(0, 1);
                    rightLimit++;
                }
                else if (dir.Y == 1 && cursor.Y == downLimit)
                {
                    dir = new Point(-1, 0);
                    if (downLimit < GUI.CraftMenu.StartY + GUI.CraftMenu.Height - 1)
                        downLimit++;
                    break;
                }

                cursor = new Point(cursor.X + dir.X, cursor.Y + dir.Y);

                // the animation ends once the cursor is less than the leftBoundary
                if (cursor.X < leftBoundary)
                {
                    Delete();
                    break;
                }
            }

        }
    }

    class CloseCraftMenu : Animation
    {
        private Point cursor;
        private Point dir = new Point(0, 1); // the current direction
        private Point endPoint = new Point(GUI.CraftMenu.StartX + (GUI.CraftMenu.Width - 1) / 2, GUI.CraftMenu.StartY + (GUI.CraftMenu.Height - 1) / 2); // the maximal edge of animation
        private int upLimit, rightLimit, leftLimit, downLimit; // the current edges of animation

        public CloseCraftMenu(int frames = 0) : base(frames)
        {
            upLimit = GUI.CraftMenu.StartY;
            rightLimit = GUI.CraftMenu.StartX + GUI.CraftMenu.Width - 1;
            downLimit = GUI.CraftMenu.StartY + GUI.CraftMenu.Height;
            leftLimit = GUI.CraftMenu.StartX;
            cursor = new Point(leftLimit, upLimit);

        }

        public override void Play()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            while (true)
            {
                Point startPoint = Program.Window.CalculateMapStartPoint();
                Program.WorldMap[Program.Player.WorldIndex.X, Program.Player.WorldIndex.Y].DrawCell(cursor.X + startPoint.X - InventoryPanel.Width, cursor.Y + startPoint.Y, Program.Player, Program.Console, Program.Window);

                if (dir.X == 1 && cursor.X == rightLimit)
                {
                    dir = new Point(0, 1);
                    rightLimit--;
                }
                else if (dir.Y == 1 && cursor.Y == downLimit)
                {
                    dir = new Point(-1, 0);
                    if (downLimit > endPoint.Y)
                        downLimit--;
                }
                if (dir.X == -1 && cursor.X == leftLimit)
                {
                    dir = new Point(0, -1);
                    leftLimit++;
                }
                else if (dir.Y == -1 && cursor.Y == upLimit)
                {
                    dir = new Point(1, 0);
                    if (upLimit < endPoint.Y)
                        upLimit++;
                    break;
                }

                cursor = new Point(cursor.X + dir.X, cursor.Y + dir.Y);

                // the animation ends once the cursor is less than the leftBoundary
                if (cursor.Equals(endPoint))
                {
                    Program.Player.Unequip(Program.Player.Body.MainHand);
                    Program.CurrentState = new Play();
                    Delete();
                    break;
                }
            }
        }
    }
}
