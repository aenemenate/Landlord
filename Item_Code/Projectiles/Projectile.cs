using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class Projectile
    {
        private Point position;
        private Item item;
        private List<Point> path;

        public Projectile(Block[] blocks, int width, int height, Point startPoint, Point endPoint, Item item)
        {
            bool PlotPath(int x, int y)
            {
                if (new Point(x, y).Equals(startPoint))
                    return true;
                if (x >= width || y >= height || x < 0 || y < 0 || blocks[x * width + y].Solid) {
                    if (blocks[x * width + y] is Creature) path.Add(new Point(x, y));
                    return false;
                }
                path.Add(new Point(x, y));
                return true;
            }
            this.item = item;
            path = new List<Point>();
            Bresenhams.Line(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, PlotPath);
            if (path.Count != 0)
                position = path[0];
            else position = startPoint;
        }

        public Projectile() { }

        // if this returns null, it means that it's ready to be deleted
        public bool Update(Block[] blocks, int width)
        {
            int startIndex = path.FindIndex(i => i.Equals(position));
            if (startIndex == path.Count - 1 
                  || (blocks[path[startIndex + 1].X * width + path[startIndex + 1].Y].Solid 
                        & blocks[path[startIndex + 1].X * width + path[startIndex + 1].Y] is Creature == false)
                     ) {
                DropItem(path[startIndex], blocks, width);
                return true;
            }
            else if (blocks[path[startIndex + 1].X * width + path[startIndex + 1].Y] is Creature c) {
                LaunchAttack(c);
                return true;
            }
            position = path[startIndex + 1];
            return false;
        }

        public void LaunchAttack(Creature defender)
        {
            Random rng = new Random();
            if (defender.Alive == false)
                return;
            Item arrow = item;
            int damage = (int)arrow.Damage;
            DamageType dmgType = arrow.GetWepDmgType();
            int dmgDealt = defender.DefendAgainstDmg(dmgType, damage);

            Program.MsgConsole.WriteLine($"The {item.Name} struck {defender.Name} for {dmgDealt} damage!");
            if (defender.Alive == false) Program.MsgConsole.WriteLine($"{defender.Name} died.");
        }

        private void DropItem(Point pos, Block[] blocks, int width)
        {
            item.Visible = false;
            blocks[pos.X * width + pos.Y] = item.Copy();
            Program.MsgConsole.WriteLine($"The {item.Name} landed.");
        }

        public Point Position { get { return position; } set { position = value; } }
        public Item Item { get { return item; } set { item = value; } }
        public List<Point> Path { get { return path; } set { path = value; } }
    }
}
