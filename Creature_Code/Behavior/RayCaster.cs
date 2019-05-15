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
                CheckLineOfSight(x1, y1, creature.Position, radius, ref visiblePoints);
            }
            return visiblePoints;
        }

        static public void SetAllInvis(Point position, int radius)
        {
            int radPlus2 = radius + 2;
            for (int i = position.X - radPlus2; i < position.X + radPlus2; i++)
                for (int j = position.Y - radPlus2; j < position.Y + radPlus2; j++)
                    if (Program.WorldMap.LocalTile.PointWithinBounds(new Point(i, j)))
                    {
                        Program.WorldMap.LocalTile[i, j].Visible = false;
                        Program.WorldMap.LocalTile.Floor[i * Program.WorldMap.LocalTile.Width + j].Visible = false;
                    }
        }

        static private void CheckLineOfSight(double x1, double y1, Point pos, int radius, ref HashSet<Point> visiblePoints)
        {
            int i;
            double cx, cy;
            cx = pos.X + 0.5f;
            cy = pos.Y + 0.5f;
            for (i = 0; i < radius; i++)
            {
                Point spot = new Point((int)cx, (int)cy);
                if (!Program.WorldMap.LocalTile.PointWithinBounds( spot))
                    continue;
                visiblePoints.Add( new Point( (int)cx, (int)cy));
                if (Program.WorldMap.LocalTile[(int)cx, (int)cy].Opaque == true && !((int)cx == pos.X && (int)cy == pos.Y))
                    break;
                cx += x1;
                cy += y1;
            }
        }
    }
}
