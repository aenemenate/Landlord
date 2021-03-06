﻿using Microsoft.Xna.Framework;
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
        private bool edible;

        // CONSTRUCTORS
        public Plant(byte graphic, string name, int growthInterval, int seedRadius, List<byte> growthStages, string requirement, bool edible, Color? color = null, bool explored = false, bool solid = false, bool opaque = false, BlockType type = BlockType.Plant, bool interactive = true, bool enterable = false) : base(graphic, name, type, explored, solid, opaque, interactive, enterable)
        {
            if (color != null)
                this.ForeColor = (Color)color;
            else this.ForeColor = Color.AntiqueWhite;
            this.BackColor = Color.Pink;

            this.growthInterval = growthInterval;
            this.seedRadius = seedRadius;
            this.growthStages = growthStages;
            this.requirement = requirement;
            this.edible = edible;
        }
        public Plant() : base() { }

        // FUNCTIONS
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
        public void TrySeed(MapTile map, Point position, Random rng)
        {
            if (!map.PointWithinBounds(position))
                return;
            bool explored = map.Blocks[position.X * map.Width + position.Y].Explored;
            if (map.Floor[position.X * map.Width + position.Y] is DirtFloor)
                if ((map.Blocks[position.X * map.Width + position.Y] is Plant && rng.Next(0, 50) <= 1) || map.Blocks[position.X * map.Width + position.Y] is Air)
                    map[position.X, position.Y] = new Plant(growthStages[0], Name, growthInterval, seedRadius, growthStages, requirement, edible, ForeColor, explored);
        }
        public bool RequirementsMet(MapTile map, Point position)
        {
            if (requirement.Equals(""))
                return true;
            if (requirement.Contains("tile_nearby;")) {
                if (requirement.Contains("water;"))
                {
                    int dist = System.Convert.ToInt32(requirement.Replace("tile_nearby;water;", ""));
                    if (map.GetClosestOfTileTypeToPos(position, new Water()).DistFrom(position) < dist)
                        return true;
                }
            }
            if (requirement.Contains("block_nearby;")) {
                if (requirement.Contains("wall;")) {
                    int dist = System.Convert.ToInt32(requirement.Replace("block_nearby;wall;", ""));
                    if (map.GetClosestOfBlockTypeToPos(position, BlockType.Wall).DistFrom(position) < dist)
                        return true;
                }
                else if (requirement.Contains("tree;")) {
                    int dist = System.Convert.ToInt32(requirement.Replace("block_nearby;tree;", ""));
                    if (map.GetClosestOfBlockTypeToPos(position, BlockType.Tree).DistFrom(position) < dist)
                        return true;
                }
            }
            return false;
        }
        public Block DropHarvest()
        {
            bool fullyGrown = growthStages.IndexOf(Graphic) != growthStages.Count - 1;
            if (!Edible || fullyGrown) return new Leaf(true, Name.Split(' ')[0], ForeColor);
            else return new Food(DietType.Herbivore, Name.Split(' ')[0] + " bundle", 15 * 16 + 10, .013, ForeColor);
        }
        public override void Activate(Creature user)
        {
        }

        // PROPERTIES
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
        public bool Edible {
            get { return edible; }
            set { edible = value; }
        }
    }
}
