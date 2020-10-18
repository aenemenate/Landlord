using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodStoneMillBlueprint : Blueprint
    {
        public WoodStoneMillBlueprint( bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodStoneMillBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new StoneMill(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.WoodWheel, RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log,
                                                   RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Plank, RecipeComponent.Plank,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Stone };
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden stone mill. Stone mills allow you to shape stone into simple shapes like circles and rectangles.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden stone mill blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
