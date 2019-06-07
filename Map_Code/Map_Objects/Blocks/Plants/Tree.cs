using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Tree : Plant
    {
        private int thickness;

        // CONSTRUCTORS //
        public Tree(Material material, byte graphic = 10, string name = "tree", bool solid = true, bool opaque = true, BlockType type = BlockType.Tree)
            : base(graphic, name, solid, opaque, type)
        {
            void DetermineThickness()
            {
                int minThickness = 160, maxThickness = 320;

                thickness = Program.RNG.Next(minThickness, maxThickness);
            }

            this.Material = material;
            this.ForeColor = new Color(205, 133, 63);
            DetermineThickness();
        }

        public Tree() : base()
        {
        }


        // FUNCTIONS //

        public override void Activate(Creature user)
        {
            if (user.Body.MainHand != null && (user.Body.MainHand is Axe || user.Body.MainHand is Sword))
                user.ChopTree(this);
        }

        public override void Grow()
        {

        }

        public void DropLogs(Point pos, Creature user)
        {
            Random rng = new Random();
            for (int i = Math.Max(pos.X - 5, 0); i <= Math.Min(pos.X + 5, Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y].Width - 1); i++)
                for (int j = Math.Max(pos.Y - 5, 0); j <= Math.Min(pos.Y + 5, Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y].Height - 1); j++)
                {
                    int treeRoll = rng.Next(1, 11), maxChance = 10;
                    double distFromTree = new Point(i, j).DistFrom(pos);
                    bool pointCloserToTreeThanfeller = new Point(i, j).DistFrom(user.Position) > distFromTree;

                    if (treeRoll < maxChance - distFromTree * 2 && pointCloserToTreeThanfeller && Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][i, j] is Air)
                        Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][i, j] = new Log(true);
                }

            Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][pos.X, pos.Y] = new Log(true);

            Program.MsgConsole.WriteLine("The tree was felled!");
        }

        // PROPERTIES //

        public int Thickness {
            get { return thickness; }
            set { thickness = value; }
        }
    }

}
