using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Landlord
{
    abstract class Creature : Block , IComparable<Creature>, IEquatable<Creature>
    {
        private Point position;
        private Block currentBlock;

        private List<Item> inventory = new List<Item>();
        private int sightDist;
        private List<Point> visiblePoints = new List<Point>();
        
        private string gender;
        private float gold;
        private Body body = new Body();
        private Stats stats;
        private Class uclass;
        private List<Effect> effects;

        private bool friendly;
        private bool alive = true;

        private Time nextActionTime;


        // CONSTRUCTORS //

        public Creature (Block[] map, Point position, int sightDist, byte graphic, string name, string gender, bool friendly,
            bool solid, bool opaque, BlockType type = BlockType.Creature, bool interactive = true, bool enterable = false) 
            : base (graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.position = position;
            this.sightDist = sightDist;
            currentBlock = map[position.X * 100 + position.Y];
            BackColor = Color.Pink;
            this.gender = gender;
            this.friendly = friendly;
            nextActionTime = new Time(Program.TimeHandler.CurrentTime);
            effects = new List<Effect>();
            
            gold = 0F;
        }

        public Creature() : base()
        {

        }

        // FUNCTIONS //

        
        // abstract/override funcs //
        public abstract void DetermineAction();

        public abstract void DetermineStats();
        
        public abstract void HandleVisibility();

        public override void Activate(Creature user)
        {
            if (alive)
            {
                if (friendly == false)
                    user.LaunchAttack(this);
                else
                    user.StartDialog(this);
            }
            else
            {
                Program.Animations.Add(new OpenLootView());
                Program.CurrentState = new ViewLoot(inventory, Name);
            }
        }

        public bool Equals(Creature other)
        {
            return Name.Equals(other.Name) && Position.Equals(other.Position);
        }
        
        public void DetermineEquipment()
        {
            if (Class.MajorSkills.Contains(Skill.HeavyWeapons))
                Body.MainHand = new Axe(true);
            else if (Class.MajorSkills.Contains(Skill.LongBlades))
                Body.MainHand = new Sword(true);
            else if (Class.MajorSkills.Contains(Skill.ShortBlade))
                Body.MainHand = new Dagger(true);
            else if (Class.MajorSkills.Contains(Skill.Spear))
                Body.MainHand = new Spear(true);

            if (Class.MajorSkills.Contains(Skill.Block) || Class.MinorSkills.Contains(Skill.Block))
                Body.OffHand = new Shield(true);

            // determine valid Materials
            Skill armorSkill = Skill.Alchemy;
            foreach (Skill skill in Class.MinorSkills)
                if (skill == Skill.HeavyArmor || skill == Skill.LightArmor)
                    armorSkill = skill;
            foreach (Skill skill in Class.MajorSkills)
                if (skill == Skill.HeavyArmor || skill == Skill.LightArmor)
                    armorSkill = skill;

            List<Material> validMaterials;
            if (armorSkill == Skill.Alchemy)
                validMaterials = new List<Material>() { Material.Cloth };
            else
                validMaterials = Physics.GetArmorSkillMaterials(armorSkill);

            // create the armor with chosen materials
            if (Program.RNG.Next(0, 100) < 50)
            do { Body.Helmet = new Helmet(true); } while (validMaterials.Contains(Body.Helmet.Material) == false);
            if (Program.RNG.Next(0, 100) < 50)
                do { Body.ChestPiece = new ChestPiece(true); } while (validMaterials.Contains(Body.ChestPiece.Material) == false);
            do { Body.Shirt = new Shirt(true); } while (validMaterials.Contains(Body.Shirt.Material) == false);
            if (Program.RNG.Next(0, 100) < 50)
                do { Body.Gauntlets = new Gauntlets(true); } while (validMaterials.Contains(Body.Gauntlets.Material) == false);
            do { Body.Leggings = new Leggings(true); } while (validMaterials.Contains(Body.Leggings.Material) == false);
            do { Body.Boots = new Boots(true); } while (validMaterials.Contains(Body.Boots.Material) == false);
        }

        public int CompareTo(Creature other)
        {
            return NextActionTime.Minus(other.NextActionTime);
        }
        

        // moving maps //
        public bool MoveMaps(Point to, MapTile mapFrom, MapTile mapTo)
        {
            if (mapTo[to.X, to.Y].Solid)
                return false;

            mapFrom[position.X, position.Y] = currentBlock;

            currentBlock = mapTo[to.X, to.Y];

            mapFrom.Creatures.Remove(this);
            mapTo.Creatures.Add(this);

            mapTo[to.X, to.Y] = this;

            position = to;

            ApplyActionCost(8);
            return true;
        }

        public void TakeStairsDown()
        {
            if (!Program.WorldMap.LocalTile.Owned)
            {
                Menus.DisplayIncorrectUsage("You need to buy this hectare to enter the dungeon. Click a plot on the world map to buy it.");
                return;
            }

            bool dungeonNotCreated = Program.WorldMap.LocalTile.Dungeon == null;
            if (dungeonNotCreated)
            {
                Program.CurrentState = new GeneratingMap();
                return; // program will stop here and generate the map, then recall this function and skip to the rest
            }

            bool onFinalFloor = Program.WorldMap.LocalTile.CurrentFloor == Program.WorldMap.LocalTile.Dungeon.Floors.GetLength(0) - 1;
            if (onFinalFloor)
                return;

            ApplyActionCost(12);

            Program.WorldMap.LocalTile.Blocks[this.Position.X * Program.WorldMap.LocalTile.Width + this.Position.Y] = this.CurrentBlock;

            Program.WorldMap.LocalTile.Creatures.Remove(this);

            Program.WorldMap.LocalTile.InDungeon = true;
            Program.WorldMap.LocalTile.CurrentFloor += 1; // go down a floor


            Program.WorldMap.LocalTile.Dungeon.Floors[Program.WorldMap.LocalTile.CurrentFloor].Creatures.Add(this);

            CurrentBlock = Program.WorldMap.LocalTile.Dungeon.Floors[Program.WorldMap.LocalTile.CurrentFloor][this.Position.X, this.Position.Y];
            Program.WorldMap.LocalTile.Dungeon.Floors[Program.WorldMap.LocalTile.CurrentFloor][this.Position.X, this.Position.Y] = this;

            Program.WorldMap.LocalTile.DijkstraMaps.CallItemPosChanged(this);
            Program.WorldMap.LocalTile.DijkstraMaps.CallPlayerMoved(this);
            
            Scheduler.InitCreatureListScheduling(Program.WorldMap.LocalTile);
        }

        public void TakeStairsUp()
        {
            ApplyActionCost(12);

            bool removed = Program.WorldMap.LocalTile.Creatures.Remove(this);
            Program.WorldMap.LocalTile.CurrentFloor -= 1;

            if (Program.WorldMap.LocalTile.CurrentFloor == -1)
                Program.WorldMap.LocalTile.InDungeon = false;

            Program.WorldMap.LocalTile.Creatures.Add(this);
            Program.WorldMap.LocalTile.Dungeon.Floors[Program.WorldMap.LocalTile.CurrentFloor + 1][Position.X, Position.Y] = CurrentBlock;

            CurrentBlock = Program.WorldMap.LocalTile.Blocks[Position.X * Program.WorldMap.LocalTile.Width + Position.Y];
            Program.WorldMap.LocalTile.Blocks[Position.X * Program.WorldMap.LocalTile.Width + Position.Y] = this;

            Program.WorldMap.LocalTile.DijkstraMaps.CallItemPosChanged(this);

            Scheduler.InitCreatureListScheduling( Program.WorldMap.LocalTile );
        }

        
        // checks //
        public bool PointNextToSelf(Point point)
        {
            return (Math.Abs(Position.X - point.X) <= 1 && Math.Abs(position.Y - point.Y) <= 1);
        }

        public bool CanCarryItem(Item item)
        {
            double carrying = 0;

            if (inventory.Count != 0)
                foreach (Item i in inventory)
                    carrying += i.Weight;

            if (carrying + item.Weight <= Stats.Attributes[Attribute.Strength] * 2)
                return true;
            return false;
        }


        // changing resources //
        public void ChangeResource(Resource resource, int effect)
        {
            Stats.Resources[resource] += effect;

            if (Stats.Resources[resource] > Stats.Resources[Stats.ReturnResourceMax(resource)])
                Stats.Resources[resource] = Stats.Resources[Stats.ReturnResourceMax(resource)];
            else if (Stats.Resources[resource] < 0)
                Stats.Resources[resource] = 0;

            if (Stats.Resources[Resource.HP] == 0)
                Die();
        }

        public void ApplyActionCost( int maxNumOfSeconds ) {
            double granularity = stats.Attributes[Attribute.Agility] / 300;
            nextActionTime.AddTime( (int)( maxNumOfSeconds * ( 1 - granularity ) ) );
        }


        // get values //

        private int GetArmorSkill(Armor armor)
        {
            if (armor != null)
            {
                if (armor.Material == Material.Brass || armor.Material == Material.Bronze || armor.Material == Material.Copper
                    || armor.Material == Material.Iron || armor.Material == Material.Platinum || armor.Material == Material.Steel)
                    return Stats.Skills[Skill.HeavyArmor];
                else if (armor.Material == Material.Bone || armor.Material == Material.Cloth || armor.Material == Material.Glass
                    || armor.Material == Material.Leather || armor.Material == Material.Silk)
                    return Stats.Skills[Skill.LightArmor];
                else return -1;
            }
            else
                return Stats.Skills[Skill.Unarmored];
        }

        private int GetDefenseValue(DamageType dmgType)
        {
            double helmetPer = .15, chestPer = .35, shirtPer = .1, gauntletsPer = .1, leggingsPer = .2, bootsPer = .1;

            // contains the percentage that a piece of armor gives to the overall armor value.
            double[] armorPercents = new double[6] { helmetPer, chestPer, shirtPer, gauntletsPer, leggingsPer, bootsPer };
            Armor[] armorPieces = Body.GetArmorList().ToArray();

            int armorVal = 0;
            int tempVal = 0;

            int i = 0;
            foreach (Armor armorPiece in armorPieces)
            {
                if (dmgType == DamageType.Blunt)
                    tempVal = (int)(Physics.ImpactYields[armorPiece.Material] * armorPercents[i] + .5);
                else if (dmgType == DamageType.Shear)
                    tempVal = (int)(Physics.ShearYields[armorPiece.Material] * armorPercents[i] + .5);

                foreach (ArmorEnchantment enchant in armorPiece.Enchantments)
                    if (enchant.DamageType == dmgType)
                        tempVal += (int)(enchant.Effect * armorPercents[i]);

                armorVal += (int)(tempVal * (.5 + ((double)GetArmorSkill(armorPiece) / 200)));

                i++;
            }

            return armorVal;
        }

        private int GetWeaponSkill(Item weapon)
        {
            if (weapon != null)
            {
                if (weapon is MeleeWeapon)
                {
                    if (weapon is Sword)
                        return Stats.Skills[Skill.LongBlades];
                    else if (weapon is Dagger)
                        return Stats.Skills[Skill.ShortBlade];
                    else if (weapon is Mace || weapon is Axe)
                        return Stats.Skills[Skill.HeavyWeapons];
                    else if (weapon is Spear)
                        return Stats.Skills[Skill.Spear];
                    else return -1;
                }
                else
                    return 20;
            }
            else
                return Stats.Skills[Skill.Brawling];
        }

        private int GetWeaponCost(Item weapon)
        {
            if (weapon != null)
            {
                if (weapon is MeleeWeapon)
                {
                    if (weapon is Sword)
                        return 8 - (GetWeaponSkill(weapon) / 25);
                    else if (weapon is Dagger)
                        return 6 - (GetWeaponSkill(weapon) / 33);
                    else if (weapon is Mace)
                        return 16 - (GetWeaponSkill(weapon) / 12);
                    else if (weapon is Axe)
                        return 16 - (GetWeaponSkill(weapon) / 12);
                    else if (weapon is Spear)
                        return 8 - (GetWeaponSkill(weapon) / 25);
                    else return -1;
                }
                else
                    return 8 - (GetWeaponSkill(weapon) / 25);
            }
            else
                return 4 - (Stats.Skills[Skill.Brawling] / 25);
        }

        public int GetWepDmg(Item weapon)
        {
            int damage;
            // determine the damage value
            if (weapon != null)
            {
                // minimum skill deals half dmg, max skill deals full damage
                damage = (int)(weapon.Damage * (.5 + (double)GetWeaponSkill(weapon) / 200));
            }
            else // the damage type is bone, for your fist
                damage = (int)(Physics.ShearYields[Material.Bone] * (.5 + (double)Stats.Skills[Skill.Brawling] / 200));
            return damage;

        }

        public DamageType GetWepDmgType(Item weapon)
        {
            DamageType dmgType = DamageType.Blunt;
            if (weapon != null)
                dmgType = weapon.DamageType;

            return dmgType;
        }
        

        // get positions //
        public Point GetNearbyBlockClosestToTargetForMovement(Point target, MapTile map)
        {
            List<Point> nearbyPoints = new List<Point>();
            List<Point> walkablePoints = new List<Point>();

            Point closestPoint = new Point();
            Point nextClosestPoint = new Point();
            Point closestWalkablePoint = new Point();

            nearbyPoints.Add(new Point(Position.X, Position.Y + 1));
            nearbyPoints.Add(new Point(Position.X, Position.Y - 1));
            nearbyPoints.Add(new Point(Position.X + 1, Position.Y));
            nearbyPoints.Add(new Point(Position.X - 1, Position.Y));
            nearbyPoints.Add(new Point(Position.X - 1, Position.Y + 1));
            nearbyPoints.Add(new Point(Position.X + 1, Position.Y - 1));
            nearbyPoints.Add(new Point(Position.X + 1, Position.Y + 1));
            nearbyPoints.Add(new Point(Position.X - 1, Position.Y - 1));

            foreach (Point point in nearbyPoints)
            {
                if (map.PointWithinBounds(point) && map[point.X, point.Y].Solid == false)
                    walkablePoints.Add(point);

                if (point.DistFrom(target) < closestPoint.DistFrom(target))
                {
                    nextClosestPoint = closestPoint;
                    closestPoint = point;
                }
            }

            if (map[closestPoint.X, closestPoint.Y].Solid == true && map[target.X, target.Y].Solid == false)
                closestPoint = nextClosestPoint;

            foreach (Point point in walkablePoints)
            {
                int deltaX = point.X - target.X;
                int deltaY = point.Y - target.Y;
                int closestDeltaX = closestPoint.X - target.X;
                int closestDeltaY = closestPoint.Y - target.Y;

                if (Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= Math.Sqrt(closestDeltaX * closestDeltaX + closestDeltaY * closestDeltaY))
                    closestWalkablePoint = point;
            }

            return closestWalkablePoint;
        }
        
        public Point GetAdjacentFreePointClosestToTarget(Point target, MapTile map)
        {
            List<Point> nearbyPoints = new List<Point>();
            List<Point> walkablePoints = new List<Point>();

            Point closestPoint = new Point(0, 0);

            nearbyPoints.Add(new Point(position.X, position.Y + 1));
            nearbyPoints.Add(new Point(position.X, position.Y - 1));
            nearbyPoints.Add(new Point(position.X + 1, position.Y));
            nearbyPoints.Add(new Point(position.X - 1, position.Y));
            nearbyPoints.Add(new Point(position.X - 1, position.Y + 1));
            nearbyPoints.Add(new Point(position.X + 1, position.Y - 1));
            nearbyPoints.Add(new Point(position.X + 1, position.Y + 1));
            nearbyPoints.Add(new Point(position.X - 1, position.Y - 1));

            foreach (Point point in nearbyPoints)
            {
                Block block = map[point.X, point.Y];

                if (block.Solid == false)
                    walkablePoints.Add(point);
            }

            foreach (Point point in walkablePoints)
            {
                int deltaX = point.X - target.X;
                int deltaY = point.Y - target.Y;
                int closestDeltaX = closestPoint.X - target.X;
                int closestDeltaY = closestPoint.Y - target.Y;

                if (Math.Sqrt(deltaX * deltaX + deltaY * deltaY) <= Math.Sqrt(closestDeltaX * closestDeltaX + closestDeltaY * closestDeltaY))
                    closestPoint = point;
            }

            return closestPoint;
        }

        public List<Point> GetNearbyBlocksOfType(BlockType type)
        {
            List<Point> nearbyPointsOfType = new List<Point>();
            for (int i = Math.Max(0, Position.X - 1); i <= Math.Min(Program.WorldMap.LocalTile.Width - 1, Position.X + 1); i++)
                for (int j = Math.Max(0, Position.Y - 1); j <= Math.Min(Program.WorldMap.LocalTile.Height - 1, Position.Y + 1); j++)
                    if (Program.WorldMap.LocalTile[i, j].Type == type && Position.Equals(new Point(i, j)) == false)
                        nearbyPointsOfType.Add(new Point(i, j));

            return nearbyPointsOfType;
        }

        private Point GetNearbyTreePos(Tree tree)
        {
            for (int i = Position.X - 1; i <= Position.X + 1; i++)
            {
                for (int j = Position.Y - 1; j <= Position.Y + 1; j++)
                {
                    if (Program.WorldMap.LocalTile[i, j] is Tree foundTree && foundTree.Thickness == tree.Thickness)
                        return new Point(i, j);
                }
            }
            return new Point();
        }


        // behaviors //
        public void UpdateFOV()
        {
            visiblePoints = RayCaster.CalculateFOV(sightDist, this).ToList();
        }

        // actions //
        // combat
        public void LaunchAttack(Creature defender)
        {
            if (defender.Alive == false)
                return;
            
            Item weapon;

            // FUNCTIONS
            void DetermineWeapon()
            {
                weapon = Body.MainHand;
            } // unfinished! needs to switch from main to off hand.

            bool DetermineIfAttackLanded()
            {
                double attackersHitRate =
                (GetWeaponSkill(this.Body.MainHand) + (this.Stats.Attributes[Attribute.Dexterity] / 2.5) + (Program.RNG.Next(0, this.Stats.Attributes[Attribute.Luck]) / 10))
                    * (0.75 + 0.5 * this.Stats.Resources[Resource.SP] / this.Stats.Resources[Resource.MaxSP]);

                double defendersEvasion =
                    ((defender.Stats.Attributes[Attribute.Agility] / 5) + (Program.RNG.Next(0, defender.Stats.Attributes[Attribute.Luck]) / 10))
                        * (0.75 + 0.5 * defender.Stats.Resources[Resource.SP] / defender.Stats.Resources[Resource.MaxSP]);

                double maxMissChance = 150;
                double chanceToMiss = maxMissChance - attackersHitRate;
                double chanceToDodge = attackersHitRate - (attackersHitRate - defendersEvasion);

                int diceRoll = Program.RNG.Next( 0, (int)maxMissChance );
                if (diceRoll <= chanceToMiss)
                {
                    Program.MsgConsole.WriteLine($"{Name}'s attack missed.");
                    LvlWeaponSkill(weapon, 5);
                    return false;
                }

                diceRoll = Program.RNG.Next(0, (int)attackersHitRate + 2);
                if (diceRoll <= chanceToDodge)
                {
                    Program.MsgConsole.WriteLine($"{defender.Name} evaded {Name}'s attack.");
                    LvlWeaponSkill(weapon, 10);
                    return false;
                }

                LvlWeaponSkill(weapon, 25);

                return true;
            }


            // START
            DetermineWeapon();
            
            ApplyActionCost(GetWeaponCost(weapon));

            bool attackLanded = DetermineIfAttackLanded();
            if (attackLanded)
            {
                int damage = GetWepDmg(weapon);

                DamageType dmgType = GetWepDmgType(weapon);

                int dmgDealt = defender.DefendAgainstDmg(dmgType, damage);

                if (dmgDealt <= 0)
                    goto Finish;

                //
                ////
                //////
                if (weapon != null && weapon is MeleeWeapon && ((MeleeWeapon)weapon).Enchantments.Count > 0)
                    foreach (WeaponEnchantment enchantment in ((MeleeWeapon)weapon).Enchantments)
                        enchantment.Apply(this, defender);
                //////
                ////
                //

                Program.MsgConsole.WriteLine($"{Name} attacked for {dmgDealt} damage!");

                Finish:
                if (defender.Alive == false)
                    Program.MsgConsole.WriteLine($"{defender.Name} died.");
            }

            // deplete stamina
            if (weapon != null)
                ChangeResource(Resource.SP, -(int)(weapon.Weight * 2));
            else
                ChangeResource( Resource.SP, -8 );
        }

        private void LvlWeaponSkill( Item weapon, int amount )
        {
            if (weapon is MeleeWeapon mWeapon)
                Stats.LvlSkill( mWeapon.GetWeaponSkill(), amount, this );
        }

        private void LvlArmorSkill(int amount)
        {
            List<Armor> equipment = Body.GetArmorList();
            int lightArmorPieces = equipment.Count( armor => armor.GetSkill() == Skill.LightArmor );
            int heavyArmorPieces = equipment.Count( armor => armor.GetSkill() == Skill.HeavyArmor );

            if (lightArmorPieces == heavyArmorPieces)
            {
                Stats.LvlSkill( Skill.LightArmor, amount / 2, this );
                Stats.LvlSkill( Skill.HeavyArmor, amount / 2, this );
            }
            if (lightArmorPieces > heavyArmorPieces)
                Stats.LvlSkill( Skill.LightArmor, amount, this );
            else if (lightArmorPieces == 0 && heavyArmorPieces == 0)
                Stats.LvlSkill( Skill.Unarmored, amount, this );
            else
                Stats.LvlSkill( Skill.HeavyArmor, amount, this );
        }

        public int DefendAgainstDmg( DamageType dmgType, int dmg )
        {
            // Note: this function will return a negative value if the defender blocked. This is for message handling.

            int armorVal = GetDefenseValue(dmgType);
            
            double finalDmg = Math.Max(dmg - armorVal, 1);

            // blocking //
            if ( Body.OffHand is Shield && (dmgType == DamageType.Blunt || dmgType == DamageType.Shear) )
            {
                int blockChance = 10 + ( Stats.Skills[Skill.Block] / 5 );
                if (Program.RNG.Next(0, 101) <= blockChance)
                {
                    finalDmg /= -((double)2 + ((Stats.Attributes[Attribute.Strength] + Stats.Attributes[Attribute.Strength]) / 400));
                    Program.MsgConsole.WriteLine($"{Name} blocked the attack, taking {Math.Abs((int)finalDmg)} damage.");
                    LvlWeaponSkill((MeleeWeapon)body.OffHand, 25);
                }
            }

            LvlArmorSkill( 25 );

            ChangeResource(Resource.HP, Math.Abs((int)finalDmg) * -1);

            ChangeResource(Resource.SP, Math.Abs((int)finalDmg) * -1);

            return (int)finalDmg;
        }
        
        private void Die()
        {
            Graphic = 37;
            ForeColor = Color.Red;
            Solid = false;
            Opaque = false;
            alive = false;
            if (this is Player == false) UnequipAll();
        }
        // movement
        public void Move( Point to, MapTile map, bool openDoors = false )
        {
            if ( map[to.X, to.Y] is Door door && door.Solid == true && openDoors ) {
                OpenDoor( door );
                return;
            }
            if ( map[to.X, to.Y] is Cart cart && cart.Solid == true )
            {
                int xDir = to.X - position.X, yDir = to.Y - position.Y;
                PushCart( map, to, new Point(to.X + xDir, to.Y + yDir) );
                return;
            }

            bool destinationNotValid = ((to == Position || to == new Point()) || (to.X < 0 || to.X >= map.Width) || (to.Y < 0 || to.Y >= map.Width)) || map[to.X, to.Y].Solid;

            if (destinationNotValid)
                return;
            
            map[position.X, position.Y] = currentBlock;
            currentBlock = map[to.X, to.Y];

            map[to.X, to.Y] = this;
            position = to;

            ChangeResource(Resource.SP, 2);

            if (this is Player)
                Program.WorldMap.LocalTile.DijkstraMaps.CallPlayerMoved(this);

            ApplyActionCost(8);
        }

        public void Wait()
        {
            ApplyActionCost(4);
            ChangeResource(Resource.SP, 2);
        }
        // block interactions
        public void OpenDoor( Block door )
        {
            if (door.Solid == true)
            {
                door.Graphic = 45; // -
                door.Solid = false;
                door.Opaque = false;
            }
            else
            {
                door.Graphic = 43; // +
                door.Solid = true;
                door.Opaque = true;
            }
            ApplyActionCost(4);
        }

        public void PushCart( MapTile map, Point oldPos, Point newPos )
        {
            if (this is Player && Program.CurrentState is Play play && play.PlayMode == PlayMode.BuildMode)
                return;

            Block cart = map[oldPos.X, oldPos.Y];

            void PushToMap(MapTile pushTo)
            {
                if (pushTo[newPos.X, newPos.Y].Solid == false)
                {
                    pushTo[newPos.X, newPos.Y] = cart;
                    map[oldPos.X, oldPos.Y] = new Air();
                    Program.MsgConsole.WriteLine( $"{Name} pushed the {cart.Name}" );
                } else
                    Program.MsgConsole.WriteLine( $"{Name} tried to push the {cart.Name} into a {pushTo[newPos.X, newPos.Y].Name}" );
            }

            void PushToDifferentMap()
            {
                int xDir = newPos.X >= 0 && newPos.X < map.Width ? 0 : newPos.X >= map.Width ?  1 : -1, 
                    yDir = newPos.Y >= 0 && newPos.Y < map.Height ? 0 : newPos.Y >= map.Height ? 1 : -1;
                MapTile newMap = Program.WorldMap[Program.WorldMap.WorldIndex.X + xDir, Program.WorldMap.WorldIndex.Y + yDir];
                newPos = new Point( newPos.X - (xDir * map.Width ) + xDir, newPos.Y - (yDir * map.Height) + yDir );

                PushToMap( newMap );
            }

            if (newPos.X >= 0 && newPos.Y >= 0 && newPos.X < map.Width && newPos.Y < map.Height)
                PushToMap( map );
            else
                PushToDifferentMap();
            ApplyActionCost( 20 );
        }

        public void ChopTree( Tree tree )
        {
            Item weapon = body.MainHand;
            if (weapon is Axe || weapon is Sword)
            {
                Point pos = GetNearbyTreePos(tree);

                tree.Thickness -= GetWepDmg(weapon);
                ApplyActionCost(GetWeaponCost(weapon));
                if (Program.RNG.Next(0, 100) < 10)
                    LvlWeaponSkill(weapon, 5);
                // deplete stamina
                ChangeResource( Resource.SP, -(int)( weapon.Weight * 2 ) );
                Program.MsgConsole.WriteLine($"{Name} chopped the tree.");
                
                if (tree.Thickness <= 0)
                {
                    tree.DropLogs(pos);
                    if (Program.CurrentState is Play play && play.PlayMode == PlayMode.BuildMode)
                        Wield(inventory.FindIndex(i => i is BlueprintPouch), true);
                }
            }
        }

        public void PickWall( Wall wall )
        {
            if ( !(wall.Material == Material.Stone || wall.Material == Material.Coal) )
                return;

            if (Body.MainHand is Axe axe && axe.WeaponName == "pickaxe")
            {
                Program.MsgConsole.WriteLine( $"{Name} struck the {wall.Name}" );
                ApplyActionCost( GetWeaponCost( axe ) );
                if (Program.RNG.Next( 0, 100 ) < 10)
                    LvlWeaponSkill( axe, 5 );
                ChangeResource( Resource.SP, -(int)( axe.Weight * 2 ) );
                wall.HP -= (int)Physics.ImpactYields[axe.Material] - (int)Physics.ImpactYields[Material.Stone];
            }

            if (wall.HP <= 0)
            {
                Item drop = new Stone( true );
                if (wall.Material == Material.Coal)
                    drop = new CoalOre( true );
                Point pos = new Point();
                List<Point> potentialSpots = GetNearbyBlocksOfType( BlockType.Wall );
                potentialSpots.AddRange( GetNearbyBlocksOfType( BlockType.OreWall ) );
                pos = potentialSpots.Find( w => ( (Wall)Program.WorldMap.LocalTile[w.X, w.Y] ).HP == wall.HP );
                
                Program.WorldMap.LocalTile[pos.X, pos.Y] = drop;
                Program.MsgConsole.WriteLine( $"The {wall.Name} collapsed!" );
            }
        }
        // npc interactions
        public void StartDialog(Creature conversant)
        {
            Console.WriteLine("Initiated an imaginary conversation!");
        }
        // item interactions
        public void GetItem(Point itemPos)
        {
            if (Program.WorldMap.LocalTile[itemPos.X, itemPos.Y] is Item item)
            {
                bool itemAdded = AddItem(item);
                if (itemAdded)
                {
                    Block replacementBlock = item.BlockPlacedOn ?? new Air();
                    Program.WorldMap.LocalTile[itemPos.X, itemPos.Y] = replacementBlock;
                    Program.WorldMap.LocalTile.DijkstraMaps.CallItemPosChanged(this);
                    if (this.Visible)
                        Program.MsgConsole.WriteLine($"{Name} picked up the {item.Name}");
                    ApplyActionCost(4);
                } else
                    Program.MsgConsole.WriteLine($"{Name} tried to pick up the {item.Name} but didn't have enough space.");
            }
        }

        public bool AddItem(Item item)
        {
            ApplyActionCost(6);
            bool canCarry = CanCarryItem(item);

            if (canCarry)
                inventory.Add(item);

            return canCarry;
        }

        public void Drink(Item item)
        {
            if (item.ItemType == ItemType.Potion)
            {
                int index = Inventory.IndexOf(item);

                Potion potion = (Potion)item;

                potion.Activate(this);

                inventory.Remove(item);
                inventory.Insert(index, new EmptyBottle(true));

                ApplyActionCost(6);

                if (this.Visible)
                    Program.MsgConsole.WriteLine($"{Name} drank the {item.Name}");
            }
            else
                Menus.DisplayIncorrectUsage("You can't drink that.");
        }

        public void Drop(Item item)
        {
            List<Point> freeDirections = Program.WorldMap.LocalTile.GetEmptyAdjacentBlocks(position);

            if (freeDirections.Count == 0)
            {
                Menus.DisplayIncorrectUsage("There's no space to drop that.");
                return;
            }

            Point point = freeDirections[new Random().Next(0, freeDirections.Count)];

            item.BlockPlacedOn = Program.WorldMap.LocalTile[point.X, point.Y];
            Program.WorldMap.LocalTile[point.X, point.Y] = item;

            inventory.Remove(item);

            Program.MsgConsole.WriteLine($"{Name} dropped the {item.Name}.");

            ApplyActionCost(10);

            Program.WorldMap.LocalTile.DijkstraMaps.CallItemPosChanged(this);
        }
        // equipment interactions
        public void Wield(int itemIndex, bool mainHand)
        {
            if (mainHand)
            {
                if (body.MainHand != null)
                    inventory.Add(body.MainHand);
                body.MainHand = inventory[itemIndex];
            }
            else
            {
                if (body.OffHand != null)
                    inventory.Add(body.OffHand);
                body.OffHand = inventory[itemIndex];
            }
            
            if (this.Visible)
                Program.MsgConsole.WriteLine($"{Name} wielded the {inventory[itemIndex].Name}");

            inventory.RemoveAt(itemIndex);

            ApplyActionCost(10);
        }

        public void Equip(Armor armorPiece)
        {
            if (armorPiece is Helmet helmet)
            {
                if (body.Helmet != null)
                    inventory.Add(body.Helmet);
                body.Helmet = helmet;
            }
            else if (armorPiece is ChestPiece chestPiece)
            {
                if (body.ChestPiece != null)
                    inventory.Add(body.ChestPiece);
                body.ChestPiece = chestPiece;
            }
            else if (armorPiece is Shirt shirt)
            {
                if (body.Shirt != null)
                    inventory.Add(body.Shirt);
                body.Shirt = shirt;
            }
            else if (armorPiece is Gauntlets gauntlets)
            {
                if (body.Gauntlets != null)
                    inventory.Add(body.Gauntlets);
                body.Gauntlets = gauntlets;
            }
            else if (armorPiece is Leggings leggings)
            {
                if (body.Leggings != null)
                    inventory.Add(body.Leggings);
                body.Leggings = leggings;
            }
            else if (armorPiece is Boots boots)
            {
                if (body.Boots != null)
                    inventory.Add(body.Boots);
                body.Boots = boots;
            }

            if (this.Visible)
                Program.MsgConsole.WriteLine($"{Name} equipped the {armorPiece.Name}");

            inventory.Remove(armorPiece);

            ApplyActionCost(18);
        }

        public void Unequip(Item item)
        {
            if (item is Armor)
            {
                if (item is Helmet)
                {
                    bool addedItem = AddItem(body.Helmet);
                    if (addedItem) body.Helmet = null;
                }
                else if (item is ChestPiece)
                {
                    bool addedItem = AddItem(body.ChestPiece);
                    if (addedItem) body.ChestPiece = null;
                }
                else if (item is Shirt)
                {
                    bool addedItem = AddItem(body.Shirt);
                    if (addedItem) body.Shirt = null;
                }
                else if (item is Gauntlets)
                {
                    bool addedItem = AddItem(body.Gauntlets);
                    if (addedItem) body.Gauntlets = null;
                }
                else if (item is Leggings)
                {
                    bool addedItem = AddItem(body.Leggings);
                    if (addedItem) body.Leggings = null;
                }
                else if (item is Boots)
                {
                    bool addedItem = AddItem(body.Boots);
                    if (addedItem) body.Boots = null;
                }

                ApplyActionCost(16);
            }
            else
            {
                if (body.OffHand != null && body.OffHand.Equals(item))
                {
                    bool addedItem = AddItem(body.OffHand);
                    if (addedItem) body.OffHand = null;
                }
                else if (body.MainHand != null && body.MainHand.Equals(item))
                {
                    bool addedItem = AddItem(body.MainHand);
                    if (addedItem)
                        body.MainHand = null;
                    else
                    {
                        inventory.Add( body.MainHand );
                        Drop( body.MainHand );
                        body.MainHand = null;
                        ApplyActionCost( 8 );
                    }
                }

                ApplyActionCost(8);
            }

            if (this.Visible && this.Alive)
                Program.MsgConsole.WriteLine($"{Name} unequipped the {item.Name}");
        }

        public void UnequipAll()
        {
            if (body.Helmet != null)
                Unequip(body.Helmet);
            if (body.ChestPiece != null)
                Unequip(body.ChestPiece);
            if (body.Shirt != null)
                Unequip(body.Shirt);
            if (body.Gauntlets != null)
                Unequip(body.Gauntlets);
            if (body.Leggings != null)
                Unequip(body.Leggings);
            if (body.Boots != null)
                Unequip(body.Boots);
            if (body.MainHand != null)
                Unequip(body.MainHand);
            if (body.OffHand != null)
                Unequip(body.OffHand);
        }


        // PROPERTIES //

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public int SightDist
        {
            get { return sightDist; }
            set { sightDist = value; }
        }

        public float Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public Stats Stats
        {
            get { return stats; }
            set { stats = value; }
        }

        public Class Class
        {
            get { return uclass; }
            set { uclass = value; }
        }

        public List<Effect> Effects
        {
            get { return effects; }
            set { effects = value; }
        }

        public Block CurrentBlock
        {
            get { return currentBlock; }
            set { currentBlock = value;}
        }

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        public bool Friendly
        {
            get { return friendly; }
            set { friendly = value; }
        }

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }

        public Time NextActionTime
        {
            get { return nextActionTime; }
            set { nextActionTime = value; }
        }

        public List<Item> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public List<Point> VisiblePoints
        {
            get { return visiblePoints; }
            set { visiblePoints = value; }
        }

        public Body Body
        {
            get { return body; }
            set { body = value; }
        }
    }
}
