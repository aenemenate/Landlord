using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Landlord
{
    static class WorldMapGeneration
    {
        public static float StoneCutoff = 167f, WaterCutoff = 65f;

        // funcs that work on individual tiles

        public static void GenerateForestMap(Random rng, MapTile map, List<string> plantTypes, List<string> creatureTypes, Point worldIndex, float[,] heightMap)
        {
            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++) {
                    if (heightMap[i + map.Width * worldIndex.X, j + map.Height * worldIndex.Y] >= StoneCutoff) {
                        map.Blocks[i * map.Width + j] = new Wall(Material.Stone);
                        map.Floor[i * map.Width + j] = new DirtFloor();
                    }
                    else if (heightMap[i + map.Width * worldIndex.X, j + map.Height * worldIndex.Y] >= WaterCutoff + 6f) {
                        map.Blocks[i * map.Width + j] = new Air();
                        map.Floor[i * map.Width + j] = new DirtFloor();
                    }
                    else if (heightMap[i + map.Width * worldIndex.X, j + map.Height * worldIndex.Y] >= WaterCutoff) {
                        map.Blocks[i * map.Width + j] = new Air();
                        map.Floor[i * map.Width + j] = new Sand();
                    }
                    else {
                        map.Blocks[i * map.Width + j] = new Air();
                        map.Floor[i * map.Width + j] = new Water();
                    }
                }
            GenerateTrees(map, rng, 7, 16);
            GeneratePlants(map, rng, plantTypes, 500, 24);
            GenerateOre(map, rng, 0.025f);
            GenerateCreatures(map, rng, creatureTypes);
        }
        private static void GenerateTrees(MapTile map, Random rng, int numOfSeedTrees, int growthGenerations)
        {
            // place seed trees
            List<Point> availableSpots = map.GetAllDirtTiles();
            List<Point> treeSpots = new List<Point>();
            Point nextSpot = null;
            int placedTrees = 0;

            // placement algorithm
            while (placedTrees < numOfSeedTrees)
            {
                Point potentialSpot;
                int count = 0;
                while (nextSpot == null && count < availableSpots.Count) {
                    count++;
                    potentialSpot = availableSpots[rng.Next(0, availableSpots.Count)];
                    if (potentialSpot.DistFrom(map.GetClosestOfTileTypeToPos(potentialSpot, new Water())) <= 16) {
                        nextSpot = potentialSpot;
                    }
                }

                if (nextSpot != null) {
                    treeSpots.Add(nextSpot);
                    map.Blocks[nextSpot.X * map.Width + nextSpot.Y] = new Tree(Material.Wood);
                }
                placedTrees++;
            }

            // run growth algorithm
            for (int i = 0; i < growthGenerations; i++) {
                int treesThisGen = treeSpots.Count;
                for (int j = 0; j < treesThisGen; j++) {
                    Point currentTree = treeSpots[j];
                    int count = 0;
                    do {
                        count++;
                        nextSpot = new Point(rng.Next(Math.Max(currentTree.X - 8, 0), Math.Min(currentTree.X + 8, map.Width - 1)), rng.Next(Math.Max(currentTree.Y - 8, 0), Math.Min(currentTree.Y + 8, map.Height - 1)));
                        for (int x = Math.Max(nextSpot.X - 3, 0); x <= Math.Min(nextSpot.X + 3, map.Width - 1); x++)
                            for (int y = Math.Max(nextSpot.Y - 3, 0); y <= Math.Min(nextSpot.Y + 3, map.Height - 1); y++)
                                if (map[x, y] is Tree)
                                    availableSpots.Remove(nextSpot);
                    } while ((map.Blocks.GetEmptyAdjacentBlocks(new Point(map.Width, map.Height), nextSpot).Count != 8 || availableSpots.Contains(nextSpot) == false) && count <= 7);
                    if (count <= 7) {
                        treeSpots.Add(nextSpot);
                        map.Blocks[nextSpot.X * map.Width + nextSpot.Y] = new Tree(Material.Wood);
                    }
                }
            }
        }
        private static void GeneratePlants(MapTile map, Random rng, List<string> plantTypes, int numOfSeedPlants, int growthGenerations)
        {
            // place seed plants
            List<Point> availableSpots = map.GetAllDirtTiles();
            Point nextSpot = null;
            int placedPlants = 0;
            int maxTries = availableSpots.Count();
            int tries = 0;
            // placement algorithm
            while (placedPlants < numOfSeedPlants || tries > maxTries) {
                ++tries;
                Plant p;
                nextSpot = availableSpots[rng.Next(0, availableSpots.Count)];
                if (nextSpot != null) {
                    placedPlants++;
                    p = DataReader.GetPlant(plantTypes[rng.Next(0, plantTypes.Count)]);
                    if (p.RequirementsMet(map, nextSpot))
                        map.Blocks[nextSpot.X * map.Width + nextSpot.Y] = p;
                }
            }
            // growth algorithm
            for (int g = 0; g < growthGenerations; g++)
                for (int i = 0; i < map.Width; i++)
                    for (int j = 0; j < map.Height; j++)
                        if (map[i, j] is Plant p && g % p.GrowthInterval == 0) {
                            p.Grow(map, new Point(i, j), rng);
                        }
        }
        private static void GenerateOre( MapTile map, Random rng, float scale )
        {
            float[,] coalMap = SimplexNoise.Calc2D( map.Width, map.Height, scale );

            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++) {
                    if (map[i,j].Name.Equals("stone wall")
                        && coalMap[i,j] >= 190F)
                        map[i, j] = new OreWall(Material.Coal);
                }
        }
        private static void GenerateCreatures( MapTile map, Random rng, List<string> creatureTypes )
        {
            List<Point> availableSpots = map.Blocks.GetAllTraversablePoints(new Point(map.Width, map.Height));
            int desiredCreatureCount = rng.Next(1, 7), placedCreatures = 0;

            while (placedCreatures < desiredCreatureCount) {
                Point nextPoint = availableSpots[rng.Next(0, availableSpots.Count)];
                if (map[nextPoint.X, nextPoint.Y].Solid == false) {
                    Animal a = DataReader.GetAnimal(creatureTypes[rng.Next(0, creatureTypes.Count)], map.Blocks, nextPoint, map.WorldIndex, -1);
                    int maxHV = a.Stats.Resources[Resource.HV];
                    a.Stats.Resources[Resource.HV] = rng.Next(maxHV / 3, maxHV);
                    if (a != null) {
                        map[nextPoint.X, nextPoint.Y] = a;
                        placedCreatures++;
                    }
                }
            }
        }
        public static void GenerateDungeonEntrance(MapTile map)
        {

            List<Point> potentialDungeonEntrances = new List<Point>();


            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++)
                    if (map.Blocks[i * map.Width + j] is Wall && map.AdjacentBlocksEmpty(new Point(i, j)) && !map.PointOnEdge(new Point(i, j)))
                        potentialDungeonEntrances.Add(new Point(i, j));

            if (potentialDungeonEntrances.Count != 0)
            {
                Random rng = new Random();
                int rand = rng.Next(0, potentialDungeonEntrances.Count);
                map.DungeonEntrance = potentialDungeonEntrances[rand];
                map.Blocks[map.DungeonEntrance.X * map.Width + map.DungeonEntrance.Y] = new DownStair(Material.Stone);
            }
        }

        //carves the heightmap
        public static void GenerateRivers( Random rng, int worldwidth, int worldheight, int tilewidth, int tileheight, float[,] heightMap )
        {
            int width = worldwidth * tilewidth;
            int height = worldheight * tileheight;
            Point leftPoint;
            Point rightPoint;
            do {
                leftPoint = new Point(1, rng.Next(2, height - 3));
                rightPoint = new Point(width - 2, rng.Next(2, height - 3));
            } while (leftPoint.Y % tileheight > 90 || leftPoint.Y % tileheight < 10 || rightPoint.Y % tileheight > 90 || rightPoint.Y % tileheight < 10 ||
                     heightMap[leftPoint.X, leftPoint.Y] >= StoneCutoff ||
                       heightMap[rightPoint.X, rightPoint.Y] >= StoneCutoff);
            Generate5x5River( leftPoint, rightPoint, heightMap );

            int numOfRivers = 8;
            bool CheckedCanSpawnRiverHere( int i, int j )
            {
                if (heightMap[i, j] >= StoneCutoff)
                {
                    for (int k = i - 1; k <= i + 1; k++)
                        for (int m = j - 1; m <= j + 1; m++)
                            if (heightMap[k, m] > heightMap[i, j])
                                return false;
                    return true;
                }
                return false;
            }
            bool CheckedCanDepositRiverHere( int i, int j )
            {
                if (heightMap[i, j] < WorldMapGeneration.WaterCutoff)
                {
                    for (int k = i - 1; k <= i + 1; k++)
                        for (int m = j - 1; m <= j + 1; m++) {
                            if (heightMap[k, m] < heightMap[i, j])
                                return false;
                        }
                    return true;
                }
                return false;
            }

            // find high and low spots
            List<Point> potentialRiverSprings = new List<Point>(); // prospects will have a height over 165 while having no higher neighbors
            List<Point> potentialRiverDeposits = new List<Point>();
            for (int i = 1; i < width - 1; i++)
                for (int j = 1; j < height - 1; j++)
                {
                    if (CheckedCanSpawnRiverHere( i, j ) && rng.Next( 0, 100 ) > 40)
                        potentialRiverSprings.Add( new Point( i, j ) );
                    else if (CheckedCanDepositRiverHere( i, j ) && rng.Next( 0, 100 ) > 40)
                        potentialRiverDeposits.Add( new Point( i, j ) );
                }

            for (int c = 0; c <= numOfRivers; c++)
            {
                int springIndex = 0;
                int depositIndex = 0;
                while (( springIndex == 0 && depositIndex == 0 )
                      || potentialRiverSprings[springIndex].DistFrom( potentialRiverDeposits[depositIndex] ) > 500)
                {
                    springIndex = rng.Next( 0, potentialRiverSprings.Count );
                    depositIndex = rng.Next( 0, potentialRiverDeposits.Count );
                }
                if (rng.Next( 0, 100 ) < 75)
                    Generate1x1River( potentialRiverSprings[springIndex], potentialRiverDeposits[depositIndex], heightMap );
                else
                    Generate3x3River( potentialRiverSprings[springIndex], potentialRiverDeposits[depositIndex], heightMap );
            }
        }
        private static void Generate1x1River( Point start, Point end, float[,] heightMap )
        {
            List<Point> path = Riverfinder.FindPath( start, end, heightMap );
            foreach (Point point in path)
            {
                if (heightMap[point.X, point.Y] >= WaterCutoff)
                    heightMap[point.X, point.Y] = WaterCutoff - 1;
            }
        }
        private static void Generate3x3River( Point start, Point end, float[,] heightMap )
        {
            List<Point> path = Riverfinder.FindPath( start, end, heightMap );
            foreach (Point point in path)
            {
                if (heightMap[point.X, point.Y] >= WaterCutoff)
                    heightMap[point.X, point.Y] = WaterCutoff - 1;
                if (heightMap[point.X + 1, point.Y] >= WaterCutoff)
                    heightMap[point.X + 1, point.Y] = WaterCutoff - 1;
                if (heightMap[point.X, point.Y + 1] >= WaterCutoff)
                    heightMap[point.X, point.Y + 1] = WaterCutoff - 1;
                if (heightMap[point.X - 1, point.Y] >= WaterCutoff)
                    heightMap[point.X - 1, point.Y] = WaterCutoff - 1;
                if (heightMap[point.X, point.Y - 1] >= WaterCutoff)
                    heightMap[point.X, point.Y - 1] = WaterCutoff - 1;
            }
        }
        private static void Generate5x5River( Point start, Point end, float[,] heightMap )
        {
            List<Point> path = Riverfinder.FindPath( start, end, heightMap );
            foreach (Point point in path)
            {
                for (int i = Math.Max( 0, point.X - 2 ); i <= Math.Min( 399, point.X + 2 ); i++)
                    for (int j = Math.Max( 0, point.Y - 2 ); j <= Math.Min( 399, point.Y + 2 ); j++)
                    {
                        if (heightMap[i, j] >= WaterCutoff)
                            heightMap[i, j] = WaterCutoff - 1;
                    }
            }
        }
    }
}
