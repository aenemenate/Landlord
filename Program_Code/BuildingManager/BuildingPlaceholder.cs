using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Landlord
{
    class BuildingPlaceholder : Element
    {
        private List<Item> heldComponents;
        private Block buildTarget;


        // CONSTRUCTORS //

        public BuildingPlaceholder(byte graphic, string name, Block buildTarget, bool explored = true, Color? foreColor = null, Color? backColor = null) : base(graphic, name, explored, foreColor, backColor)
        {
            ForeColor = Color.SlateGray;
            BackColor = Color.AntiqueWhite;

            this.buildTarget = buildTarget;
            this.heldComponents = new List<Item>();
        }

        public BuildingPlaceholder() : base()
        {

        }


        // PROPERTIES //

        public List<Item> HeldComponents
        {
            get { return heldComponents; }
            set { heldComponents = value; }
        }

        public Block BuildTarget
        {
            get { return buildTarget; }
            set { buildTarget = value; }
        }
    }
}
