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
        private string requirement;
        public Plant(byte graphic, string name, int growthInterval, int seedRadius, List<byte> growthStages, string requirement, Color? color = null, bool explored = false, bool solid = false, bool opaque = false, BlockType type = BlockType.Plant, bool interactive = true, bool enterable = false) : base(graphic, name, type, explored, solid, opaque, interactive, enterable)
        {
            if (color != null)
                this.ForeColor = (Color)color;
            else
                this.ForeColor = Color.AntiqueWhite;
            this.BackColor = Color.Pink;

            this.growthInterval = growthInterval;
            this.seedRadius = seedRadius;
            this.growthStages = growthStages;
            this.requirement = requirement;
        }
        public Plant() : base()
        {

        }
        public void Grow(MapTile map, Point position, Random rng)
        {
            int currentStage = growthStages.IndexOf(Graphic);
            if (currentStage < growthStages.Count - 1) {
                Graphic = growthStages[currentStage + 1];
                return;
            }
            int randX = rng.Next(position.X - seedRadius, position.X + seedRadius + 1), randY = rng.Next(position.Y - seedRadius, position.Y + seedRadius + 1);
            Point seedSpot = new Point(Math.Min(map.Width - 1, Math.Max(0, randX)), Math.Min(map.Height - 1, Math.Max(0, randY)));
            TrySeed(map, seedSpot, rng);
        }
        // tries to place a seed at the specified position
        public void TrySeed(MapTile map, Point position, Random rng)
        {
            bool explored = map.Blocks[position.X * map.Width + position.Y].Explored;
            if (map.Floor[position.X * map.Width + position.Y] is DirtFloor)
                if ((map.Blocks[position.X * map.Width + position.Y] is Plant && rng.Next(0, 20) < 5) || map.Blocks[position.X * map.Width + position.Y] is Air)
                    if (RequirementsMet(map, position))
                        map[position.X, position.Y] = new Plant(growthStages[0], Name, growthInterval, seedRadius, growthStages, requirement, ForeColor, explored);
        }
        public bool RequirementsMet(MapTile map, Point position)
        {
            if (requirement.Equals(""))
                return true;
            if (requirement.Contains("tile_nearby;")) {
                if (requirement.Contains("water;")) {
                    int dist = System.Convert.ToInt32(requirement.Replace("tile_nearby;water;", ""));
                    if (map.GetClosestOfTileTypeToPos(position, new Water()).DistFrom(position) < dist)
                        return true;
                }
            }
            return false;
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
        public string Requirement {
            get { return requirement; }
            set { requirement = value; }
        }
    }
}
