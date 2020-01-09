using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Landlord
{
    public enum DietType
    {
        Herbivore,
        Carnivore,
        Omnivore
    }
    abstract class Creature : Block , IComparable<Creature>, IEquatable<Creature>
    {
        private Point position;
        private Block currentBlock;
        private Point worldIndex;
        private int currentFloor;
        private string faction;
        private UInt64 id;

        private int sightDist;
        private List<Point> visiblePoints;

        private double gold;
        private List<Item> inventory;
        private string gender;
        private DietType diet;
        private Body body;
        private bool alive;
        private Stats stats;
        private Class uclass;
        private List<Effect> effects;

        private Time nextActionTime;
        
        // CONSTRUCTORS //

        public Creature (Block[] map, Point position, Point worldIndex, int currentFloor, int sightDist, byte graphic, string name, string gender, DietType diet, string faction,
            bool solid, bool opaque, BlockType type = BlockType.Creature, bool interactive = true, bool enterable = false) 
                : base (graphic, name, type, solid, opaque, interactive, enterable)
        {
            id = (UInt64)((DateTime.Now - new DateTime(year: 2019, month: 6, day: 27)).TotalSeconds * 100);
            inventory = new List<Item>();
            visiblePoints = new List<Point>();
            body = new Body();
            currentBlock = map[position.X * 100 + position.Y];
            BackColor = Color.Pink;
            nextActionTime = new Time(Program.TimeHandler.CurrentTime);
            effects = new List<Effect>();
            gold = 0F;
            alive = true;

            this.position = position;
            this.worldIndex = worldIndex;
            this.currentFloor = currentFloor;
            this.sightDist = sightDist;
            this.gender = gender;
            this.diet = diet;
            this.faction = faction;
        }

        public Creature() : base() { }

        // FUNCTIONS //

        
        // abstract/override funcs //
        public abstract void DetermineAction();
        public abstract void DetermineStats();
        public abstract void HandleVisibility();
        public override void Activate(Creature user)
        {
            if (alive) {
                if (faction.Equals(user.Faction) == false)
                    user.LaunchAttack(this);
                else
                    user.StartDialog(this);
            }
            else {
                Program.Animations.Add(new OpenLootView());
                Program.CurrentState = new ViewLoot(inventory, 20, Name);
            }
        }

        public bool Equals(Creature other)
        {
            return id.Equals(other.ID);
        }
        public int CompareTo(Creature other)
        {
            return nextActionTime.Minus(other.NextActionTime);
        }

        public void DetermineEquipment()
        {
            Random rng = new Random();
            bool twoHanded = true;
            if (Class.MajorSkills.Contains(Skill.Block) || Class.MinorSkills.Contains(Skill.Block)) {
                body.OffHand = new Shield(false);
                twoHanded = false;
            }
            if (Class.MajorSkills.Contains(Skill.HeavyWeapons))
                body.MainHand = new Mace(twoHanded);
            else if (Class.MajorSkills.Contains(Skill.LongBlades))
                body.MainHand = new Sword(twoHanded);
            else if (Class.MajorSkills.Contains(Skill.Spear))
                body.MainHand = new Spear(true);
            else if (Class.MajorSkills.Contains(Skill.ShortBlade)) {
                body.MainHand = new Dagger(false);
                if (twoHanded) body.OffHand = new Dagger(false);
            }
            if (Class.MajorSkills.Contains(Skill.Marksmanship) || Class.MinorSkills.Contains(Skill.Marksmanship))
                    { inventory.Add(new Bow(true)); inventory.Add(new Quiver(false)); }
            
            // determine valid Materials
            Skill armorSkill = Skill.Alchemy;
            foreach (Skill skill in Class.MinorSkills)
                if (skill == Skill.HeavyArmor || skill == Skill.LightArmor)
                    armorSkill = skill;
            foreach (Skill skill in Class.MajorSkills)
                if (skill == Skill.HeavyArmor || skill == Skill.LightArmor)
                    armorSkill = skill;

            List<Material> validMaterials;
            if (armorSkill == Skill.Alchemy) return; // skip generating armor
            else  validMaterials = Physics.GetArmorSkillMaterials(armorSkill);

            // create the armor with chosen materials
            if (rng.Next(0, 100) < 50)
            do { body.Helmet = new Helmet(true); } while (validMaterials.Contains(body.Helmet.Material) == false);
            if (rng.Next(0, 100) < 50)
                do { body.ChestPiece = new ChestPiece(true); } while (validMaterials.Contains(body.ChestPiece.Material) == false);
            do { body.Shirt = new Shirt(true); } while (validMaterials.Contains(body.Shirt.Material) == false);
            if (rng.Next(0, 100) < 50)
                do { Body.Gauntlets = new Gauntlets(true); } while (validMaterials.Contains(body.Gauntlets.Material) == false);
            do { body.Leggings = new Leggings(true); } while (validMaterials.Contains(body.Leggings.Material) == false);
            do { body.Boots = new Boots(true); } while (validMaterials.Contains(body.Boots.Material) == false);
        }
        
        public bool MoveMaps(Point to, int xDir, int yDir)
        {
            int width = Program.WorldMap.TileWidth;

            if (currentFloor >= 0)
                return false;
            if (Program.WorldMap[WorldIndex.X + xDir, WorldIndex.Y + yDir].Blocks[to.X * width + to.Y].Solid)
                return false;

            Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks[position.X * width + position.Y] = currentBlock;

            currentBlock = Program.WorldMap[WorldIndex.X + xDir, WorldIndex.Y + yDir].Blocks[to.X * width + to.Y];
            position = to;
            worldIndex = new Point(worldIndex.X + xDir, worldIndex.Y + yDir);

            Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures.Add(this);
            Program.WorldMap[worldIndex.X - xDir, worldIndex.Y - yDir].Creatures.Remove(this);
            Program.WorldMap[worldIndex.X, worldIndex.Y][to.X, to.Y] = this;

            if (this is Player)
                Scheduler.InitCreatureListScheduling(Program.WorldMap[worldIndex.X, worldIndex.Y]);

            ApplyActionCost(8);
            return true;
        }
        public void TakeStairsDown()
        {
            if (!Program.WorldMap[worldIndex.X, worldIndex.Y].Owned && this is Player) {
                Menus.DisplayIncorrectUsage("You must purchase this plot. Use [m].");
                return;
            }

            bool dungeonNotCreated = Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon == null;
            if (dungeonNotCreated) {
                if (this is Player)
                    Program.CurrentState = new GeneratingDungeon();
                return; // program will stop here and generate the map, then recall this function and skip to the rest
            }

            bool onFinalFloor = currentFloor == Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors.GetLength(0) - 1;
            if (onFinalFloor)
                return;

            ApplyActionCost(12);

            Block[] blocksFrom = currentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Blocks;
            List<Creature> creaturesFrom = currentFloor >= 0 ? Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[WorldIndex.X, WorldIndex.Y].Creatures;

            blocksFrom[this.Position.X * Program.WorldMap.TileWidth + this.Position.Y] = currentBlock;
            creaturesFrom.Remove(this);

            currentFloor += 1; // go down a floor

            Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[currentFloor].Creatures.Add(this);
            CurrentBlock = Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[currentFloor][this.Position.X, this.Position.Y];
            Program.WorldMap[WorldIndex.X, WorldIndex.Y].Dungeon.Floors[currentFloor][this.Position.X, this.Position.Y] = this;

            if (this is Player) {
                Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.CallItemPosChanged(this);
                Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.CallPlayerMoved(this);
                Scheduler.InitCreatureListScheduling(Program.WorldMap[worldIndex.X, worldIndex.Y]);
            }
            
        }
        public void TakeStairsUp()
        {
            ApplyActionCost(12);

            Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor][Position.X, Position.Y] = currentBlock;
            Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures.Remove(this);
            currentFloor -= 1;

            List<Creature> creatures = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Creatures : Program.WorldMap[worldIndex.X, worldIndex.Y].Creatures;
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;

            creatures.Add(this);

            currentBlock = blocks[Position.X * Program.WorldMap.TileWidth + Position.Y];
            blocks[Position.X * Program.WorldMap.TileWidth + Position.Y] = this;

            if (this is Player) {
                Program.WorldMap[worldIndex.X, worldIndex.Y].DijkstraMaps.CallItemPosChanged(this);
                Scheduler.InitCreatureListScheduling(Program.WorldMap[worldIndex.X, worldIndex.Y]);
            }
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
        public void ApplyActionCost( int maxNumOfSeconds )
        {
            double granularity = stats.Attributes[Attribute.Agility] / 300;
            int timeToAdd = (int)(maxNumOfSeconds * (1 - granularity));
            nextActionTime.AddTime(timeToAdd);
            if (new Random().Next(0, 100) <= 2)
                ChangeResource(Resource.HV, -(timeToAdd / 4));
            if (Stats.Resources[Resource.HV] == 0 && new Random().Next(0, 100) <= 1)
                ChangeResource(Resource.HP, -1);
            if (stats.Resources[Resource.SP] >= stats.Resources[Resource.SP] * 0.8F && Program.TimeHandler.CurrentTime.Second % 10 == 0) {
                if (stats.Resources[Resource.HP] < stats.Resources[Resource.MaxHP]) stats.Resources[Resource.HP] += 1;
            }
            if (stats.Resources[Resource.HP] > stats.Resources[Resource.MaxHP] * 0.8)
                this.Splattered = false;
            ApplyEffects();
        }

        public List<Point> GetNearbyBlocksOfType(BlockType type)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            List<Point> nearbyPointsOfType = new List<Point>();
            for (int i = Math.Max(0, position.X - 1); i <= Math.Min(width - 1, position.X + 1); i++)
                for (int j = Math.Max(0, position.Y - 1); j <= Math.Min(height - 1, position.Y + 1); j++)
                    if (blocks[i * width + j].Type == type)
                        nearbyPointsOfType.Add(new Point(i, j));

            return nearbyPointsOfType;
        }

        public void UpdateFOV()
        {
            int effectiveSightDist = (Body.MainHand is Torch == false) ? 
                                        ((currentFloor < 0) ? Program.TimeHandler.GetOutsideSightDist(sightDist) : (int)(sightDist * 0.3F)) : 
                                                    sightDist;
            visiblePoints = RayCaster.CalculateFOV(effectiveSightDist, this).ToList();
        }

        public void LaunchAttack(Creature defender)
        {
            Random rng = new Random();
            if (defender.Alive == false)
                return;
            
            Item weapon;

            // FUNCTIONS
            void DetermineWeapon() { weapon = Body.MainHand; } 
            // unfinished! needs to switch from main to off hand.

            bool DetermineIfAttackLanded()
            {
                double attackersHitRate =
                (GetWeaponSkill(this.Body.MainHand) + (this.Stats.Attributes[Attribute.Dexterity] / 2.5) + (rng.Next(0, this.Stats.Attributes[Attribute.Luck]) / 10))
                    * (0.75 + 0.5 * this.Stats.Resources[Resource.SP] / this.Stats.Resources[Resource.MaxSP]);

                double defendersEvasion =
                    ((defender.Stats.Attributes[Attribute.Agility] / 5) + (rng.Next(0, defender.Stats.Attributes[Attribute.Luck]) / 10))
                        * (0.75 + 0.5 * defender.Stats.Resources[Resource.SP] / defender.Stats.Resources[Resource.MaxSP]);

                double maxMissChance = 150;
                double chanceToMiss = maxMissChance - attackersHitRate;
                double chanceToDodge = attackersHitRate - (attackersHitRate - defendersEvasion);

                int diceRoll = rng.Next( 0, (int)maxMissChance );
                if (diceRoll <= chanceToMiss) {
                    Program.MsgConsole.WriteLine($"{Name}'s attack missed.");
                    LvlWeaponSkill(weapon, 10);
                    return false;
                }

                diceRoll = rng.Next(0, (int)attackersHitRate + 2);
                if (diceRoll <= chanceToDodge) {
                    Program.MsgConsole.WriteLine($"{defender.Name} evaded {Name}'s attack.");
                    LvlWeaponSkill(weapon, 20);
                    return false;
                }

                LvlWeaponSkill(weapon, 40);

                return true;
            }


            // START
            DetermineWeapon();
            
            ApplyActionCost(weapon.GetWeaponCost(this));

            bool attackLanded = DetermineIfAttackLanded();
            if (attackLanded) {
                int multiplier = 1;
                if (weapon is Weapon w && w.TwoHanded) multiplier = 2;
                int damage = weapon.GetWepDmg(this) * multiplier;

                DamageType dmgType = weapon.GetWepDmgType();

                int dmgDealt = defender.DefendAgainstDmg(dmgType, damage, position);

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
        public int DefendAgainstDmg(DamageType dmgType, int dmg, Point dmgAngle)
        {
            // Note: this function will return a negative value if the defender blocked. This is for message handling.
            Random rng = new Random();
            int armorVal = GetDefenseValue(dmgType);

            double finalDmg = Math.Max(dmg - armorVal, 1);

            // blocking //
            if (Body.OffHand is Shield && (dmgType == DamageType.Blunt || dmgType == DamageType.Shear))
            {
                int blockChance = 10 + (Stats.Skills[Skill.Block] / 5);
                if (rng.Next(0, 101) <= blockChance)
                {
                    finalDmg /= -((double)2 + ((Stats.Attributes[Attribute.Strength] + Stats.Attributes[Attribute.Strength]) / 400));
                    Program.MsgConsole.WriteLine($"{Name} blocked the attack, taking {Math.Abs((int)finalDmg)} damage.");
                    LvlWeaponSkill((MeleeWeapon)body.OffHand, 25);
                }
            }

            LvlArmorSkill(25);

            ChangeResource(Resource.HP, Math.Abs((int)finalDmg) * -1);

            if (finalDmg > 0) SplatterBlood(dmgAngle);

            ChangeResource(Resource.SP, Math.Abs((int)finalDmg) * -1);

            if (this is Monster m)
            {
                m.TurnsSinceAttentionCaught = m.Persistence;
            }

            return (int)finalDmg;
        }
        public void Shoot(Point pos)
        {
            Random rng = new Random();
            Item weapon = body.MainHand;

            bool DetermineIfAttackLanded()
            {
                double attackersHitRate =
                (GetWeaponSkill(this.Body.MainHand) + (this.Stats.Attributes[Attribute.Dexterity] / 5) + (rng.Next(0, this.Stats.Attributes[Attribute.Luck]) / 10))
                    * (0.75 + 0.5 * this.Stats.Resources[Resource.SP] / this.Stats.Resources[Resource.MaxSP]);

                double maxMissChance = 150;
                double chanceToMiss = maxMissChance - attackersHitRate;

                int diceRoll = rng.Next(0, (int)maxMissChance);
                if (diceRoll <= chanceToMiss) {
                    LvlWeaponSkill(weapon, 15);
                    return false;
                }

                LvlWeaponSkill(weapon, 50);
                return true;
            }

            Item GetArrow()
            {
                Quiver q = null;
                for(int i = inventory.Count - 1; i >= 0; i--) {
                    Item item = inventory[i];
                    if (item is Arrow /* || item is Bolt */) { inventory.RemoveAt(i); return item; }
                    else if (item is Quiver) q = (Quiver)item;
                }
                if (q != null && q.Arrows.Exists(it => it is Arrow)) { int index = q.Arrows.FindIndex(it => it is Arrow); Item item = q.Arrows[index]; q.Arrows.RemoveAt(index); return item; }
                return null;
            }
            Item arrow = GetArrow();
            if (body.MainHand is RangedWeapon rw && arrow != null) {
                List<Projectile> projectiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Projectiles : Program.WorldMap[worldIndex.X, worldIndex.Y].Projectiles;
                Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

                bool shotTrue = DetermineIfAttackLanded();
                if (!shotTrue) {
                    List<Point> otherSpots = pos.GetAdjacentPoints();
                    pos = otherSpots[rng.Next(0, otherSpots.Count)];
                }
                projectiles.Add(new Projectile(blocks, width, height, position, pos, arrow));
                if (this is Player) Program.MsgConsole.WriteLine(shotTrue ? $"Shot the {arrow.Name}" : $"Clumsily shot the {arrow.Name}");
                ChangeResource(Resource.SP, -(int)(rw.Weight * 10));
                ApplyActionCost(6);
            }
            else if (this is Player) {
                if (body.MainHand is RangedWeapon == false) Program.MsgConsole.WriteLine("Nothing to shoot with!");
                else if (arrow == null) Program.MsgConsole.WriteLine("Out of arrows!");
            }
        }

        public void Move( Point to, bool openDoors = true )
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            bool destinationNotValid = ((to.Equals(position) || to.Equals(new Point()) || (to.X < 0 || to.X >= width) || (to.Y < 0 || to.Y >= height)));

            if (destinationNotValid)
                return;

            if (blocks[to.X * width + to.Y].Solid) {
                if (blocks[to.X * width + to.Y] is Door door && openDoors) {
                    OpenDoor(door);
                    return;
                }
                else if (blocks[to.X * width + to.Y] is Cart cart) {
                    int xDir = to.X - position.X, yDir = to.Y - position.Y;
                    PushCart(to, new Point(to.X + xDir, to.Y + yDir));
                    return;
                }
                else
                    return;
            }

            if (to.Equals(new Point()))
                return;

            blocks[position.X * width + position.Y] = currentBlock;
            currentBlock = blocks[to.X * width + to.Y];

            blocks[to.X * width + to.Y] = this;
            position = to;

            ChangeResource(Resource.SP, 2);
            if (Program.RNG.Next(0, 30) < 1)
                Stats.LvlSkill(Skill.Acrobatics, 3, this);

            if (this is Player)
                Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.CallPlayerMoved(this);

            ApplyActionCost(6);
        }
        public void Wait()
        {
            ApplyActionCost(1);
            stats.Resources[Resource.SP]++;
        }

        public void OpenDoor( Block door )
        {
            string verb;
            if (door.Solid == true) {
                door.Graphic = 45; // -
                door.Solid = false;
                door.Opaque = false;
                verb = "opened";
            }
            else {
                door.Graphic = 43; // +
                door.Solid = true;
                door.Opaque = true;
                verb = "closed";
            }
            if (this.Visible)
                Program.MsgConsole.WriteLine($"{Name} {verb} the {door.Name}");
            ApplyActionCost(4);
        }
        public void PushCart( Point oldPos, Point newPos )
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;

            if (this is Player && Program.CurrentState is Play play && play.PlayMode == PlayMode.BuildMode)
                return;

            Block cart = blocks[oldPos.X * width + oldPos.Y];

            void PushToMap(MapTile pushTo)
            {
                if (pushTo[newPos.X, newPos.Y].Solid == false) {
                    pushTo[newPos.X, newPos.Y] = cart;
                    blocks[oldPos.X * width + oldPos.Y] = new Air();
                    if (this.Visible)
                        Program.MsgConsole.WriteLine( $"{Name} pushed the {cart.Name}" );
                }
                else
                    Program.MsgConsole.WriteLine( $"{Name} tried to push the {cart.Name} into a {pushTo[newPos.X, newPos.Y].Name}" );
            }

            void PushToDifferentMap()
            {
                int xDir = newPos.X >= 0 && newPos.X < width ? 0 : newPos.X >= width ?  1 : -1, 
                    yDir = newPos.Y >= 0 && newPos.Y < height ? 0 : newPos.Y >= height ? 1 : -1;
                MapTile newMap = Program.WorldMap[worldIndex.X + xDir, worldIndex.Y + yDir];
                newPos = new Point( newPos.X - (xDir * width ) + xDir, newPos.Y - (yDir * height) + yDir );

                PushToMap( newMap );
            }

            if (newPos.X >= 0 && newPos.Y >= 0 && newPos.X < width && newPos.Y < height)
                PushToMap( Program.WorldMap[worldIndex.X, worldIndex.Y] );
            else
                PushToDifferentMap();
            ApplyActionCost( 20 );
        }

        public void ChopTree( Point pos )
        {
            if (Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y] is Tree == false)
                return;
            Item weapon = body.MainHand;
            if (weapon is Axe || weapon is Sword) {
                Random rng = new Random();
                Tree tree = (Tree)Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y];
                tree.Thickness -= weapon.GetWepDmg(this);
                ApplyActionCost(weapon.GetWeaponCost(this));
                if (rng.Next(0, 100) < 30)
                    LvlWeaponSkill(weapon, 10);
                // deplete stamina
                ChangeResource( Resource.SP, -(int)( weapon.Weight * 2 ) );
                Program.MsgConsole.WriteLine($"{Name} chopped the tree.");
                
                if (tree.Thickness <= 0) {
                    tree.DropLogs(pos, this);
                    if (Program.CurrentState is Play play && play.PlayMode == PlayMode.BuildMode)
                        Wield(inventory.FindIndex(i => i is BlueprintPouch), true);
                }
            }
        }
        public void HarvestPlant( Point pos )
        {
            if (Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y] is Plant == false)
                return;
            Plant plant = (Plant)Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y];
            Item weapon = body.MainHand;
            if (!plant.Name.Equals("grass") || this is Animal || (weapon is Dagger || weapon is Sword || weapon is Axe)) {
                Random rng = new Random();
                if (weapon != null) {
                    ApplyActionCost(weapon.GetWeaponCost(this));
                    if (rng.Next(0, 100) < 30)
                        LvlWeaponSkill(weapon, 10);
                    ChangeResource(Resource.SP, -(int)(weapon.Weight * 2));
                }
                ChangeResource(Resource.SP, -20);

                Block harvest = plant.DropHarvest();
                if (harvest is Food f) {
                    if (this is Player) { inventory.Add((Item)harvest); Program.MsgConsole.WriteLine($"{Name} harvested the {plant.Name}."); }
                    else { if (Visible) Program.MsgConsole.WriteLine($"{Name} ate the {plant.Name}."); Eat((Item)harvest); }
                    Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y] = new Air();
                }
                else Program.WorldMap[worldIndex.X, worldIndex.Y][pos.X, pos.Y] = harvest;

            }
        }
        public void PickWall( Wall wall )
        {
            Random rng = new Random();
            if ( !(wall.Material == Material.Stone || wall.Material == Material.Coal) )
                return;
            if (Body.MainHand is Axe axe && axe.WeaponName == "pickaxe")
            {
                Program.MsgConsole.WriteLine( $"{Name} struck the {wall.Name}" );
                ApplyActionCost(axe.GetWeaponCost(this) );
                if (rng.Next( 0, 100 ) < 30)
                    LvlWeaponSkill( axe, 10 );
                ChangeResource( Resource.SP, -(int)( axe.Weight * 2 ) );
                wall.HP -= (int)Physics.ImpactYields[axe.Material] - (int)Physics.ImpactYields[Material.Stone];
            }
            if (wall.HP <= 0) {
                Item drop = new Stone( true );
                if (wall.Material == Material.Coal)
                    drop = new CoalOre( true );
                Point pos = new Point();
                List<Point> potentialSpots = GetNearbyBlocksOfType( BlockType.Wall );
                potentialSpots.AddRange( GetNearbyBlocksOfType( BlockType.OreWall ) );

                Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
                int width = Program.WorldMap.TileWidth;
                pos = potentialSpots.Find( w => ( (Wall)blocks[w.X * width + w.Y] ).HP == wall.HP );
                blocks[pos.X * width + pos.Y] = drop;
                Program.MsgConsole.WriteLine( $"The {wall.Name} collapsed!" );
            }
        }

        public void StartDialog(Creature conversant)
        {
            Console.WriteLine("Initiated an imaginary conversation!");
        }

        public void GetItem(Point itemPos)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth;
            bool itemAdded = false;
            string itemName = "";
            string containerName = "inventory";

            if (blocks[itemPos.X * width + itemPos.Y] is Item item) {
                // handle various cases of items which must be managed uniquely
                itemName = item.Name;
                if (item is Arrow && inventory.Exists(i => i is Quiver)) {
                    Quiver q = (Quiver)inventory.Find(i => i is Quiver);
                    if (q.Arrows.Count <= 20) {
                        q.Arrows.Add(item);
                        itemAdded = true;
                        containerName = q.Name;
                    }
                }
                else if (item is Quiver q1 && inventory.Exists(i => i is Quiver)) {
                    Quiver q2 = (Quiver)inventory.Find(i => i is Quiver);
                    foreach (Arrow a in q1.Arrows) {
                        if (q2.Arrows.Count <= 20)
                            q2.Arrows.Add(a);
                        else inventory.Add(a);
                    }
                    itemAdded = true;
                    itemName += "'s arrows";
                    containerName = q2.Name;
                }
                else if (item is Blueprint && inventory.Exists(i => i is BlueprintPouch)) {
                    BlueprintPouch bp = (BlueprintPouch)inventory.Find(i => i is BlueprintPouch);
                    bp.Blueprints.Add(item);
                    itemAdded = true;
                    containerName = bp.Name;
                }
                else if (item is CraftingRecipe && inventory.Exists(i => i is RecipePouch))
                {
                    RecipePouch rp = (RecipePouch)inventory.Find(i => i is RecipePouch);
                    rp.Recipes.Add(item);
                    itemAdded = true;
                    containerName = rp.Name;
                }
                // normal case
                if (!itemAdded) itemAdded = AddItem(item);

                // apply action cost, update map, write to msg console
                if (itemAdded) {
                    Block replacementBlock = item.BlockPlacedOn ?? new Air();
                    blocks[itemPos.X * width + itemPos.Y] = replacementBlock;
                    if (currentFloor == Program.Player.CurrentFloor && worldIndex.Equals(Program.Player.WorldIndex))
                        Program.WorldMap[worldIndex.X, worldIndex.Y].DijkstraMaps.CallItemPosChanged(this);
                    if (this.Visible)
                        Program.MsgConsole.WriteLine($"{Name} put the {itemName} in their {containerName}.");
                    ApplyActionCost(6);
                }
                else if (this.Visible)
                    Program.MsgConsole.WriteLine($"{Name} tried to get the {itemName} but didn't have enough space.");
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

        public void Eat()
        {
            Food food = (Food)inventory.Find(i => i.ItemType == ItemType.Food);
            if (food == null)
                return;

            food.Activate(this);
            inventory.Remove(food);
            ApplyActionCost(6);
            if (this.Visible)
                Program.MsgConsole.WriteLine($"{Name} ate the {food.Name}");
        }
        public void Eat(Item item)
        {
            if (item.ItemType == ItemType.Food) {
                Food food = (Food)item;

                food.Activate(this);

                if (inventory.Contains(item))
                    inventory.Remove(item);

                ApplyActionCost(6); 
                if (this.Visible) Program.MsgConsole.WriteLine($"{Name} ate the {item.Name}");
            }
            else Menus.DisplayIncorrectUsage("You can't eat that.");
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
        public bool Drop(Item item)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            int width = Program.WorldMap.TileWidth, height = Program.WorldMap.TileHeight;
            List<Point> freeDirections = blocks.GetEmptyAdjacentBlocks(new Point(width, height), position); 
            if (freeDirections.Count == 0) {
                Menus.DisplayIncorrectUsage("There's no space to drop that.");
                return false;
            }
            Point point = freeDirections[new Random().Next(0, freeDirections.Count)];
            item.BlockPlacedOn = blocks[point.X * width + point.Y];
            blocks[point.X * width + point.Y] = item;
            inventory.Remove(item);
            Program.MsgConsole.WriteLine($"{Name} dropped the {item.Name}.");
            ApplyActionCost(10);
            Program.WorldMap[WorldIndex.X, WorldIndex.Y].DijkstraMaps.CallItemPosChanged(this);
            return true;
        }

        public void Wield(int itemIndex, bool mainHand)
        {
            Item item = inventory[itemIndex];
            if (mainHand) {
                if (body.MainHand != null)
                    inventory.Add(body.MainHand);
                if (item is Weapon w)
                    if (w.TwoHanded)
                        if (body.OffHand != null) {
                            inventory.Add(body.OffHand);
                            body.OffHand = null;
                        }
                body.MainHand = inventory[itemIndex];

                // enter build/craft mode if necessary
                if (this is Player && Program.CurrentState is Play play && play.PlayMode == PlayMode.Roguelike) {
                    if (body.MainHand is BlueprintPouch) {
                        play.PlayMode = PlayMode.BuildMode;
                        BuildingManager.Paused = true;
                        Program.MsgConsole.WriteLine("You entered Build Mode!");
                    }
                    else if (body.MainHand is RecipePouch)
                        Program.CurrentState = new CraftMenu();
                }
            }
            else {
                if (body.MainHand != null && body.MainHand is Weapon w && w.TwoHanded) {
                    Program.MsgConsole.WriteLine($"{Name} tried to wield the {inventory[itemIndex].Name} in their offhand but had no free hand");
                }
                else {
                    if (body.OffHand != null)
                        inventory.Add(body.OffHand);
                    body.OffHand = inventory[itemIndex];
                }

            }
            if (Visible)
                Program.MsgConsole.WriteLine($"{Name} wielded the {inventory[itemIndex].Name}");
            inventory.RemoveAt(itemIndex);
            ApplyActionCost(10);
        }
        public void Equip(Armor armorPiece)
        {
            if (armorPiece is Helmet helmet) {
                if (body.Helmet != null)
                    inventory.Add(body.Helmet);
                body.Helmet = helmet;
            }
            else if (armorPiece is ChestPiece chestPiece) {
                if (body.ChestPiece != null)
                    inventory.Add(body.ChestPiece);
                body.ChestPiece = chestPiece;
            }
            else if (armorPiece is Shirt shirt) {
                if (body.Shirt != null)
                    inventory.Add(body.Shirt);
                body.Shirt = shirt;
            }
            else if (armorPiece is Gauntlets gauntlets) {
                if (body.Gauntlets != null)
                    inventory.Add(body.Gauntlets);
                body.Gauntlets = gauntlets;
            }
            else if (armorPiece is Leggings leggings) {
                if (body.Leggings != null)
                    inventory.Add(body.Leggings);
                body.Leggings = leggings;
            }
            else if (armorPiece is Boots boots) {
                if (body.Boots != null)
                    inventory.Add(body.Boots);
                body.Boots = boots;
            }

            if (this.Visible)
                Program.MsgConsole.WriteLine($"{Name} equipped the {armorPiece.Name}");

            inventory.Remove(armorPiece);
            ApplyActionCost(18);
        }
        public void Disarm()
        {
            Unequip(body.MainHand);
            Unequip(body.OffHand);
        }
        public void Unequip(Item item)
        {
            if (item is Armor) {
                if (item is Helmet) {
                    bool addedItem = AddItem(body.Helmet);
                    if (addedItem) body.Helmet = null;
                }
                else if (item is ChestPiece) {
                    bool addedItem = AddItem(body.ChestPiece);
                    if (addedItem) body.ChestPiece = null;
                }
                else if (item is Shirt) {
                    bool addedItem = AddItem(body.Shirt);
                    if (addedItem) body.Shirt = null;
                }
                else if (item is Gauntlets) {
                    bool addedItem = AddItem(body.Gauntlets);
                    if (addedItem) body.Gauntlets = null;
                }
                else if (item is Leggings) {
                    bool addedItem = AddItem(body.Leggings);
                    if (addedItem) body.Leggings = null;
                }
                else if (item is Boots) {
                    bool addedItem = AddItem(body.Boots);
                    if (addedItem) body.Boots = null;
                }

                ApplyActionCost(16);
            }
            else {
                if (body.OffHand != null && body.OffHand.Equals(item)) {
                    bool addedItem = AddItem(body.OffHand);
                    if (addedItem) body.OffHand = null;
                }
                else if (body.MainHand != null && body.MainHand.Equals(item)) {
                    bool addedItem = AddItem(body.MainHand);
                    if (addedItem)
                        body.MainHand = null;
                    else {
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

        private void LvlWeaponSkill(Item weapon, int amount)
        {
            if (weapon is MeleeWeapon mWeapon)
                Stats.LvlSkill(mWeapon.GetWeaponSkill(), amount, this);
            else if (weapon is RangedWeapon)
                Stats.LvlSkill(Skill.Marksmanship, amount, this);
        }
        private void LvlArmorSkill(int amount)
        {
            List<Armor> equipment = Body.GetArmorList();
            int lightArmorPieces = equipment.Count(armor => armor.GetSkill() == Skill.LightArmor);
            int heavyArmorPieces = equipment.Count(armor => armor.GetSkill() == Skill.HeavyArmor);

            if (lightArmorPieces == heavyArmorPieces)
            {
                Stats.LvlSkill(Skill.LightArmor, amount / 2, this);
                Stats.LvlSkill(Skill.HeavyArmor, amount / 2, this);
            }
            if (lightArmorPieces > heavyArmorPieces)
                Stats.LvlSkill(Skill.LightArmor, amount, this);
            else if (lightArmorPieces == 0 && heavyArmorPieces == 0)
                Stats.LvlSkill(Skill.Unarmored, amount, this);
            else
                Stats.LvlSkill(Skill.HeavyArmor, amount, this);
        }
        private void SplatterBlood(Point recievingAngle)
        {
            Block[] blocks = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Blocks : Program.WorldMap[worldIndex.X, worldIndex.Y].Blocks;
            Tile[] tiles = currentFloor >= 0 ? Program.WorldMap[worldIndex.X, worldIndex.Y].Dungeon.Floors[currentFloor].Floor : Program.WorldMap[worldIndex.X, worldIndex.Y].Floor;
            int tileWidth = Program.WorldMap.TileWidth;
            int tileHeight = Program.WorldMap.TileHeight;
            Random rng = new Random();
            for (int i = Math.Max(position.X - 4, 0); i <= Math.Min(position.X + 4, tileWidth - 1); i++)
                for (int j = Math.Max(position.Y - 4, 0); j <= Math.Min(position.Y + 4, tileHeight - 1); j++)
                {
                    int treeRoll = rng.Next(1, 11), maxChance = 5;
                    double distFromTree = new Point(i, j).DistFrom(position);
                    bool pointCloserToDefThanAtt = new Point(i, j).DistFrom(recievingAngle) >= distFromTree;
                    Block block = blocks[i * tileWidth + j];
                    Tile tile = tiles[i * tileWidth + j];
                    if (treeRoll < maxChance - distFromTree * 2 && pointCloserToDefThanAtt && visiblePoints.Contains(new Point(i, j)))
                    {
                        block.Splattered = block.Visible;
                        tile.Splattered = tile.Visible;
                    }
                }
        }
        private void Die()
        {
            if (!alive)
                return;
            ForeColor = Color.DarkRed;
            Solid = false;
            Opaque = false;
            alive = false;
            inventory.Add(new Food(DietType.Carnivore, $"{Name} meat slab", 238, 0.15));
            if (this is Player == false) UnequipAll();
            else Menus.DeathNotification();
        }
        private void ApplyEffects()
        {
            for (int i = 0; i < effects.Count; i++)
                effects[i].Apply(this);
        }

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
        public int GetWeaponSkill(Item weapon)
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
                else if (weapon is RangedWeapon)
                    return Stats.Skills[Skill.Marksmanship];
                return 0;
            }
            else
                return Stats.Skills[Skill.Brawling];
        }

        // PROPERTIES //
        public Point Position {
            get { return position; }
            set { position = value; }
        }
        public Block CurrentBlock {
            get { return currentBlock; }
            set { currentBlock = value; }
        }
        public Point WorldIndex {
            get { return worldIndex; }
            set { worldIndex = value; }
        }
        public int CurrentFloor {
            get { return currentFloor; }
            set { currentFloor = value; }
        }
        public string Faction {
            get { return faction; }
            set { faction = value; }
        }
        public UInt64 ID {
            get { return id; }
            set { id = value; }
        }
        public int SightDist {
            get { return sightDist; }
            set { sightDist = value; }
        }
        public List<Point> VisiblePoints {
            get { return visiblePoints; }
            set { visiblePoints = value; }
        }
        public double Gold {
            get { return gold; }
            set { gold = value; }
        }
        public List<Item> Inventory {
            get { return inventory; }
            set { inventory = value; }
        }
        public string Gender {
            get { return gender; }
            set { gender = value; }
        }
        public DietType Diet {
            get { return diet; }
            set { diet = value; }
        }
        public Body Body {
            get { return body; }
            set { body = value; }
        }
        public bool Alive {
            get { return alive; }
            set { alive = value; }
        }
        public Stats Stats {
            get { return stats; }
            set { stats = value; }
        }
        public Class Class {
            get { return uclass; }
            set { uclass = value; }
        }
        public List<Effect> Effects {
            get { return effects; }
            set { effects = value; }
        }
        public Time NextActionTime {
            get { return nextActionTime; }
            set { nextActionTime = value; }
        }
    }
}
