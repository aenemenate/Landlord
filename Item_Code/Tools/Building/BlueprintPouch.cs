using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Landlord
{
    class BlueprintPouch : Item
    {
        private List<Blueprint> blueprints;

        public BlueprintPouch(bool instantiating, byte graphic = 11, ItemType type = ItemType.BlueprintPouch, double volume = 0.02, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.Beige;
            Init(!instantiating);
        }

        public BlueprintPouch() : base()
        {

        }


        // FUNCTIONS

        public void Init(bool addBasicBlueprints)
        {
            blueprints = new List<Blueprint>();
            if (addBasicBlueprints)
            {
                blueprints.Add( new WoodWallBlueprint( true ) );
                blueprints.Add( new WoodFloorBlueprint( true ) );
                blueprints.Add( new WoodDoorBlueprint( true ) );
                blueprints.Add( new WoodChestBlueprint( true ) );
                blueprints.Add( new WoodCraftingTableBlueprint( true ) );
                blueprints.Add( new WoodStoneMillBlueprint( true ) );
                blueprints.Add( new WoodCartBlueprint( true ) );
            }
            Identify();
        }

        public override void Activate(Creature user)
        {

        }

        public override string DetermineDescription()
        {
            return "Equip this in your Main Hand to enter building mode.";
        }

        public override string DetermineName(bool identifying)
        {
            return "blueprint pouch";
        }

        public override Material DetermineMaterial()
        {
            return Material.Cloth;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }

        // PROPERTIES //

        public List<Blueprint> Blueprints
        {
            get { return blueprints; }
            set { blueprints = value; }
        }
    }
}
