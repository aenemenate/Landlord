using System;
using System.Collections.Generic;

namespace Landlord
{
    static class Scheduler
    {
        private static DateTime lastUpdate = DateTime.Now;
        private static TimeSpan updateTimeSpan = new TimeSpan(1250000 / 8);


        // FUNCTIONS //

        public static void HandleRoguelikeScheduling()
        {
            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;

            UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y]);

            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            int width = Program.WorldMap.TileWidth;
            creatures.Sort();

            for (int i = 0; i < creatures.Count; i++)
            {
                if (creatures[i] is Player) {
                    Program.Player.DetermineAction();
                    for (i = 0; i < creatures.Count; i++)
                        if (creatures[i] is Player)
                            creatures[i] = Program.Player;
                    break;
                }
                creatures[i].DetermineAction();
            }

            // set the current time to whichever creature has not yet moved
            creatures.Sort();
            for (int i = 0; i < creatures.Count; i++)
                if (creatures[i].NextActionTime.IsGreaterThan(Program.TimeHandler.CurrentTime))
                    Program.TimeHandler.CurrentTime = creatures[i].NextActionTime;
        }

        public static void HandleBuildModeScheduling()
        {
            if (DateTime.Now - updateTimeSpan < lastUpdate)
                return;
            lastUpdate = DateTime.Now;

            if (BuildingManager.Paused == false)
                Program.TimeHandler.CurrentTime.AddTime(1);
            else
                return;

            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;

            UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y]);

            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            int width = Program.WorldMap.TileWidth;


            creatures.Sort();
            for (int i = 0; i < creatures.Count; i++)
                if (creatures[i].NextActionTime.IsLessThan(Program.TimeHandler.CurrentTime) || creatures[i].NextActionTime.Equals(Program.TimeHandler.CurrentTime))
                {
                    if (creatures[i] is Player == false)
                        creatures[i].DetermineAction();
                    else
                    {
                        BuildingManager.DeterminePlayerAction();
                        for (i = 0; i < creatures.Count; i++)
                            if (creatures[i] is Player)
                                creatures[i] = Program.Player;
                    }
                }
        }

        public static void HandleCraftingScheduling(int secondsToAdvance)
        {
            int counter = 0;
            int interval = 50;

            while (counter < secondsToAdvance)
            {
                Program.TimeHandler.CurrentTime.AddTime( interval );
                counter += interval;

                int currentFloor = Program.Player.CurrentFloor;
                Point worldIndex = Program.Player.WorldIndex;

                UpdateCreatureList(Program.WorldMap[worldIndex.X, worldIndex.Y]);

                List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
                int width = Program.WorldMap.TileWidth;

                creatures.Sort();
                for (int i = 0; i < creatures.Count; i++)
                    if (creatures[i].NextActionTime.IsLessThan( Program.TimeHandler.CurrentTime ) || creatures[i].NextActionTime.Equals( Program.TimeHandler.CurrentTime )) {
                        if (creatures[i] is Player == false)
                            creatures[i].DetermineAction();
                        else {
                            BuildingManager.DeterminePlayerAction( true );
                            for (i = 0; i < creatures.Count; i++)
                                if (creatures[i] is Player)
                                    creatures[i] = Program.Player;
                        }
                    }
            }
        }

        private static void UpdateCreatureList(MapTile map)
        {
            map.Creatures = new List<Creature>();

            for (int i = map.Blocks.GetLength( 0 ) - 1; i >= 0; i--)
                if (map.Blocks[i] is Creature creature)
                    map.Creatures.Add(creature);

            if (map.Dungeon != null)
                for (int i = 0; i < map.Dungeon.Floors.GetLength(0); i++) {
                    map.Dungeon.Floors[i].Creatures = new List<Creature>();

                    for (int j = map.Dungeon.Floors[i].Blocks.GetLength(0) - 1; j > 0; j--)
                        if (map.Dungeon.Floors[i].Blocks[j] is Creature creature)
                            map.Dungeon.Floors[i].Creatures.Add(creature);
                }
        }

        public static void InitCreatureListScheduling(MapTile map, int floor)
        {
            UpdateCreatureList(map);
            // set the action times of each entity on this floor to equal the current time
            int creatureListLength = map.Creatures.Count;
            if (creatureListLength > 1)
                for (int i = 0; i < creatureListLength; i++)
                    map.Dungeon.Floors[floor].Creatures[i].NextActionTime = new Time( Program.TimeHandler.CurrentTime );
        }


        // PROPERTIES //

        public static DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }
    }
}
