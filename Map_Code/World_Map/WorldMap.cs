using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polenter.Serialization;

namespace Landlord
{
    class WorldMap
    {
        public event EventHandler<EventArgs> OnFinishedGenerating;

        private int tileWidth, tileHeight;
        private MapTile[,] worldMap;
        private float[,] heightMap;
        private int seed;
        private string name;


        // CONSTRUCTORS //

        public WorldMap (int width, int height, string name, int seed = 0)
        {
            Random rng = new Random();
            if (seed == 0)
                this.seed = rng.Next(1, int.MaxValue);
            else
                this.seed = seed;

            this.tileWidth = width;
            this.tileHeight = height;
            this.name = name;

            //GenerateWorldMap(width, height, name);
        }

        public WorldMap()
        {
        }


        // FUNCTIONS //

        public void GenerateWorldMap()
        {
            worldMap = new MapTile[5, 5];

            Random rng = new Random(seed);
            SimplexNoise.Seed = this.seed;
            float scale = .005f;

            heightMap = SimplexNoise.Calc2D(tileWidth * 5, tileHeight * 5, scale);
            List<Point> potentialDungeons = new List<Point>();
            List<Point> dungeons = new List<Point>();
            bool dungeon = false;

            WorldMapGeneration.GenerateRivers( rng, tileWidth, tileHeight, heightMap );

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    potentialDungeons.Add(new Point(i, j));
            for (int c = 0; c < 10; c++) {
                int index = rng.Next(0, potentialDungeons.Count);
                dungeons.Add(potentialDungeons[index]);
                potentialDungeons.RemoveAt(index);
            }

            for (int i = 0; i < 5; i++) {
                for (int j = 0; j < 5; j++) {
                    foreach (Point point in dungeons)
                        if (point.X == i && point.Y == j)
                            dungeon = true;
                    worldMap[i, j] = new MapTile(new Point(tileWidth, tileHeight), new Point(i, j), heightMap, dungeon);
                    dungeon = false;
                }
            }
            //for (int i = 1; i < 499; i++)
            //    for (int j = 1; j < 499; j++)
            //    {
            //        Point worldIndex = new Point( i / 100, j / 100 ), tilePosition = new Point( i % 100, j % 100 );
            //        worldMap[worldIndex.X, worldIndex.Y].Floor[tilePosition.X * 100 + tilePosition.Y].Explored = true;
            //        worldMap[worldIndex.X, worldIndex.Y].Blocks[tilePosition.X * 100 + tilePosition.Y].Explored = true;
            //    }
            OnFinishedGenerating(this, EventArgs.Empty);
        }

        // PROPERTIES //

        public MapTile this[int x, int y]
        {
            get { return worldMap[x,y]; }
            set { worldMap[x,y] = value; }
        }
        public MapTile[,] WorldTiles { // DO NOT DELETE! Used for SERIALIZATION.
            get { return worldMap; }
            set { worldMap = value; }
        }
        public int TileWidth {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        public int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value; }
        }
        public float[,] HeightMap
        {
            get { return heightMap; }
            set { heightMap = value; }
        }
        public string Name
        {
            get { return name; ; }
            set { name = value; }
        }
    }
}
