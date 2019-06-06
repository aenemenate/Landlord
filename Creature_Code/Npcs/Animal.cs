using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Animal : Creature
    {
        private List<Creature> visibleCreatures;
        private Dictionary<DesireType, int> baseDesires;
        private Dictionary<DesireType, int> currentDesires;


        // CONSTRUCTORS //

        public Animal(Block[] map, Point position, Point worldIndex, int currentFloor, Color? color, int sightDist, int persistence, Dictionary<DesireType, int> baseDesires, Class uclass, string name, string gender, DietType diet, string faction, byte graphic,
            bool solid = true, bool opaque = true) : base(map, position, worldIndex, currentFloor, sightDist, graphic, name, gender, diet, faction, solid, opaque)
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

            visibleCreatures = new List<Creature>();
            foreach (Point point in VisiblePoints) {
                if (blocks[point.X * width + point.Y] is Creature c) {
                    visibleCreatures.Add(c);
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

            //MapTile map = Program.WorldMap[WorldIndex.X, WorldIndex.Y];
            //Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            
            MoveSomewhere();
        }

        public void MoveSomewhere()
        {
            Creature cFocus = null;
            foreach (Creature c in visibleCreatures)
                if (!c.Faction.Equals(Faction))
                    cFocus = c;

            if (cFocus != null) {
                switch (Diet) {
                    case DietType.Carnivore:
                        Move(Position.GetClosestNearbyWalkablePos(cFocus.Position));
                        return;
                    case DietType.Herbivore:
                        if (cFocus.Diet != DietType.Herbivore)
                            Move(Position.GetFarthestNearbyWalkablePos(cFocus.Position));
                        return;
                    case DietType.Omnivore:
                        if (cFocus.Diet == DietType.Carnivore)
                            Move(Position.GetFarthestNearbyWalkablePos(cFocus.Position));
                        if (cFocus.Diet == DietType.Herbivore)
                            Move(Position.GetClosestNearbyWalkablePos(cFocus.Position));
                        return;
                }
            }
            else {
                int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width, height = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Height;
                int x = Math.Min(Math.Max(0, Program.RNG.Next(Position.X - 1, Position.X + 2)), width - 1);
                int y = Math.Min(Math.Max(0, Program.RNG.Next(Position.Y - 1, Position.Y + 2)), height - 1);
                Move(new Point(x, y));
            }
        }

        public override void DetermineStats()
        {
            Stats = new Stats(Class);
        }

        // PROPERTIES //

        public Dictionary<DesireType, int> BaseDesires {
            get { return baseDesires; }
            set { baseDesires = value; }
        }
        public Dictionary<DesireType, int> CurrentDesires {
            get { return currentDesires; }
            set { currentDesires = value; }
        }
    }
}