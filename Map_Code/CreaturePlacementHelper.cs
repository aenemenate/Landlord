using System;

namespace Landlord
{
    static class CreaturePlacementHelper
    {
        static public bool flag = false;

        public static bool HandleMapSwitching(Creature creature)
        {
            if (creature is Player) {
                // set all visibile cells to nonvisible before switching maps
                RayCaster.SetAllInvis(creature.Position, creature.SightDist + 5, creature.WorldIndex, creature.CurrentFloor);
            }

            bool success = false;

            if (creature.Position.X == 0 && creature.WorldIndex.X != 0) {
                success = creature.MoveMaps(new Point(Program.WorldMap.TileWidth - 1, creature.Position.Y), -1, 0);
            }
            else if (creature.Position.Y == Program.WorldMap.TileHeight - 1 && creature.WorldIndex.Y != 4) {
                success = creature.MoveMaps(new Point(creature.Position.X, 0), 0, +1);
            }
            else if (creature.Position.X == Program.WorldMap.TileWidth - 1 && creature.WorldIndex.X != 4) {
                success = creature.MoveMaps(new Point(0, creature.Position.Y), +1, 0);
            }
            else if (creature.Position.Y == 0 && creature.WorldIndex.Y != 0) {
                success = creature.MoveMaps(new Point(creature.Position.X, Program.WorldMap[0, 0].Height - 1), 0, -1);
            }
            return success;
        }
        
        public static void PlacePlayer(Class uclass, string gender, string name)
        {
            Point playerPos = new Point( 0, 0 );
            Point worldIndex = new Point(  playerPos.X / Program.WorldMap.TileWidth, playerPos.Y / Program.WorldMap.TileHeight);
            Random rng = new Random();

            while (true) {
                int side = rng.Next( 0, 4 );
                switch (side)
                {
                    case 0:
                        playerPos = new Point(  0, rng.Next( 0, Program.WorldMap.HeightMap.GetLength(1) )  );
                        break;
                    case 1:
                        playerPos = new Point(  Program.WorldMap.HeightMap.GetLength(0) - 1, rng.Next( 0, Program.WorldMap.HeightMap.GetLength(1) )  );
                        break;
                    case 2:
                        playerPos = new Point(  rng.Next( 0, Program.WorldMap.HeightMap.GetLength(0) ), 0  );
                        break;
                    case 3:
                        playerPos = new Point(  rng.Next( 0, Program.WorldMap.HeightMap.GetLength(0) ), Program.WorldMap.HeightMap.GetLength(1) - 1  );
                        break;
                }
                worldIndex = new Point(playerPos.X / Program.WorldMap.TileWidth, playerPos.Y / Program.WorldMap.TileHeight);
                bool playerPosIsEmpty = Program.WorldMap[worldIndex.X, worldIndex.Y][playerPos.X - (worldIndex.X * Program.WorldMap.TileWidth), playerPos.Y - (worldIndex.Y * Program.WorldMap.TileHeight)].Type == BlockType.Empty;
                if (playerPosIsEmpty)
                    break;
            }

            // figure out where in the local map this position translates to
            playerPos = new Point(playerPos.X - (worldIndex.X * Program.WorldMap.TileWidth), playerPos.Y - (worldIndex.Y * Program.WorldMap.TileHeight));

            // place the player
            Program.Player = new Player(Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks, playerPos, worldIndex, -1, 20, name, gender, true, uclass);
            Program.WorldMap[worldIndex.X, worldIndex.Y][playerPos.X, playerPos.Y] = Program.Player;
        }
    }
}
