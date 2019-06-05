using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Animal : Creature
    {
        private List<Creature> visibleCreatures;
        private List<Item> visibleItems;
        private Dictionary<DesireType, int> baseDesires;
        private Dictionary<DesireType, int> currentDesires;


        // CONSTRUCTORS //

        public Animal(Block[] map, Point position, Point worldIndex, int currentFloor, Color? color, int sightDist, int persistence, Dictionary<DesireType, int> baseDesires, Class uclass, string name, string gender, string faction, byte graphic,
            bool solid = true, bool opaque = true) : base(map, position, worldIndex, currentFloor, sightDist, graphic, name, gender, faction, solid, opaque)
        {
            if (color != null)
                this.ForeColor = (Color)color;
            this.Class = uclass;

            this.baseDesires = baseDesires;
            currentDesires = baseDesires;

            DetermineStats();
            DetermineEquipment();
        }

        public Animal() : base()
        {
        }


        // FUNCTIONS //

        //abstract/override
        public override void HandleVisibility()
        {
            Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width;

            visibleItems = new List<Item>();
            visibleCreatures = new List<Creature>();
            foreach (Point point in VisiblePoints) {
                if (blocks[point.X * width + point.Y] is Creature c) {
                    visibleCreatures.Add(c);
                    return;
                }
                if (blocks[point.X * width + point.Y] is Item i) {
                    visibleItems.Add(i);
                    return;
                }
            }
        }

        public override void DetermineAction()
        {
            if (!Alive)
                return;

            UpdateFOV();
            HandleVisibility();

            MapTile map = Program.WorldMap[WorldIndex.X, WorldIndex.Y];
            Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width;

            Creature cFocus;
            Item iFocus;

            foreach (Creature c in visibleCreatures) {
                if (!c.Faction.Equals(Faction))
                    cFocus = c;
            }
        }

        public override void DetermineStats()
        {
            Stats = new Stats(Class);
        }

        // PROPERTIES //

        public Dictionary<DesireType, int> BaseDesires
        {
            get { return baseDesires; }
            set { baseDesires = value; }
        }
        public Dictionary<DesireType, int> CurrentDesires
        {
            get { return currentDesires; }
            set { currentDesires = value; }
        }
    }
}