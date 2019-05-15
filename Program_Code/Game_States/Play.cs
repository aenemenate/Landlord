using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{

    class Play : GameState
    {
        PlayMode playMode;

        // CONSTRUCTOR

        public Play() : base()
        {
            playMode = PlayMode.Roguelike;
            GUI.Console.Clear();
        }


        // UPDATE FUNCS

        public override void Update()
        {
            if (playMode == PlayMode.Roguelike)
            {
                Scheduler.HandleRoguelikeScheduling(Program.WorldMap.LocalTile);
                StatusPanel.HandleStatus();
            }
            else
            {
                Scheduler.HandleBuildModeScheduling(Program.WorldMap.LocalTile);
                GUI.BuildPanel.HandleBuildPanel();
                BuildingManager.HandleInput();
            }

            InventoryPanel.HandleInventory();
        }


        // RENDERING FUNCS

        public override void Render()
        {
            RenderMap();
            RenderGUI();
            Program.MsgConsole.Render();
        }

        public static void RenderMap()
        {
            Point startPoint = Program.Window.CalculateMapStartPoint();
            for (int i = startPoint.X; i - startPoint.X < GUI.MapWidth; i++)
                for (int j = startPoint.Y; j - startPoint.Y < Program.Window.Height; j++)
                    Program.WorldMap.LocalTile.DrawCell(i, j);
        }

        public override void ClientSizeChanged()
        {
        }

        private void RenderGUI()
        {
            if (playMode == PlayMode.Roguelike)
                RenderRoguelikeGUI();
            else
                RenderBuildModeGUI();

            GUI.DrawFPS();
        }
        
        private void RenderRoguelikeGUI()
        {
            if (SadConsole.Global.MouseState.RightButtonDown)
                RenderPath();
            RenderInteractivity();
        }

        private void RenderInteractivity()
        {
            Color interactiveColor = Color.RoyalBlue;

            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                  SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point startPoint = Program.Window.CalculateMapStartPoint();
            Point mapPos = new Point(startPoint.X + (mousePos.X), startPoint.Y + mousePos.Y);


            if (!Program.WorldMap.LocalTile.PointWithinBounds(mapPos))
                return;

            if (Program.Player.PointNextToSelf(mapPos))
            {
                if (Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Interactive && !mapPos.Equals(Program.Player.Position))
                    Program.Console.SetGlyph(mousePos.X, mousePos.Y, Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Graphic,
                                Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].ForeColor, interactiveColor);
                else if (Program.Player.CurrentBlock.Enterable && mapPos.Equals(Program.Player.Position))
                    Program.Console.SetGlyph(mousePos.X, mousePos.Y, Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].Graphic,
                            Program.WorldMap.LocalTile[mapPos.X, mapPos.Y].ForeColor, interactiveColor);
            }
        }

        private void RenderPath()
        {
            List<Point> guiPath = null;
            Point startPoint = Program.Window.CalculateMapStartPoint();
            guiPath = GUI.CalculatePath(startPoint, Program.Window.Width);
            if (guiPath != null)
            {
                foreach (Point point in guiPath)
                {
                    bool pointOutsideMapViewer = point.Y - startPoint.Y < 0
                        || point.Y - startPoint.Y >= Program.Window.Height
                        || point.X - startPoint.X < 0
                        || point.X - startPoint.X >= Program.Window.Width - StatusPanel.Width;

                    if (pointOutsideMapViewer)
                        continue;

                    if (Program.WorldMap.LocalTile[point.X, point.Y] is Air)
                    {
                        Program.Console.SetGlyph(point.X - startPoint.X, point.Y - startPoint.Y, Program.WorldMap.LocalTile.Floor[point.X * Program.WorldMap.LocalTile.Width + point.Y].Graphic,
                            Program.WorldMap.LocalTile.Floor[point.X * Program.WorldMap.LocalTile.Width + point.Y].ForeColor, Color.RoyalBlue);
                        continue;
                    }

                    Program.Console.SetGlyph(point.X - startPoint.X, point.Y - startPoint.Y, Program.WorldMap.LocalTile[point.X, point.Y].Graphic,
                            Program.WorldMap.LocalTile[point.X, point.Y].ForeColor, Color.CornflowerBlue);
                }
            }
        }

        private void RenderBuildModeGUI()
        {
            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                  SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point startPoint = Program.Window.CalculateMapStartPoint();
            Point mapPos = new Point(startPoint.X + (mousePos.X), startPoint.Y + mousePos.Y);
            
            BuildingManager.RenderConstructionMap();

            bool mouseOutsideMapViewer = mousePos.Y < 0
                || mousePos.Y >= Program.Window.Height
                || mousePos.X < 0
                || mousePos.X + StatusPanel.Width >= Program.Window.Width - 1;


            if (mouseOutsideMapViewer)
                return;

            if (GUI.BuildPanel.CurrentlySelectedBlueprint != null && GUI.BuildPanel.CurrentlySelectedBlueprint is Blueprint bp)
                Program.Console.SetGlyph(mousePos.X, mousePos.Y, bp.BlueprintTarget.Graphic,
                                Color.SlateGray, Color.AntiqueWhite);
        }


        // PROPERTIES //

        public PlayMode PlayMode
        {
            get { return playMode; }
            set { playMode = value; }
        }
    }

}
