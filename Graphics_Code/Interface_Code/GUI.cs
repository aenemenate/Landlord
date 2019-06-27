using System;
using System.Collections.Generic;
using SadConsole;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Media;

namespace Landlord
{
    static class GUI
    {
        private static SadConsole.Console console;
        private static List<Point> path;
        private static Point prevMapPos;
        private static DateTime lastFrameTime = DateTime.Now;
        private static int fps = 0, fSinceLastSec = 0;

        // CLASSES //

        public class IItemList
        {
            private int currentPage;
            private int pageCount;
            private int itemsPerPage;
            private int itemPadding;

            private Point startPos;
            private int width, height;
            private Item currentlyViewedItem;
            private bool mouseOnItem;

            private List<Item> containerInventory;
            private List<Item> containerItems;
            private Dictionary<string, int> itemsCount;

            private Point rButtonPos;
            private Point lButtonPos;


            // CONSTRUCTOR //

            public IItemList(List<Item> containerInventory, Point topLeftCorner, int width, int height, int itemPadding, Point rButtonPos, Point lButtonPos)
            {
                this.containerInventory = containerInventory;
                startPos = topLeftCorner;
                this.width = width;
                this.height = height;
                this.rButtonPos = rButtonPos;
                this.lButtonPos = lButtonPos;

                containerItems = new List<Item>();
                itemsCount = new Dictionary<string, int>();

                currentlyViewedItem = null;
                mouseOnItem = false;

                itemsPerPage = height / 3;
                currentPage = 0;
                pageCount = 1;
                this.itemPadding = itemPadding;
            }


            // FUNCTIONS //

            // RENDER //
            // <itemList> = the items to be rendered
            public void Render(List<Item> itemList, Color itemColor, Color highlightedColor, Color itemBgColor, Color bgColor)
            {
                itemsPerPage = height / itemPadding;
                if (height % itemPadding != 0)
                    itemsPerPage++;

                bool highlighting = false;
                bool highlighted = false;

                Color tempBgColor = itemBgColor;

                // print the right page turning button
                Program.Console.Print(rButtonPos.X, rButtonPos.Y, " ", itemColor, bgColor);
                if (Program.Window.MousePos.Equals(rButtonPos)) tempBgColor = highlightedColor * 0.95F;
                if (pageCount > 1 && currentPage < pageCount - 1) Program.Console.Print(rButtonPos.X, rButtonPos.Y, ">", Color.AntiqueWhite, tempBgColor);
                tempBgColor = itemBgColor;

                // print the left page turning button
                Program.Console.Print(lButtonPos.X, lButtonPos.Y, " ", itemColor, bgColor);
                if (Program.Window.MousePos.Equals(lButtonPos)) tempBgColor = highlightedColor * 0.95F;
                if (pageCount > 1 && currentPage > 0) Program.Console.Print(lButtonPos.X, lButtonPos.Y, "<", Color.AntiqueWhite, tempBgColor);

                // print the items
                for (int i = 0, index = currentPage * itemsPerPage; i < height; i += itemPadding, index++)
                {
                    string name = "";
                    bool indexGreaterThanPlayersItems = index >= itemsCount.Count;

                    itemColor = Color.AntiqueWhite;

                    Point itemPos = new Point(startPos.X, startPos.Y + i);

                    // this is to clear the line
                    Program.Console.Print(itemPos.X, itemPos.Y, new String(' ', Width), Color.Black, bgColor);
                    Program.Console.Print(itemPos.X, itemPos.Y + 1, new String(' ', Width), Color.Black, bgColor);

                    if (itemsCount.Any() && !indexGreaterThanPlayersItems)
                    {
                        Item item = containerItems[index];
                        name = item.Name;
                        string invCounter = "x" + itemsCount[item.Name];

                        string pt2 = "";
                        Tuple<string, string> nameParts = Window.SplitNameIfGreaterThanLength(name, pt2, Width - invCounter.Length - 1);
                        name = nameParts.Item1;
                        if (nameParts.Item2 != "")
                            pt2 = nameParts.Item2;

                        // check if the mouse is on the item that is currently being drawn so it will be highlighted
                        if (Program.Window.MousePos.Y >= StartPos.Y && Program.Window.MousePos.X >= startPos.X && Program.Window.MousePos.X <= (startPos.X + Width))
                            if (Program.Window.MousePos.Y - StartPos.Y == i || Program.Window.MousePos.Y - startPos.Y - 1 == i)
                            {
                                highlighted = true;
                                highlighting = true;
                            }

                        if (item.Identified)
                            itemColor = item.ReturnRarityColor();

                        itemBgColor = (index % 2 == 0) ? InventoryPanel.color : InventoryPanel.lighterColor; // alternate the background color

                        itemColor = (highlighted == true) ? highlightedColor : itemColor;
                        
                        // print the name's first line
                        Program.Console.Print(itemPos.X, itemPos.Y, name + new String(' ', Width - name.Length - invCounter.Length), itemColor, itemBgColor);
                        // print the count
                        Program.Console.Print(startPos.X + Width - invCounter.Length, itemPos.Y, invCounter, Color.AntiqueWhite, itemBgColor);

                        // print / clear the name's second line
                        Program.Console.Print(itemPos.X, itemPos.Y + 1, new String(' ', Width), itemColor, itemBgColor);
                        if (pt2 != "")
                            Program.Console.Print(itemPos.X, itemPos.Y + 1, pt2, itemColor, itemBgColor);

                        Tuple<byte, Color> comparisonArrow = GetItemArrow(item);
                        if (comparisonArrow.Item1 != 0)
                            Program.Console.SetGlyph(itemPos.X + (pt2 == "" ? name.Length : pt2.Length), itemPos.Y + (pt2 == "" ? 0 : 1), comparisonArrow.Item1, comparisonArrow.Item2);

                    }
                    highlighted = false;
                }

                mouseOnItem = highlighting;
            }

            public void Update()
            {
                DeterminePages();

                // page switching logic
                if (Program.Window.MousePos.Equals(rButtonPos) && Global.MouseState.LeftClicked)
                    IncreasePage();
                else if (Program.Window.MousePos.Equals(lButtonPos) && Global.MouseState.LeftClicked)
                    DecreasePage();

                ConvertInventoryForDisplay();
            }

            private void IncreasePage()
            {
                if (currentPage < pageCount - 1)
                    currentPage += 1;
            }

            private void DecreasePage()
            {
                if (currentPage > 0)
                    currentPage -= 1;
            }

            public bool DetermineClickedItem()
            {
                int index = -1;
                if (Program.Window.MousePos.Y >= startPos.Y)
                {
                    index = (Program.Window.MousePos.Y - startPos.Y) / 3 + (currentPage * itemsPerPage);
                    if (containerItems.Count <= index || index < 0)
                        return false;
                    currentlyViewedItem = containerItems[index];
                }
                return true;
            }

            private void ConvertInventoryForDisplay()
            {
                // items are stored in the inventory on an individual basis.
                // they are displayed in chunks that have a counter
                containerItems = new List<Item>();
                itemsCount = new Dictionary<string, int>();

                bool InventoryItemsContains(Item item)
                {
                    foreach (Item i in containerItems)
                        if (item.Name == i.Name && item.Rarity == i.Rarity)
                            return true;
                    return false;
                }


                containerInventory.Sort();
                foreach (Item item in containerInventory)
                {
                    if (!InventoryItemsContains(item))
                    {
                        itemsCount[item.Name] = 1;
                        containerItems.Add(item);
                    }
                    else
                        itemsCount[item.Name] += 1;
                }
            }

            private void DeterminePages()
            {
                pageCount = containerItems.Count / itemsPerPage;
                if (containerItems.Count % itemsPerPage != 0)
                    pageCount ++;
                if (currentPage > pageCount - 1 & containerItems.Count > 2)
                    currentPage = pageCount - 1;
            }


            // PROPERTIES //

            public int CurrentPage
            {
                get { return currentPage; }
                set { currentPage = value; }
            }

            public int PageCount
            {
                get { return pageCount; }
                set { pageCount = value; }
            }

            public int ItemsPerPage
            {
                get { return itemsPerPage; }
                set { itemsPerPage = value; }
            }

            public int ItemPadding
            {
                get { return itemPadding; }
                set { itemPadding = value; }
            }

            public Point StartPos
            {
                get { return startPos; }
                set { startPos = value; }
            } 

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

            public Item CurrentlyViewedItem
            {
                get { return currentlyViewedItem; }
                set { currentlyViewedItem = value; }
            }

            public bool MouseOnItem
            {
                get { return mouseOnItem; }
                set { mouseOnItem = value; }
            }

            public List<Item> ContainerInventory
            {
                get { return containerInventory; }
                set { containerInventory = value; }
            }

            public List<Item> ContainerItems
            {
                get { return containerItems; }
                set { containerItems = value; }
            }

            public Dictionary<string, int> ItemsCount
            {
                get { return itemsCount; }
                set { itemsCount = value; }
            }

            public Point RButtonPos
            {
                get { return rButtonPos; }
                set { rButtonPos = value; }
            }

            public Point LButtonPos
            {
                get { return lButtonPos; }
                set { lButtonPos = value; }
            }
        }

        public class LootMenu
        {
            public static int Width { get { return (MapWidth -  InventoryPanel.Width) / 3 * 2; } }
            public static int Height { get { return Program.Window.Height / 3 * 2; } }
            public static int StartX { get { return ( Program.Console.Width ) / 2 - Width / 2; } }
            public static int StartY { get { return Program.Console.Height / 2 - Height / 2; } }

            public IItemList iItemList;
            
            private string containerName;
            private static bool clickedContainer;

            private Point TakeAllPos { get { return new Point(StartX + Width - 1 - "Take All".Length, StartY + Height - 2); } }


            // CONSTRUCTOR //

            public LootMenu(List<Item> containerInventory, string containerName)
            {
                this.containerName = containerName;

                iItemList = new IItemList(containerInventory, new Point(StartX + 1, StartY + 3), Width - 2, Height - 6, 3, new Point(StartX + Width - 2, StartY + 1), new Point(StartX + 1, StartY + 1));

            }

            // FUNCTIONS //

            public void RenderLootMenu()
            {
                if (Program.Animations.Count > 0)
                    return;

                void PrintHeader()
                {
                    // fill the background
                    if (!Program.Console.GetBackground(StartX, StartY).Equals(InventoryPanel.darkerColor))
                        for (int i = StartX; i < StartX + Width; i++)
                            for (int j = StartY; j < StartY + Height; j++)
                                Program.Console.SetGlyph(i, j, ' ', Color.White, InventoryPanel.darkerColor);
                    // print the header
                    Program.Console.Print(StartX + Width / 2 - containerName.Length / 2, StartY + 1, containerName, Color.AntiqueWhite);
                    
                    // print the take all button
                    Program.Console.Print(TakeAllPos.X, TakeAllPos.Y, "        ", Color.AntiqueWhite, InventoryPanel.darkerColor);

                    Color bgColor = InventoryPanel.color;
                    if (Program.Window.MousePos.X >= TakeAllPos.X && Program.Window.MousePos.X < TakeAllPos.X + "Take All".Length && Program.Window.MousePos.Y == TakeAllPos.Y)
                        bgColor = Color.Green * 0.95F;
                    if (iItemList.ContainerInventory.Count > 0)
                        Program.Console.Print(TakeAllPos.X, TakeAllPos.Y, "Take All", Color.AntiqueWhite, bgColor);
                }
                
                PrintHeader();

                iItemList.Render(iItemList.ContainerInventory, Color.White, Color.Green, InventoryPanel.color, InventoryPanel.darkerColor);
            }

            public void HandleLootMenu()
            {
                iItemList.Update();

                void TakeAll()
                {
                    for (int i = iItemList.ContainerInventory.Count - 1; i >= 0; i--)
                    {
                        bool itemAdded = Program.Player.AddItem(iItemList.ContainerInventory[i]);
                        if (itemAdded) iItemList.ContainerInventory.RemoveAt(i);
                    }
                }

                void TakeAllOfItem(Item item)
                {
                    for (int i = iItemList.ContainerInventory.Count - 1; i >= 0; i--)
                    {
                        if (iItemList.ContainerInventory[i].CompareTo(item) == 0)
                        {
                            bool itemAdded = Program.Player.AddItem(iItemList.ContainerInventory[i]);
                            if (itemAdded)
                                iItemList.ContainerInventory.RemoveAt(i);
                            else
                                System.Console.WriteLine("Not enough space");
                        }
                    }
                }


                //item selection logic

                bool clickedOutsideOfWindow = Program.Window.MousePos.X < StartX || Program.Window.MousePos.Y < StartY || Program.Window.MousePos.X >= StartX + Width || Program.Window.MousePos.Y >= StartY + Height;
                bool clickedInventory = Program.Window.MousePos.X < InventoryPanel.Width;
                bool clickedTakeAll = Program.Window.MousePos.X >= TakeAllPos.X && Program.Window.MousePos.X < TakeAllPos.X + "Take All".Length && Program.Window.MousePos.Y == TakeAllPos.Y;


                if (SadConsole.Global.MouseState.LeftClicked && !clickedContainer) {
                    if (iItemList.MouseOnItem)
                    {
                        bool determined = iItemList.DetermineClickedItem();
                        if (!determined)
                            return;
                        if (Global.KeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                            TakeAllOfItem(iItemList.CurrentlyViewedItem);
                        else {
                            bool itemAdded = Program.Player.AddItem(iItemList.CurrentlyViewedItem);
                            if (itemAdded)
                                iItemList.ContainerInventory.Remove(iItemList.CurrentlyViewedItem);
                            else
                                System.Console.WriteLine("Not enough space");
                        }
                    } else if (clickedOutsideOfWindow && !clickedInventory)
                        Program.Animations.Add( new CloseLootView() );
                    else if (clickedTakeAll)
                        TakeAll();
                }
                clickedContainer = false;
            }


            // PROPERTIES //

            public List<Item> ContainerInventory
            {
                get { return iItemList.ContainerInventory; }
                set { iItemList.ContainerInventory = value; }
            }

            public static bool ClickedContainer
            {
                get { return clickedContainer; }
                set { clickedContainer = value; }
            }
        }

        public static class CraftMenu
        {
            public static int Width { get { return (MapWidth / 3) * 2; } }
            public static int Height { get { return (Program.Window.Height / 4) * 3; } }
            public static int StartX { get { return ( Program.Console.Width - 22 ) / 2 - Width / 2; } }
            public static int StartY { get { return Program.Console.Height / 2 - Height / 2; } }

            private static int invCurrentPage = 0;
            private static int inventoryPages = 1;
            private static int itemsPerPage;

            private static CraftingRecipe currentlyViewedRecipe = null;

            private static Color color = Color.White, highlightedColor = Color.Green, bgColor = InventoryPanel.darkerColor;
            
        #pragma warning disable IDE1006 // Naming Styles
            private static Point rightButtonPos { get { return new Point(StartX + Width - 2, StartY + 1); } }
            private static Point leftButtonPos { get { return new Point(StartX + 1, StartY + 1); } }
            private static Point backButtonPos { get { return new Point(StartX + Width - 1 - "Back".Length, StartY + Height - 2); } }
            private static Point craftButtonPos { get { return new Point(StartX + Width / 2 - "Craft!".Length / 2, StartY + Height - 2); } }
            private static Point cancelButtonPos { get { return new Point(StartX + Width - 1 - "Cancel".Length, StartY + Height - 2); } }
        #pragma warning restore IDE1006 // Naming Styles


            // FUNCTIONS //
            
            // INPUT HANDLING //
            public static void HandleCraftMenu()
            {
                if (Program.Animations.Count > 0)
                    return;

                Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                    SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);

                HandleExitMenu(mousePos);

                if (currentlyViewedRecipe == null)
                    HandleSelectingRecipe( mousePos );
                else
                    HandleViewingRecipe(mousePos);

                if (CraftingManager.Crafting)
                    CraftingManager.ContinueCrafting();
            }

            public static void HandleSelectingRecipe(Point mousePos)
            {
                List<CraftingRecipe> recipes = ((RecipePouch)Program.Player.Body.MainHand).Recipes;

                bool DetermineCurrentItem()
                {
                    int index = -1;
                    if (mousePos.Y >= StartY + 3)
                    {
                        index = (mousePos.Y - (StartY + 3)) / 3 + (invCurrentPage * itemsPerPage);
                        if (recipes.Count <= index || index < 0)
                            return false;
                        currentlyViewedRecipe = recipes[index];
                    }
                    return true;
                }


                //item selection logic
                bool highlighting = RenderCraftMenu(mousePos);

                if (SadConsole.Global.MouseState.LeftClicked && Menus.ClickedDialog == false)
                {
                    if (highlighting)
                    {
                        bool determined = DetermineCurrentItem();
                        if (!determined)
                            return;
                    } else if (mousePos.Equals( rightButtonPos ) && invCurrentPage < inventoryPages - 1)
                        invCurrentPage += 1;
                    else if (mousePos.Equals( leftButtonPos ) && invCurrentPage > 0)
                        invCurrentPage -= 1;
                } else
                    Menus.ClickedDialog = false;

            }

            private static void HandleViewingRecipe(Point mousePos)
            {
                RenderRecipeView(mousePos);

                if (SadConsole.Global.MouseState.LeftClicked)
                {
                    if (!CraftingManager.Crafting)
                    {
                        bool mouseOnBackButton = mousePos.X >= backButtonPos.X && mousePos.X < backButtonPos.X + "Back".Length && mousePos.Y == backButtonPos.Y;
                        bool mouseOnCraftBUtton = mousePos.X >= craftButtonPos.X && mousePos.X < craftButtonPos.X + "Craft!".Length && mousePos.Y == craftButtonPos.Y;

                        if (mouseOnBackButton)
                            currentlyViewedRecipe = null;
                        else if (mouseOnCraftBUtton)
                            CraftingManager.StartCrafting(currentlyViewedRecipe);
                    }
                    else
                    {
                        bool mouseOnCancelButton = mousePos.X >= cancelButtonPos.X && mousePos.X < cancelButtonPos.X + "Cancel".Length && mousePos.Y == cancelButtonPos.Y;

                        if (mouseOnCancelButton)
                            CraftingManager.Crafting = false;
                    }
                }
            }

            private static void HandleExitMenu(Point mousePos)
            {
                bool mouseOutsideOfWindow = mousePos.X < StartX || mousePos.Y < StartY || mousePos.X >= StartX + Width || mousePos.Y >= StartY + Height;
                bool mouseOverInventory = mousePos.X < InventoryPanel.Width;

                if (SadConsole.Global.MouseState.LeftClicked)
                {
                    if (mouseOutsideOfWindow && !mouseOverInventory)
                    {
                        Program.Animations.Add(new CloseCraftMenu());
                        currentlyViewedRecipe = null;
                    }
                }
            }

            // RENDERING //
            private static void PrintWindow(string title, Point mousePos, Color color, Color bgColor, Color highlightedColor)
            {
                // fill the background
                if (!Program.Console.GetBackground(StartX, StartY).Equals(InventoryPanel.darkerColor))
                    for (int i = StartX; i < StartX + Width; i++)
                        for (int j = StartY; j < StartY + Height; j++)
                            Program.Console.SetGlyph(i, j, ' ', color, bgColor);
                // print the header
                Program.Console.Print(StartX + Width / 2 - title.Length / 2, StartY + 1, title, color);

                if (currentlyViewedRecipe == null)
                {
                    // print the right page turning button
                    Program.Console.Print(rightButtonPos.X, rightButtonPos.Y, " ", color, bgColor);
                    if (mousePos.Equals(rightButtonPos))
                        bgColor = highlightedColor * 0.95F;
                    if (inventoryPages > 1 && invCurrentPage < inventoryPages - 1)
                        Program.Console.Print(rightButtonPos.X, rightButtonPos.Y, ">", color, bgColor);

                    // print the left page turning button
                    bgColor = InventoryPanel.darkerColor;
                    Program.Console.Print(leftButtonPos.X, leftButtonPos.Y, " ", color, InventoryPanel.darkerColor);
                    if (mousePos.Equals(leftButtonPos))
                        bgColor = highlightedColor * 0.95F;
                    if (inventoryPages > 1 && invCurrentPage > 0)
                        Program.Console.Print(leftButtonPos.X, leftButtonPos.Y, "<", color, bgColor);
                }
            }

            public static bool RenderCraftMenu( Point mousePos )
            {

                bool noWorkbenchRequirement( CraftingRecipe rc )
                {
                    return rc.WorkbenchRequirement == BlockType.Empty;
                }
                bool craftingTableRequirementMet( CraftingRecipe rc )
                {
                    return ( Program.Player.GetNearbyBlocksOfType( BlockType.CraftingTable ).Count > 0 && rc.WorkbenchRequirement == BlockType.CraftingTable );
                }
                bool stoneMillRequirementMet( CraftingRecipe rc )
                {
                    return ( Program.Player.GetNearbyBlocksOfType( BlockType.StoneMill ).Count > 0 && rc.WorkbenchRequirement == BlockType.StoneMill );
                }


                RecipePouch rp = (RecipePouch)Program.Player.Body.MainHand;
                List<CraftingRecipe> recipes =
                    rp.Recipes.FindAll( rc => ( noWorkbenchRequirement( rc ) || craftingTableRequirementMet( rc ) || stoneMillRequirementMet( rc ) ) );

                int padding = 3;
                int heightOfItems = recipes.Count;
                int lootPortHeight = (Height - 6);
                itemsPerPage = lootPortHeight / padding;


                // FUNCTIONS

                void DeterminePages()
                {
                    if (lootPortHeight % padding != 0)
                        itemsPerPage += 1;
                    inventoryPages = heightOfItems / itemsPerPage;
                    if (heightOfItems % itemsPerPage != 0)
                        inventoryPages += 1;
                    if (invCurrentPage > inventoryPages - 1 & heightOfItems > 2)
                        invCurrentPage = inventoryPages - 1;
                }

                // START

                DeterminePages();

                PrintWindow("RECIPE MENU", mousePos, color, InventoryPanel.darkerColor, highlightedColor);

                bool highlighting = false;
                bool highlighted = false;

                // print the items
                for (int i = 0, index = invCurrentPage * itemsPerPage; i < lootPortHeight; i += padding, index++)
                {
                    string name = "";
                    bool indexGreaterThanRecipeCount = index >= recipes.Count;
                    color = Color.AntiqueWhite;

                    Point itemPos = new Point(StartX + 1, StartY + 3 + i);

                    // this is to clear the line
                    Program.Console.Print(itemPos.X, itemPos.Y, new String(' ', Width - 2), color, InventoryPanel.darkerColor);
                    Program.Console.Print(itemPos.X, itemPos.Y + 1, new String(' ', Width - 2), color, InventoryPanel.darkerColor);

                    if (recipes.Any() && !indexGreaterThanRecipeCount)
                    {
                        Item item = recipes[index];
                        name = item.Name;

                        string pt2 = "";
                        Tuple<string, string> nameParts = Window.SplitNameIfGreaterThanLength(name, pt2, Width - 3);
                        name = nameParts.Item1;
                        if (nameParts.Item2 != "")
                            pt2 = nameParts.Item2;

                        // check if the mouse is on the item that is currently being drawn so it will be highlighted
                        if (mousePos.Y >= StartY + 3 && mousePos.X >= StartX + 1 && mousePos.X <= (StartX + Width - 1))
                            if (mousePos.Y - StartY - 3 == i || mousePos.Y - StartY - 1 - 3 == i)
                            {
                                highlighted = true;
                                highlighting = true;
                            }

                        if (item.Identified)
                            color = item.ReturnRarityColor();

                        bgColor = (index % 2 == 0) ? InventoryPanel.color : InventoryPanel.lighterColor; // alternate the background color

                        color = (highlighted == true) ? highlightedColor : color;

                        string spaces = "";
                        for (int c = 0; c <= (Width - 2) - (1 + name.Length); c++)
                            spaces += ' ';

                        // print the name's first line
                        Program.Console.Print(itemPos.X, itemPos.Y, name + spaces, color, bgColor);

                        // print / clear the name's second line
                        Program.Console.Print(itemPos.X, itemPos.Y + 1, new String(' ', Width - 2), color, bgColor);
                        if (pt2 != "")
                            Program.Console.Print(itemPos.X, itemPos.Y + 1, pt2, color, bgColor);
                    }
                    highlighted = false;
                }

                return highlighting;
            }

            public static void RenderRecipeView(Point mousePos)
            {
                void PrintDescription()
                {
                    if (Program.Console.GetGlyph(StartX + 1, StartY + 4) != currentlyViewedRecipe.Description[0])
                        Program.Window.Print(StartX + 1, StartY + 4, currentlyViewedRecipe.Description, Width - 2, new Color(91, 105, 124) * 1.1F);
                }

                void PrintTimeRequired()
                {
                    string text = "Time to craft: ";
                    text += $"{currentlyViewedRecipe.CraftTime / 60} min";
                    if (currentlyViewedRecipe.CraftTime % 60 != 0)
                        text += $" & {currentlyViewedRecipe.CraftTime % 60} sec";

                    Program.Window.Print(StartX + Width / 2 - text.Length / 2, StartY + 8, text);
                }

                void PrintRequiredComponents()
                {
                    string text = currentlyViewedRecipe.Recipe.Count > 1 ? "Required components: " : "Required component: ";
                    Dictionary<string, int> components = new Dictionary<string, int>();
                    for (int i = 0; i < currentlyViewedRecipe.Recipe.Count; i++)
                    {
                        string componentName = currentlyViewedRecipe.Recipe[i].ToString();
                        if (!components.ContainsKey( componentName ))
                            components.Add( componentName, 1 );
                        else
                            components[componentName]++;
                    }
                    int c = 0;
                    foreach (KeyValuePair<string, int> componentCount in components)
                    {
                        if (c > 0)
                            text += ", ";
                        text += componentCount.Key + $" x {componentCount.Value}";
                        c++;
                    }

                    Program.Window.Print( StartX + Width / 2 - text.Length / 2, StartY + 10, text );
                }

                void PrintRequiredSkillLevel()
                {
                    string text = "Minimum Skill Required: ";

                    text += $"{currentlyViewedRecipe.MinCraftingSkill}";

                    Program.Window.Print( StartX + Width / 2 - text.Length / 2, StartY + 12, text );
                }

                void PrintCraftButton()
                {
                    Program.Console.Print(craftButtonPos.X, craftButtonPos.Y, "      ", color, bgColor);
                    bgColor = InventoryPanel.color;
                    if (mousePos.X >= craftButtonPos.X && mousePos.X < craftButtonPos.X + "Craft!".Length && mousePos.Y == craftButtonPos.Y)
                        bgColor = highlightedColor * 0.95F;

                    Program.Console.Print(craftButtonPos.X, craftButtonPos.Y, "Craft!", color, bgColor);

                    bgColor = InventoryPanel.darkerColor;
                }

                void PrintBackButton()
                {
                    Program.Console.Print(backButtonPos.X, backButtonPos.Y, "    ", color, bgColor);
                    bgColor = InventoryPanel.color;
                    if (mousePos.X >= backButtonPos.X && mousePos.X < backButtonPos.X + "Back".Length && mousePos.Y == backButtonPos.Y)
                        bgColor = highlightedColor * 0.95F;

                    Program.Console.Print(backButtonPos.X, backButtonPos.Y, "Back", color, bgColor);

                    bgColor = InventoryPanel.darkerColor;
                }

                void PrintProgressBar()
                {

                }

                void PrintCancelButton()
                {

                    Program.Console.Print(cancelButtonPos.X, cancelButtonPos.Y, "    ", color, bgColor);
                    bgColor = InventoryPanel.color;
                    if (mousePos.X >= cancelButtonPos.X && mousePos.X < cancelButtonPos.X + "Cancel".Length && mousePos.Y == cancelButtonPos.Y)
                        bgColor = highlightedColor * 0.95F;

                    Program.Console.Print(cancelButtonPos.X, cancelButtonPos.Y, "Cancel", color, bgColor);

                    bgColor = InventoryPanel.darkerColor;
                }
                
                PrintWindow(currentlyViewedRecipe.Name, mousePos, color, bgColor, highlightedColor);
                PrintDescription();
                PrintTimeRequired();
                PrintRequiredComponents();
                PrintRequiredSkillLevel();
                if (!CraftingManager.Crafting)
                {
                    PrintCraftButton();
                    PrintBackButton();
                }
                else
                {
                    PrintProgressBar();
                    PrintCancelButton();
                }
            }

            public static CraftingRecipe CurrentlyViewedRecipe
            {
                get { return currentlyViewedRecipe; }
                set { currentlyViewedRecipe = value; }
            }
        }

        static public class BuildPanel
        {
            static private int blueprintsCurrentPage = 0;
            static private int blueprintsPages = 1;
            static private List<Blueprint> blueprints = new List<Blueprint>();
            static private int blueprintsStartY = 8;

            static private Blueprint currentlySelectedBlueprint = null;
            static private Color panelColorLighter = new Color(79, 121, 66), panelColor = new Color(75, 115, 63), panelColorDarker = new Color(54, 82, 45);


            // FUNCTIONS //

            public static bool HandleBuildPanel()
            {
                Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                    SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);

                void HandlePageSwitching()
                {
                    // page switching logic
                    if (mousePos.Equals(new Point((Program.Window.Width - 2), blueprintsStartY - 2)) && SadConsole.Global.MouseState.LeftClicked)
                        blueprintsCurrentPage += 1;
                    else if (mousePos.Equals(new Point(StartX + 1, blueprintsStartY - 2)) && SadConsole.Global.MouseState.LeftClicked)
                        blueprintsCurrentPage -= 1;
                }

                bool DetermineCurrentItem(bool itemSelected)
                {
                    int index = -1;
                    if (mousePos.Y >= blueprintsStartY && itemSelected)
                    {
                        index = (mousePos.Y - blueprintsStartY) / 3;
                        if (index > blueprints.Count || index < 0)
                            return false;
                        currentlySelectedBlueprint = blueprints[index];
                    }
                    else
                        return false;
                    return true;
                }

                HandlePageSwitching();

                // item selection logic
                bool highlighting = RenderBuildPanel();

                if (SadConsole.Global.MouseState.LeftClicked && Program.CurrentState is Play play && play.PlayMode == PlayMode.BuildMode)
                {
                    bool determined = DetermineCurrentItem(highlighting);
                    if (determined)
                        Program.AudioEngine.PlaySound( Program.AudioEngine.CachedSoundFX["grabBlueprint"] );
                }
                else if (Global.MouseState.RightClicked && Program.CurrentState is Play)
                {
                    bool determined = DetermineCurrentItem(highlighting);
                    if (!determined)
                        return false;
                    currentlySelectedBlueprint = null;
                }
                return highlighting;
            }

            public static bool RenderBuildPanel()
            {

                void AddBlueprints()
                {
                    if (Program.Player.Body.MainHand is BlueprintPouch bpPouch)
                        blueprints = bpPouch.Blueprints;
                }

                Color color = Color.AntiqueWhite, highlightedColor = panelColorLighter * 1.25F, highlightedBgColor = panelColor, bgColor = panelColorDarker;
                blueprints = new List<Blueprint>();

                AddBlueprints();

                int padding = 3, heightOfItems = blueprints.Count, equipmentLen = (Program.Console.Height - 7) - blueprintsStartY, itemsPerPage = equipmentLen / padding;

                Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                    SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);


                // FUNCTIONS //

                void PrintLookFunc()
                {
                    Point worldIndex = Program.Player.WorldIndex;
                    Point mapPos = Program.WorldMap[worldIndex.X, worldIndex.Y].GetMousePos(mousePos);

                    bool mouseIsOnMap = !(mousePos.X < 0 || mousePos.X >= Program.Console.Width - StatusPanel.Width);

                    Program.Window.Print(StartX + 1, Program.Window.Height - 3, "                  ", 18);
                    Program.Window.Print(StartX + 1, Program.Window.Height - 2, "                  ", 18);
                    Program.Window.Print(StartX + 1, Program.Window.Height - 1, "                  ", 18);

                    int currentFloor = Program.Player.CurrentFloor;
                    Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                    Tile[] tiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Floor : Program.WorldMap[worldIndex.X, worldIndex.Y].Floor;
                    int width = Program.WorldMap.TileWidth;
                    if (Program.CurrentState is Play && Program.WorldMap[worldIndex.X, worldIndex.Y].PointWithinBounds(mapPos) && mouseIsOnMap && blocks[mapPos.X * width + mapPos.Y].Explored) {
                        if (blocks[mapPos.X * width + mapPos.Y].Type != BlockType.Empty) {
                            if (blocks[mapPos.X * width + mapPos.Y] is Item item) {
                                Program.Window.Print(StartX + 1, Program.Window.Height - 3, item.Name, 18);
                                Tuple<byte, Color> comparisonArrow = GetItemArrow(item);
                                if (comparisonArrow.Item1 != 0)
                                    Program.Console.SetGlyph(StartX + 1 + item.Name.Length, Program.Window.Height - 3, comparisonArrow.Item1, comparisonArrow.Item2);
                            }
                            else if (blocks[mapPos.X * width + mapPos.Y] is Creature creature) {
                                if (creature is Player == false) {
                                    if (creature.CurrentBlock.Visible == true)
                                        Program.Window.Print(StartX + 1, Program.Window.Height - 3, creature.Name
                                            + $"({creature.Stats.Resources[Resource.HP]}/{creature.Stats.Resources[Resource.MaxHP]})", 18);
                                    else
                                        Program.Window.Print(StartX + 1, Program.Window.Height - 3, creature.CurrentBlock.Name, 18);
                                }
                                else
                                    Program.Window.Print(StartX + 1, Program.Window.Height - 3, creature.Name, 18);
                            }
                            else
                                Program.Window.Print( StartX + 1, Program.Window.Height - 3, blocks[mapPos.X * width + mapPos.Y].Name, 18);
                        }
                        else
                            Program.Window.Print( StartX + 1, Program.Window.Height - 3, tiles[mapPos.X * width + mapPos.Y].Name, 18);
                    }
                }

                void PrintBuildingRecipe()
                {
                    // <clearLines>
                    string spaces = "                    ";
                    Program.Window.Print( StartX + 1, Program.Window.Height - 6, spaces );
                    Program.Window.Print( StartX + 1, Program.Window.Height - 5, spaces );
                    Program.Window.Print( StartX + 1, Program.Window.Height - 4, spaces );
                    // </clearLines>

                    if (currentlySelectedBlueprint == null)
                        return;

                    // <generateText>
                    string text = "Requires: ";
                    Dictionary<string, int> components = new Dictionary<string, int>();
                    for (int i = 0; i < currentlySelectedBlueprint.Recipe.Count; i++)
                    {
                        string componentName = currentlySelectedBlueprint.Recipe[i].ToString();
                        if (!components.ContainsKey( componentName ))
                            components.Add( componentName, 1 );
                        else
                            components[componentName]++;
                    }
                    int c = 0;
                    foreach (KeyValuePair<string, int> componentCount in components)
                    {
                        if (c > 0)
                            text += ", ";
                        text += componentCount.Key + $" x {componentCount.Value}";
                        c++;
                    }
                    // </generateText>

                    Program.Window.Print( StartX + 1, Program.Window.Height - 6, text, StatusPanel.Width - 2 );
                }

                void DeterminePages()
                {
                    if (equipmentLen % padding != 0)
                        itemsPerPage += 1;
                    blueprintsPages = heightOfItems / itemsPerPage;
                    if (heightOfItems % itemsPerPage != 0)
                        blueprintsPages += 1;
                    if (blueprintsCurrentPage > blueprintsPages - 1 & heightOfItems > 2)
                        blueprintsCurrentPage = blueprintsPages - 1;
                }

                void PrintHeader()
                {
                    string header = "BUILD MENU";

                    // fill the background
                    if (!Program.Console.GetBackground(Program.Window.Width - 1, 0).Equals(panelColorDarker))
                        for (int i = Program.Window.Width - 1; i >= StartX; i--)
                            for (int j = 0; j < Program.Console.Height; j++)
                                Program.Console.SetGlyph(i, j, ' ', Color.White, panelColorDarker);

                    // print the header
                    Program.Console.Print(Program.Window.Width - (StatusPanel.Width / 2 + header.Length / 2), 1, header, color);
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

                    Program.Console.Print( StartX + (StatusPanel.Width / 2 - time.Length / 2), blueprintsStartY - 4, time);
                }

                bool PrintBlueprints()
                {
                    Program.Console.Print(Program.Window.Width - (StatusPanel.Width / 2 + "BLUEPRINTS:".Length / 2), blueprintsStartY - 2, "BLUEPRINTS:");

                    bgColor = panelColorDarker;

                    // print the right page turning button
                    Program.Console.Print(Program.Window.Width - 2, blueprintsStartY - 2, " ", color, bgColor);

                    if (mousePos.Equals(new Point(Program.Window.Width - 2, blueprintsStartY - 2)))
                        bgColor = highlightedBgColor;
                    if (blueprintsPages > 1 && blueprintsCurrentPage < blueprintsPages - 1)
                        Program.Console.Print(Program.Window.Width - 2, blueprintsStartY - 2, ">", color, bgColor);

                    bgColor = panelColorDarker;

                    // print the left page turning button

                    Program.Console.Print( StartX + 1, blueprintsStartY - 2, " ", color, bgColor);

                    if (mousePos.Equals(new Point( StartX + 1, blueprintsStartY - 2)))
                        bgColor = highlightedBgColor;
                    if (blueprintsPages > 1 && blueprintsCurrentPage > 0)
                        Program.Console.Print( StartX + 1, blueprintsStartY - 2, "<", color, bgColor);
                    
                    bool highlighting = false;
                    bool highlighted = false;
                    
                    for (int i = 0, index = 0 + blueprintsCurrentPage * itemsPerPage; i < equipmentLen; i += padding, index++)
                    {
                        string name = "";
                        bool indexGreaterThanPlayersItems = index >= blueprints.Count;
                        color = Color.AntiqueWhite;

                        // this is to clear the line
                        Program.Console.Print( StartX + 1, blueprintsStartY + i, "                    ", color, panelColorDarker);
                        Program.Console.Print( StartX + 1, blueprintsStartY + i + 1, "                    ", color, panelColorDarker);

                        if (blueprints.Any() && !indexGreaterThanPlayersItems)
                        {
                            Blueprint blueprint = blueprints[index];
                            name = blueprint.Name;

                            string pt2 = "";
                            Tuple<string, string> nameParts = Window.SplitNameIfGreaterThanLength(name, pt2, StatusPanel.Width - 2);
                            name = nameParts.Item1;
                            if (name[name.Length - 1] == ' ')
                                name = name.Substring(0, name.Length - 1);
                            if (nameParts.Item2 != "")
                                pt2 = nameParts.Item2;

                            // check if the mouse is on the item that is currently being drawn
                            if (mousePos.Y >= 3 && mousePos.X >= StartX + 1 && mousePos.X <= (Program.Console.Width - 2))
                                if (mousePos.Y - blueprintsStartY == i || mousePos.Y - 1 - blueprintsStartY == i)
                                    highlighted = true;

                            if (blueprint.Identified)
                                color = blueprint.ReturnRarityColor();

                            bgColor = index % 2 == 0 ? panelColor * 0.97F : panelColor * 0.98F;

                            if (blueprint == currentlySelectedBlueprint)
                                bgColor = highlightedBgColor;
                            
                            string spaces = "";

                            for (int c = 0; c < ( StatusPanel.Width - 2) - name.Length; c++)
                                spaces += ' ';

                            if (highlighted)
                            {
                                // print the name's first line
                                Program.Console.Print( StartX + 1, blueprintsStartY + i, name + spaces, highlightedColor, bgColor);
                                // print / clear the name's second line
                                Program.Console.Print( StartX + 1, blueprintsStartY + i + 1, "                    ", color, bgColor);

                                if (pt2 != "")
                                    Program.Console.Print( StartX + 1, blueprintsStartY + i + 1, pt2, highlightedColor, bgColor);

                                highlighting = true;
                            }
                            else
                            {
                                // print the name's first line
                                Program.Console.Print( StartX + 1, blueprintsStartY + i, name + spaces, color, bgColor);
                                // print / clear the name's second line
                                Program.Console.Print( StartX + 1, blueprintsStartY + i + 1, "                    ", color, bgColor);

                                if (pt2 != "")
                                    Program.Console.Print( StartX + 1, blueprintsStartY + i + 1, pt2, color, bgColor);
                            }
                        }

                        highlighted = false;
                    }
                    return highlighting;
                }


                // START //
                DeterminePages();
                PrintHeader();
                PrintTime();
                PrintBuildingRecipe();
                PrintLookFunc();
                return PrintBlueprints();
            }


            // PROPERTIES //

            static public int StartX
            {
                get { return Program.Window.Width - StatusPanel.Width; }
            }

            static public Blueprint CurrentlySelectedBlueprint
            {
                get { return currentlySelectedBlueprint; }
                set { currentlySelectedBlueprint = value; }
            }
        }

        static public class CharacterSheet
        {
            public static int Width { get { return ( MapWidth / 3 ) * 2; } }
            public static int Height { get { return ( Program.Window.Height / 4 ) * 3; } }
            public static int StartX { get { return (Program.Console.Width - 22) / 2 - Width / 2; } }
            public static int StartY { get { return Program.Console.Height / 2 - Height / 2; } }
            private static List<Attribute> selectedAttributes = new List<Attribute>();

            private static Color textColor = new Color( Color.AntiqueWhite, 0.98F );

            internal static void HandleCharacterSheet()
            {

            }

            internal static void RenderCharacterSheet(Creature character)
            {
                PrintSheet(character.Name, character.Stats.Level.Lvl);
                PrintAttributes(character.Stats.Attributes);
                PrintSkills( character.Class, character.Stats.Skills );
            }

            private static void PrintSheet(string charName, int charLevel)
            {
                string title = charName + " / LVL. " + charLevel;

                // fill the background
                if (!GUI.Console.GetBackground( StartX, StartY ).Equals( InventoryPanel.darkerColor ))
                    for (int i = StartX; i <= StartX + Width; i++)
                        for (int j = StartY; j <= StartY + Height; j++)
                            GUI.Console.SetGlyph( i, j, ' ', textColor, new Color( Color.Black, 0.99F ) );
                // print the header
                GUI.Console.Print( StartX + Width / 2 - title.Length / 2, StartY + 2, title, textColor );

                // print the border
                byte topLeft = 218, topRight = 191, bottomLeft = 192, bottomRight = 217, horizontal = 196, vertical = 179;

                if (GUI.Console.GetGlyph( StartX, StartY + 1 ) != vertical)
                    // print left and right side of tab window border
                    for (int j = StartY + 1; j <= StartY + Height - 1; j++)
                    {
                        GUI.Console.SetGlyph( StartX, j, vertical, textColor );
                        GUI.Console.SetGlyph( StartX + Width, j, vertical, textColor );
                    }
                if (GUI.Console.GetGlyph( StartX + 1, StartY ) != horizontal)
                    // print top and bottom side of tab window border
                    for (int i = StartX + 1; i <= StartX + Width - 1; i++)
                    {
                        GUI.Console.SetGlyph( i, StartY, horizontal, textColor );
                        GUI.Console.SetGlyph( i, StartY + Height, horizontal, textColor );
                    }
                // print the corners
                GUI.Console.SetGlyph( StartX, StartY, topLeft, textColor );
                GUI.Console.SetGlyph( StartX + Width, StartY, topRight, textColor );
                GUI.Console.SetGlyph( StartX, StartY + Height, bottomLeft, textColor );
                GUI.Console.SetGlyph( StartX + Width, StartY + Height, bottomRight, textColor );
            }

            private static void PrintAttributes(Dictionary<Attribute, int> attributes)
            {
                string title = "ATTRIBUTES:";
                string[] lines = new string[] { $"END: {attributes[Attribute.Endurance]} | STR: {attributes[Attribute.Strength]} | AGL: {attributes[Attribute.Agility]} | DEX: {attributes[Attribute.Dexterity]}",
                                                $"INT: {attributes[Attribute.Intelligence]} | WPR: {attributes[Attribute.Willpower]} | PER: {attributes[Attribute.Personality]} | LCK: {attributes[Attribute.Luck]}"};
                GUI.Console.Print( StartX + Width / 2 - title.Length / 2, StartY + 5, title, textColor );
                GUI.Console.Print( StartX + Width / 2 - lines[0].Length / 2, StartY + 7, lines[0], textColor );
                GUI.Console.Print( StartX + Width / 2 - lines[1].Length / 2, StartY + 9, lines[1], textColor );
            }

            private static void PrintSkills( Class @class, Dictionary<Skill, int> skills )
            {
                int skillsStartY = StartY + 13, 
                    startX = StartX + (Width / 2) - (39 / 2), 
                    xIncrement = 39 / 3;
                string title = "SKILLS:";
                GUI.Console.Print( StartX + Width / 2 - title.Length / 2, skillsStartY - 2, title, textColor );
                
                // Sort the skills before printing
                List<string> sortedSkillList = Enum.GetNames( typeof( Skill ) ).ToList();
                sortedSkillList.Sort();
                int i, index;
                int third = sortedSkillList.Count / 3;
                Color numbersColor = new Color( Color.Gray, 0.98F );
                // print major skills
                for (i = 0, index= 0; i < sortedSkillList.Count; i++) {
                    Color skillColor = new Color( Color.AntiqueWhite, 0.98F );
                    string skillName = sortedSkillList[i];
                    Enum.TryParse( skillName, out Skill currentSkill );

                    if (!( @class.MajorSkills.Contains( currentSkill ) || @class.MinorSkills.Contains( currentSkill ) ))
                        continue;
                    if (@class.MajorSkills.Contains( currentSkill ))
                        skillColor = new Color( Color.RoyalBlue, 0.99F );
                    else if (@class.MinorSkills.Contains( currentSkill ))
                        skillColor = new Color( Color.LimeGreen, 0.99F );

                    int yVal = ( index < third ) ? index : ( index % third );
                    int curColumn = ( index < third ) ? 0 : ( ( index < (third * 2) ) ? 1 : 2 );
                    GUI.Console.Print( startX + curColumn * xIncrement, skillsStartY + yVal, skillName, skillColor );
                    GUI.Console.Print( startX + curColumn * xIncrement + xIncrement - 2, skillsStartY + yVal, $"{skills[currentSkill]}", numbersColor );
                    index++;
                }
                for (i = 0; i < sortedSkillList.Count; i++) {
                    string skillName = sortedSkillList[i];
                    Enum.TryParse( skillName, out Skill currentSkill );
                    if (@class.MajorSkills.Contains( currentSkill ) || @class.MinorSkills.Contains( currentSkill ))
                        continue;
                    int yVal = ( index < third ) ? index : ( index % third );
                    int curColumn = ( index < third ) ? 0 : ( ( index < ( third * 2 ) ) ? 1 : 2 );
                    GUI.Console.Print( startX + curColumn * xIncrement, skillsStartY + yVal, skillName, textColor );
                    GUI.Console.Print( startX + curColumn * xIncrement + xIncrement - 2, skillsStartY + yVal, $"{skills[currentSkill]}", numbersColor );
                    index++;
                }
            }

            public static void HandleLevelUp ( Attribute selectedAttribute )
            {
                Point mousePos = Program.Window.MousePos;
                if (SadConsole.Global.MouseState.LeftClicked && selectedAttribute != Attribute.Luck) {
                    if (selectedAttributes.Contains( selectedAttribute ) == false) {
                        if (selectedAttributes.Count < 3)
                            selectedAttributes.Add( selectedAttribute );
                    }
                    else
                        selectedAttributes.Remove( selectedAttribute );
                }
                else if (SadConsole.Global.MouseState.LeftClicked && mousePos.Y == StartY + Height - 2 && mousePos.X >= StartX + Width / 2 - 11 / 2 && mousePos.X < StartX + Width / 2 + 11 / 2) {
                    LevelUp currentState = (LevelUp)Program.CurrentState;
                    currentState.FinalizeLevelUp( selectedAttributes );
                }
            }

            public static Attribute RenderLevelUp ( Creature character, Dictionary<Attribute, int> lvlProgress )
            {
                PrintSheet( character.Name, character.Stats.Level.Lvl );
                PrintSelectButton();
                return PrintLevelAttributes( character, lvlProgress );
            }

            private static void PrintSelectButton()
            {
                string buttonText = "<Level Up!>";
                Point mousePos = Program.Window.MousePos;
                bool canLevel = selectedAttributes.Count == 3;
                bool mousedOn = mousePos.Y == StartY + Height - 2 && mousePos.X >= StartX + Width / 2 - 11 / 2 && mousePos.X < StartX + Width / 2 + 11 / 2;
                Color bgColor = mousedOn && canLevel ? new Color( Color.Gray, 0.98F) : new Color( Color.Black, 0.98F );
                GUI.Console.Print( StartX + Width / 2 - buttonText.Length / 2, StartY + Height - 2, buttonText, textColor, bgColor );
            }

            private static Attribute PrintLevelAttributes( Creature character, Dictionary<Attribute, int> lvlProgress )
            {
                int i = 0;
                Point mousePos = Program.Window.MousePos;
                Attribute mouseOnAttribute = Attribute.Luck;
                foreach (KeyValuePair<Attribute, int> attVal in character.Stats.Attributes) {
                    if (attVal.Key == Attribute.Luck)
                        continue;
                    string text = attVal.Key.ToString();
                    bool mousedOn = mousePos.Y == StartY + 4 + i && mousePos.X >= StartX + Width / 2 - 10 && mousePos.X < StartX + Width / 2 - 10 + text.Length;
                    if (mousedOn) mouseOnAttribute = attVal.Key;
                    Color attributeColor = selectedAttributes.Contains(attVal.Key) ? new Color(Color.ForestGreen, 0.98F) : textColor;
                    Color numberColor = selectedAttributes.Contains( attVal.Key ) ? textColor : new Color( Color.Gray, 0.98F );
                    Color bgColor = mousedOn ? new Color( Color.Gray, 0.99F ) : new Color( Color.Black, 0.99F );
                    int valToAdd = selectedAttributes.Contains( attVal.Key ) ? (lvlProgress.ContainsKey(attVal.Key) ? Math.Min( 5, lvlProgress[attVal.Key] + 1 ) : 1) : 0;
                    GUI.Console.Print( StartX + Width / 2 - 10, StartY + 4 + i, text, attributeColor, bgColor );
                    GUI.Console.Print( StartX + Width / 2 + 8, StartY + 4 + i, $"{attVal.Value + valToAdd}", numberColor );
                    i++;
                }

                return mouseOnAttribute;
            }
        }

        // FUNCTIONS //

        static public List<Point> CalculatePath(Point mapStart, int windowWidth)
        {
            Point mousePos = new Point(SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                   SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y);
            Point mapPos = new Point(mapStart.X + ( mousePos.X ), mapStart.Y + mousePos.Y);

            if ((mousePos.X < InventoryPanel.Width || mousePos.X > windowWidth - StatusPanel.Width - 1) || !SadConsole.Global.MouseState.IsOnScreen) {
                path = null;
                return path;
            }
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap[worldIndex.X, worldIndex.Y].Width;
            if (blocks[mapPos.X * width + mapPos.Y].Explored) {
                if (prevMapPos != mapPos) {
                    path = Pathfinder.FindPath( worldIndex, currentFloor, Program.Player.Position, mapPos );

                    prevMapPos = mapPos;
                    if (path.Count >= windowWidth)
                        path = null;
                }
            }
            else
                path = null;
            return path;
        }

        static public void DrawFPS()
        {
            if (DateTime.Now - lastFrameTime >= new TimeSpan(0, 0, 1))
            {
                lastFrameTime = DateTime.Now;
                fps = fSinceLastSec;
                fSinceLastSec = 0;
            }
            else
                fSinceLastSec++;
            GUI.Console.Print( InventoryPanel.Width, 0, $"{fps}", new Color(Color.AntiqueWhite, 0.95f));
        }

        static public Tuple<byte, Color> GetItemArrow(Item item)
        {
            Tuple<byte, Color> comparisonArrow = Tuple.Create((byte)0, Color.Black);
            List<Item> equipmentList = Program.Player.Body.GetEquipmentList();

            foreach (Item equip in equipmentList)
            {
                comparisonArrow = item.GetComparisonArrow(equip);
                if (comparisonArrow.Item1 != 0)
                    break;
            }

            return comparisonArrow;
        }


        // PROPERTIES //

        static public SadConsole.Console Console
        {
            get { return console; }
            set { console = value; }
        }

        static public List<Point> Path
        {
            get { return path; }
            set { path = value; }
        }

        static public int MapWidth
        {
            get { return Program.Console.Width - (StatusPanel.Width); }
        }
    }
}
