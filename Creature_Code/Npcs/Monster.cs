using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum DesireType
    {
        Treasure,
        KillPlayer,
        Flee,
        Eat
    }

    class Monster : Creature
    {
        private int curPatrolDestIndex;
        private int persistence; // affects how long they will continue to chase something after losing sight of it
        private int turnsSinceAttentionCaught;
        private bool patrolling;
        private Dictionary<DesireType, int> baseDesires;
        private Dictionary<DesireType, int> currentDesires;


        // CONSTRUCTORS //

        public Monster(Block[] map, Point position, Point worldIndex, int currentFloor, Color? color, int sightDist, int persistence, Dictionary<DesireType, int> baseDesires, Class uclass, string name, string gender, DietType diet, string faction, byte graphic,
            bool solid = true, bool opaque = true) : base(map, position, worldIndex, currentFloor, sightDist, graphic, name, gender, diet, faction, solid, opaque)
        {
            this.persistence = persistence;
            patrolling = true;
            curPatrolDestIndex = Program.RNG.Next(1, 5);
            turnsSinceAttentionCaught = 0;
            if (color != null)
                this.ForeColor = (Color)color;
            this.Class = uclass;

            this.baseDesires = baseDesires;
            currentDesires = baseDesires;

            DetermineStats();
            DetermineEquipment();
        }

        public Monster() : base()
        {
        }


        // FUNCTIONS //

        //abstract/override
        public override void HandleVisibility()
        {
            turnsSinceAttentionCaught++;
            if (turnsSinceAttentionCaught > persistence) {
                Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
                int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width;
                patrolling = true;

                foreach (Point point in VisiblePoints) {
                    if (point.Equals(Program.Player.Position)) {
                        turnsSinceAttentionCaught = 0;
                        patrolling = false;
                        break;
                    }
                    if (blocks[point.X * width + point.Y].Type == BlockType.Item) {
                        turnsSinceAttentionCaught = persistence;
                        patrolling = false;
                        break;
                    }
                }
            }
        }
        public override void DetermineAction()
        {
            if (!Alive)
                return;

            MapTile map = Program.WorldMap[WorldIndex.X, WorldIndex.Y];
            Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;

            Point movePos = GetNextPosition();

            if (patrolling && movePos.Equals( new Point()))
                curPatrolDestIndex = Program.RNG.Next( 0, map.Dungeon.Floors[CurrentFloor].PatrolPoints.Count);

            List<Point> itemSpots = GetNearbyBlocksOfType(BlockType.Item);
            List<Point> doorSpots = GetNearbyBlocksOfType(BlockType.Door);

            bool nextToPlayer = Position.NextToPoint(Program.Player.Position);
            bool wantToKillPlayer = CurrentDesires[DesireType.KillPlayer] > 0;
            if (nextToPlayer && wantToKillPlayer) {
                LaunchAttack(Program.Player);
                CreaturePlacementHelper.flag = true; // What's going on here?
            }
            else if (itemSpots.Count > 0 && CurrentDesires[DesireType.Treasure] > 0) {
                if (CanCarryItem((Item)blocks[itemSpots[0].X * width + itemSpots[0].Y])) {
                    GetItem(itemSpots[0]);
                    CurrentDesires[DesireType.Treasure] = BaseDesires[DesireType.Treasure];
                }
                else CurrentDesires[DesireType.Treasure]--;
            }
            else if (doorSpots.Count > 0 && blocks[doorSpots[0].X * width + doorSpots[0].Y].Solid)
                blocks[doorSpots[0].X * map.Width + doorSpots[0].Y].Activate(this);
            else if (movePos.Equals(Position) == false)
                Move(movePos);
            else
                Wait();

            UpdateFOV();
            HandleVisibility();
        }
        public override void DetermineStats()
        {
            Stats = new Stats(Class);
        }

        // other

        private Point GetNextPosition()
        {
            Block[] blocks = CurrentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            int width = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Width, height = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Height;

            int GetPosValue(Point pos) {
                bool playerIsNearby = WorldIndex.Equals(Program.Player.WorldIndex) && CurrentFloor == Program.Player.CurrentFloor && (Position.DistFrom(Program.Player.Position) <= SightDist);
                return (playerIsNearby ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.DistToPlayerMap[pos.X * width + pos.Y] * CurrentDesires[DesireType.KillPlayer] : 0)
                        + Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.DistToItemsMap[pos.X * width + pos.Y] * CurrentDesires[DesireType.Treasure];
            }

            int GetPatrolPosValue(Point pos)
            {
                return Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[CurrentFloor].PatrolMaps.PatrolGoals[curPatrolDestIndex][pos.X * width + pos.Y];
            }

            List<Point> bestPositions = new List<Point>();
            int bestVal = !patrolling ? GetPosValue(Position) : GetPatrolPosValue(Position);

            for (int i = Math.Max(0, Position.X - 1); i <= Math.Min(width - 1, Position.X + 1); i++)
                for (int j = Math.Max(0, Position.Y - 1); j <= Math.Min(height - 1, Position.Y + 1); j++) {
                    if (Position.Equals( new Point( i, j ) ) || blocks[i * width + j].Solid)
                        continue;

                    int thisVal = !patrolling ? GetPosValue(new Point(i, j)) : GetPatrolPosValue(new Point(i, j));
                    

                    if (thisVal <= bestVal) {
                        bestPositions.Add( new Point( i, j ) );
                        bestVal = thisVal;
                    }
                }

            for ( int i = bestPositions.Count - 1; i >= 0; i-- )
                if ((!patrolling ? GetPosValue(bestPositions[i]) : GetPatrolPosValue(bestPositions[i])) != bestVal)
                    bestPositions.RemoveAt(i);

            return bestPositions.Count > 0 ? bestPositions[Program.RNG.Next(0, bestPositions.Count)] : new Point();
        }


        // PROPERTIES //

        public int CurPatrolDestIndex {
            get { return curPatrolDestIndex; }
            set { curPatrolDestIndex = value; }
        }
        public int Persistence {
            get { return persistence; }
            set { persistence = value; }
        }
        public int TurnsSinceAttentionCaught {
            get { return turnsSinceAttentionCaught; }
            set { turnsSinceAttentionCaught = value; }
        }
        public bool Patrolling {
            get { return patrolling; }
            set { patrolling = value; }
        }
        public Dictionary<DesireType, int> BaseDesires
        {
            get { return baseDesires; }
            set { baseDesires = value; }
        }
        public Dictionary<DesireType, int> CurrentDesires {
            get { return currentDesires; }
            set { currentDesires = value; }
        }
    }
}