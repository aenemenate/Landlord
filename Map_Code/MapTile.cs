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

        private int currentFloor;
        private bool inDungeon;
        private bool loading;


        // CONSTRUCTOR //

		public MapTile(Point size, Point worldIndex, float[,] heightMap, bool containsDungeon)
        {
            width = size.X;
            height = size.Y;
            map = new Block[width * height];
            memoryMap = new Block[width * height];
            floor = new Tile[width * height];
            creatures = new List<Creature>();
            dijkstraMaps = new DijkstraMaps(width, height);
            Init(worldIndex, heightMap, containsDungeon);
            owned = false;
            inDungeon = false;
            loading = false;
            currentFloor = -1;
        }

        public MapTile()
        {
        }


        // FUNCTIONS //
        // init functions
        public void Init(Point worldIndex, float[,] heightMap, bool containsDungeon)
        {
            WorldMapGeneration.GenerateForestMap(this, worldIndex, heightMap);
            if (containsDungeon == true)
                WorldMapGeneration.GenerateDungeonEntrance(this);
            DetermineCost();
        }

        public void DetermineCost()
        {
            cost = 0;
            sqMeters = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (!(floor[i * width + j] is Water) && map[i * width + j] is Air)
                    {
                        cost += .5F;
                        sqMeters += 1;
                    }
                    if (map[i * width + j] is DownStair)
                        cost += 1000F;
                }
            }
        }

        // draw function
        public void DrawCell(int x, int y)
        {
            Point startPoint = Program.Window.CalculateMapStartPoint();
            Color floorForeColor = Floor[x * width + y].ForeColor;
            Color floorBackColor = Floor[x * width + y].BackColor;

            float nonVisibleMultiplierFore, nonVisibleMultiplierBack, visibleMultiplierFore, visibleMultiplierBack, heightMultiplierFore, heightMultiplierBack;

            if (inDungeon == true)
            {
                heightMultiplierFore = 0F;
                heightMultiplierBack = 0F;
            }
            else
            {
                float cellHeight = Program.WorldMap.HeightMap[x + 100 * Program.WorldMap.WorldIndex.X, y + 100 * Program.WorldMap.WorldIndex.Y];
                float playerHeight = Program.WorldMap.HeightMap[Program.Player.Position.X + 100 * Program.WorldMap.WorldIndex.X, Program.Player.Position.Y + 100 * Program.WorldMap.WorldIndex.Y];
                heightMultiplierFore = ((90F - cellHeight) / 128F) * 0.25F;
                heightMultiplierBack = ((90F - cellHeight) / 128F) * 0.1F;
            }

            nonVisibleMultiplierFore = 0.85F - heightMultiplierFore;
            nonVisibleMultiplierBack = 0.9F - heightMultiplierBack;
            visibleMultiplierFore = 0.95F - heightMultiplierFore;
            visibleMultiplierBack = 1F - heightMultiplierBack;
            

            void RenderFloorTile()
            {
                if (!Floor[x * width + y].Explored)
                    Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, Floor[x * width + y].Graphic, Color.Black, Color.Black);
                else
                    switch (Floor[x * width + y].Visible)
                    {
                        case (false):
                            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, Floor[x * width + y].Graphic, floorForeColor * nonVisibleMultiplierFore, floorBackColor * nonVisibleMultiplierBack);
                            break;
                        case (true):
                            Program.Console.SetGlyph(x - startPoint.X, y - startPoint.Y, Floor[x * width + y].Graphic, floorForeColor * visibleMultiplierFore, floorBackColor * visibleMultiplierBack);
                            break;
                    }
            }

            void RenderBlock()
            {
                Block block = Blocks[x * width + y];
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
                {
                        Program.Console.SetGlyph( x - startPoint.X, y - startPoint.Y, block.Graphic, Color.Black, Color.Black );
                    
                } else
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


            if (Blocks[x * width + y].Type == BlockType.Empty)
                RenderFloorTile();
            else
                RenderBlock();
        }
        
        // get functions
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

        internal List<Point> GetEmptyAdjacentBlocks(Point point)
        {
            List<Point> emptyBlocks = new List<Point>();

            int maxX = Math.Min(height - 1, point.X + 1), maxY = Math.Min(width - 1, point.Y + 1);
            int minX = Math.Max(0, point.X - 1), minY = Math.Max(0, point.Y - 1);

            for (int i = minX; i <= maxX; i++)
                for (int j = minY; j <= maxY; j++)
                    if (!point.Equals(new Point(i, j)) && this[i, j].Solid == false && this[i, j] is Item == false)
                        emptyBlocks.Add(new Point(i, j));

            return emptyBlocks;
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

        internal Creature GetCreatureAtPosition(Point position)
        {
            // Program.Player is the actual player, this is just a placeholder
            if (Program.Player.Position.Equals(position))
                return Program.Player;

            foreach (Creature creature in Creatures)
                if (creature.Position.Equals(position))
                    return creature;
            return null;
        }

        internal Point GetClosestOfBlockTypeToPos(Point pos, BlockType blockType, Material material = Material.Null)
        {
            Point closestPoint = new Point();
            double nearestDist = 100000;
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++) {
                    double dist = new Point(i, j).DistFrom(pos);
                    if (Program.WorldMap.LocalTile[i, j].Type == blockType  && (material == Material.Null || material == Program.WorldMap.LocalTile[i, j].Material) && dist < nearestDist) {
                        nearestDist = dist;
                        closestPoint = new Point(i, j);
                    }
                }
            return closestPoint;
        }

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

        internal List<Point> GetAllGrassTiles()
        {
            List<Point> grassTiles = new List<Point>();
            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                {
                    if (Floor[i * width + j] is Grass)
                        grassTiles.Add(new Point(i, j));
                }
            return grassTiles;
        }

        internal Point GetMousePos(Point mousePos)
        {
            return new Point(Program.Window.CalculateMapStartPoint().X + (mousePos.X), Program.Window.CalculateMapStartPoint().Y + mousePos.Y);
        }

        
        // PROPERTIES //

        public Block this[int x, int y]
        {
            get { if (!inDungeon || loading) return map[x * width + y]; else return dungeon.Floors[currentFloor][x, y]; }
            set { if (!inDungeon || loading) map[x * width + y] = value; else dungeon.Floors[currentFloor][x, y] = value; }
        }

        public Block[] Blocks
        {
            get { if (!inDungeon || loading) return map; else return dungeon.Floors[currentFloor].Blocks; }
            set { if (!inDungeon || loading) map = value; else dungeon.Floors[currentFloor].Blocks = value; }
        }

        public Block[] MemoryMap
        {
            get { if (!inDungeon || loading) return memoryMap; else return dungeon.Floors[currentFloor].MemoryMap; }
            set { if (!inDungeon || loading) memoryMap = value; else dungeon.Floors[currentFloor].MemoryMap = value; }
        }

        public Tile[] Floor
        {
            get { if (!inDungeon || loading) return floor; else return dungeon.Floors[currentFloor].Floor; }
            set { if (!inDungeon || loading) floor = value; else dungeon.Floors[currentFloor].Floor = value; }
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

        public bool Owned
        {
            get { return owned; }
            set { owned = value; }
        }

        public float Cost
        {
            get { return cost; }
            set { cost = value; }
        }

        public int SqMeters
        {
            get { return sqMeters; }
            set { sqMeters = value; }
        }

        public List<Creature> Creatures
        {
            get { if (!inDungeon || loading) return creatures; else return dungeon.Floors[currentFloor].Creatures; }
            set { if (!inDungeon || loading) creatures = value; else dungeon.Floors[currentFloor].Creatures = value; }
        }

        public DijkstraMaps DijkstraMaps
        {
            get { return dijkstraMaps; }
            set { dijkstraMaps = value; }
        }

        public Point DungeonEntrance
        {
            get { return dungeonEntrance; }
            set { dungeonEntrance = value; }
        }

        public bool InDungeon
        {
            get { return inDungeon; }
            set { inDungeon = value; }
        }

        public bool Loading
        {
            get { return loading; }
            set { loading = value; }
        }

        public int CurrentFloor
        {
            get { return currentFloor; }
            set { currentFloor = value; }
        }

        public Dungeon Dungeon
        {
            get { return dungeon; }
            set { dungeon = value; }
        }
    }
}
