using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Landlord
{
    class Plant : Block
    {
        private int growthInterval;
        private int seedRadius;
        private List<byte> growthStages;
        public Plant(byte graphic, string name, int growthInterval, int seedRadius, List<byte> growthStages, Color? color = null, bool solid = false, bool opaque = false, BlockType type = BlockType.Plant, bool interactive = true, bool enterable = false) : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            if (color != null)
                this.ForeColor = (Color)color;
            else
                this.ForeColor = Color.AntiqueWhite;
            this.BackColor = Color.Pink;

            this.growthInterval = growthInterval;
            this.seedRadius = seedRadius;
            this.growthStages = growthStages;
        }
        public Plant() : base()
        {

        }
        public void Grow(MapTile map, Point position)
        {
            int currentStage = 0;
            for (int i = 0; i < growthStages.Count; i++)
                if (Graphic == growthStages[i])
                    currentStage = i;
            if (currentStage < growthStages.Count - 1) {
                Graphic = growthStages[currentStage + 1];
                return;
            }
            Random rng = new Random();
            int randX = rng.Next(position.X - seedRadius, position.X + seedRadius + 1), randY = rng.Next(position.Y - seedRadius, position.Y + seedRadius + 1);
            Point seedSpot = new Point(Math.Min(map.Width - 1, Math.Max(0, randX)), Math.Min(map.Height - 1, Math.Max(0, randY)));
            TrySeed(map, seedSpot);
        }
        // tries to place a seed at the specified position
        public void TrySeed(MapTile map, Point position)
        {
            Block block = map.Blocks[position.X * map.Width + position.Y];
            Tile tile = map.Floor[position.X * map.Width + position.Y];
            if (tile is DirtFloor && ((block is Plant p && p.Graphic == p.GrowthStages[p.GrowthStages.Count - 1]) || block is Air)) {
                Plant plant = (Plant)this.Copy();
                plant.Graphic = growthStages[0];
                map.Blocks[position.X * map.Width + position.Y] = plant;
            }
        }
        public override void Activate(Creature user)
        {

        }

        public int GrowthInterval {
            get { return growthInterval; }
            set { growthInterval = value; }
        }
        public int SeedRadius {
            get { return seedRadius; }
            set { seedRadius = value; }
        }
        public List<byte> GrowthStages {
            get { return growthStages; }
            set { growthStages = value; }
        }
    }
}
