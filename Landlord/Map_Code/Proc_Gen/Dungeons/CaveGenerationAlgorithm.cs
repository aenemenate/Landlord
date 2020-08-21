using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class CaveGenerationAlgorithm : DungeonGenerationAlgorithm
    {
        public CaveGenerationAlgorithm(DungeonFloor dungeonFloor, List<string> monsterTypes) : base(dungeonFloor, monsterTypes) { }

        public override void GenerateDungeon(int size, int floor, Point worldIndex)
        {
            firstRoomPos = floor > 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[floor - 1].GetDownStairPos() : Program.Player.Position;

            DungeonHelper.InitializeMap(dungeonFloor);

            // fill the map with random blocks
            for (int i = Math.Max(0, firstRoomPos.X - size/2); i < Math.Min(dungeonFloor.Width, firstRoomPos.X + size / 2); i++)
                for (int j = Math.Max(0, firstRoomPos.Y - size / 2); j < Math.Min(dungeonFloor.Height, firstRoomPos.Y + size / 2); j++)
                {
                    if ((rng.Next(0, 100) < 40 && (new Point(i, j)).DistFrom(firstRoomPos) > 5))
                        dungeonFloor.Blocks[i * dungeonFloor.Width + j] = new Wall(Material.Stone);
                    else
                        dungeonFloor.Blocks[i * dungeonFloor.Width + j] = new Air();
                    dungeonFloor.Floor[i * Program.WorldMap.TileWidth + j] = new DirtFloor();
                }
            for (int i = 0; i < 3; i++)
                DoAutomataStep();

            for (int i = 0; i < 15; i++)
            {
                Point pos = new Point(rng.Next(Math.Max(0, firstRoomPos.X - size / 2), Math.Min(dungeonFloor.Width, firstRoomPos.X + size / 2)), 
                    rng.Next(Math.Max(0, firstRoomPos.Y - size / 2), Math.Min(dungeonFloor.Height, firstRoomPos.Y + size / 2)));
                if (dungeonFloor[pos.X, pos.Y] is Air)
                    dungeonFloor.PatrolPoints.Add(pos);
            }

            DungeonHelper.PlaceStairs(dungeonFloor, firstRoomPos);
            DungeonHelper.PlaceItemsAndChests(dungeonFloor, firstRoomPos);
            DungeonHelper.PlaceCreatures(dungeonFloor, firstRoomPos, monsterTypes, worldIndex, floor);

            for (int i = 0; i < dungeonFloor.Width; i++)
                for (int j = 0; j < dungeonFloor.Height; j++) {
                    if (dungeonFloor.Blocks[i * dungeonFloor.Width + j] == null)
                        dungeonFloor.Blocks[i * dungeonFloor.Width + j] = new Air();
                }
        }
        private void DoAutomataStep()
        {
            bool[] map = new bool[dungeonFloor.Width * dungeonFloor.Height];
            for (int x = 0; x < dungeonFloor.Width; x++)
                for (int y = 0; y < dungeonFloor.Height; y++) {
                    map[x * dungeonFloor.Width + y] = dungeonFloor[x, y].Solid;
                }
            for (int x = 0; x < dungeonFloor.Width; x++)
            {
                for (int y = 0; y < dungeonFloor.Height; y++)
                {
                    int nbs = CountAliveNeighbours(x, y);
                    //The new value is based on our simulation rules
                    //First, if a cell is alive but has too few neighbours, kill it.
                    if (dungeonFloor[x, y].Opaque)
                    {
                        if (nbs < 3)
                            map[x * dungeonFloor.Width + y] = false;
                        else
                            map[x * dungeonFloor.Width + y] = true;
                    } //Otherwise, if the cell is dead now, check if it has the right number of neighbours to be 'born'
                    else
                    {
                        if (nbs > 4)
                            map[x * dungeonFloor.Width + y] = true;
                        else
                            map[x * dungeonFloor.Width + y] = false;
                    }
                }
            }
            for (int x = 0; x < dungeonFloor.Width; x++)
                for (int y = 0; y < dungeonFloor.Height; y++) {
                    if (map[x * dungeonFloor.Width + y] && !dungeonFloor[x, y].Solid)
                        dungeonFloor[x, y] = new Wall(Material.Stone);
                    else if(!map[x * dungeonFloor.Width + y] && dungeonFloor[x, y].Solid)
                        dungeonFloor[x, y] = new Air();
                }
        }

        private int CountAliveNeighbours(int x, int y)
        {
            int count = 0;
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int neighbour_x = x + i;
                    int neighbour_y = y + j;
                    //If we're looking at the middle point
                    if (i == 0 && j == 0)
                    {
                        //Do nothing, we don't want to add ourselves in!
                    }
                    //In case the index we're looking at it off the edge of the map
                    else if (neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= dungeonFloor.Width || neighbour_y >= dungeonFloor.Height)
                    {
                        count = count + 1;
                    }
                    //Otherwise, a normal check of the neighbour
                    else if (dungeonFloor[neighbour_x, neighbour_y].Solid)
                    {
                        count = count + 1;
                    }
                }
            }
            return count;
        }
    }
}
