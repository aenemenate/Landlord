using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodCartBlueprint : Blueprint
    {
        public WoodCartBlueprint( bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodCartBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new Cart(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log,
                                                   RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Plank, RecipeComponent.Plank,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.StoneWheel, RecipeComponent.StoneWheel,
                                                   RecipeComponent.StoneWheel, RecipeComponent.StoneWheel };
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden cart. Carts can be loaded with items and pushed around.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden cart blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
