using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodWallBlueprint : Blueprint
    {
        public WoodWallBlueprint(bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodWallBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new Wall(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log, RecipeComponent.Log,
                RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank };
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden wall.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden wall blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
