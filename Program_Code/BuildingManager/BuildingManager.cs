using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum PlayerState
    {
        GetMaterial,
        PlaceMaterials,
        Idle
    }

    static class BuildingManager
    {
        private static BuildingModeInput inputHandler = new BuildingModeInput();
        private static bool paused = false;
        private static PlayerState playerState = PlayerState.Idle;

        private static BuildingPlaceholder[,] constructionMap = new BuildingPlaceholder[Program.WorldMap.TileWidth, Program.WorldMap.TileHeight];

        private static Point currentConstructionPos = new Point();
        private static List<RecipeComponent> currentConstructRecipe = null;
        private static int getIndex = 0;

        private static CraftingRecipe currentCraftingRecipe = null;


        // FUNCTIONS //

        // INPUT
        public static void HandleInput()
        {
            bool pausePressed = InputHandler.HandleKeys();
            if (pausePressed)
                paused = !paused;

            InputHandler.HandleMouse();
        }


        // CONSTRUCTION HANDLING

        internal static void DetermineNextConstruction()
        {
            if (currentConstructionPos.Equals(new Point() ))
            {
                for (int i = 0; i < constructionMap.GetLength(0); i++)
                    for (int j = 0; j < constructionMap.GetLength(1); j++)
                    {
                        if (constructionMap[i, j] != null)
                        {
                            currentConstructionPos = new Point(i, j);
                            bool enoughMaterials = DetermineIfEnoughMaterials();
                            if (!enoughMaterials)
                            {
                                Menus.DisplayIncorrectUsage("You don't have the required materials!");
                                RemoveAllConstructionsWithName(constructionMap[i, j].Name);
                                currentConstructionPos = new Point();
                                DetermineNextConstruction();
                            }
                            else
                                currentConstructRecipe = GetConstructRecipe();
                        }
                    }
            }
        }

        internal static void RemoveAllConstructionsWithName(string name)
        {
            for (int i = 0; i < constructionMap.GetLength(0); i++)
                for (int j = 0; j < constructionMap.GetLength(1); j++)
                {
                    if (constructionMap[i, j] != null && constructionMap[i, j].Name == name)
                        constructionMap[i, j] = null;
                }
        }

        internal static List<RecipeComponent> GetConstructRecipe()
        {
            List<RecipeComponent> recipe = new List<RecipeComponent>();
            BuildingPlaceholder currentConstruct = constructionMap[currentConstructionPos.X, currentConstructionPos.Y];

            // populate recipe list
            foreach (Blueprint bp in ((BlueprintPouch)Program.Player.Body.MainHand).Blueprints)
                if (bp.BlueprintTarget.Name == currentConstruct.Name)
                {
                    foreach (RecipeComponent rc in bp.Recipe)
                        recipe.Add(rc);
                    break;
                }

            return recipe;
        }

        internal static bool DetermineIfEnoughMaterials()
        {
            List<RecipeComponent> recipe = GetConstructRecipe();
            if (recipe == null) {
                return false;
            }
            
            // check player inventory
            foreach (Item item in Program.Player.Inventory)
            {
                RecipeComponent itemC = item.ToComponent();
                if (recipe.Contains(itemC))
                    recipe.Remove(itemC);
            }

            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            // check map
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++) {
                    if (blocks[i * width + j] is Item item) {
                        RecipeComponent itemC = item.ToComponent();
                        if (recipe.Contains( itemC ))
                            recipe.Remove( itemC );
                    } else if (blocks[i * width + j] is Chest chest)
                        foreach (Item cItem in chest.Inventory) {
                            RecipeComponent itemC = cItem.ToComponent();
                            if (recipe.Contains( itemC ))
                                recipe.Remove( itemC );
                        }
                }
                    


            // check for other methods of resource aquisition
            if (recipe.Count > 0) {
                if (recipe.Contains(RecipeComponent.Log)) // find a tree if the recipe calls for a log
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                            if (blocks[i * width + j] is Tree)
                                recipe.RemoveAll(rc => rc == RecipeComponent.Log);

                if (recipe.Contains(RecipeComponent.Plank)) // find a tree or a log if the recipe calls for a plank
                    for (int i = 0; i < width; i++)
                        for (int j = 0; j < height; j++)
                            if (blocks[i * width + j] is Tree || blocks[i * width + j] is Log)
                                recipe.RemoveAll(rc => rc == RecipeComponent.Plank);
            }

            if (recipe.Count == 0)
                return true;

            return false;
        }

        internal static Point GetClosestMaterialPos(RecipeComponent recipe, bool checkInventory)
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            // check player inventory
            if (checkInventory) {
                foreach (Item item in Program.Player.Inventory) {
                    RecipeComponent itemC = item.ToComponent();
                    if (recipe.Equals(itemC))
                        return new Point();
                }
            }

            // check map
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++) {
                    if (blocks[i * width + j] is Item item) {
                        RecipeComponent itemC = item.ToComponent();
                        if (recipe.Equals( itemC ))
                            return new Point( i, j );
                    }
                    else if (blocks[i * width + j] is Chest chest)
                        foreach (Item cItem in chest.Inventory) {
                            RecipeComponent itemC = cItem.ToComponent();
                            if (recipe.Equals( itemC ))
                                return new Point( i, j );
                        }
                }
                    

            return null;
        }


        // LOGIC FOR DETERMINING PLAYER ACTION //

        internal static void DeterminePlayerAction( bool calledFromCraftingScheduler = false )
        {
            if (calledFromCraftingScheduler) {
                Program.Player.DetermineAction();
                return;
            }

            DetermineNextConstruction();
            if (currentConstructionPos.Equals(new Point()) == false) // this means a pending construction has been found and it can definitely be built
            {
                RecipeComponent nextComponent = RecipeComponent.Null;

                if (currentConstructRecipe.Count > getIndex) // if get has not gotten up to the count
                    nextComponent = currentConstructRecipe[getIndex];
                else {
                    Program.Player.Path = null;
                    playerState = PlayerState.PlaceMaterials;
                }

                if (playerState == PlayerState.Idle) 
                    playerState = PlayerState.GetMaterial;

                if (playerState == PlayerState.GetMaterial)
                    HandleGetMaterial(nextComponent);
                else if (playerState == PlayerState.PlaceMaterials)
                    HandlePlaceMaterials(nextComponent);
            }
            else /* there is no pending construction */ {
                playerState = PlayerState.Idle;
                paused = true;
            }

            Program.Player.DetermineAction();
        }

        private static void PathToPoint(Point pos)
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            if (pos.Equals(new Point()) == true)
                throw new Exception();
            if (Program.Player.Path == null)
            {
                List<Point> freeSpots = blocks.GetEmptyAdjacentBlocks(new Point(width, height), pos);
                Point nextPos = freeSpots.Count > 0 ? freeSpots[Program.RNG.Next(0, freeSpots.Count)] : new Point();
                if (!nextPos.Equals(new Point()))
                    Program.Player.SetPath( nextPos );
                else
                {
                    Program.MsgConsole.WriteLine( $"The {constructionMap[currentConstructionPos.X, currentConstructionPos.Y]} couldn't be built because a path couldn't be found." );
                    constructionMap[currentConstructionPos.X, currentConstructionPos.Y] = null;
                    currentConstructionPos = new Point();

                    DetermineNextConstruction();
                    playerState = PlayerState.Idle;
                    Program.Player.Path = null;
                }
            }
        }

        private static void HandleGetMaterial(RecipeComponent nextComponent, bool forCrafting = false)
        {
            Point nextPos = GetClosestMaterialPos(nextComponent, true); // returns new Point() if item is in player's inventory, returns null if the object can't be found
            
            // if the material is not on the map or in your inventory
            if (nextPos == null)
            {
                if (nextComponent == RecipeComponent.Log)
                    HandleChopTree();
                else if (nextComponent == RecipeComponent.Stone)
                {
                }
                else {
                    if (currentCraftingRecipe == null && Program.Player.Inventory.Exists(i => i is RecipePouch)) {
                        RecipePouch rp = (RecipePouch)Program.Player.Inventory.Find(i => i is RecipePouch);
                        foreach (CraftingRecipe r in rp.Recipes)
                            if (r.CraftingTarget.Exists(e => e.ToComponent() == nextComponent)) {
                                currentCraftingRecipe = r;
                                break;
                            }
                    }
                    if (currentCraftingRecipe != null)
                    {
                        if (!currentCraftingRecipe.CraftingTarget.Exists(e => e.ToComponent() == nextComponent))
                            currentCraftingRecipe = null;
                        if (Program.Player.Inventory.Exists(i => i.ToComponent() == currentCraftingRecipe.Recipe[0]))
                            HandleCraftComponent();
                        else
                            HandleGetMaterial(currentCraftingRecipe.Recipe[0], true);
                    }
                }
                return;
            } 
            
            bool hasItem = nextPos.Equals(new Point());
            if (hasItem == true)
                nextPos = GetClosestMaterialPos(nextComponent, false);


            if (nextPos == null) {
                playerState = PlayerState.PlaceMaterials;
                return;
            }

            if (hasItem == false || Program.Player.CanCarryItem(nextComponent.ToItem()) == true)
            {
                bool nextToItem = Program.Player.PointNextToSelf(nextPos);
                int currentFloor = Program.Player.CurrentFloor;
                Point worldIndex = Program.Player.WorldIndex;
                Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

                if (nextToItem) {
                    if ( blocks[nextPos.X * width + nextPos.Y] is Chest chest ) {
                        for (int i = chest.Inventory.Count - 1; i >= 0; i--) {
                            if (chest.Inventory[i].ToComponent() == nextComponent) {
                                bool itemAdded = Program.Player.AddItem( chest.Inventory[i] );
                                if (itemAdded)
                                    chest.Inventory.RemoveAt( i );
                                else if (DropUnnecessaryItems(nextComponent)) {
                                    itemAdded = Program.Player.AddItem( chest.Inventory[i] );
                                    if (itemAdded)
                                        chest.Inventory.RemoveAt( i );
                                    else
                                        playerState = PlayerState.PlaceMaterials;
                                }
                                else
                                    playerState = PlayerState.PlaceMaterials;
                            }
                        }
                    }
                    else {
                        if (Program.Player.CanCarryItem( (Item)blocks[nextPos.X * width + nextPos.Y] )) {
                            Program.Player.GetItem( nextPos );
                            getIndex++;
                        }
                        else {
                            bool droppedItems = DropUnnecessaryItems(nextComponent);
                            if (droppedItems == false)
                                playerState = PlayerState.PlaceMaterials;
                        }
                    }
                }
                else if (Program.Player.Path == null)
                    Program.Player.SetPath(nextPos);
            }
            else {
                Program.Player.Path = null;
                playerState = PlayerState.PlaceMaterials;
            }
        }

        private static void HandleChopTree()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Point closestTree = blocks.GetClosestOfBlockTypeToPos( Program.Player.Position, new Point(width, height), BlockType.Tree);
            bool nextToTree = Program.Player.PointNextToSelf(closestTree);
            if (!nextToTree)
                PathToPoint(closestTree);
            else {
                for (int i = 0; i < Program.Player.Inventory.Count; i++) {
                    Item I = Program.Player.Inventory[i];
                    if (I is Axe || I is Sword)
                        Program.Player.Wield(i, true);
                }
                Program.Player.ChopTree(closestTree);
            }

        }

        private static void HandleMineRock()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Point closestStoneWall = blocks.GetClosestOfBlockTypeToPos( Program.Player.Position, new Point(width, height), BlockType.Wall, Material.Stone );
            bool nextToStoneWall = Program.Player.PointNextToSelf( closestStoneWall );
            if (!nextToStoneWall) {
                PathToPoint( closestStoneWall );
            }
            else {
                for (int i = 0; i < Program.Player.Inventory.Count; i++) {
                    Item I = Program.Player.Inventory[i];
                    if (I is Axe axe && axe.Name == "pickaxe")
                        Program.Player.Wield( i, true );
                }
                if (Program.Player.Body.MainHand is MeleeWeapon mWeapon && ( mWeapon is Axe || mWeapon is Sword ))
                    Program.Player.PickWall( ( Wall)blocks[closestStoneWall.X * width + closestStoneWall.Y] );
                else {
                    Program.MsgConsole.WriteLine( $"{Program.Player.Name} can't mine a stone because they don't have a pickaxe!" );
                    paused = true;
                }
            }
        }

        private static void HandleCraftComponent()
        {
            CraftingManager.CraftingRecipe = currentCraftingRecipe;

            Point worldIndex = Program.Player.WorldIndex;
            Scheduler.HandleCraftingScheduling(currentCraftingRecipe.CraftTime);

            foreach (RecipeComponent rc in currentCraftingRecipe.Recipe)
                Program.Player.Inventory.RemoveAt( Program.Player.Inventory.FindIndex( i => i.ToComponent() == rc ) );

            foreach (Item i in currentCraftingRecipe.CraftingTarget) {
                Program.Player.Inventory.Add( i );
                Program.Player.Stats.LvlSkill( Skill.Crafting, i.ToComponent().ToSkillValue(), Program.Player );
            }

            currentCraftingRecipe = null;
            playerState = PlayerState.GetMaterial;
        }

        private static void HandlePlaceMaterials( RecipeComponent nextComponent )
        {
            bool nextToConstruction = Program.Player.PointNextToSelf(currentConstructionPos);
            
            if (nextToConstruction == false)
                PathToPoint(currentConstructionPos);
            else {
                for (int i = Program.Player.Inventory.Count - 1; i >= 0 ; i--) {
                    Item item = Program.Player.Inventory[i];
                    RecipeComponent itemC = item.ToComponent();
                    if (currentConstructRecipe.Contains(itemC)) {
                        Program.Player.Inventory.Remove(item);
                        constructionMap[currentConstructionPos.X, currentConstructionPos.Y].HeldComponents.Add(nextComponent.ToItem());
                        currentConstructRecipe.Remove(itemC);
                    }
                }

                playerState = PlayerState.GetMaterial;
                getIndex = 0;

                if (currentConstructRecipe.Count == 0)
                    FinishConstruction();
            }
        }

        private static bool DropUnnecessaryItems( RecipeComponent nextComponent )
        {
            bool droppedItems = false;
            for (int i = Program.Player.Inventory.Count - 1; i >= 0; i--) {
                Item I = Program.Player.Inventory[i];
                if (I is MeleeWeapon == false && I is RecipePouch == false && I is Potion == false && I is BlueprintPouch == false && !I.ToComponent().Equals(nextComponent)) {
                    List<Point> nearbyChests = Program.Player.GetNearbyBlocksOfType( BlockType.Chest );
                    if (nearbyChests.Count > 0) {
                        int currentFloor = Program.Player.CurrentFloor;
                        Point worldIndex = Program.Player.WorldIndex;
                        Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                        int width = Program.WorldMap.TileWidth;

                        Chest chest = (Chest)blocks[nearbyChests[0].X * width + nearbyChests[0].Y];
                        chest.Inventory.Add( I );
                        Program.Player.Inventory.RemoveAt(i);
                    }
                    Program.Player.Drop(I);
                    droppedItems = true;
                }
            }
            return droppedItems;
        }

        private static void FinishConstruction()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;

            Block building = constructionMap[currentConstructionPos.X, currentConstructionPos.Y].BuildTarget;
            if (!currentConstructionPos.Equals(Program.Player.Position))
                blocks[currentConstructionPos.X * width + currentConstructionPos.Y] = building.Copy();
            else
                Program.Player.CurrentBlock = building.Copy();

            constructionMap[currentConstructionPos.X, currentConstructionPos.Y] = null;
            currentConstructionPos = new Point();

            DetermineNextConstruction();
            playerState = PlayerState.Idle;
            Program.Player.Path = null;
        }


        // RENDERING

        public static void RenderConstructionMap()
        {
            Point startPoint = Program.Window.CalculateMapStartPoint();
            for (int i = startPoint.X; i - startPoint.X < Program.Window.Width - (StatusPanel.Width); i++)
                for (int j = startPoint.Y; j - startPoint.Y < Program.Window.Height; j++)
                    DrawCell(i, j);
        }
        
        public static void DrawCell(int x, int y)
        {
            Point startPoint = Program.Window.CalculateMapStartPoint();

            BuildingPlaceholder construction = constructionMap[x, y];

            if (construction == null || new Point(x, y).Equals(Program.Player.Position))
                return;

            Color foreColor = construction.ForeColor;
            Color backColor = construction.BackColor;

            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, construction.Graphic, foreColor, backColor);
        }


        // PROPERTIES //

        public static BuildingModeInput InputHandler
        {
            get { return inputHandler; }
            set { inputHandler = value; }
        }
        public static bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }
        public static BuildingPlaceholder[,] ConstructionMap
        {
            get { return constructionMap; }
            set { constructionMap = value; }
        }
        public static Point CurrentConstructionPos
        {
            get { return currentConstructionPos; }
            set { currentConstructionPos = value; }
        }
    }
}
