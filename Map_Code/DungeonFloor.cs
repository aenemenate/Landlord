using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    enum DungeonType
    {
        Cave_s,
        Cave_l,
        RoomPlacement_s,
        RoomPlacement_l
    }
    class DungeonFloor
    {

        private int width, height;
        private Block[] map;
        private Block[] memoryMap;
        private Tile[] floor;
        private List<Creature> creatures;
        private List<Projectile> projectiles;
        private PatrolMaps patrolMaps;
        private List<Point> patrolPoints;

        public DungeonFloor(Point size)
        {
            width = size.X;
            height = size.Y;
            map = new Block[width * height];
            memoryMap = new Block[width * height];
            floor = new Tile[width * height];
            creatures = new List<Creature>();
            projectiles = new List<Projectile>();
            patrolPoints = new List<Point>();
        }
        public DungeonFloor()
        {

        }

        public void Init(DungeonType dungeonType, List<string> monsterTypes, Point worldIndex, int floor)
        {
            RoomPlacementAlgorithm roomPlacementAlgo;
            switch (dungeonType)
            {
                case (DungeonType.RoomPlacement_s):
                    roomPlacementAlgo = new RoomPlacementAlgorithm(this, monsterTypes);
                    roomPlacementAlgo.GenerateDungeon(250, 20, floor, worldIndex);
                    break;
                case (DungeonType.RoomPlacement_l):
                    roomPlacementAlgo = new RoomPlacementAlgorithm(this, monsterTypes );
                    roomPlacementAlgo.GenerateDungeon(500, 40, floor, worldIndex);
                    break;
                default:
                    roomPlacementAlgo = new RoomPlacementAlgorithm(this, monsterTypes );
                    roomPlacementAlgo.GenerateDungeon(250, 15, floor, worldIndex);
                    break;
            }
            patrolMaps = new PatrolMaps(100, 100, patrolPoints, this);
        }
        public bool PointWithinBounds(Point point)
        {
            if (point.X >= 0 && point.X < Width)
                if (point.Y >= 0 && point.Y < Height)
                    return true;
            return false;
        }
        public bool PointOnEdge(Point point)
        {
            if (point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1)
                return true;
            return false;
        }
        public bool AdjacentBlockEmpty(Point point)
        {
            if (PointWithinBounds(new Point(point.X + 1, point.Y)) && !map[point.X + 1 * Program.WorldMap.TileWidth + point.Y].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X - 1, point.Y)) && !map[point.X - 1 * Program.WorldMap.TileWidth + point.Y].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X, point.Y + 1)) && !map[point.X * Program.WorldMap.TileWidth + point.Y + 1].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X, point.Y - 1)) && !map[point.X * Program.WorldMap.TileWidth + point.Y - 1].Solid)
                return true;
            return false;
        }
        public Point GetDownStairPos()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    if (map[i * width + j] is DownStair)
                        return new Point(i, j);
            return new Point();
        }

        // ACCESSORS //
        public Block this[int x, int y]
        {
            get { if (!new Point(x, y).Equals(new Point())) return map[x * Program.WorldMap.TileWidth + y]; else return null; }
            set { map[x * Program.WorldMap.TileWidth + y] = value; }
        }
        public Block[] Blocks
        {
            get { return map; }
            set { map = value; }
        }
        public Block[] MemoryMap
        {
            get { return memoryMap; }
            set { memoryMap = value; }
        }
        public Tile[] Floor
        {
            get { return floor; }
            set { floor = value; }
        }
        public List<Creature> Creatures
        {
            get { return creatures; }
            set { creatures = value; }
        }
        public List<Projectile> Projectiles
        {
            get { return projectiles; }
            set { projectiles = value; }
        }
        public PatrolMaps PatrolMaps
        {
            get { return patrolMaps; }
            set { patrolMaps = value; }
        }
        public List<Point> PatrolPoints
        {
            get { return patrolPoints; }
            set { patrolPoints = value; }
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
    }
}
