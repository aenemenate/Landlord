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
            List<Creature> creatures = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Creatures : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Creatures;
            
            visibleCreatures = new List<Creature>();
            foreach (Creature c in creatures) {
                if (VisiblePoints.Exists(p => p.Equals(c.Position))) {
                    visibleCreatures.Add(c);
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
            Point pFocus = new Point(); // this is the nearest edible/harvestable plant, only used by herbivores and omnivores
            foreach (Point point in VisiblePoints) {
                Block block = Program.WorldMap[WorldIndex.X, WorldIndex.Y][point.X, point.Y];
                if (block is Creature c) {
                    if (Diet == DietType.Herbivore ? c.Diet != DietType.Herbivore : !c.Faction.Equals(Faction))
                        if (cFocus == null || cFocus.Position.DistFrom(Position) > c.Position.DistFrom(Position))
                            cFocus = c;
                }
                else if ((block is Plant p && p.IsEdible()) || (block is Food f && f.FoodType == DietType.Herbivore))
                    if (pFocus.Equals(new Point()) || (Program.WorldMap[WorldIndex.X, WorldIndex.Y][pFocus.X, pFocus.Y] is Food == false && pFocus.DistFrom(Position) > point.DistFrom(Position)))
                        pFocus = point;
            }

            int hunger = Stats.Resources[Resource.HV], maxHunger = Stats.Resources[Resource.MaxHV];

            switch (this.Diet) {
                case DietType.Carnivore:
                case DietType.Omnivore:
                    if (hunger <= (maxHunger / 3) * 2) {
                        if (!FoodInInventory()) {
                            if (cFocus == null)
                                RandomMove();
                            else if (this.Position.NextToPoint(cFocus.Position)) {
                                if (cFocus.Alive)
                                    LaunchAttack(cFocus);
                                else
                                    GetMeatFromCreatureInventory(cFocus);
                            }
                            else
                                Move(Position.GetClosestNearbyWalkablePos(cFocus.Position, WorldIndex, CurrentFloor));
                        }
                        else
                            Eat();
                    } else
                        RandomMove();
                    return;
                case DietType.Herbivore:
                    if (cFocus != null && cFocus.Alive)
                        Move(Position.GetFarthestNearbyWalkablePos(cFocus.Position, WorldIndex, CurrentFloor));
                    else if (hunger <= (maxHunger / 3) * 2) {
                        if (!FoodInInventory()) {
                            if (pFocus.Equals(new Point())) {
                                RandomMove();
                            }
                            else if (!Position.NextToPoint(pFocus))
                                Move(Position.GetClosestNearbyWalkablePos(pFocus, WorldIndex, CurrentFloor));
                            else {
                                if (Program.WorldMap[WorldIndex.X, WorldIndex.Y][pFocus.X, pFocus.Y] is Plant)
                                    HarvestPlant(pFocus);
                                else if (Program.WorldMap[WorldIndex.X, WorldIndex.Y][pFocus.X, pFocus.Y] is Food)
                                    GetItem(pFocus);
                            }
                        }
                        else
                            Eat();
                    }
                    else
                        RandomMove();
                    return;
            }
        }

        private void GetMeatFromCreatureInventory(Creature cFocus)
        {
            Program.MsgConsole.WriteLine($"Got meat from inventory of {cFocus.Name}");
        }

        private bool FoodInInventory()
        {
            return Inventory.Exists(i => i.ItemType == ItemType.Food);
        }

        private void RandomMove()
        {
            int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width, height = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Height;
            int x = Math.Min(Math.Max(0, Program.RNG.Next(Position.X - 1, Position.X + 2)), width - 1);
            int y = Math.Min(Math.Max(0, Program.RNG.Next(Position.Y - 1, Position.Y + 2)), height - 1);
            Move(new Point(x, y));
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