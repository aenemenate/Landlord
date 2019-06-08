﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class WorldMapGeneration
    {
        // if height higher than 180f a tile is stone, lower than that but higher than 65f it's grass, lower than or equal to that that it's water.
        public static float StoneCutoff = 180f, WaterCutoff = 65f;

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
            GenerateTrees(map, rng, 7, 15);
            GeneratePlants(map, rng, plantTypes, 100, 25);
            GenerateOre(map, rng, 0.025f);
            //GenerateCreatures(map, rng, creatureTypes);
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
                    nextSpot = new Point(rng.Next(Math.Max(currentTree.X - 8, 0), Math.Min(currentTree.X + 8, map.Width - 1)), rng.Next(Math.Max(currentTree.Y - 8, 0), Math.Min(currentTree.Y + 8, map.Height - 1)));
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
            // place seed trees
            List<Point> availableSpots = map.GetAllDirtTiles();
            Point nextSpot = null;
            int placedPlants = 0;

            // placement algorithm
            while (placedPlants < numOfSeedPlants) {
                Plant p;
                nextSpot = availableSpots[rng.Next(0, availableSpots.Count)];
                if (nextSpot != null){
                    placedPlants++;
                    p = DataReader.GetPlant(plantTypes[rng.Next(0, plantTypes.Count)]);
                    map.Blocks[nextSpot.X * map.Width + nextSpot.Y] = p;
                }
            }
            // growth algorithm
            for (int g = 0; g < growthGenerations; g++)
                for (int i = 0; i < map.Width; i++)
                    for (int j = 0; j < map.Height; j++)
                        if (map[i, j] is Plant p && g % p.GrowthInterval == 0) {
                            int currentStage = p.GrowthStages.IndexOf(p.Graphic);
                            if (currentStage < p.GrowthStages.Count - 1) {
                                p.Graphic = p.GrowthStages[currentStage + 1];
                                continue;
                            }
                            int randX = rng.Next(i - p.SeedRadius, i + p.SeedRadius + 1), randY = rng.Next(j - p.SeedRadius, j + p.SeedRadius + 1);
                            Point seedSpot = new Point(Math.Min(map.Width - 1, Math.Max(0, randX)), Math.Min(map.Height - 1, Math.Max(0, randY)));
                            if (map.Floor[seedSpot.X * map.Width + seedSpot.Y] is DirtFloor && map.Blocks[seedSpot.X * map.Width + seedSpot.Y].Solid == false) {
                                map[seedSpot.X, seedSpot.Y] = new Plant(p.GrowthStages[0], p.Name, p.GrowthInterval, p.SeedRadius, p.GrowthStages, p.ForeColor);
                            }
                        }
        }
        private static void GenerateOre( MapTile map, Random rng, float scale )
        {
            float[,] coalMap = SimplexNoise.Calc2D( map.Width, map.Height, scale );

            for (int i = 0; i < map.Width; i++)
                for (int j = 0; j < map.Height; j++) {
                    if (map[i,j].Name.Equals("stone wall")
                        && coalMap[i,j] >= StoneCutoff) {
                        map[i, j] = null;
                        map[i, j] = new OreWall(Material.Coal);
                    }
                }
        }
        private static void GenerateCreatures( MapTile map, Random rng, List<string> creatureTypes )
        {
            List<Point> availableSpots = map.Blocks.GetAllTraversablePoints(new Point(map.Width, map.Height));
            int desiredCreatureCount = availableSpots.Count / 750, placedCreatures = 0;

            while (placedCreatures < desiredCreatureCount) {
                Point nextPoint = availableSpots[rng.Next(0, availableSpots.Count)];
                if (map[nextPoint.X, nextPoint.Y].Solid == false) {
                    Animal a = DataReader.GetAnimal(creatureTypes[rng.Next(0, creatureTypes.Count)], map.Blocks, nextPoint, map.WorldIndex, -1);
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

        // funcs that work on entire world map

        public static void GenerateRivers( Random rng, int width, int height, float[,] heightMap )
        {
            Point leftPoint = new Point( 1, rng.Next( 2, 497 ) );
            Point rightPoint = new Point( 498, rng.Next( 2, 497 ) );
            while (leftPoint.Y % 100 == 0 || rightPoint.Y % 100 == 0 ||
                    heightMap[leftPoint.X, leftPoint.Y] >= StoneCutoff ||
                      heightMap[rightPoint.X, rightPoint.Y] >= StoneCutoff)
            {
                leftPoint = new Point( 1, rng.Next( 2, 497 ) );
                rightPoint = new Point( 498, rng.Next( 2, 497 ) );
            }
            Generate5x5River( leftPoint, rightPoint, heightMap );

            int numOfRivers = 10;
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
            for (int i = 1; i < 499; i++)
                for (int j = 1; j < 499; j++)
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
                for (int i = Math.Max( 0, point.X - 2 ); i <= Math.Min( 499, point.X + 2 ); i++)
                    for (int j = Math.Max( 0, point.Y - 2 ); j <= Math.Min( 499, point.Y + 2 ); j++)
                    {
                        if (heightMap[i, j] >= WaterCutoff)
                            heightMap[i, j] = WaterCutoff - 1;
                    }
            }
        }
    }
}
