using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class RayCaster
    {
        static public HashSet<Point> CalculateFOV(int radius, Creature creature)
        {
            double x1, y1;

            HashSet<Point> visiblePoints = new HashSet<Point>() { creature.Position };
            
            for (int i = 0; i < 360; i++)
            {
                x1 = Math.Cos((double)i * 0.01745f);
                y1 = Math.Sin((double)i * 0.01745f);
                CheckLineOfSight(x1, y1, creature.Position, creature.WorldIndex, creature.CurrentFloor, radius, ref visiblePoints);
            }
            return visiblePoints;
        }

        static public void SetAllInvis(Point position, int radius, Point worldIndex, int currentFloor)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            Tile[] tiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Floor : Program.WorldMap[worldIndex.X, worldIndex.Y].Floor;
            int width = Program.WorldMap.TileWidth;

            int radPlus2 = radius + 2;
            for (int i = position.X - radPlus2; i < position.X + radPlus2; i++)
                for (int j = position.Y - radPlus2; j < position.Y + radPlus2; j++)
                    if (Program.WorldMap[worldIndex.X, worldIndex.Y].PointWithinBounds(new Point(i, j))) {
                        blocks[i * Program.WorldMap.TileWidth + j].Visible = false;
                        tiles[i * Program.WorldMap.TileWidth + j].Visible = false;
                    }
        }

        static private void CheckLineOfSight(double x1, double y1, Point pos, Point worldIndex, int currentFloor, int radius, ref HashSet<Point> visiblePoints)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;

            int i;
            double cx, cy;
            cx = pos.X + 0.5f;
            cy = pos.Y + 0.5f;
            for (i = 0; i < radius; i++)
            {
                Point spot = new Point((int)cx, (int)cy);
                if (!Program.WorldMap[worldIndex.X, worldIndex.Y].PointWithinBounds( spot ))
                    continue;
                visiblePoints.Add( new Point( (int)cx, (int)cy));
                if (blocks[(int)cx * width + (int)cy].Opaque == true && !((int)cx == pos.X && (int)cy == pos.Y))
                    break;
                cx += x1;
                cy += y1;
            }
        }
    }
}
