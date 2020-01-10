using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum CreatureState
    {
        GetMaterial,
        PlaceMaterials,
        Idle
    }

    static class BuildingManager
    {
        private static BuildingModeInput inputHandler = new BuildingModeInput();
        private static bool paused = true;
        private static Dictionary<UInt64, CreatureState> creatureStates = new Dictionary<UInt64, CreatureState>() { {Program.Player.ID, CreatureState.Idle} };

        private static BuildingPlaceholder[,] constructionMap = new BuildingPlaceholder[Program.WorldMap.TileWidth, Program.WorldMap.TileHeight];

        private static Point nextConstruction = new Point();
        private static List<RecipeComponent> currentConstructRecipe = null;
        private static CraftingRecipe currentCraftingRecipe = null;
        private static int getIndex = 0;


        // FUNCTIONS //

        // INPUT
        public static void HandleInput()
        {
            inputHandler.HandleKeys();
            inputHandler.HandleMouse();
        }


        // CONSTRUCTION HANDLING

        private static void DetermineNextConstruction()
        {
            if (nextConstruction.Equals(new Point() )) {
                for (int i = 0; i < constructionMap.GetLength(0); i++)
                    for (int j = 0; j < constructionMap.GetLength(1); j++) {
                        if (constructionMap[i, j] != null) {
                            nextConstruction = new Point(i, j);
                            bool enoughMaterials = DetermineIfEnoughMaterials();
                            if (!enoughMaterials)
                            {
                                Menus.DisplayIncorrectUsage("You don't have the required materials!");
                                RemoveAllConstructionsWithName(constructionMap[i, j].Name);
                                nextConstruction = new Point();
                                DetermineNextConstruction();
                            }
                            else
                                currentConstructRecipe = GetConstructRecipe();
                        }
                    }
            }
        }
        private static void RemoveAllConstructionsWithName(string name)
        {
            for (int i = 0; i < constructionMap.GetLength(0); i++)
                for (int j = 0; j < constructionMap.GetLength(1); j++) {
                    if (constructionMap[i, j] != null && constructionMap[i, j].Name == name)
                        constructionMap[i, j] = null;
                }
        }
        private static List<RecipeComponent> GetConstructRecipe()
        {
            List<RecipeComponent> recipe = new List<RecipeComponent>();
            BuildingPlaceholder currentConstruct = constructionMap[nextConstruction.X, nextConstruction.Y];

            // populate recipe list, always draws from you the player.
            foreach (Blueprint bp in ((BlueprintPouch)Program.Player.Body.MainHand).Blueprints)
                if (bp.BlueprintTarget.Name == currentConstruct.Name) {
                    foreach (RecipeComponent rc in bp.Recipe)
                        recipe.Add(rc);
                    break;
                }

            return recipe;
        }
        private static bool DetermineIfEnoughMaterials()
        {
            List<RecipeComponent> recipe = GetConstructRecipe();
            if (recipe == null) {
                return false;
            }
            
            // check player inventory
            foreach (Item item in Program.Player.Inventory) {
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
        private static Point GetClosestMaterialPos(Creature c, RecipeComponent recipe, bool checkInventory)
        {
            int currentFloor = c.CurrentFloor;
            Point worldIndex = c.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            // check inventory
            if (checkInventory) {
                foreach (Item item in c.Inventory) {
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

        internal static void DetermineCreatureAction(Creature c, bool calledFromCraftingScheduler = false)
        {
            if (!creatureStates.ContainsKey(c.ID))
                creatureStates.Add(c.ID, CreatureState.Idle);

            if (calledFromCraftingScheduler) {
                c.Wait();
                return;
            }

            DetermineNextConstruction();
            if (nextConstruction.Equals(new Point()) == false) // this means a pending construction has been found and it can definitely be built
            {
                RecipeComponent nextComponent = RecipeComponent.Null;
                if (currentConstructRecipe.Count > getIndex) // if get has not gotten up to the count
                    nextComponent = currentConstructRecipe[getIndex];
                else {
                    c.Path = null;
                    creatureStates[c.ID] = CreatureState.PlaceMaterials;
                }
                if (creatureStates[c.ID] == CreatureState.Idle)
                    creatureStates[c.ID] = CreatureState.GetMaterial;

                if (creatureStates[c.ID] == CreatureState.GetMaterial)
                    HandleGetMaterial(c, nextComponent);
                else if (creatureStates[c.ID] == CreatureState.PlaceMaterials)
                    HandlePlaceMaterials(c, nextComponent);
            }
            else /* there is no pending construction */ {
                creatureStates[c.ID] = CreatureState.Idle;
                paused = true;
            }

            c.DetermineAction();
        }
        private static void PathToPoint(Creature c, Point pos)
        {
            int currentFloor = c.CurrentFloor;
            Point worldIndex = c.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            if (pos.Equals(new Point()) == true)
                throw new Exception();
            if (c.Path == null)
            {
                List<Point> freeSpots = blocks.GetEmptyAdjacentBlocks(new Point(width, height), pos);
                Point nextPos = freeSpots.Count > 0 ? freeSpots[Program.RNG.Next(0, freeSpots.Count)] : new Point();
                if (!nextPos.Equals(new Point()))
                    c.SetPath( nextPos );
                else
                {
                    Program.MsgConsole.WriteLine( $"The {constructionMap[nextConstruction.X, nextConstruction.Y]} couldn't be built because a path couldn't be found." );
                    constructionMap[nextConstruction.X, nextConstruction.Y] = null;
                    nextConstruction = new Point();

                    DetermineNextConstruction();
                    creatureStates[c.ID] = CreatureState.Idle;
                    c.Path = null;
                }
            }
        }
        private static void HandleGetMaterial(Creature c, RecipeComponent nextComponent, bool forCrafting = false)
        {
            Point nextPos = GetClosestMaterialPos(c, nextComponent, true); // returns new Point() if item is in player's inventory, returns null if the object can't be found
            
            // if the material is not on the map or in your inventory
            if (nextPos == null)
            {
                if (nextComponent == RecipeComponent.Log)
                    HandleChopTree(c);
                else if (nextComponent == RecipeComponent.Stone)
                    HandleMineRock(c);
                else {
                    // if there is no crafting recipe stored and the player has a recipe pouch
                    if (currentCraftingRecipe == null && c.Inventory.Exists(i => i is RecipePouch)) {
                        RecipePouch rp = (RecipePouch)c.Inventory.Find(i => i is RecipePouch);
                        foreach (CraftingRecipe r in rp.Recipes)
                            if (r.CraftingTarget.Exists(e => e.ToComponent() == nextComponent)) {
                                currentCraftingRecipe = r;
                                break;
                            }
                    }
                    else if (currentCraftingRecipe != null)
                    {
                        if (!currentCraftingRecipe.CraftingTarget.Exists(e => e.ToComponent() == nextComponent))
                            currentCraftingRecipe = null;
                        if (c.Inventory.Exists(i => i.ToComponent() == currentCraftingRecipe.Recipe[0]))
                            HandleCraftComponent(c);
                        else
                            HandleGetMaterial(c, currentCraftingRecipe.Recipe[0], true);
                    }
                }
                return;
            } 
            
            if (nextPos.Equals(new Point()))
                nextPos = GetClosestMaterialPos(c, nextComponent, false);


            if (nextPos == null) {
                creatureStates[c.ID] = CreatureState.PlaceMaterials;
                return;
            }

            if (!nextPos.Equals(new Point()) || c.CanCarryItem(nextComponent.ToItem()) == true)
            {
                bool nextToItem = c.Position.NextToPoint(nextPos);
                int currentFloor = c.CurrentFloor;
                Point worldIndex = c.WorldIndex;
                Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

                if (nextToItem) {
                    if ( blocks[nextPos.X * width + nextPos.Y] is Chest chest ) {
                        for (int i = chest.Inventory.Count - 1; i >= 0; i--) {
                            if (chest.Inventory[i].ToComponent() == nextComponent) {
                                bool itemAdded = c.AddItem( chest.Inventory[i] );
                                if (itemAdded)
                                    chest.Inventory.RemoveAt( i );
                                else if (DropUnnecessaryItems(c, nextComponent)) {
                                    itemAdded = c.AddItem( chest.Inventory[i] );
                                    if (itemAdded)
                                        chest.Inventory.RemoveAt( i );
                                    else
                                        creatureStates[c.ID] = CreatureState.PlaceMaterials;
                                }
                                else
                                    creatureStates[c.ID] = CreatureState.PlaceMaterials;
                            }
                        }
                    }
                    else {
                        if (c.CanCarryItem( (Item)blocks[nextPos.X * width + nextPos.Y] )) {
                            c.GetItem( nextPos );
                            getIndex++;
                        }
                        else {
                            bool droppedItems = DropUnnecessaryItems(c, nextComponent);
                            if (droppedItems == false)
                                creatureStates[c.ID] = CreatureState.PlaceMaterials;
                        }
                    }
                }
                else if (c.Path == null)
                    c.SetPath(nextPos);
            }
            else {
                c.Path = null;
                creatureStates[c.ID] = CreatureState.PlaceMaterials;
            }
        }
        private static void HandleChopTree(Creature c)
        {
            int currentFloor = c.CurrentFloor;
            Point worldIndex = c.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Point closestTree = blocks.GetClosestOfBlockTypeToPos( c.Position, new Point(width, height), BlockType.Tree);
            bool nextToTree = c.Position.NextToPoint(closestTree);
            if (!nextToTree)
                PathToPoint(c, closestTree);
            else {
                if (c.Body.MainHand != null && c.Body.MainHand is Axe == false)
                    for (int i = 0; i < c.Inventory.Count; i++) {
                        Item I = c.Inventory[i];
                        if (I is Axe || I is Sword)
                            c.Wield(i, true);
                    }
                c.ChopTree(closestTree);
            }

        }
        private static void HandleMineRock(Creature c)
        {
            int currentFloor = c.CurrentFloor;
            Point worldIndex = c.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Point closestStoneWall = blocks.GetClosestOfBlockTypeToPos( c.Position, new Point(width, height), BlockType.Wall, Material.Stone );
            bool nextToStoneWall = c.Position.NextToPoint( closestStoneWall );
            if (!nextToStoneWall) {
                PathToPoint( c, closestStoneWall );
            }
            else {
                for (int i = 0; i < c.Inventory.Count; i++) {
                    Item I = c.Inventory[i];
                    if (I is Axe axe && axe.Name == "pickaxe")
                        c.Wield( i, true );
                }
                if (c.Body.MainHand is MeleeWeapon mWeapon && ( mWeapon is Axe || mWeapon is Sword ))
                    c.PickWall( ( Wall)blocks[closestStoneWall.X * width + closestStoneWall.Y] );
                else {
                    Program.MsgConsole.WriteLine( $"{c.Name} can't mine a stone because they don't have a pickaxe!" );
                    paused = true;
                }
            }
        }
        private static void HandleCraftComponent(Creature c)
        {
            CraftingManager.CraftingRecipe = currentCraftingRecipe;

            Point worldIndex = c.WorldIndex;
            Scheduler.HandleCraftingScheduling(currentCraftingRecipe.CraftTime);

            foreach (RecipeComponent rc in currentCraftingRecipe.Recipe)
                c.Inventory.RemoveAt( c.Inventory.FindIndex( i => i.ToComponent() == rc ) );

            foreach (Item i in currentCraftingRecipe.CraftingTarget) {
                c.Inventory.Add( i );
                c.Stats.LvlSkill( Skill.Crafting, i.ToComponent().ToSkillValue(), c );
            }

            currentCraftingRecipe = null;
            creatureStates[c.ID] = CreatureState.GetMaterial;
        }
        private static void HandlePlaceMaterials(Creature c, RecipeComponent nextComponent)
        {
            bool nextToConstruction = c.Position.NextToPoint(nextConstruction);
            
            if (nextToConstruction == false)
                PathToPoint(c, nextConstruction);
            else {
                for (int i = c.Inventory.Count - 1; i >= 0 ; i--) {
                    Item item = c.Inventory[i];
                    RecipeComponent itemC = item.ToComponent();
                    if (currentConstructRecipe.Contains(itemC)) {
                        c.Inventory.Remove(item);
                        constructionMap[nextConstruction.X, nextConstruction.Y].HeldComponents.Add(nextComponent.ToItem());
                        currentConstructRecipe.Remove(itemC);
                    }
                }

                creatureStates[c.ID] = CreatureState.GetMaterial;
                getIndex = 0;

                if (currentConstructRecipe.Count == 0)
                    FinishConstruction(c);
            }
        }
        private static bool DropUnnecessaryItems(Creature c, RecipeComponent nextComponent)
        {
            bool droppedItems = false;
            for (int i = c.Inventory.Count - 1; i >= 0; i--) {
                Item I = c.Inventory[i];
                if (!(I is MeleeWeapon || I is RangedWeapon || I is RecipePouch || I is BlueprintPouch || I is Quiver || I is Potion || I is Food) && !I.ToComponent().Equals(nextComponent)) {
                    List<Point> nearbyChests = c.GetNearbyBlocksOfType( BlockType.Chest );
                    if (nearbyChests.Count > 0) {
                        int currentFloor = c.CurrentFloor;
                        Point worldIndex = c.WorldIndex;
                        Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                        int width = Program.WorldMap.TileWidth;

                        Chest chest = (Chest)blocks[nearbyChests[0].X * width + nearbyChests[0].Y];
                        chest.Inventory.Add( I );
                        c.Inventory.RemoveAt(i);
                    }
                    bool success = c.Drop(I);
                    if (!success) creatureStates[c.ID] = CreatureState.Idle;
                    else droppedItems = true;
                }
            }
            return droppedItems;
        }
        private static void FinishConstruction(Creature c)
        {
            int currentFloor = c.CurrentFloor;
            Point worldIndex = c.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;

            Block building = constructionMap[nextConstruction.X, nextConstruction.Y].BuildTarget;
            if (!nextConstruction.Equals(c.Position))
                blocks[nextConstruction.X * width + nextConstruction.Y] = building.Copy();
            else
                c.CurrentBlock = building.Copy();

            constructionMap[nextConstruction.X, nextConstruction.Y] = null;
            nextConstruction = new Point();

            DetermineNextConstruction();
            creatureStates[c.ID] = CreatureState.Idle;
            c.Path = null;
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
        public static Point NextConstruction
        {
            get { return nextConstruction; }
            set { nextConstruction = value; }
        }
    }
}
