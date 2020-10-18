using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    public static class BlocksHelpers
    {
        internal static List<Point> GetEmptyAdjacentBlocks(this Block[] blocks, int mWidth, int mHeight, Point point) {
            List<Point> emptyBlocks = new List<Point>();

            int maxX = Math.Min(mHeight - 1, point.X + 1), maxY = Math.Min(mWidth - 1, point.Y + 1);
            int minX = Math.Max(0, point.X - 1), minY = Math.Max(0, point.Y - 1);

            for (int i = minX; i <= maxX; i++)
                for (int j = minY; j <= maxY; j++)
                    if (!point.Equals(new Point(i, j)) && blocks[i * mWidth + j].Solid == false && blocks[i * mWidth + j] is Item == false)
                        emptyBlocks.Add(new Point(i, j));

            return emptyBlocks;
        }
        internal static Point GetClosestOfBlockTypeToPos(this Block[] blocks, int mWidth, int mHeight, Point pos, BlockType blockType, Material material = Material.Null) {
            Point closestPoint = new Point();
            double nearestDist = 100000;
            for (int i = 0; i < mWidth; i++)
                for (int j = 0; j < mHeight; j++) {
                    double dist = new Point(i, j).DistFrom(pos);
                    if (blocks[i * mWidth + j].Type == blockType && (material == Material.Null || material == blocks[i * mWidth + j].Material) && dist < nearestDist) {
                        nearestDist = dist;
                        closestPoint = new Point(i, j);
                    }
                }
            return closestPoint;
        }

    }
}
