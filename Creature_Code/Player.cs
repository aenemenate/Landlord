using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    
    class Player : Creature
    {
        private List<Point> path = null;
        private bool pausePathing = false;
        private int dangerCount = 0;
        private PlayerInput inputModule = new PlayerInput();
        private List<Creature> localCreatures = new List<Creature>();

        // CONSTRUCTORS

        public Player (Block[] map, Point position, int sightDist, string name, string gender, bool friendly, Class uclass, byte graphic = 1,
            bool solid = true, bool opaque = true) : base (map, position, sightDist, graphic, name, gender, friendly, solid, opaque)
        {
            ForeColor = Color.AntiqueWhite;

            BlueprintPouch bpPouch = new BlueprintPouch( false );
            RecipePouch craftPouch = new RecipePouch( false );
            Inventory.Add( bpPouch );
            Inventory.Add( craftPouch );
            Inventory.Add( new Axe( true ) { WeaponName = "pickaxe", Material = Material.Copper } );

            Gold = 6000;
            Class = uclass;
            DetermineStats();
            DetermineEquipment();
        }

        public Player() : base()
        {
        }


        // FUNCTIONS
        public override void DetermineStats()
        {
            Stats = new Stats(Class);
        }

        public override void HandleVisibility()
        {
            localCreatures = new List<Creature>();

            RayCaster.SetAllInvis(Position, SightDist + 5);

            int localDangerCount = 0;
            foreach (Point point in VisiblePoints)
            {
                Program.WorldMap.LocalTile[point.X, point.Y].Visible = true;
                Program.WorldMap.LocalTile[point.X, point.Y].Explored = true;
                if (Program.WorldMap.LocalTile[point.X, point.Y].Opaque == false)
                {
                    Program.WorldMap.LocalTile.Floor[point.X * Program.WorldMap.LocalTile.Width + point.Y].Explored = true;
                    Program.WorldMap.LocalTile.Floor[point.X * Program.WorldMap.LocalTile.Width + point.Y].Visible = true;
                }

                // add items to memory map, remove them if there is a mismatch between memory map and local map
                if (Program.WorldMap.LocalTile[point.X, point.Y] is Item)
                    Program.WorldMap.LocalTile.MemoryMap[point.X * Program.WorldMap.LocalTile.Width + point.Y] = Program.WorldMap.LocalTile[point.X, point.Y];
                else if (Program.WorldMap.LocalTile.MemoryMap[point.X * Program.WorldMap.LocalTile.Width + point.Y] != null)
                    Program.WorldMap.LocalTile.MemoryMap[point.X * Program.WorldMap.LocalTile.Width + point.Y] = null;
                
                if (Program.WorldMap.LocalTile[point.X, point.Y] is Monster && ((Creature)Program.WorldMap.LocalTile[point.X, point.Y]).Friendly == false && ((Creature)Program.WorldMap.LocalTile[point.X, point.Y]).Alive)
                    localDangerCount++;

                if (Program.WorldMap.LocalTile[point.X, point.Y] is Creature creature)
                    localCreatures.Add( creature );

            }

            if (localDangerCount > 0)
            {
                if (dangerCount < localDangerCount)
                    Program.Player.Path = null;
                dangerCount = localDangerCount;
            }
            else
                dangerCount = localDangerCount;
        }

        public override void DetermineAction()
        {

            if (!Alive) {
                Menus.DeathNotification();
                return;
            }

            if (Program.CurrentState is Play play)
            {
                if (play.PlayMode == PlayMode.Roguelike)
                {
                    if (Body.MainHand is BlueprintPouch)
                    {
                        play.PlayMode = PlayMode.BuildMode;
                        BuildingManager.Paused = true;
                        Program.MsgConsole.WriteLine("You entered Build Mode!");
                    }
                    else if (Body.MainHand is RecipePouch)
                        Program.CurrentState = new CraftMenu();
                    else
                    {
                        pausePathing = inputModule.HandleInput(true, true);

                        if (!pausePathing)
                            FollowPath();
                    }
                }
                else
                    FollowPath();
            }

            
            UpdateFOV();
            HandleVisibility();
        }


        public void SetPath(Point goal)
        {
            try
            {
                path = Pathfinder.FindPath( Position, goal );
            }
            catch
            {
                Program.MsgConsole.WriteLine( "A path couldn't be found." );
            }
        }

        public void FollowPath()
        {
            MapTile map = Program.WorldMap.LocalTile;

            if (path == null)
                return;

            if (path.Count != 0)
            {
                Move(path[0], map, true);
                path.RemoveAt(0);
                return;
            }

            path = null;
        }



        // PROPERTIES

        public List<Point> Path
        {
            get { return path; }
            set { path = value; }
        }

        public int DangerCount
        {
            get { return dangerCount; }
            set { dangerCount = value; }
        }

        public PlayerInput InputModule
        {
            get { return inputModule; }
        }

        public List<Creature> LocalCreatures
        {
            get { return localCreatures; }
            set { localCreatures = value; }
        }
    }
}