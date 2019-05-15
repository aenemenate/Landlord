using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class BuildingModeInput
    {

        // CONSTRUCTOR //
        public BuildingModeInput()
        {

        }


        // FUNCTIONS //

        public bool HandleKeys()
        {
            bool pausePressed = SadConsole.Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Space);

            if (pausePressed)
                return true;

            return false;
        }

        public void HandleMouse()
        {
            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point mapPos = new Point(Program.Window.CalculateMapStartPoint().X + (mousePos.X), Program.Window.CalculateMapStartPoint().Y + mousePos.Y);
            bool mouseIsOnMap = mousePos.X >= 0 && mousePos.X < Program.Console.Width - StatusPanel.Width && !InventoryPanel.Displaying;

            if (!SadConsole.Global.MouseState.IsOnScreen)
                return;

            if (SadConsole.Global.MouseState.LeftButtonDown)
                HandleLeftButtonDown(mapPos, mouseIsOnMap);
            else if (SadConsole.Global.MouseState.RightButtonDown)
                HandleRightButtonDown(mapPos, mouseIsOnMap);

        }

        private void HandleLeftButtonDown(Point mapPos, bool mouseIsOnMap)
        {
            Blueprint bp = GUI.BuildPanel.CurrentlySelectedBlueprint;

            if (bp == null || mouseIsOnMap == false)
                return;
            
            if (BuildingManager.ConstructionMap[mapPos.X, mapPos.Y] == null || BuildingManager.ConstructionMap[mapPos.X, mapPos.Y].Name != bp.BlueprintTarget.Name)
            {
                BuildingManager.ConstructionMap[mapPos.X, mapPos.Y] = new BuildingPlaceholder(bp.BlueprintTarget.Graphic, bp.BlueprintTarget.Name, bp.BlueprintTarget);
                BuildingManager.DetermineNextConstruction();
            }
        }

        private void HandleRightButtonDown(Point mapPos, bool mouseIsOnMap)
        {
            if (mouseIsOnMap == true)
            {
                BuildingManager.CurrentConstructionPos = new Point();
                BuildingManager.ConstructionMap[mapPos.X, mapPos.Y] = null;
            }
        }
    }
}
