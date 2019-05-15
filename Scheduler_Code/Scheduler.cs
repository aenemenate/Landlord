using System;
using System.Collections.Generic;

namespace Landlord
{
    static class Scheduler
    {
        private static DateTime lastUpdate = DateTime.Now;
        private static TimeSpan updateTimeSpan = new TimeSpan(1250000 / 8);


        // FUNCTIONS //

        public static void HandleRoguelikeScheduling(MapTile map)
        {
            UpdateCreatureList(map);
            Program.WorldMap.LocalTile.Creatures.Sort();


            for (int i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
            {
                if (Program.WorldMap.LocalTile.Creatures[i] is Player) {
                    Program.Player.DetermineAction();
                    for (i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                        if (Program.WorldMap.LocalTile.Creatures[i] is Player)
                            Program.WorldMap.LocalTile.Creatures[i] = Program.Player;
                    break;
                }
                Program.WorldMap.LocalTile.Creatures[i].DetermineAction();
            }

            // set the current time to whichever creature has not yet moved
            Program.WorldMap.LocalTile.Creatures.Sort();
            for (int i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                if (Program.WorldMap.LocalTile.Creatures[i].NextActionTime.IsGreaterThan(Program.TimeHandler.CurrentTime))
                    Program.TimeHandler.CurrentTime = Program.WorldMap.LocalTile.Creatures[i].NextActionTime;
        }

        public static void HandleBuildModeScheduling(MapTile map)
        {
            if (DateTime.Now - updateTimeSpan < lastUpdate)
                return;
            lastUpdate = DateTime.Now;

            if (BuildingManager.Paused == false)
                Program.TimeHandler.CurrentTime.AddTime(1);
            else
                return;

            UpdateCreatureList(map);
            Program.WorldMap.LocalTile.Creatures.Sort();
            for (int i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                if (Program.WorldMap.LocalTile.Creatures[i].NextActionTime.IsLessThan(Program.TimeHandler.CurrentTime) || Program.WorldMap.LocalTile.Creatures[i].NextActionTime.Equals(Program.TimeHandler.CurrentTime))
                {
                    if (Program.WorldMap.LocalTile.Creatures[i] is Player == false)
                        Program.WorldMap.LocalTile.Creatures[i].DetermineAction();
                    else
                    {
                        BuildingManager.DeterminePlayerAction();
                        for (i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                            if (Program.WorldMap.LocalTile.Creatures[i] is Player)
                                Program.WorldMap.LocalTile.Creatures[i] = Program.Player;
                    }
                }
        }

        public static void HandleCraftingScheduling(MapTile map, int secondsToAdvance)
        {
            int counter = 0;
            int interval = 50;

            while (counter < secondsToAdvance)
            {
                Program.TimeHandler.CurrentTime.AddTime( interval );
                counter += interval;

                UpdateCreatureList( map );
                Program.WorldMap.LocalTile.Creatures.Sort();
                for (int i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                    if (Program.WorldMap.LocalTile.Creatures[i].NextActionTime.IsLessThan( Program.TimeHandler.CurrentTime ) || Program.WorldMap.LocalTile.Creatures[i].NextActionTime.Equals( Program.TimeHandler.CurrentTime ))
                    {
                        if (Program.WorldMap.LocalTile.Creatures[i] is Player == false)
                            Program.WorldMap.LocalTile.Creatures[i].DetermineAction();
                        else
                        {
                            BuildingManager.DeterminePlayerAction( true );
                            for (i = 0; i < Program.WorldMap.LocalTile.Creatures.Count; i++)
                                if (Program.WorldMap.LocalTile.Creatures[i] is Player)
                                    Program.WorldMap.LocalTile.Creatures[i] = Program.Player;
                        }
                    }
            }
        }

        private static void UpdateCreatureList(MapTile map)
        {
            map.Creatures = new List<Creature>();

            for (int i = map.Blocks.GetLength( 0 ) - 1; i > 0; i--)
                if (map.Blocks[i] is Creature creature)
                    map.Creatures.Add(creature);
        }

        public static void InitCreatureListScheduling(MapTile map)
        {
            UpdateCreatureList(map);
            // set the action times of each entity on this floor to equal the current time
            int creatureListLength = map.Creatures.Count;
            if (creatureListLength > 1)
                for (int i = 0; i < creatureListLength; i++)
                    map.Dungeon.Floors[map.CurrentFloor].Creatures[i].NextActionTime = new Time( Program.TimeHandler.CurrentTime );
        }


        // PROPERTIES //

        public static DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }
    }
}
