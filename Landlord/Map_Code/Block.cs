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
        OreWall,
        Plant
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
        public Block(byte graphic, string name, BlockType type, bool explored, bool solid, bool opaque, bool interactive, bool enterable, Color? foreColor = null, Color? backColor = null)
            : base(graphic, name, explored, foreColor, backColor)
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
                Block block = (Block)s.Deserialize(ms);
                System.GC.Collect();
                return block;
            }
        }


        // PROPERTIES //
        public bool Solid {
            get { return solid; }
            set { solid = value; }
        }
        public bool Opaque {
            get { return opaque; }
            set { opaque = value; }
        }
        public BlockType Type {
            get { return type; }
            set { type = value; }
        }
        public Material Material {
            get { return material; }
            set { material = value; }
        }
        public bool Interactive {
            get { return interactive; }
            set { interactive = value; }
        }
        public bool Enterable {
            get { return enterable; }
            set { enterable = value; }
        }
        
    }
}
