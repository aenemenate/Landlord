using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Quiver : Item
    {
        private List<Item> arrows;

        public Quiver(bool instantiating, byte graphic = 11, ItemType type = ItemType.Quiver, double volume = 0.02, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.Beige;
            arrows = new List<Item>();
            Init(!instantiating);
        }

        public Quiver() : base()
        {

        }


        // FUNCTIONS

        public void Init(bool fillWithArrows)
        {
            if (!fillWithArrows)
                return;
            int arrowCount = new Random().Next(7, 22);
            for (int i = 0; i < arrowCount; i++)
                arrows.Add(new Arrow(true));
        }

        public override void Activate(Creature user)
        {

        }

        public override string DetermineDescription()
        {
            return "You can carry arrows and other things in here.";
        }

        public override string DetermineName(bool identifying)
        {
            return "quiver";
        }

        public override Material DetermineMaterial()
        {
            return Material.Leather;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }

        // PROPERTIES //

        public List<Item> Arrows
        {
            get { return arrows; }
            set { arrows = value; }
        }
    }
}
