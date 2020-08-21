using System;
using System.Collections.Generic;

namespace Landlord
{
    class PatrolMaps
    {
        // VARIABLES //
        private const short dijkstraDefaultVal = short.MaxValue;
        private int width, height;
        private List<short[]> patrolGoals;


        // CONSTRUCTORS //

        public PatrolMaps(int width, int height, List<Point> roomCenters, DungeonFloor dungeonFloor)
        {
            this.width = width;
            this.height = height;
            patrolGoals = new List<short[]>();
            CreatePatrolMaps(roomCenters, dungeonFloor);
        }

        public PatrolMaps()
        {
            width = 100;
            height = 100;
        }


        // FUNCTIONS //

        public void CreatePatrolMaps(List<Point> roomCenters, DungeonFloor dungeonMap)
        {
            DungeonFloor map = dungeonMap;
            int currentPatrolMap = 0;

            void DijkstraNeighbors(HashSet<Point> spots, short dist)
            {
                HashSet<Point> neighbors = new HashSet<Point>();
                foreach (Point spot in spots)
                {
                    for (int i = Math.Max(0, spot.X - 1); i <= Math.Min(spot.X + 1, width - 1); i++)
                        for (int j = Math.Max(0, spot.Y - 1); j <= Math.Min(spot.Y + 1, height - 1); j++)
                        {
                            Point point = new Point(i, j);
                            bool blockIsPassable = !(map.Blocks[i * Program.WorldMap.TileWidth + j].Solid && map.Blocks[i * Program.WorldMap.TileWidth + j].Type != BlockType.Door);
                            if ((patrolGoals[currentPatrolMap][point.X * width + point.Y] > dist) && blockIsPassable)
                            {
                                neighbors.Add(point);
                                patrolGoals[currentPatrolMap][point.X * width + point.Y] = dist;
                            }
                        }
                }
                if (neighbors.Count > 0)
                    DijkstraNeighbors(neighbors, (short)(dist + 1));
            }

            foreach (Point point in roomCenters)
            {
                patrolGoals.Add(new short[width * height]);

                for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        patrolGoals[currentPatrolMap][i * width + j] = dijkstraDefaultVal;

                patrolGoals[currentPatrolMap][roomCenters[currentPatrolMap].X * width + roomCenters[currentPatrolMap].Y] = 0;

                DijkstraNeighbors(new HashSet<Point>(new Point[] { roomCenters[currentPatrolMap] }), 1);

                currentPatrolMap++;
            }
        }

        // PROPERTIES //

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

        public List<short[]> PatrolGoals
        {
            get { return patrolGoals; }
            set { patrolGoals = value; }
        }
    }
}