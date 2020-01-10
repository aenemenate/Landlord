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
        private int width, height;
        private List<string> creatureTypes;
        private List<string> plantTypes;
        private MapTile[,] worldMap;
        private float[,] heightMap;
        private int seed;
        private string name;

        private int updateInterval; // how many times updates, used to modulo for certain events
        private int completedUpdateHour;


        // CONSTRUCTORS //

        public WorldMap (int width, int height, int tilewidth, int tileheight, string name, int seed = 0)
        {
            Random rng = new Random();
            if (seed == 0)
                this.seed = rng.Next(1, int.MaxValue);
            else
                this.seed = seed;

            this.width = width;
            this.height = height;
            tileWidth = tilewidth;
            tileHeight = tileheight;
            this.name = name;
            updateInterval = 0;
            completedUpdateHour = 8;

            //GenerateWorldMap(width, height, name);
        }

        public WorldMap()
        {
        }


        // FUNCTIONS //

        public void GenerateWorldMap()
        {
            worldMap = new MapTile[width, height];
            creatureTypes = DataReader.GetOverworldCreatureList();
            plantTypes = DataReader.GetPlantList();

            Random rng = new Random(seed);
            SimplexNoise.Seed = this.seed;
            float scale = .005f;

            heightMap = SimplexNoise.Calc2D(tileWidth * width, tileHeight * height, scale);
            List<Point> potentialDungeons = new List<Point>();
            List<Point> dungeons = new List<Point>();
            bool dungeon = false;

            WorldMapGeneration.GenerateRivers( rng, width, height, tileWidth, tileHeight, heightMap );

            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    potentialDungeons.Add(new Point(i, j));
            for (int c = 0; c < 8; c++) {
                int index = rng.Next(0, potentialDungeons.Count);
                dungeons.Add(potentialDungeons[index]);
                potentialDungeons.RemoveAt(index);
            }

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    foreach (Point point in dungeons)
                        if (point.X == i && point.Y == j)
                            dungeon = true;
                    worldMap[i, j] = new MapTile(new Point(tileWidth, tileHeight), new Point(i, j), heightMap, plantTypes, creatureTypes, dungeon);
                    dungeon = false;
                }
            }
            //for (int i = 1; i < 499; i++)
            //    for (int j = 1; j < 499; j++) {
            //        Point worldIndex = new Point( i / 100, j / 100 ), tilePosition = new Point( i % 100, j % 100 );
            //        worldMap[worldIndex.X, worldIndex.Y].Floor[tilePosition.X * 100 + tilePosition.Y].Explored = true;
            //        worldMap[worldIndex.X, worldIndex.Y].Blocks[tilePosition.X * 100 + tilePosition.Y].Explored = true;
            //    }
            OnFinishedGenerating(this, EventArgs.Empty);
        }
        public void UpdatePlants()
        {
            for (int i = 0; i < worldMap.GetLength(0); i++)
                for (int j = 0; j < worldMap.GetLength(1); j++)
                    worldMap[i, j].UpdatePlants(updateInterval);
            updateInterval++;
            completedUpdateHour = Program.TimeHandler.CurrentTime.Hour;
        }
        public void CleanSplatters()
        {
            for (int i = 0; i < worldMap.GetLength(0); i++)
                for (int j = 0; j < worldMap.GetLength(1); j++)
                    worldMap[i, j].CleanSplatters();
        }

        // PROPERTIES //
        public MapTile this[int x, int y] {
            get { return worldMap[x,y]; }
            set { worldMap[x,y] = value; }
        }
        public MapTile[,] WorldTiles {
            get { return worldMap; }
            set { worldMap = value; }
        }
        public int TileWidth {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        public int TileHeight {
            get { return tileHeight; }
            set { tileHeight = value; }
        }
        public int Width {
            get { return width; }
            set { width = value; }
        }
        public int Height {
            get { return height; }
            set { height = value; }
        }
        public List<string> CreatureTypes {
            get { return creatureTypes; }
            set { creatureTypes = value; }
        }
        public List<string> PlantTypes {
            get { return plantTypes; }
            set { plantTypes = value; }
        }
        public float[,] HeightMap {
            get { return heightMap; }
            set { heightMap = value; }
        }
        public int Seed {
            get { return seed; }
            set { seed = value; }
        }
        public string Name {
            get { return name; ; }
            set { name = value; }
        }
        public int UpdateInterval {
            get { return updateInterval; }
            set { updateInterval = value; }
        }
        public int CompletedUpdateHour {
            get { return completedUpdateHour; }
            set { completedUpdateHour = value; }
        }
    }
}
