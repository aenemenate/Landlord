using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Tile : Element
    {
        private bool harmful;
        public Tile(byte graphic, string name, Color? foreColor, Color? backColor, 
            bool harmful) : base(graphic, name, foreColor, backColor)
        {
            this.harmful = harmful;
        }
        public Tile()
        {

        }
        public bool Harmful
        {
            get { return harmful; }
            set { harmful = value; }
        }
    }
    
    class DirtFloor : Tile
    {
        public DirtFloor() 
            : base()
        {
            Graphic = 46;
            Name = "dirt";
            ForeColor = new Color(120, 72, 0);
            BackColor = Color.Black;
            Harmful = false;
        }
    }
    
    class Grass : Tile
    {
        public Grass() : base()
        {
            Graphic = 34;
            Name = "grass";
            ForeColor = Color.LawnGreen;
            BackColor = new Color(87, 59, 12);
            Harmful = false;
        }
    }
    
    class Water : Tile
    {
        public Water() : base()
        {
            Graphic = 176;
            Name = "water";
            ForeColor = new Color(64, 164, 223);
            BackColor = new Color(0, 90, 149);
            Harmful = false;
        }
    }

    class Sand : Tile
    {
        public Sand() : base()
        {
            Graphic = 176;
            Name = "sand";
            ForeColor = new Color( 237, 201, 175 );
            BackColor = new Color(150, 113, 23);
            Harmful = false;
        }
    }
}
