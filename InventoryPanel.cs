using System;
using System.Collections.Generic;
using SadConsole;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Landlord
{
    static class InventoryPanel
    {
        static private int width = 22;
        private static bool displaying = false;
        static private bool displayingEquipment = false;

        static private int invCurrentPage = 0, inventoryPages = 1;
        static private int itemsPerPage;
        static private int padding = 3;

        static private int equipCurrentPage = 0, equipmentPages = 1;
        static private int equipmentHeight { get { return displayingEquipment ? Program.Console.Height / 2 - 3 : 0; } }
        static private int equipmentStartY { get { return Program.Console.Height - 3 - equipmentHeight; } }
        static private int equipmentLen { get { return ( Program.Console.Height - 4 ) - equipmentStartY; } }
        static private int equipmentPerPage { get { return equipmentLen / padding + ( equipmentLen % padding != 0 ? 1 : 0 ); } }

        static private Item currentlyViewedItem = null;
        static public Color color = new Color( 35, 55, 82 ), lighterColor = new Color( 52, 69, 92 ), darkerColor = new Color( 21, 41, 70 );

        static private List<Item> inventoryItems = new List<Item>();
        static private List<Item> equipment = new List<Item>();
        static private Dictionary<string, int> inventoryCount = new Dictionary<string, int>();


        // FUNCTIONS //

        private static void ClearViewArea()
        {
            for (int i = width; i < width + 20; ++i) {
                for (int j = 0; j < Program.Console.Height; j++) {
                    GUI.Console.SetGlyph(i, j, ' ', new Color(color, 0.97F), new Color(color, 0.97F));
                }

            }
        }

        public static bool HandleInventory()
        {
            Point mousePos = new Point( Global.MouseState.ScreenPosition.X / Global.FontDefault.Size.X,
                Global.MouseState.ScreenPosition.Y / Global.FontDefault.Size.Y );

            bool leftDown = Global.MouseState.LeftButtonDown, rightDown = Global.MouseState.RightButtonDown;
            bool leftClicked = Global.MouseState.LeftClicked, rightClicked = Global.MouseState.RightClicked;
            bool shiftPressed = Global.KeyboardState.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.LeftShift );

            if (( leftDown || rightDown ) && ( mousePos.X > width && mousePos.X < StatusPanel.StartX ) && Program.CurrentState is Play)
                Displaying = false;
            if (displaying == false)
                return false;

            bool DetermineCurrentItem()
            {
                if (mousePos.Y < equipmentStartY - 2)
                {
                    int index = -1;
                    if (mousePos.Y >= 4)
                    {
                        index = ( mousePos.Y - 4 ) / 3 + invCurrentPage * itemsPerPage;
                        if (inventoryItems.Count <= index || index < 0)
                            return false;
                        currentlyViewedItem = inventoryItems[index];
                    }
                    return true;
                } else
                {
                    if (rightClicked)
                        return false;
                    int index = -1;
                    if (mousePos.Y >= equipmentStartY)
                    {
                        index = ( mousePos.Y - equipmentStartY ) / 3 + equipCurrentPage * equipmentPerPage;
                        if (index > equipment.Count || index < 0)
                            return false;
                        currentlyViewedItem = equipment[index];
                    } else
                        return false;
                    return true;
                }
            }

            void HandleEquipmentPageSwitching()
            {
                // page switching logic
                if (mousePos.Equals( new Point( ( width - 2 ), equipmentStartY - 2 ) ) && equipCurrentPage < equipmentPages - 1 && Global.MouseState.LeftClicked)
                    equipCurrentPage += 1;
                else if (mousePos.Equals( new Point( 1, equipmentStartY - 2 ) ) && equipCurrentPage > 0 && Global.MouseState.LeftClicked)
                    equipCurrentPage -= 1;
            }

            void HandleInventoryPageSwitching()
            {
                // page switching logic
                if (mousePos.Equals( new Point( ( width - 2 ), 1 ) ) && invCurrentPage < inventoryPages - 1 && Global.MouseState.LeftClicked)
                    invCurrentPage += 1;
                else if (mousePos.Equals( new Point( 1, 1 ) ) && invCurrentPage > 0 && Global.MouseState.LeftClicked)
                    invCurrentPage -= 1;
            }

            void HandleInventoryInput()
            {
                if (shiftPressed && leftClicked) {
                    if ((Program.CurrentState is Play
                           || Program.CurrentState is ViewItem
                              || Program.CurrentState is ViewEquipment)) {
                        if (currentlyViewedItem is Potion)
                            Program.Player.Drink(currentlyViewedItem);
                        else if (currentlyViewedItem is Food)
                            Program.Player.Eat(currentlyViewedItem);
                        else if (currentlyViewedItem is Armor armorPiece)
                            Program.Player.Equip(armorPiece);
                        else if (currentlyViewedItem is BlueprintPouch || currentlyViewedItem is RecipePouch || currentlyViewedItem is Quiver) {
                            int maxItems = (currentlyViewedItem is Quiver) ? 20 : 40;
                            Program.Animations.Add(new OpenLootView());
                            Program.CurrentState = new ViewLoot(currentlyViewedItem is RecipePouch ? ((RecipePouch)currentlyViewedItem).Recipes :
                                                                    ((currentlyViewedItem is Quiver) ? ((Quiver)currentlyViewedItem).Arrows :
                                                                      ((BlueprintPouch)currentlyViewedItem).Blueprints), maxItems, currentlyViewedItem.Name);
                        }
                        else Program.Player.Wield(Program.Player.Inventory.FindIndex(i => i.Name == currentlyViewedItem.Name), true);
                    }
                    else if (Program.CurrentState is ViewLoot viewLoot) {
                        // if viewing loot in a container, clicking an item will add it to the container.
                        if (Program.Animations.Count == 0) {
                            for (int i = Program.Player.Inventory.Count - 1; i >= 0; --i) {
                                if (Program.Player.Inventory[i].Name.Equals(currentlyViewedItem.Name)) {
                                    viewLoot.LootMenu.ContainerInventory.Add(Program.Player.Inventory[i]);
                                    Program.Player.Inventory.RemoveAt(i);
                                }
                            }
                        }
                    }
                    currentlyViewedItem = null;
                }
                else if (leftClicked) {
                    if (Program.CurrentState is Play 
                        || Program.CurrentState is ViewItem 
                          || Program.CurrentState is ViewEquipment)
                    {
                        if (Program.CurrentState is ViewItem == false)
                            Program.Animations.Add(new OpenItemView(color));
                        else
                            ClearViewArea();
                        Program.CurrentState = new ViewItem();
                    }
                    else if (Program.CurrentState is ViewLoot viewLoot) {
                        // if viewing loot in a container, clicking an item will add it to the container.
                        if (Program.Animations.Count == 0 && viewLoot.LootMenu.ContainerMaxItems > viewLoot.LootMenu.ContainerInventory.Count) {
                            viewLoot.LootMenu.ContainerInventory.Add(currentlyViewedItem);
                            Program.Player.Inventory.Remove(currentlyViewedItem);
                        }
                        currentlyViewedItem = null;
                    }
                }
                else if (rightClicked && Program.CurrentState is Play) {
                    Program.Player.Drop( currentlyViewedItem );
                    currentlyViewedItem = null;
                }
            }

            void HandleEquipmentInput()
            {
                if (leftClicked && shiftPressed && Program.CurrentState is Play) {
                    Program.Player.Unequip( currentlyViewedItem );
                    currentlyViewedItem = null;
                }
                else if (leftClicked && ( Program.CurrentState is Play || Program.CurrentState is ViewEquipment || Program.CurrentState is ViewItem)) {
                    if (Program.CurrentState is ViewEquipment == false)
                        Program.Animations.Add( new OpenItemView( color ) );
                    else ClearViewArea();
                    Program.CurrentState = new ViewEquipment();
                }
            }

            HandleEquipmentPageSwitching();
            HandleInventoryPageSwitching();

            // item selection logic
            bool highlighting = RenderInventory();

            if (leftClicked || rightClicked)
            {
                if (mousePos.Y == equipmentStartY - 2 && mousePos.X >= width / 2 - "  Equipment  ".Length / 2 && mousePos.X < width / 2 + "  Equipment  ".Length / 2 + 1)
                {
                    displayingEquipment = !displayingEquipment;
                    for (int j = Program.Console.Height / 2 - 2; j < Program.Console.Height - 3; j++)
                        GUI.Console.Print( 0, j, "                      ", new Color( darkerColor, 0.99F ), new Color( darkerColor, 0.99F ) );
                }

                if (highlighting == false)
                    return false;
                bool determined = DetermineCurrentItem();
                if (!determined)
                    return false;
            }

            bool mouseInInventory = mousePos.Y < equipmentStartY - 2;

            if (mouseInInventory)
                HandleInventoryInput();
            else
                HandleEquipmentInput();

            return true;
        } // input handling
        public static bool RenderInventory() // display handling
        {
            int heightOfItems = inventoryItems.Count;
            int itemPortHeight = Program.Console.Height - 8 - ( displayingEquipment ? equipmentStartY : 3 );
            itemsPerPage = itemPortHeight / padding;

            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            float bgAlpha = 0.99F, fgAlpha = 0.99F;

            Color textColor = new Color( Color.White, fgAlpha );
            Color highlightedColor = new Color( Color.Green, bgAlpha );
            Color bgColor = new Color( darkerColor, bgAlpha );

            // FUNCTIONS

            void AddEquipment()
            {
                if (Program.Player.Body.MainHand != null)
                    equipment.Add( Program.Player.Body.MainHand );
                if (Program.Player.Body.OffHand != null)
                    equipment.Add( Program.Player.Body.OffHand );
                if (Program.Player.Body.Helmet != null)
                    equipment.Add( Program.Player.Body.Helmet );
                if (Program.Player.Body.ChestPiece != null)
                    equipment.Add( Program.Player.Body.ChestPiece );
                if (Program.Player.Body.Shirt != null)
                    equipment.Add( Program.Player.Body.Shirt );
                if (Program.Player.Body.Gauntlets != null)
                    equipment.Add( Program.Player.Body.Gauntlets );
                if (Program.Player.Body.Leggings != null)
                    equipment.Add( Program.Player.Body.Leggings );
                if (Program.Player.Body.Boots != null)
                    equipment.Add( Program.Player.Body.Boots );
            }

            string AddEquipSlotToName( string name, int index )
            {
                if (Program.Player.Body.MainHand != null
                    && name.CompareTo( Program.Player.Body.MainHand.Name ) == 0)
                    name = $"main hand: {name}";
                else if (Program.Player.Body.OffHand != null
                         && name.CompareTo( Program.Player.Body.OffHand.Name ) == 0)
                    name = $"off hand: {name}";
                else if (Program.Player.Body.Helmet != null
                         && name.CompareTo( Program.Player.Body.Helmet.Name ) == 0)
                    name = $"head: {name}";
                else if (Program.Player.Body.ChestPiece != null
                         && name.CompareTo( Program.Player.Body.ChestPiece.Name ) == 0)
                    name = $"chest: {name}";
                else if (Program.Player.Body.Shirt != null
                         && name.CompareTo( Program.Player.Body.Shirt.Name ) == 0)
                    name = $"shirt: {name}";
                else if (Program.Player.Body.Gauntlets != null
                         && name.CompareTo( Program.Player.Body.Gauntlets.Name ) == 0)
                    name = $"hands: {name}";
                else if (Program.Player.Body.Leggings != null
                         && name.CompareTo( Program.Player.Body.Leggings.Name ) == 0)
                    name = $"legs: {name}";
                else if (Program.Player.Body.Boots != null
                         && name.CompareTo( Program.Player.Body.Boots.Name ) == 0)
                    name = $"feet: {name}";

                return name;
            }

            void ConvertInventoryForDisplay()
            {
                // items are stored in the inventory on an individual basis.
                // they are displayed in chunks that have a counter
                inventoryCount = new Dictionary<string, int>();
                inventoryItems = new List<Item>();
                bool InventoryItemsContains( Item item )
                {
                    foreach (Item i in inventoryItems)
                        if (item.Name == i.Name)
                            return true;
                    return false;
                }
                Program.Player.Inventory.Sort();
                foreach (Item item in Program.Player.Inventory)
                {
                    if (!InventoryItemsContains( item ))
                    {
                        inventoryCount[item.Name] = 1;
                        inventoryItems.Add( item );
                    } else
                        inventoryCount[item.Name] += 1;
                }
            }

            void DetermineInventoryPages()
            {
                if (itemPortHeight % padding != 0)
                    itemsPerPage += 1;
                inventoryPages = heightOfItems / itemsPerPage;
                if (heightOfItems % itemsPerPage != 0)
                    inventoryPages += 1;
                if (invCurrentPage > inventoryPages - 1 & heightOfItems > 2)
                    invCurrentPage = inventoryPages - 1;
            }

            void DetermineEquipmentPages()
            {
                equipmentPages = 1;
                equipmentPages = equipment.Count / equipmentPerPage;
                if (equipment.Count % equipmentPerPage != 0)
                    equipmentPages += 1;
                if (equipCurrentPage > equipmentPages - 1 & equipment.Count > 2)
                    equipCurrentPage = equipmentPages - 1;
            }

            void PrintHeader()
            {

                // fill the background
                for (int i = 0; i < width; i++)
                    for (int j = 0; j < Program.Console.Height; j++)
                        GUI.Console.SetGlyph( i, j, ' ', bgColor, bgColor );
                // print the header
                GUI.Console.Print( width / 2 - "INVENTORY".Length / 2, 1, "INVENTORY", textColor );

                // print the right page turning button
                GUI.Console.Print( width - 2, 1, " ", textColor, bgColor );
                if (mousePos.Equals( new Point( ( width - 2 ), 1 ) ))
                    bgColor = highlightedColor * 0.95F;
                if (inventoryPages > 1 && invCurrentPage < inventoryPages - 1)
                    GUI.Console.Print( width - 2, 1, ">", textColor, bgColor );
                bgColor = new Color( darkerColor, bgAlpha );

                // print the left page turning button
                GUI.Console.Print( 1, 1, " ", textColor, bgColor );
                if (mousePos.Equals( new Point( 1, 1 ) ))
                    bgColor = highlightedColor * 0.95F;
                if (inventoryPages > 1 && invCurrentPage > 0)
                    GUI.Console.Print( 1, 1, "<", textColor, bgColor );
                bgColor = new Color( color, bgAlpha );
            }

            bool PrintInventory()
            {
                ConvertInventoryForDisplay();
                DetermineInventoryPages();

                bool highlighting = false;
                bool highlighted = false;

                for (int i = 0, index = 0 + invCurrentPage * itemsPerPage; i < itemPortHeight; i += padding, index++)
                {
                    string name = "";
                    bool indexGreaterThanInventory = index >= inventoryItems.Count;
                    textColor = new Color( Color.AntiqueWhite, fgAlpha );

                    // this is to clear the line
                    GUI.Console.Print( 1, 4 + i, "                     ", new Color( darkerColor, bgAlpha ), new Color( darkerColor, bgAlpha ) );
                    GUI.Console.Print( 1, 4 + i + 1, "                     ", new Color( darkerColor, bgAlpha ), new Color( darkerColor, bgAlpha ) );

                    if (inventoryCount.Any() && !indexGreaterThanInventory)
                    {
                        Item item = inventoryItems[index];
                        name = item.Name;
                        string invCounter = "x" + inventoryCount[item.Name];

                        string pt2 = "";
                        Tuple<string, string> nameParts = Window.SplitNameIfGreaterThanLength( name, pt2, width - 3 - invCounter.Length );
                        name = nameParts.Item1;
                        if (nameParts.Item2 != "")
                            pt2 = nameParts.Item2;

                        // check if the mouse is on the item that is currently being drawn
                        if (mousePos.Y >= 3 && mousePos.X >= 1 && mousePos.X <= ( width - 2 ))
                            if (mousePos.Y - 4 == i || mousePos.Y - 1 - 4 == i)
                                highlighted = true;

                        if (item.Identified)
                            textColor = item.ReturnRarityColor();
                        if (index % 2 == 0)
                            bgColor = new Color( color, bgAlpha );
                        else
                            bgColor = new Color( lighterColor, bgAlpha );

                        if (item == currentlyViewedItem)
                            bgColor = highlightedColor * bgAlpha;
                        string spaces = "";
                        for (int c = 0; c <= ( width - 2 ) - ( 1 + name.Length ) - invCounter.Length; c++)
                            spaces += ' ';
                        if (highlighted)
                        {
                            // print the name's first line
                            GUI.Console.Print( 1, 4 + i, name + spaces, highlightedColor, bgColor );
                            // print the count
                            GUI.Console.Print( width - 1 - invCounter.Length, 4 + i, invCounter, textColor, bgColor );
                            // print / clear the name's second line
                            GUI.Console.Print( 1, 4 + i + 1, "                    ", textColor, bgColor );
                            if (pt2 != "")
                                GUI.Console.Print( 1, 4 + i + 1, pt2, highlightedColor, bgColor );
                            highlighting = true;
                        } else
                        {
                            // print the name's first line
                            GUI.Console.Print( 1, 4 + i, name + spaces, textColor, bgColor );
                            // print the count
                            GUI.Console.Print( width - 1 - invCounter.Length, 4 + i, invCounter, textColor, bgColor );
                            // print / clear the name's second line
                            GUI.Console.Print( 1, 4 + i + 1, "                    ", textColor, bgColor );
                            if (pt2 != "")
                                GUI.Console.Print( 1, 4 + i + 1, pt2, textColor, bgColor );
                        }
                    }
                    highlighted = false;
                }
                return highlighting;
            }

            bool PrintEquipment()
            {
                string equipmentTitle = " EQUIPMENT ";
                if (displayingEquipment)
                    equipmentTitle = (char)31 + equipmentTitle + (char)31;
                else
                    equipmentTitle = (char)30 + equipmentTitle + (char)30;
                GUI.Console.Print( width / 2 - "  EQUIPMENT  ".Length / 2, equipmentStartY - 2, equipmentTitle, textColor );

                if (!displayingEquipment)
                    return false;

                DetermineEquipmentPages();

                bgColor = new Color( darkerColor, bgAlpha );

                // print the right page turning button
                GUI.Console.Print( width - 2, equipmentStartY - 2, " ", textColor, bgColor );

                if (mousePos.Equals( new Point( width - 2, equipmentStartY - 2 ) ))
                    bgColor = highlightedColor * 0.95F;
                if (equipmentPages > 1 && equipCurrentPage < equipmentPages - 1)
                    GUI.Console.Print( width - 2, equipmentStartY - 2, ">", textColor, bgColor );

                bgColor = new Color( darkerColor, bgAlpha );

                // print the left page turning button
                GUI.Console.Print( 1, equipmentStartY - 2, " ", textColor, bgColor );

                if (mousePos.Equals( new Point( 1, equipmentStartY - 2 ) ))
                    bgColor = highlightedColor * 0.95F;
                if (equipmentPages > 1 && equipCurrentPage > 0)
                    GUI.Console.Print( 1, equipmentStartY - 2, "<", textColor, bgColor );

                bgColor = new Color( color, bgAlpha );

                bool highlighting = false;
                bool highlighted = false;

                for (int i = 0, index = 0 + equipCurrentPage * equipmentPerPage; i < equipmentLen; i += padding, index++)
                {
                    string name = "";

                    bool indexGreaterThanPlayersItems = index >= equipment.Count;

                    textColor = new Color( Color.AntiqueWhite, bgAlpha );

                    // this is to clear the line
                    GUI.Console.Print( 1, equipmentStartY + i, "                    ", new Color( darkerColor, bgAlpha ), new Color( darkerColor, bgAlpha ) );
                    GUI.Console.Print( 1, equipmentStartY + i + 1, "                    ", new Color( darkerColor, bgAlpha ), new Color( darkerColor, bgAlpha ) );

                    if (equipment.Any() && !indexGreaterThanPlayersItems)
                    {
                        Item item = equipment[index];
                        name = AddEquipSlotToName( item.Name, index );

                        string pt2 = "";
                        Tuple<string, string> nameParts = Window.SplitNameIfGreaterThanLength( name, pt2, StatusPanel.Width - 2 );
                        name = nameParts.Item1;
                        if (name[name.Length - 1] == ' ')
                            name = name.Substring( 0, name.Length - 1 );
                        if (nameParts.Item2 != "")
                            pt2 = nameParts.Item2;

                        // check if the mouse is on the item that is currently being drawn
                        if (mousePos.X >= 1 && mousePos.X <= ( width - 1 ))
                            if (mousePos.Y - equipmentStartY == i || mousePos.Y - 1 - equipmentStartY == i)
                                highlighted = true;

                        if (item.Identified)
                            textColor = new Color( item.ReturnRarityColor(), bgAlpha );

                        bgColor = index % 2 == 0 ? new Color( color, bgAlpha ) : new Color( lighterColor, bgAlpha );

                        if (item == currentlyViewedItem)
                            bgColor = new Color( highlightedColor * 0.95F, bgAlpha );

                        string spaces = "";

                        for (int c = 0; c < ( StatusPanel.Width - 2 ) - name.Length; c++)
                            spaces += ' ';

                        if (highlighted)
                        {
                            // print the name's first line
                            GUI.Console.Print( 1, equipmentStartY + i, name + spaces, highlightedColor, bgColor );
                            // print / clear the name's second line
                            GUI.Console.Print( 1, equipmentStartY + i + 1, "                    ", bgColor, bgColor );

                            if (pt2 != "")
                                GUI.Console.Print( 1, equipmentStartY + i + 1, pt2, highlightedColor, bgColor );

                            highlighting = true;
                        } else
                        {
                            // print the name's first line
                            GUI.Console.Print( 1, equipmentStartY + i, name + spaces, textColor, bgColor );
                            // print / clear the name's second line
                            GUI.Console.Print( 1, equipmentStartY + i + 1, "                    ", textColor, bgColor );

                            if (pt2 != "")
                                GUI.Console.Print( 1, equipmentStartY + i + 1, pt2, textColor, bgColor );
                        }
                    }

                    highlighted = false;
                }
                return highlighting;
            }

            // START

            equipment = new List<Item>();

            AddEquipment();
            PrintHeader();

            // print the gold
            int goldX = width - 1 - $"gold: {Program.Player.Gold}".Length;
            GUI.Console.Print( goldX, Program.Console.Height - 2, $"gold: {Program.Player.Gold}", textColor );

            return PrintInventory() | PrintEquipment();
        }

        public static void HandleItemView( bool highlighting )
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                 SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            if (Program.Animations.Count == 0)
                RenderItemView();

            int actionsStartY = Program.Console.Height - 7;

            bool selectingEat = mousePos.Y == actionsStartY && mousePos.X >= width + 1 && mousePos.X <= width + 5;
            bool selectingDrink = mousePos.Y == actionsStartY && mousePos.X >= width + 12 && mousePos.X <= width + 18;
            bool selectingDrop = mousePos.Y == actionsStartY + 2 && mousePos.X >= width + 1 && mousePos.X <= width + 6;
            bool selectingEquip = mousePos.Y == actionsStartY + 2 && mousePos.X >= width + 12 && mousePos.X <= width + 18;
            bool selectingOpen = mousePos.Y == actionsStartY + 4 && mousePos.X >= width + 1 && mousePos.X <= width + 6;

            bool mouseOutsideOfItemWindow = ( mousePos.X > width || mousePos.X < width );

            if (SadConsole.Global.MouseState.LeftClicked && ( mouseOutsideOfItemWindow || selectingDrop || selectingEquip || selectingDrink || selectingEat ) && !highlighting)
            {
                if (selectingEat)
                    Program.Player.Eat(currentlyViewedItem);
                else if (selectingDrink)
                    Program.Player.Drink(currentlyViewedItem);
                else if (selectingDrop)
                    Program.Player.Drop(currentlyViewedItem);
                else if (selectingEquip) {
                    if (currentlyViewedItem is Armor == false) {
                        Menus.SelectWieldingHand(currentlyViewedItem);
                        return;
                    }
                    else
                        Program.Player.Equip((Armor)currentlyViewedItem);
                }
                else if (selectingOpen) {
                    if (!(currentlyViewedItem is BlueprintPouch 
                        || currentlyViewedItem is RecipePouch
                          || currentlyViewedItem is Quiver)) {
                        Program.MsgConsole.WriteLine($"You can't open the {currentlyViewedItem.Name}");
                        return;
                    }
                    int maxItems = (currentlyViewedItem is Quiver) ? 20 : 40;
                    Program.Animations.Add(new OpenLootView());
                    Program.CurrentState = new ViewLoot(currentlyViewedItem is RecipePouch ? ((RecipePouch)currentlyViewedItem).Recipes :
                                                            ((currentlyViewedItem is Quiver) ? ((Quiver)currentlyViewedItem).Arrows :
                                                              ((BlueprintPouch)currentlyViewedItem).Blueprints), maxItems, currentlyViewedItem.Name);
                }
                currentlyViewedItem = null;
                if (Program.CurrentState is DialogWindow == false)
                    Program.Animations.Add( new CloseItemView() );
            }
        }
        public static void RenderItemView()
        {
            if (currentlyViewedItem == null)
                return;
            // initialize variables
            Item item = currentlyViewedItem;
            string name = item.Name;
            float fgAlpha = 0.99F;
            Color color = new Color( Color.AntiqueWhite, fgAlpha );

            string pt2 = "";

            // split the name into two lines if necessary
            Tuple<string, string> stringParts = Window.SplitNameIfGreaterThanLength(name, pt2, 18);
            name = stringParts.Item1;
            pt2 = stringParts.Item2;


            // print the name at top
            if (item.Identified)
                color = item.ReturnRarityColor();

            GUI.Console.Print(width + 1, 1, name, color);
            GUI.Console.Print(width + 1, 2, pt2, color);

            Tuple<byte, Color> comparisonArrow = GUI.GetItemArrow(item);
            if (comparisonArrow.Item1 != 0)
                GUI.Console.SetGlyph(width + 1 + (pt2 == "" ? name.Length : pt2.Length), pt2 == "" ? 1 : 2, comparisonArrow.Item1, comparisonArrow.Item2);


            //// print the splash art
            //bool thisItemHasSplashArt = item.splash != null;
            //if (thisItemHasSplashArt)
            //    for (int i = 0; i < 18; i++)
            //        for (int j = 0; j < 10; j++)
            //            Program.mapConsole.Set(i, j + 3, item.splash[i, j].color, Swatch.SecondaryDarker, item.splash[i, j].character);


            //print the item description
            color = new Color(100, 116, 136, 0.99F);
            int numlines = Program.Window.Print(GUI.Console, width + 1, 13, item.Description, 18, color);

            // print the weight, volume, and quality
            color = new Color(Color.LightGray, 0.99F);
            string plural = ".";
            if (item.Weight > 1)
                plural = "s.";
            GUI.Console.Print(width + 1, 14 + numlines, "Weight: " + Math.Round(item.Weight, 2) + " lb" + plural, color);
            GUI.Console.Print(width + 1, 14 + numlines + 2, "Size: " + Math.Round(Convert.FromCubicFeetToCubicInches(item.Volume), 2), color);
            GUI.Console.Print( width + 7, 14 + numlines + 3, "cubic inches", color );

            int actionsStartY = Program.Console.Height - 7;

            // print the actions
            for (int x = width + 1; x <= width + 12; x += 11)
            {
                color = Color.Orange;
                Color highlightingColor = Color.Green;
                bool highlightingAction = false;
                bool thisIsTheFirstIteration = x == width + 1;

                string action = thisIsTheFirstIteration ? "[eat]" : "[drink]";
                if (Program.Window.MousePos.Y == actionsStartY && Program.Window.MousePos.X >= x && Program.Window.MousePos.X < x + action.Length)
                    highlightingAction = true;
                GUI.Console.Print(x, actionsStartY, action, highlightingAction ? highlightingColor : color, lighterColor);

                highlightingAction = false;
                action = thisIsTheFirstIteration ? "[drop]" : "[equip]";
                if (Program.Window.MousePos.Y == actionsStartY + 2 && Program.Window.MousePos.X >= x && Program.Window.MousePos.X < x + action.Length)
                    highlightingAction = true;
                GUI.Console.Print(x, actionsStartY + 2, action, highlightingAction ? highlightingColor : color, lighterColor);

                if (thisIsTheFirstIteration) {
                    highlightingAction = false;
                    action = "[open]";
                    if (Program.Window.MousePos.Y == actionsStartY + 4 && Program.Window.MousePos.X >= x && Program.Window.MousePos.X < x + action.Length)
                        highlightingAction = true;
                    GUI.Console.Print(x, actionsStartY + 4, action, highlightingAction ? highlightingColor : color, lighterColor);
                }
            }
        }

        public static void HandleEquipmentView( bool highlighting )
        {
            Point mousePos = new Point( SadConsole.Global.MouseState.ScreenPosition.X / SadConsole.Global.FontDefault.Size.X,
                 SadConsole.Global.MouseState.ScreenPosition.Y / SadConsole.Global.FontDefault.Size.Y );

            int viewStartX = width;

            if (Program.Animations.Count == 0)
                RenderEquipmentView( mousePos );

            bool selectingUnequip = mousePos.Y == 27 && mousePos.X >= viewStartX + 10 - "[unequip]".Length / 2 && mousePos.X <= viewStartX + 10 + "[unequip]".Length / 2;
            bool clickedOutsideOfItemWindow = ( mousePos.X > viewStartX + 20 || mousePos.X < viewStartX );

            int index = Program.Player.Inventory.IndexOf( currentlyViewedItem );

            if (SadConsole.Global.MouseState.LeftClicked && ( clickedOutsideOfItemWindow || selectingUnequip ) && !highlighting)
            {
                if (selectingUnequip)
                    Program.Player.Unequip( currentlyViewedItem );

                currentlyViewedItem = null;

                Program.Animations.Add( new CloseItemView() );
            }
        }
        public static void RenderEquipmentView( Point mousePos )
        {
            Item item = currentlyViewedItem;

            string name = item.Name;
            string pt2 = "";

            Color textColor = new Color( Color.AntiqueWhite, 0.99F ),
                  bgColor = new Color( color, 0.99F );

            int viewStartX = width;

            // split the name into two lines if necessary
            Tuple<string, string> stringParts = Window.SplitNameIfGreaterThanLength( name, pt2, 18 );
            name = stringParts.Item1;
            pt2 = stringParts.Item2;

            if (item.Identified)
                textColor = item.ReturnRarityColor();


            // print the name at top
            GUI.Console.Print( viewStartX + 1, 1, name, textColor );
            GUI.Console.Print( viewStartX + 1, 2, pt2, textColor );

            //// print the splash art
            //bool thisItemHasSplashArt = item.splash != null;
            //if (thisItemHasSplashArt)
            //    for (int i = 0; i < 18; i++)
            //        for (int j = 0; j < 10; j++)
            //            Program.mapConsole.Set(i, j + 3, item.splash[i, j].color, Swatch.SecondaryDarker, item.splash[i, j].character);

            textColor = new Color( 100, 116, 136, 0.99F );

            //print the item description
            Program.Window.Print( GUI.Console, viewStartX + 1, 13, item.Description, 18, textColor );

            // print the weight, volume, and quality
            string plural = ".";
            if (item.Weight > 1)
                plural = "s.";

            textColor = Color.LightGray;
            GUI.Console.Print( viewStartX + 1, 21, "Weight: " + Math.Round( item.Weight, 2 ) + " lb" + plural, textColor );
            GUI.Console.Print( viewStartX + 1, 23, "Size: " + Math.Round( Convert.FromCubicFeetToCubicInches( item.Volume ), 2 ), textColor );
            GUI.Console.Print( viewStartX + 7, 24, "cubic inches", textColor );

            int x = viewStartX + 10 - "[unequip]".Length / 2;

            bool highlightingAction = false;

            textColor = Color.Orange;
            Color highlightingColor = Color.Green;

            string action = "[unequip]";

            if (mousePos.Y == 27 && mousePos.X >= x && mousePos.X < x + action.Length)
                highlightingAction = true;

            if (highlightingAction)
                GUI.Console.Print( x, 27, action, highlightingColor, lighterColor );
            else
                GUI.Console.Print( x, 27, action, textColor, lighterColor );
        }


        // PROPERTIES //

        static public int Width
        {
            get { return displaying ? width : 0; }
            set { width = value; }
        }

        public static bool Displaying
        {
            get { return displaying; }
            set { GUI.Console.Clear(); displaying = value; }
        }

        public static bool DisplayingEquipment
        {
            get { return displayingEquipment; }
            set { displayingEquipment = value; }
        }
    }

}
