using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    abstract class Element : IComparable<Element>
    {
        private byte graphic;       // <---- VAR graphic is the index in the ascii spirite sheet that the engine draws for this element. //
        private Color? foreColor;  // <---- VAR foreColor is the color of the graphic. //
        private Color? backColor; // <---- VAR backColor is the color of the square encompassing the graphic. //
        private bool visible;    // <---- VAR visible is set true if this is within the playable character's field of view. //
        private bool explored;  // <---- VAR explored is set true if this has been seen by the playable character since his or her creation. //
        private bool splattered; // <--- VAR splattered is set true when blood is splattered on this element, this indicates to draw it red. //
        private string name;   // <---- VAR name is simply its name. //

        protected Element(byte graphic, string name, Color? foreColor, Color? backColor)
        {
            this.graphic = graphic;
            this.foreColor = foreColor;
            this.backColor = backColor;
            visible = false;
            explored = false;
            splattered = false;
            this.name = name;
        }
        protected Element(byte graphic, string name, bool explored, Color? foreColor, Color? backColor)
        {
            this.graphic = graphic;    
            this.foreColor = foreColor; 
            this.backColor = backColor; 
            visible = false;            
            this.explored = explored;
            splattered = false;
            this.name = name;           
        }
        protected Element() { }

        // This function compares two elements by their name to determine if they are the same. It is inherited from IComparable.
        public int CompareTo(Element other)
        {
            if (this is Item item) {
                if (other is Item otherI)
                    return item.Name.CompareTo(otherI.Name);
                else
                    return item.Name.CompareTo(other.Name);
            }
            else {
                if (other is Item otherI)
                    return Name.CompareTo(otherI.Name);
                else return Name.CompareTo(other.Name);
            }
        }

        public byte Graphic {
            get { return graphic; }
            set { graphic = value; }
        }
        public Color ForeColor {
            get { return (Color)foreColor; }
            set { foreColor = value; }
        }
        public Color BackColor {
            get { return (Color)backColor; }
            set { backColor = value; }
        }
        public bool Visible {
            get { return visible; }
            set { visible = value; }
        }
        public bool Explored {
            get { return explored; }
            set { explored = value; }
        }
        public bool Splattered {
            get { return splattered; }
            set { splattered = value; }
        }
        public string Name {
            get { return name; }
            set { name = value; }
        }
    }
}
