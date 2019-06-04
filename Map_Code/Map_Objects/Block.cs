using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using System.IO;
using Polenter.Serialization;

namespace Landlord
{
    public enum BlockType
    {
        Empty,
        Wall,
        Floor,
        Door,
        UpStair,
        DownStair,
        Chest,
        Cart,
        Creature,
        Item,
        Tree,
        CraftingTable,
        StoneMill,
        OreWall
    }

    abstract class Block : Element
    {
        private bool solid;
        private bool opaque;
        private BlockType type;
        private bool interactive;
        private bool enterable;
        private Material material;


        // CONSTRUCTORS //
        public Block(byte graphic, string name, BlockType type, bool solid, bool opaque, bool interactive, bool enterable, Color? foreColor = null, Color? backColor = null) 
            : base(graphic, name, foreColor, backColor)
        {
            this.type = type;
            this.solid = solid;
            this.opaque = opaque;
            this.interactive = interactive;
            this.enterable = enterable;
        }
        public Block() : base()
        {

        }


        // FUNCTIONS //
        public abstract void Activate(Creature user);

        public Block Copy()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SharpSerializer s = new SharpSerializer(true);
                s.Serialize(this, ms);
                ms.Position = 0;
                return (Block)s.Deserialize(ms);
            }
        }


        // PROPERTIES //
        public bool Solid
        {
            get { return solid; }
            set { solid = value; }
        }

        public bool Opaque
        {
            get { return opaque; }
            set { opaque = value; }
        }

        public BlockType Type
        {
            get { return type; }
            set { type = value; }
        }

        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        public bool Interactive
        {
            get { return interactive; }
            set { interactive = value; }
        }

        public bool Enterable
        {
            get { return enterable; }
            set { enterable = value; }
        }
        
    }
    
    class Air : Block
    {
        public Air() : base()
        {
            this.Graphic = 1;
            this.Name = "air";
            this.Type = BlockType.Empty;
            this.ForeColor = new Color(0, 0, 0, 0);
            this.BackColor = new Color(0, 0, 0, 0);
            this.Solid = false;
            this.Opaque = false;
            Interactive = false;
            Enterable = false;
        }
        public override void Activate(Creature user)
        {
        }
    }

    class Corridor : Block
    {
        public Corridor() : base()
        {
            this.Graphic = 1;
            this.Name = "air";
            this.Type = BlockType.Empty;
            this.ForeColor = new Color(0, 0, 0, 0);
            this.BackColor = new Color(0, 0, 0, 0);
            this.Solid = false;
            this.Opaque = false;
            Interactive = false;
            Enterable = false;
        }
        public override void Activate(Creature user)
        {
        }
    }
    
    class Wall : Block
    {
        private int hp;
        public Wall(Material material, byte graphic = 178, BlockType type = BlockType.Wall, string name = "wall", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false) 
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
            if (material == Material.Stone || material == Material.Coal)
                hp = (int)Physics.ImpactFractures[Material];
            else
                hp = 1000;
        }

        public Wall() : base()
        {
        }
        public override void Activate(Creature user)
        {
            user.PickWall(this);
        }

        public int HP
        {
            get { return hp; }
            set { hp = value; }
        }
    }
    
    class Floor : Block
    {
        public Floor(Material material, byte graphic = 46, BlockType type = BlockType.Floor, string name = "floor", bool solid = false, bool opaque = false, bool interactive = false, bool enterable = false)
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Floor() : base()
        {
        }
        public override void Activate(Creature user)
        {
        }
    }
    
    class DownStair : Block
    {
        public DownStair(Material material, byte graphic = 62, BlockType type = BlockType.DownStair, string name = "down stairs", bool solid = false, bool opaque = true, bool interactive = false, bool enterable = true)
               : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public DownStair() : base()
        {
        }

        public override void Activate(Creature user)
        {
            user.TakeStairsDown();
        }
    }
    
    class UpStair : Block
    {
        public UpStair(Material material, byte graphic = 60, BlockType type = BlockType.UpStair, string name = "up stairs", bool solid = false, bool opaque = true, bool interactive = false, bool enterable = true)
               : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }
        public UpStair() : base()
        {
        }
        public override void Activate(Creature user)
        {
            user.TakeStairsUp();
        }
    }
    
    class Door : Block
    {
        public Door(Material material, byte graphic = 43, BlockType type = BlockType.Door, string name = "door", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Physics.MaterialColors[material] * .9F;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Door() : base()
        {
        }

        public override void Activate(Creature user)
        {
            user.OpenDoor(this);
        }
    }
    
    class Chest : Block
    {
        private List<Item> inventory = new List<Item>();
        
        // CONSTRUCTORS //
        public Chest(Material material, byte graphic = 228, BlockType type = BlockType.Chest, string name = "chest", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false)
                  : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Chest() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate(Creature user)
        {
            if (user is Player)
            {
                Program.Animations.Add(new OpenLootView());
                Program.CurrentState = new ViewLoot(inventory, Name);
            }
        }

        // PROPERTIES //
        public List<Item> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
    }

    class Cart : Chest
    {
        // CONSTRUCTORS //
        public Cart( Material material, byte graphic = 15 + 16 * 13 + 9, BlockType type = BlockType.Cart, string name = "cart", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false )
                  : base( material, graphic, type, name, solid, opaque, interactive, enterable )
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public Cart() : base()
        {
        }
    }

    class CraftingTable : Block
    {

        // CONSTRUCTORS //
        public CraftingTable( Material material, byte graphic = (16 * 6 + 14), BlockType type = BlockType.CraftingTable, string name = "crafting table", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false )
                  : base( graphic, name, type, solid, opaque, interactive, enterable )
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public CraftingTable() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate( Creature user )
        {
            try
            {
                user.Wield( user.Inventory.FindIndex( i => i.Name == "recipe pouch" ), true );
            } catch
            {
                Program.MsgConsole.WriteLine( "You don't have a recipe pouch!" );
            }
        }

    }

    class StoneMill : Block
    {

        // CONSTRUCTORS //
        public StoneMill( Material material, byte graphic = ( 15 + 16 * 8 ), BlockType type = BlockType.StoneMill, string name = "stone mill", bool solid = true, bool opaque = false, bool interactive = true, bool enterable = false )
                  : base( graphic, name, type, solid, opaque, interactive, enterable )
        {
            this.Material = material;
            this.ForeColor = Physics.MaterialColors[material];
            this.BackColor = Color.Pink;
            this.Name = Physics.MaterialNames[material] + ' ' + name;
        }

        public StoneMill() : base()
        {
        }

        // FUNCTIONS//
        public override void Activate( Creature user )
        {
            try
            {
                user.Wield( user.Inventory.FindIndex( i => i.Name == "recipe pouch" ), true );
            } catch
            {
                Program.MsgConsole.WriteLine( "You don't have a recipe pouch!" );
            }
        }

    }

    class Tree : Block
    {
        private int thickness;

        // CONSTRUCTORS //
        public Tree(Material material, byte graphic = 10, BlockType type = BlockType.Tree, string name = "tree", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false)
            : base(graphic, name, type, solid, opaque, interactive, enterable)
        {
            void DetermineThickness()
            {
                int minThickness = 160, maxThickness = 320;

                thickness = Program.RNG.Next(minThickness, maxThickness);
            }

            this.Material = material;
            this.ForeColor = new Color(205, 133, 63);
            this.BackColor = Color.Pink;
            this.Name = name;
            DetermineThickness();
        }

        public Tree() : base()
        {
        }


        // FUNCTIONS //

        public override void Activate(Creature user)
        {
            if (user.Body.MainHand != null && ( user.Body.MainHand is Axe || user.Body.MainHand is Sword ))
                user.ChopTree(this);
        }

        public void DropLogs(Point pos, Creature user)
        {
            Random rng = new Random();
            for (int i = Math.Max(pos.X - 5, 0); i <= Math.Min(pos.X + 5, Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y].Width - 1); i++)
                for (int j = Math.Max(pos.Y - 5, 0); j <= Math.Min(pos.Y + 5, Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y].Height - 1); j++)
                {
                    int treeRoll = rng.Next(1, 11), maxChance = 10;
                    double distFromTree = new Point(i, j).DistFrom(pos);
                    bool pointCloserToTreeThanfeller = new Point(i, j).DistFrom(user.Position) > distFromTree;

                    if (treeRoll < maxChance - distFromTree * 2 && pointCloserToTreeThanfeller && Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][i, j] is Air)
                        Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][i, j] = new Log(true);
                }

            Program.WorldMap[user.WorldIndex.X, user.WorldIndex.Y][pos.X, pos.Y] = new Log(true);

            Program.MsgConsole.WriteLine("The tree was felled!");
        }

        // PROPERTIES //

        public int Thickness
        {
            get { return thickness; }
            set { thickness = value; }
        }
    }

    class OreWall : Wall
    {
        public OreWall( Material material, byte graphic = 178, BlockType type = BlockType.OreWall, string name = "ore", bool solid = true, bool opaque = true, bool interactive = true, bool enterable = false )
            : base( material, graphic, type, name, solid, opaque, interactive, enterable )
        {
            HP = (int)Physics.ImpactFractures[Material.Coal];
        }

        public OreWall() : base()
        {
        }
        public override void Activate( Creature user )
        {
            user.PickWall( this );
        }
    }
}
