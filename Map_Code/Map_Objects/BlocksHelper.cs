using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class BlocksHelper
    {
        internal static List<Point> GetEmptyAdjacentBlocks(this Block[] blocks, Point size, Point point)
        {
            List<Point> emptyBlocks = new List<Point>();

            int maxX = Math.Min(size.X - 1, point.X + 1), maxY = Math.Min(size.Y - 1, point.Y + 1);
            int minX = Math.Max(0, point.X - 1), minY = Math.Max(0, point.Y - 1);

            for (int i = minX; i <= maxX; i++)
                for (int j = minY; j <= maxY; j++)
                    if (!point.Equals(new Point(i, j)) && blocks[i * size.X + j].Solid == false && blocks[i * size.X + j] is Item == false)
                        emptyBlocks.Add(new Point(i, j));

            return emptyBlocks;
        }
        internal static Creature GetCreatureAtPosition(this List<Creature> creatures, Point position)
        {
            if (Program.Player.Position.Equals(position) && creatures.Exists(c => c.ID == Program.Player.ID))
                return Program.Player;

            foreach (Creature creature in creatures)
                if (creature.Position.Equals(position))
                    return creature;
            return null;
        }
        internal static Point GetClosestOfBlockTypeToPos(this Block[] blocks, Point pos, Point blocksSize, BlockType blockType, Material material = Material.Null)
        {
            Point closestPoint = new Point();
            double nearestDist = 100000;
            for (int i = 0; i < blocksSize.X; i++)
                for (int j = 0; j < blocksSize.Y; j++) {
                    double dist = new Point(i, j).DistFrom(pos);
                    if (blocks[i * blocksSize.X + j].Type == blockType && (material == Material.Null || material == blocks[i * blocksSize.X + j].Material) && dist < nearestDist) {
                        nearestDist = dist;
                        closestPoint = new Point(i, j);
                    }
                }
            return closestPoint;
        }

        internal static List<Point> GetAllTraversablePoints(this Block[] blocks, Point size)
        {
            List<Point> availablePoints = new List<Point>();
            for (int i = 0; i < size.X; i++)
                for (int j = 0; j < size.Y; j++)
                    if (blocks[i * size.X + j].Solid == false)
                        availablePoints.Add(new Point(i, j));
            return availablePoints;
        }
    }
}
