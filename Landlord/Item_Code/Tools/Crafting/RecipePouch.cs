using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Landlord
{
    class RecipePouch : Item
    {
        private List<Item> recipes;

        public RecipePouch(bool instantiating, byte graphic = 11, ItemType type = ItemType.RecipePouch, double volume = 0.02, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.Beige;
            Init(!instantiating);
        }

        public RecipePouch() : base()
        {

        }


        // FUNCTIONS

        public void Init(bool addBasicRecipes)
        {
            recipes = new List<Item>();
            if (addBasicRecipes)
            {
                recipes.Add( new ProcessLogRecipe( true ) );
            }
            Identify();
        }

        public override void Activate(Creature user)
        {

        }

        public override string DetermineDescription()
        {
            return "Equip this in your Main Hand to open the crafting menu.";
        }

        public override string DetermineName(bool identifying)
        {
            return "recipe pouch";
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

        public List<Item> Recipes
        {
            get { return recipes; }
            set { recipes = value; }
        }
    }
}
