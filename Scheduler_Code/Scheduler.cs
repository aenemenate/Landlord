using System;
using System.Collections.Generic;

namespace Landlord
{
    static class Scheduler
    {


        // FUNCTIONS //
        public static void CheckUpdates()
        {
            int lastPlantUpdate = Program.WorldMap.CompletedUpdateHour;
            if (lastPlantUpdate <= Program.TimeHandler.CurrentTime.Hour - 4 ||
                (lastPlantUpdate != 0 && Program.TimeHandler.CurrentTime.Hour == 0)) {
                Program.WorldMap.UpdatePlants();
            }
        }
        public static void HandleRoguelikeScheduling(Player player)
        {
            int currentFloor = player.CurrentFloor;
            Point worldIndex = player.WorldIndex;

            UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y]);
            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            List<Projectile> projectiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Projectiles : Program.WorldMap[worldIndex.X, worldIndex.Y].Projectiles;
            creatures.Sort();

            for (int i = 0; i < creatures.Count; i++) {
                Program.TimeHandler.CurrentTime = creatures[i].NextActionTime;
                if (projectiles.Count > 0)
                    break;
                if (creatures[i] is Player) {
                    player.DetermineAction();
                    creatures[i] = player;
                    break;
                }
                creatures[i].DetermineAction();
            }

            UpdateProjectiles(currentFloor, worldIndex);

            CheckUpdates();
        }


        public static void HandleBuildModeScheduling()
        {

            if (BuildingManager.Paused == false)
                Program.TimeHandler.CurrentTime.AddTime(2);
            else
                return;
            
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y], currentFloor);
            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            
            for (int i = 0; i < creatures.Count; i++)
                if (creatures[i].NextActionTime.IsLessThan(Program.TimeHandler.CurrentTime) || creatures[i].NextActionTime.Equals(Program.TimeHandler.CurrentTime)) {
                    if (creatures[i] is Player) {
                        BuildingManager.DeterminePlayerAction();
                        creatures[i] = Program.Player;
                    }
                    else
                        creatures[i].DetermineAction();
                }

            CheckUpdates();
        }
        public static void HandleCraftingScheduling(int secondsToAdvance)
        {
            int counter = 0;
            int interval = 50;

            Point worldIndex = Program.Player.WorldIndex;

            while (counter < secondsToAdvance) {
                Program.TimeHandler.CurrentTime.AddTime( interval );
                counter += interval;

                int currentFloor = Program.Player.CurrentFloor;

                UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y]);
                List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;

                creatures.Sort();
                for (int i = 0; i < creatures.Count; i++)
                    if (creatures[i].NextActionTime.IsLessThan( Program.TimeHandler.CurrentTime ) || creatures[i].NextActionTime.Equals( Program.TimeHandler.CurrentTime )) {
                        if (creatures[i] is Player == false)
                            creatures[i].DetermineAction();
                        else {
                            Program.Player.Wait();
                            creatures[i] = Program.Player;
                        }
                    }
                CheckUpdates();
            }

            InitCreatureListScheduling(Program.WorldMap[worldIndex.X, worldIndex.Y]);
        }
        private static void UpdateCreatureList(MapTile map, int floor = -2)
        {
            if (floor == -1 || floor == -2) {
                map.Creatures = new List<Creature>();
                for (int i = map.Blocks.GetLength(0) - 1; i >= 0; i--)
                    if (map.Blocks[i] is Creature creature) {
                        if (map.Blocks[i] is Player == false)
                            map.Creatures.Add(creature);
                        else
                            map.Creatures.Add(Program.Player);
                    }
            }
            if (floor == -2) {
                if (map.Dungeon != null) {
                    for (int i = 0; i < map.Dungeon.Floors.GetLength(0); i++) {
                        map.Dungeon.Floors[i].Creatures = new List<Creature>();
                        for (int j = map.Dungeon.Floors[i].Blocks.GetLength(0) - 1; j >= 0; j--)
                            if (map.Dungeon.Floors[i].Blocks[j] is Creature creature)
                                map.Dungeon.Floors[i].Creatures.Add(creature);
                    }
                }
            }
            else if (floor != -1) { 
                if (map.Dungeon != null) {
                    map.Dungeon.Floors[floor].Creatures = new List<Creature>();
                    for (int j = map.Dungeon.Floors[floor].Blocks.GetLength(0) - 1; j >= 0; j--)
                        if (map.Dungeon.Floors[floor].Blocks[j] is Creature creature)
                            map.Dungeon.Floors[floor].Creatures.Add(creature);
                }
            }
        }
        public static void InitCreatureListScheduling(MapTile map)
        {
            UpdateCreatureList(map);

            int creatureListLength = map.Creatures.Count;
            if (creatureListLength > 1)
                for (int i = 0; i < creatureListLength; i++)
                    map.Creatures[i].NextActionTime = new Time(Program.TimeHandler.CurrentTime);

            if (map.Dungeon != null)
                for (int i = 0; i < map.Dungeon.Floors.GetLength(0); i++) {
                    creatureListLength = map.Dungeon.Floors[i].Creatures.Count;
                    if (creatureListLength > 1)
                        for (int j = 0; j < creatureListLength; j++)
                            map.Dungeon.Floors[i].Creatures[j].NextActionTime = new Time(Program.TimeHandler.CurrentTime);
                }

        }
        private static void UpdateProjectiles(int currentFloor, Point worldIndex)
        {
            List<Projectile> projectiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Projectiles : Program.WorldMap[worldIndex.X, worldIndex.Y].Projectiles;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            for (int i = projectiles.Count - 1; i >= 0; i--) {
                bool deleted = projectiles[i].Update(blocks, Program.WorldMap.TileWidth);
                if (deleted) projectiles.RemoveAt(i);
            }
        }
    }
}
