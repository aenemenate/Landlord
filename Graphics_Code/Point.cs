using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Point : IEquatable<Point>
    {
        private int x, y;


        // CONSTRUCTORS //
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point()
        {
            x = -1000;
            y = -1000;
        }


        // FUNCTIONS //

        public bool Equals(Point other)
        {
            return other.X == x && other.Y == y;
        }

        public override int GetHashCode()
        {
            return (x.GetHashCode() + 7 * 13) ^ y.GetHashCode();
        }

        // PROPERTIES //
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }

    static class PointHelper
    {
        public static double DistFrom( this Point p1, Point p2 )
        {
            int deltaX = p1.X - p2.X;
            int deltaY = p1.Y - p2.Y;
            return Math.Sqrt( deltaX * deltaX + deltaY * deltaY );
        }


        public static List<Point> GetAdjacentWalkablePoints( this Point point )
        {
            List<Point> adjacentPoints = new List<Point>();
            for (int i = Math.Max( 0, point.X - 1 ); i <= Math.Min( Program.WorldMap.LocalTile.Width - 1, point.X + 1 ); i++)
                for (int j = Math.Max( 0, point.Y - 1 ); j <= Math.Min( Program.WorldMap.LocalTile.Height - 1, point.Y + 1 ); j++)
                    if (point.Equals( new Point( i, j ) ) == false && Program.WorldMap.LocalTile[i, j].Solid == false)
                        adjacentPoints.Add( new Point( i, j ) );
            return adjacentPoints;
        }
    }
}
