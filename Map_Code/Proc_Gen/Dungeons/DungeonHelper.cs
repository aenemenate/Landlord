using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class DungeonHelper
    {

        // Generation Funcs //

        // fill the map with dirt
        public static void InitializeMap(DungeonFloor dungeonFloor)
        {
            // fill the map with dirt
            for (int i = 0; i < dungeonFloor.Width; i++)
                for (int j = 0; j < dungeonFloor.Height; j++) {
                    dungeonFloor[i, j] = new Wall(Material.Stone);
                    dungeonFloor.Floor[i * Program.WorldMap.TileWidth + j] = new DirtFloor();
                }
        }
        // the rest are self explanatory
        public static void PlaceStairs(DungeonFloor dungeonFloor, Point upStairPos)
        {
            dungeonFloor[upStairPos.X, upStairPos.Y] = new UpStair(Material.Stone);

            List<Point> potentialSpots = new List<Point>(); // for the down stair
            for (int i = 1; i < dungeonFloor.Width - 1; i++)
                for (int j = 1; j < dungeonFloor.Height - 1; j++) {
                    if (GetNumOfAdjacentWalls(new Point(i, j), dungeonFloor) < 2 && new Point(i, j).DistFrom(upStairPos) > 20)
                        potentialSpots.Add(new Point(i, j));
                }
            Point spot = potentialSpots[Program.RNG.Next(0, potentialSpots.Count)];
            dungeonFloor[spot.X, spot.Y] = new DownStair(Material.Stone);
        }

        public static void PlaceItemsAndChests(DungeonFloor dungeonFloor, Point upStairPos)
        {
            // Figure out where items can be placed
            List<Point> itemSpots = new List<Point>();
            for (int i = 1; i < dungeonFloor.Width - 1; i++)
                for (int j = 1; j < dungeonFloor.Height - 1; j++)
                    if (GetNumOfAdjacentWalls(new Point(i, j), dungeonFloor) < 2 && dungeonFloor[i, j].Type == BlockType.Empty)
                        itemSpots.Add(new Point(i, j));

            // Generate a dijkstra map starting from the player's pos
            int[] placementMap = new int[dungeonFloor.Width * dungeonFloor.Height];
            for (int i = 0; i < dungeonFloor.Width; i++)
                for (int j = 0; j < dungeonFloor.Height; j++)
                    placementMap[i * dungeonFloor.Width + j] = new Point(i, j).Equals(upStairPos) ? 0 : 100000;
            DijkstraMaps.DijkstraNeighbors(new HashSet<Point>(new Point[] { upStairPos }), 1, ref placementMap);

            // Find out the value of the spot farthest from the player
            int maxValue = 0;
            for (int i = 0; i < dungeonFloor.Width; i++)
                for (int j = 0; j < dungeonFloor.Height; j++)
                    maxValue = (placementMap[i * dungeonFloor.Width + j] > maxValue && placementMap[i * dungeonFloor.Width + j] != 100000) ? placementMap[i * dungeonFloor.Width + j] : maxValue;

            const int chanceToSpawnCloserThanMinDist = 20;
            int desiredItemCount = itemSpots.Count / 100;
            int itemsPlaced = 0;

            while (itemsPlaced < desiredItemCount)
            {
                int roll = Program.RNG.Next(1, 101);
                Point next;
                int index;

                // repeatedly pick spots until it matches the spot that our roll corellates with
                do
                {
                    index = Program.RNG.Next(0, itemSpots.Count);
                    next = itemSpots[index];
                } while ((placementMap[next.X * dungeonFloor.Width + next.Y] > (maxValue / 3) * 2 && roll <= chanceToSpawnCloserThanMinDist) || (placementMap[next.X * dungeonFloor.Width + next.Y] < (maxValue / 3) * 2 && roll > chanceToSpawnCloserThanMinDist));

                Block itemOrChest = GetItemOrChest();
                if (itemOrChest != null)
                    dungeonFloor[next.X, next.Y] = itemOrChest;

                itemsPlaced++;
                itemSpots.RemoveAt(index);
            }
        }

        public static void PlaceCreatures(DungeonFloor dungeonFloor, Point upStairPos, List<string> monsterTypes, Point worldIndex, int floor)
        {
            List<Point> creatureSpots = new List<Point>();
            for (int i = 1; i < dungeonFloor.Width - 1; i++)
                for (int j = 1; j < dungeonFloor.Height - 1; j++)
                {
                    Point spot = new Point(i, j);
                    if (GetNumOfAdjacentWalls(spot, dungeonFloor) <= 4 && dungeonFloor[i, j].Type == BlockType.Empty && spot.DistFrom(upStairPos) > 12)
                        creatureSpots.Add(spot);
                }

            int creaturesSpawned = 0;
            int desiredcreatureCount = creatureSpots.Count / 125;
            while (creaturesSpawned < desiredcreatureCount) {
                int index = Program.RNG.Next(0, creatureSpots.Count);
                Point next = creatureSpots[index];
                Monster monster = DataReader.GetMonster( monsterTypes[Program.RNG.Next( 0, monsterTypes.Count )], dungeonFloor.Blocks, next, worldIndex, floor );
                while (monster == null)
                    monster = DataReader.GetMonster( monsterTypes[Program.RNG.Next( 0, monsterTypes.Count )], dungeonFloor.Blocks, next, worldIndex, floor);
                dungeonFloor[next.X, next.Y] = monster;
                creaturesSpawned++;
                creatureSpots.RemoveAt(index);
            }
        }


        // Get Funcs //

        // GetItemOrChest returns a random item. It has a chance to return a chest if spawnChests is set to true.
        private static Block GetItemOrChest(bool spawnChests = true)
        {
            int rand = Program.RNG.Next(0, 5);
            int chestChance = Program.RNG.Next(0, 100);
            if (chestChance >= 5 || spawnChests == false)
            {
                switch (rand) {
                    case (0):
                        return new Potion(true);
                    case (1):
                        return GetArmor();
                    case (2):
                        return GetWeapon();
                    case (3):
                        return new Quiver(false);
                    case (4):
                        return GetBlueprintOrRecipe();
                }
            }
            else
                return GetChest();

            return null;
        }

        // GetChest returns a chest containing a random amount of random items.
        private static Chest GetChest()
        {
            Chest chest = new Chest(Material.Wood);
            int capacity = Program.RNG.Next(1, 9);
            for (int count = 0; count < capacity; count++)
                chest.Inventory.Add((Item)GetItemOrChest(false));

            return chest;
        }

        // GetArmor returns a random piece of armor.
        private static Armor GetArmor()
        {
            int rand = Program.RNG.Next(0, 5);
            switch (rand)
            {
                case (0):
                    return new Helmet(true);
                case (1):
                    return new ChestPiece(true);
                case (2):
                    return new Gauntlets(true);
                case (3):
                    return new Leggings(true);
                case (4):
                    return new Boots(true);
                default:
                    return null;
            }
        }

        // GetWeapon returns a random weapon.
        private static Item GetWeapon()
        {
            int rand = Program.RNG.Next(0, 8);
            bool twoHanded = Program.RNG.Next(0, 2) == 0 ? true : false;
            switch (rand)
            {
                case (0):
                    return new Sword(twoHanded);
                case (1):
                    return new Dagger(false);
                case (2):
                    return new Mace(twoHanded);
                case (3):
                    return new Axe(twoHanded);
                case (4):
                    return new Spear(twoHanded);
                case (5):
                    return new Shield(false);
                case (6):
                    return new Bow(true);
                case (7):
                    return new Torch(twoHanded);
                default:
                    return null;
            }
        }

        private static Item GetBlueprintOrRecipe()
        {
            int rand = Program.RNG.Next(0, 8);
            switch (rand) {
                case (0):
                    return new GlueWheelRecipe(true);
                case (1):
                    return new ShapeWheelRecipe(true);
                case (2):
                    return new WoodCraftingTableBlueprint(true);
                case (3):
                    return new WoodStoneMillBlueprint(true);
                case (4):
                    return new WoodCartBlueprint(true);
                case (5):
                    return new WoodHandleRecipe(true);
                case (6):
                    return new CarveHiltRecipe(true);
                case (7):
                    return new ConstructTorchRecipe(true);
                default:
                    return null;
            }
        }

        // these functions find how many walls are adjacent to a certain point (not including the point itself)

        // this one uses a Block Map as input
        public static int GetNumOfAdjacentWalls(Point point, Block[,] room)
        {
            int adjacentWalls = 0;
            for (int x = point.X - 1; x <= point.X + 1; x++)
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                    if (room[x, y].Solid && !point.Equals(new Point(x, y)))
                        adjacentWalls += 1;
            return adjacentWalls;
        }
        // this one uses a dungeon floor as input
        public static int GetNumOfAdjacentWalls(Point point, DungeonFloor dungeonFloor)
        {
            int adjacentWalls = 0;
            for (int x = point.X - 1; x <= point.X + 1; x++)
            {
                for (int y = point.Y - 1; y <= point.Y + 1; y++)
                {
                    if (dungeonFloor[x, y].Solid && !point.Equals(new Point(x, y)))
                        adjacentWalls += 1;
                }
            }
            return adjacentWalls;
        }
    }
}
