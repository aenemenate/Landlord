using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class MapTile
    {
        private int width, height;
        private Point worldIndex;
        private Block[] map; // x * width + y
        private Block[] memoryMap; // x * width + y
        private Tile[] floor; // x * width + y
        private List<Creature> creatures;
        private DijkstraMaps dijkstraMaps;

        private bool owned;
        private float cost;
        private int sqMeters;

        private Point dungeonEntrance;
        private Dungeon dungeon;

        // CONSTRUCTOR //

		public MapTile(Point size, Point worldIndex, float[,] heightMap, List<string> plantTypes, List<string> creatureTypes, bool containsDungeon)
        {
            width = size.X;
            height = size.Y;
            this.worldIndex = worldIndex;
            map = new Block[width * height];
            memoryMap = new Block[width * height];
            floor = new Tile[width * height];
            creatures = new List<Creature>();
            dijkstraMaps = new DijkstraMaps(width, height);
            Init(new Random(), worldIndex, heightMap, plantTypes, creatureTypes, containsDungeon);
            owned = false;
        }

        public MapTile()
        {
        }

        // FUNCTIONS //
        public void Init(Random rng, Point worldIndex, float[,] heightMap, List<string> plantTypes, List<string> creatureTypes, bool containsDungeon)
        {
            WorldMapGeneration.GenerateForestMap(rng, this, plantTypes, creatureTypes, worldIndex, heightMap);
            if (containsDungeon == true)
                WorldMapGeneration.GenerateDungeonEntrance(this);
            DetermineCost();
        }
        public void DetermineCost()
        {
            cost = 0;
            sqMeters = 0;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++) {
                    if (floor[i * width + j] is DirtFloor && map[i * width + j].Solid == false) {
                        cost += .5F;
                        sqMeters += 1;
                    }
                    if (map[i * width + j] is DownStair)
                        cost += 1000F;
                }
        }
        public void DrawCell(int x, int y)
        {
            Point startPoint = Program.Window.CalculateMapStartPoint();

            int currentFloor = Program.Player.CurrentFloor;
            Point worldIndex = Program.Player.WorldIndex;
            Block[] blocks = currentFloor >= 0 ? Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            Tile[] tiles = currentFloor >= 0 ? Dungeon.Floors[currentFloor].Floor : Program.WorldMap[worldIndex.X, worldIndex.Y].Floor;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            Color floorForeColor = tiles[x * width + y].ForeColor;
            Color floorBackColor = tiles[x * width + y].BackColor;

            float nonVisibleMultiplierFore, nonVisibleMultiplierBack, visibleMultiplierFore, visibleMultiplierBack, heightMultiplierFore, heightMultiplierBack;

            if (Program.Player.CurrentFloor >= 0 == true) {
                heightMultiplierFore = 0F;
                heightMultiplierBack = 0F;
            }
            else
            {
                float cellHeight = Program.WorldMap.HeightMap[x + 100 * Program.Player.WorldIndex.X, y + 100 * Program.Player.WorldIndex.Y];
                float playerHeight = Program.WorldMap.HeightMap[Program.Player.Position.X + 100 * Program.Player.WorldIndex.X, Program.Player.Position.Y + 100 * Program.Player.WorldIndex.Y];
                heightMultiplierFore = ((90F - cellHeight) / 128F) * 0.25F;
                heightMultiplierBack = ((90F - cellHeight) / 128F) * 0.1F;
            }

            nonVisibleMultiplierFore = 0.85F - heightMultiplierFore;
            nonVisibleMultiplierBack = 0.9F - heightMultiplierBack;
            visibleMultiplierFore = 0.95F - heightMultiplierFore;
            visibleMultiplierBack = 1F - heightMultiplierBack;

            void RenderFloorTile()
            {
                if (!tiles[x * width + y].Explored)
                    Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, tiles[x * width + y].Graphic, Color.Black, Color.Black);
                else
                    switch (tiles[x * width + y].Visible)
                    {
                        case (false):
                            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, tiles[x * width + y].Graphic, floorForeColor * nonVisibleMultiplierFore, floorBackColor * nonVisibleMultiplierBack);
                            break;
                        case (true):
                            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, tiles[x * width + y].Graphic, floorForeColor * visibleMultiplierFore, floorBackColor * visibleMultiplierBack);
                            break;
                    }
            }

            void RenderBlock()
            {
                Block block = blocks[x * width + y];
                Color blockForeColor = block.ForeColor;
                Color blockBackColor = block.BackColor;

                // this code changed the block color to match that of the tile underneath a creature/item
                if (block.BackColor == Color.Pink)
                {
                    if (block.Type == BlockType.Creature)
                    {
                        Creature creature = (Creature)block;
                        bool creatureIsOnColoredBlock = creature != null && creature.CurrentBlock.Type != BlockType.Empty && creature.CurrentBlock.BackColor != Color.Pink;
                        blockBackColor = creatureIsOnColoredBlock ? creature.CurrentBlock.BackColor : floorBackColor;
                    }
                    else
                        blockBackColor = floorBackColor;
                }
                // this code prevents a creature you can't see from being rendered
                bool blockIsUnseenCreature = block is Creature && block.Visible == false;
                if (blockIsUnseenCreature)
                {
                    Creature creature = (Creature)block;
                    if (creature.Alive)
                    {
                        bool creatureIsInEmptySpace = creature.CurrentBlock.Type == BlockType.Empty;
                        if (creatureIsInEmptySpace)
                        {
                            RenderFloorTile();
                            return;
                        }
                        else
                            block = creature.CurrentBlock;
                    }
                }
                if (block.Explored == false)
                        Program.Console.SetGlyph( x - startPoint.X, y - startPoint.Y, block.Graphic, Color.Black, Color.Black );
                else
                    switch (block.Visible)
                    {
                        case (false):
                            if (memoryMap[x * width + y] == null)
                                Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, block.Graphic, blockForeColor * nonVisibleMultiplierFore, blockBackColor * nonVisibleMultiplierBack);
                            else
                                Program.Console.SetGlyph( x - startPoint.X, y - startPoint.Y, memoryMap[x * width + y].Graphic, memoryMap[x * width + y].ForeColor * nonVisibleMultiplierFore, floorBackColor * nonVisibleMultiplierBack );
                            break;
                        case (true):
                            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, block.Graphic, blockForeColor * visibleMultiplierFore, blockBackColor * visibleMultiplierBack);
                            break;
                    }
            }


            if (blocks[x * width + y].Type == BlockType.Empty)
                RenderFloorTile();
            else
                RenderBlock();
        }

        public void UpdatePlants(int plantsInterval)
        {
            Random rng = new Random();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    if (Blocks[i * width + j] is Plant p && plantsInterval % p.GrowthInterval == 0)
                        p.Grow(this, new Point(i, j), rng);
        }
        // check functions
        internal bool PointWithinBounds(Point point)
        {
            if (point.X >= 0 && point.X < Width)
                if (point.Y >= 0 && point.Y < Height)
                    return true;
            return false;
        }
        internal bool PointOnEdge(Point point)
        {
            if (point.X == 0 || point.X == Width - 1 || point.Y == 0 || point.Y == Height - 1)
                return true;
            return false;
        }
        internal bool AdjacentBlocksEmpty(Point point)
        {
            if (PointWithinBounds(new Point(point.X + 1, point.Y)) && !map[(point.X + 1) * width + point.Y].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X - 1, point.Y)) && !map[(point.X - 1) * width + point.Y].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X, point.Y + 1)) && !map[point.X * width + point.Y + 1].Solid)
                return true;
            if (PointWithinBounds(new Point(point.X, point.Y - 1)) && !map[point.X * width + point.Y - 1].Solid)
                return true;
            return false;
        }
        internal bool PointIsNextToWater(Point point)
        {
            int maxX = Math.Min(height - 1, point.X + 1), maxY = Math.Min(width - 1, point.Y + 1);
            int minX = Math.Max(0, point.X - 1), minY = Math.Max(0, point.Y - 1);

            for (int i = minX; i <= maxX; i++)
                for (int j = minY; j <= maxY; j++)
                    if (!point.Equals(new Point(i, j)) && floor[i * width + j] is Water)
                        return true;

            return false;
        }
        // get functions
        internal Point GetClosestOfTileTypeToPos( Point pos, Tile tileType  )
        {
            Point closestPoint = new Point();
            double nearestDist = 100000;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++) {
                    double dist = new Point( i, j ).DistFrom( pos );
                    if (floor[i * width + j].Name.Equals(tileType.Name) && dist < nearestDist) {
                        nearestDist = dist;
                        closestPoint = new Point( i, j );
                    }
                }
            return closestPoint;
        }
        internal Point GetClosestOfBlockTypeToPos(Point pos, BlockType blockType)
        {
            Point closestPoint = new Point();
            double nearestDist = 100000;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++) {
                    double dist = new Point(i, j).DistFrom(pos);
                    if (map[i * width + j].Type == blockType && dist < nearestDist) {
                        nearestDist = dist;
                        closestPoint = new Point(i, j);
                    }
                }
            return closestPoint;
        }
        internal List<Point> GetAllDirtTiles()
        {
            List<Point> grassTiles = new List<Point>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    if (Floor[i * width + j] is DirtFloor)
                        grassTiles.Add(new Point(i, j));
                }
            return grassTiles;
        }
        internal Point GetMousePos(Point mousePos)
        {
            return new Point(Program.Window.CalculateMapStartPoint().X + (mousePos.X), Program.Window.CalculateMapStartPoint().Y + mousePos.Y);
        }

        // PROPERTIES //
        public Block this[int x, int y] {
            get { return map[x * width + y]; }
            set { map[x * width + y] = value; }
        }
        public Point WorldIndex {
            get { return worldIndex; }
            set { worldIndex = value; }
        }
        public Block[] Blocks {
            get { return map; }
            set { map = value; }
        }
        public Block[] MemoryMap {
            get { return memoryMap; }
            set { memoryMap = value; }
        }
        public Tile[] Floor {
            get { return floor; }
            set { floor = value; }
        }
        public int Width {
			get { return width; }
			set { width = value; }
        }
        public int Height {
            get { return height; }
			set { height = value; }
        }
        public bool Owned {
            get { return owned; }
            set { owned = value; }
        }
        public float Cost {
            get { return cost; }
            set { cost = value; }
        }
        public int SqMeters {
            get { return sqMeters; }
            set { sqMeters = value; }
        }
        public List<Creature> Creatures {
            get { return creatures; }
            set { creatures = value; }
        }
        public DijkstraMaps DijkstraMaps {
            get { return dijkstraMaps; }
            set { dijkstraMaps = value; }
        }
        public Point DungeonEntrance {
            get { return dungeonEntrance; }
            set { dungeonEntrance = value; }
        }
        public Dungeon Dungeon {
            get { return dungeon; }
            set { dungeon = value; }
        }
    }
}
