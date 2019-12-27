using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    public enum Direction
    {
        South = 0,
        West = 1,
        East = 2,
        North = 3,
    }

    public enum RoomType
    {
        bedroom,
        medicalroom,
        trainingroom,
        lootstore,
        dininghall
    }

    public enum Feature
    {
        Corridor,
        Room
    }

    class RoomPlacementAlgorithm : DungeonGenerationAlgorithm
    {
        private List<Block[,]> rooms = new List<Block[,]>();

        private int cavernChance = 45, rectChance = 10, crossChance = 30;
        private int maxRoomPlaceAttempts = 10;
        private int minCrossRoomSize = 6, maxCrossRoomSize = 12, maxTunnelLength = 12;
        private int minCellularAutomataTiles = 16, maxCellularAutomataSize = 18, wallProbability = 45;


        // CONSTRUCTOR

        public RoomPlacementAlgorithm(DungeonFloor dungeonFloor, List<string> monsterTypes) : base(dungeonFloor, monsterTypes)
        {
        }


        // FUNCTIONS

        public override void GenerateDungeon(int size, int floor, Point worldIndex)
        {
            DungeonHelper.InitializeMap(dungeonFloor);
            Block[,] room;

            firstRoomPos = floor > 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[floor - 1].GetDownStairPos() : Program.Player.Position;

            for (int c = 0; c < size * 100; c++) /* create all rooms */ {
                if (rooms.Count >= size)
                    break;
                room = GenerateRoom(); // generate the first room
                if (room != null)
                    PlaceRoom(room);
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
        private void PlaceRoom(Block[,] room)
        {
            int[] direction = new int[2];

            bool TunnelPlacementValid(Point start, int tunnelLength)
            {
                for (int i = 0; i <= tunnelLength + 1; i++)
                {
                    int x = start.X + direction[0] * i;
                    int y = start.Y + direction[1] * i;
                    if (dungeonFloor[x, y].Solid == false
                        || dungeonFloor[x + direction[1], y + direction[0]].Solid == false
                        || dungeonFloor[x - direction[1], y - direction[0]].Solid == false)
                        return false;
                }
                return true;
            }

            void SetRandomDirection()
            {
                int rand = rng.Next(0, 4);
                switch (rand)
                {
                    case (0):
                        direction[0] = 1;
                        direction[1] = 0;
                        break;
                    case (1):
                        direction[0] = -1;
                        direction[1] = 0;
                        break;
                    case (2):
                        direction[0] = 0;
                        direction[1] = 1;
                        break;
                    default:
                        direction[0] = 0;
                        direction[1] = -1;
                        break;
                }
            }

            if (rooms.Count == 0) {
                // place the first room. It can't go out of bounds if the player is on the edge of the map,
                // so place it so that it will touch the edge while containing the player
                Point tempPos = new Point(firstRoomPos.X - room.GetLength(0) / 2, firstRoomPos.Y - room.GetLength(1) / 2);
                if (tempPos.X + room.GetLength(0) > dungeonFloor.Width)
                    tempPos.X = (dungeonFloor.Width - 1) - room.GetLength(0) - 1;
                else if (tempPos.X < 0)
                    tempPos.X = 1;
                if (tempPos.Y + room.GetLength(1) > dungeonFloor.Height)
                    tempPos.Y = (dungeonFloor.Height - 1) - room.GetLength(1) - 1;
                else if (tempPos.Y < 0)
                    tempPos.Y = 1;
                AddRoom(room, tempPos);
            }
            else {
                int attempts = 0;
                while (attempts < maxRoomPlaceAttempts)
                {
                    SetRandomDirection();
                    Point potentialPos = ReturnRandomPositionWithFreeSpaceInDirection(direction);
                    Point startPos = new Point();
                    while (startPos.Equals(new Point()))
                    {
                        Point randRoomPos = new Point(rng.Next(0, room.GetLength(0)), rng.Next(0, room.GetLength(1)));
                        if (room[randRoomPos.X, randRoomPos.Y] is Air) {
                            startPos.X = potentialPos.X - randRoomPos.X;
                            startPos.Y = potentialPos.Y - randRoomPos.Y;
                        }
                    }
                    for (int tunnelLength = 0; tunnelLength < maxTunnelLength; tunnelLength++)
                    {
                        Point possibleRoomPos = new Point(startPos.X + direction[0] * tunnelLength, startPos.Y + direction[1] * tunnelLength);
                        if (RoomPlacementValid(possibleRoomPos, room) && TunnelPlacementValid(potentialPos, tunnelLength)) {
                            AddRoom(room, possibleRoomPos);
                            AddTunnel(potentialPos, tunnelLength, direction);
                            return;
                        }
                    }
                    attempts++;
                }
            }
        }
        private void AddTunnel(Point startPos, int tunnelLength, int[] direction)
        {
            int x = startPos.X;
            int y = startPos.Y;
            if (rng.Next(0, 100) < 40 && dungeonFloor[x + direction[1], y + direction[0]].Solid && dungeonFloor[x - direction[1], y - direction[0]].Solid)
                dungeonFloor[x, y] = new Door(Material.Wood);
            else
                dungeonFloor[x, y] = new Air();
            for (int i = 1; i <= tunnelLength * 2; i++)
            {
                x = startPos.X + direction[0] * i;
                y = startPos.Y + direction[1] * i;
                if (dungeonFloor[x, y].Solid == false) {
                    if (rng.Next(0, 100) < 40 && !(dungeonFloor[x - direction[0] * 2, y - direction[1] * 2] is Door)
                         && dungeonFloor[x + direction[1], y + direction[0]].Solid && dungeonFloor[x - direction[1], y - direction[0]].Solid)
                        dungeonFloor[x - direction[0], y - direction[1]] = new Door(Material.Wood);
                    break;
                }
                dungeonFloor[x, y] = new Air();
            }
        }
        private void AddRoom(Block[,] room, Point pos)
        {
            for (int i = 0; i < room.GetLength(0); i++)
                for (int j = 0; j < room.GetLength(1); j++)
                    dungeonFloor[i + pos.X, j + pos.Y] = room[i, j];

            rooms.Add(room);
            if (dungeonFloor[pos.X + room.GetLength(0) / 2, pos.Y + room.GetLength(1) / 2] is Air)
                dungeonFloor.PatrolPoints.Add(new Point(pos.X + room.GetLength(0) / 2, pos.Y + room.GetLength(1) / 2));
        }
        
        // room creation
        private Block[,] GenerateRoom()
        {
            int rand = rng.Next(0, 100);
            //if (rooms.Count == 0)
            //    if (rand < cavernChance)
            //        return GenerateCavern();
            if (rand < rectChance)
                return GenerateRectRoom();
            else if (rand < rectChance + crossChance)
                return GenerateCrossRoom();
            else if (rooms.Count != 0)
                return GenerateAutomataRoom();
            else return null;
        }
        private Block[,] GenerateAutomataRoom()
        {
            while (true)
            {
                Block[,] room = new Block[maxCellularAutomataSize, maxCellularAutomataSize];

                for (int i = 0; i < room.GetLength(0); i++)
                    for (int j = 0; j < room.GetLength(1); j++)
                    {
                        if (rng.Next(0, 100) >= wallProbability && i >= 2 && i < room.GetLength(0) - 2 && j >= 2 && j < room.GetLength(1) - 2)
                            room[i, j] = new Air();
                        else
                            room[i, j] = new Wall(Material.Stone);
                    }
                for (int c = 0; c < 3; c++)
                {
                    for (int i = 1; i < room.GetLength(0) - 1; i++)
                        for (int j = 1; j < room.GetLength(1) - 1; j++)
                        {
                            if (DungeonHelper.GetNumOfAdjacentWalls(new Point(i, j), room) > 4)
                                room[i, j] = new Wall(Material.Stone);
                            else if (DungeonHelper.GetNumOfAdjacentWalls(new Point(i, j), room) < 4)
                                room[i, j] = new Air();
                        }
                }
                room = FloodFill(room);
                for (int i = 0; i < room.GetLength(0); i++)
                    for (int j = 0; j < room.GetLength(1); j++)
                        if (room[i, j].Solid == false)
                            return room;
            }
        }
        private Block[,] FloodFill(Block[,] room)
        {
            bool ListContains(Point point, List<Point> list)
            {
                foreach (Point p in list)
                    if (p.Equals(point))
                        return true;
                return false;
            }
            Point roomSize = new Point(room.GetLength(0), room.GetLength(1));
            List<Point> largestRegion = new List<Point>();
            for (int x = 0; x < roomSize.X; x++)
            {
                for (int y = 0; y < roomSize.Y; y++)
                {
                    if (room[x,y].Solid == false)
                    {
                        List<Point> newRegion = new List<Point>();
                        Point tile = new Point(x, y);
                        List<Point> toBeFilled = new List<Point>() { tile };
                        while (toBeFilled.Any())
                        {
                            tile = toBeFilled[0];
                            toBeFilled.RemoveAt(0);
                            if (!ListContains(tile, newRegion))
                            {
                                newRegion.Add(tile);
                                room[tile.X, tile.Y] = new Wall(Material.Stone);
                                List<Point> directions = new List<Point>()
                                {
                                    new Point(tile.X, tile.Y + 1),
                                    new Point(tile.X, tile.Y - 1),
                                    new Point(tile.X + 1, tile.Y),
                                    new Point(tile.X - 1, tile.Y)
                                };
                                foreach (Point direction in directions)
                                    if (room[direction.X, direction.Y].Solid == false)
                                        if (!ListContains(direction, toBeFilled) && !ListContains(direction, newRegion))
                                            toBeFilled.Add(direction);
                            }
                        }
                        if (newRegion.Count >= minCellularAutomataTiles && newRegion.Count > largestRegion.Count)
                            largestRegion = newRegion;
                    }
                }
            }
            foreach (Point tile in largestRegion)
                room[tile.X, tile.Y] = new Air();

            return room;
        }
        private Block[,] GenerateCrossRoom()
        {
            int roomHorWidth = (rng.Next(minCrossRoomSize+ 2, maxCrossRoomSize));
            int roomVerHeight = (rng.Next(minCrossRoomSize + 2, maxCrossRoomSize));
            int roomHorHeight = (rng.Next(minCrossRoomSize, roomVerHeight - 2));
            int roomVerWidth = (rng.Next(minCrossRoomSize, roomHorWidth - 2));
            Block[,] room = new Block[roomHorWidth, roomVerHeight];
            
            for (int i = 0; i < room.GetLength(0); i++)
                for (int j = 0; j < room.GetLength(1); j++)
                    room[i, j] = new Wall(Material.Stone);

            int verOffset = roomVerHeight / 2 - roomHorHeight / 2;
            for (int i = 0; i < roomHorWidth; i++)
                for (int j = verOffset; j < roomHorHeight + verOffset; j++)
                    room[i, j] = new Air();

            int horOffset = roomHorWidth / 2 - roomVerWidth / 2;
            for (int i = horOffset; i < roomVerWidth + horOffset; i++)
                for (int j = 0; j < roomVerHeight; j++)
                    room[i, j] = new Air();

            return room;
        }
        private Block[,] GenerateRectRoom()
        {
            Point size = new Point(rng.Next(minCrossRoomSize, maxCrossRoomSize), rng.Next(minCrossRoomSize, maxCrossRoomSize));
            Block[,] room = new Block[size.X, size.Y];
            for (int i = 0; i < room.GetLength(0); i++)
                for (int j = 0; j < room.GetLength(1); j++)
                    room[i, j] = new Air();
            return room;
        }
        private Block[,] GenerateCavern()
        {
            throw new NotImplementedException();
        }
        
        // checking
        private bool RoomPlacementValid(Point pos, Block[,] room)
        {
            for (int i = -1; i <= room.GetLength(0); i++)
                for (int j = -1; j <= room.GetLength(1); j++) {
                    if (!dungeonFloor.PointWithinBounds(new Point(i + pos.X, j + pos.Y)) || dungeonFloor.PointOnEdge(new Point(i + pos.X, j + pos.Y)))
                        return false;
                    if (dungeonFloor[i + pos.X, j + pos.Y].Solid == false)
                        return false;
                }
            return true;
        }

        private Point ReturnRandomPositionWithFreeSpaceInDirection(int [] direction)
        {
            List<Point> potentialPositions = new List<Point>();
            for (int i = 1; i < dungeonFloor.Width - 1; i ++)
                for (int j = 1; j < dungeonFloor.Height - 1; j ++) {
                    if (dungeonFloor[i, j].Solid == true
                        && dungeonFloor[i + direction[0], j + direction[1]].Solid == true
                          && dungeonFloor[i - direction[0], j - direction[1]].Solid == false)
                        potentialPositions.Add(new Point(i, j));
                }
            if (potentialPositions.Count == 0)
                return new Point();
            return potentialPositions[rng.Next(0, potentialPositions.Count)];
        }
    }
}
