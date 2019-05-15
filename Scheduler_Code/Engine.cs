using System;

namespace Landlord
{
    static class Engine
    {
        static public bool flag = false;

        public static bool HandleMapSwitching(Creature creature)
        {
            // set all visibile cells to invisible so before switching maps
            RayCaster.SetAllInvis(creature.Position, creature.SightDist + 5);

            bool success = false;

            if (creature.Position.X == 0 && Program.WorldMap.WorldIndex.X != 0)
            {
                success = creature.MoveMaps(new Point(Program.WorldMap.LocalTile.Width - 1, creature.Position.Y),
                    Program.WorldMap.LocalTile, Program.WorldMap[Program.WorldMap.WorldIndex.X - 1, Program.WorldMap.WorldIndex.Y]);
                if (success)
                    Program.WorldMap.WorldIndex.X -= 1;
            }
            else if (creature.Position.Y == Program.WorldMap.LocalTile.Height - 1 && Program.WorldMap.WorldIndex.Y != 4)
            {
                success = creature.MoveMaps(new Point(creature.Position.X, 0), Program.WorldMap.LocalTile,
                    Program.WorldMap[Program.WorldMap.WorldIndex.X, Program.WorldMap.WorldIndex.Y + 1]);
                if (success)
                    Program.WorldMap.WorldIndex.Y += 1;
            }
            else if (creature.Position.X == Program.WorldMap.LocalTile.Width - 1 && Program.WorldMap.WorldIndex.X != 4)
            {
                success = creature.MoveMaps(new Point(0, creature.Position.Y), Program.WorldMap.LocalTile,
                    Program.WorldMap[Program.WorldMap.WorldIndex.X + 1, Program.WorldMap.WorldIndex.Y]);
                if (success)
                    Program.WorldMap.WorldIndex.X += 1;
            }
            else if (creature.Position.Y == 0 && Program.WorldMap.WorldIndex.Y != 0)
            {
                success = creature.MoveMaps(new Point(creature.Position.X, Program.WorldMap[0, 0].Height - 1),
                    Program.WorldMap.LocalTile, Program.WorldMap[Program.WorldMap.WorldIndex.X, Program.WorldMap.WorldIndex.Y - 1]);
                if (success)
                    Program.WorldMap.WorldIndex.Y -= 1;
            }

            return success;
        }
        
        public static void PlacePlayer(Class uclass, string gender, string name)
        {
            Point playerPos = new Point( 0, 0 );
            Point worldIndex = new Point(  playerPos.X / Program.WorldMap.LocalTile.Width, playerPos.Y / Program.WorldMap.LocalTile.Height  );
            Random rng = new Random();

            while (true)
            {
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
                worldIndex = new Point(playerPos.X / Program.WorldMap.LocalTile.Width, playerPos.Y / Program.WorldMap.LocalTile.Height);
                bool playerPosIsEmpty = Program.WorldMap[worldIndex.X, worldIndex.Y][playerPos.X - (worldIndex.X * Program.WorldMap.LocalTile.Width), playerPos.Y - (worldIndex.Y * Program.WorldMap.LocalTile.Height)].Type == BlockType.Empty;
                if (playerPosIsEmpty)
                    break;
            }

            // figure out where in the local map this position translates to
            playerPos = new Point(playerPos.X - (worldIndex.X * Program.WorldMap.LocalTile.Width), playerPos.Y - (worldIndex.Y * Program.WorldMap.LocalTile.Height));

            // place the player
            Program.Player = new Player(Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks, playerPos, 20, name, gender, true, uclass);
            Program.WorldMap[worldIndex.X, worldIndex.Y][playerPos.X, playerPos.Y] = Program.Player;
            Program.WorldMap.WorldIndex = new Point(worldIndex.X, worldIndex.Y);
        }
    }
}
