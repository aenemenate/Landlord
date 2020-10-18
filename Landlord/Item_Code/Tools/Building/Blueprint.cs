using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Landlord
{
    abstract class Blueprint : Item
    {
        private Block blueprintTarget;
        private List<RecipeComponent> recipe;

        public Blueprint(bool instantiating, List<RecipeComponent> recipe, byte graphic = 237, ItemType type = ItemType.Blueprint, double volume = 0.003, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.Beige;
            Init();
        }

        public Blueprint() : base()
        {

        }


        // FUNCTIONS

        public void Init()
        {
            Identify();
            DetermineBlueprintTarget();
            DetermineRecipe();
        }

        public abstract void DetermineBlueprintTarget();

        public abstract void DetermineRecipe();

        public override void Activate(Creature user)
        {

        }

        public override Material DetermineMaterial()
        {
            return Material.Cloth;
        }


        // PROPERTIES //

        public Block BlueprintTarget
        {
            get { return blueprintTarget; }
            set { blueprintTarget = value; }
        }

        public List<RecipeComponent> Recipe
        {
            get { return recipe; }
            set { recipe = value; }
        }
    }
}
