using System;
using System.Collections.Generic;

namespace Landlord
{
    class DijkstraMaps
    {
        // EVENTS //
        public event EventHandler<EventArgs> OnPlayerMoved;
        public event EventHandler<EventArgs> OnItemMoved;

        // VARIABLES //
        private const int dijkstraDefaultVal = 100000;
        private int width, height;
        private int[] distToPlayerMap; // To access: (x * width) + y
        private int[] distToItemsMap; // To access: (x * width) + y


        // CONSTRUCTORS //

        public DijkstraMaps(int width, int height)
        {
            this.width = width;
            this.height = height;
            distToPlayerMap = new int[width * height];
            distToItemsMap = new int[width * height];
            OnPlayerMoved += UpdateDistToPlayerMap;
            OnItemMoved += UpdateDistToItemsMap;
        }

        public DijkstraMaps()
        {
            OnPlayerMoved += UpdateDistToPlayerMap;
            OnItemMoved += UpdateDistToItemsMap;
        }


        // FUNCTIONS //

        public void UpdateDistToPlayerMap(object sender, EventArgs e)
        {
            int maxDist = 40;

            MapTile map = Program.WorldMap.LocalTile;

            if (map.CurrentFloor == -1)
                return;
            
            for (int i = Math.Max(0, Program.Player.Position.X - maxDist); i <= Math.Min(width - 1, Program.Player.Position.X + maxDist); i++)
                for (int j = Math.Max(0, Program.Player.Position.Y - maxDist); j <= Math.Min(height - 1, Program.Player.Position.Y + maxDist); j++)
                    distToPlayerMap[i * width + j] = dijkstraDefaultVal;
            distToPlayerMap[Program.Player.Position.X * width + Program.Player.Position.Y] = 0;

            DijkstraNeighbors(new HashSet<Point>( new Point[] { Program.Player.Position } ), 1, ref distToPlayerMap, Program.WorldMap.LocalTile.Blocks, new Point(width, height), maxDist);
        }

        public void UpdateDistToItemsMap(object sender, EventArgs e)
        {
            MapTile map = Program.WorldMap.LocalTile;

            if ( map.CurrentFloor == -1 )
                return;
            

            HashSet<Point> itemSpots = new HashSet<Point>();

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    if (map.Blocks[i * Program.WorldMap.LocalTile.Width + j].Type == BlockType.Item)
                    {
                        itemSpots.Add(new Point(i, j));
                        distToItemsMap[i * width + j] = 0;
                    }
                    else
                        distToItemsMap[i * width + j] = dijkstraDefaultVal;
                }
            
            DijkstraNeighbors(itemSpots, 1, ref distToItemsMap, Program.WorldMap.LocalTile.Blocks, new Point(width, height));
        }

        public static void DijkstraNeighbors(HashSet<Point> spots, int dist, ref int[] dijkstraMap, Block[] map, Point size, int maxDist = dijkstraDefaultVal)
        {
            int width = size.X, height = size.Y;
            if (dist > maxDist)
                return;
            HashSet<Point> neighbors = new HashSet<Point>();
            foreach (Point spot in spots)
            {
                for (int i = Math.Max(0, spot.X - 1); i <= Math.Min(spot.X + 1, width - 1); i++)
                    for (int j = Math.Max(0, spot.Y - 1); j <= Math.Min(spot.Y + 1, height - 1); j++)
                    {
                        Point point = new Point(i, j);
                        bool blockIsPassable = !(map[i * width + j].Solid && map[i * width + j].Type != BlockType.Door);
                        if ((dijkstraMap[point.X * width + point.Y] > dist) && blockIsPassable)
                        {
                            neighbors.Add(point);
                            dijkstraMap[point.X * width + point.Y] = dist;
                        }
                    }
            }
            if (neighbors.Count > 0)
                DijkstraNeighbors(neighbors, dist + 1, ref dijkstraMap, map, new Point(width, height), maxDist);
        }


        //eventFuncs
        public void CallPlayerMoved(object sender)
        {
            OnPlayerMoved(sender, EventArgs.Empty);
        }

        public void CallItemPosChanged(object sender)
        {
            OnItemMoved(sender, EventArgs.Empty);
        }


        //Gets
        public int GetLowestNeighborVal(Point position, int[] dijkstraMap)
        {
            int lowestVal = dijkstraDefaultVal;

            for (int i = Math.Max(0, position.X - 1); i <= Math.Min(width - 1, position.X + 1); i++)
                for (int j = Math.Max(0, position.Y - 1); j <= Math.Min(height - 1, position.Y + 1); j++)
                    if (new Point(i, j).Equals(position) == false )
                        lowestVal = ( dijkstraMap[ i * width + j ] < lowestVal )  ?  dijkstraMap[ i * width + j ]  :  lowestVal;

            return lowestVal;
        }

        public Point GetLowestValNeighbor(Point position, int[] dijkstraMap)
        {
            MapTile map = Program.WorldMap.LocalTile;
            int lowestVal = dijkstraMap[position.X * width + position.Y];
            List<Point> neighbors = new List<Point>();
            List<Point> lowestPoints = new List<Point>();
            for (int i = position.X - 1; i <= position.X + 1; i++)
                for (int j = position.Y - 1; j <= position.Y + 1; j++)
                    if (new Point(i, j).Equals(position) == false && (dijkstraMap[i * width + j] <= lowestVal))
                    {
                        lowestVal = dijkstraMap[i * width + j];
                        neighbors.Add(new Point(i, j));
                    }

            foreach (Point point in neighbors)
                if (dijkstraMap[point.X * width + point.Y] == lowestVal && map.Blocks[point.X * Program.WorldMap.LocalTile.Width + point.Y] is Monster == false)
                    lowestPoints.Add(point);

            if (lowestPoints.Count != 0)
                return lowestPoints[Program.RNG.Next(0, lowestPoints.Count)];
            return position;
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

        public int[] DistToPlayerMap
        {
            get { return distToPlayerMap; }
            set { distToPlayerMap = value; }
        }

        public int[] DistToItemsMap
        {
            get { return distToItemsMap; }
            set { distToItemsMap = value; }
        }
    }
}