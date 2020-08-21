using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodChestBlueprint : Blueprint
    {
        public WoodChestBlueprint(bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodChestBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new Chest(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank};
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden chest.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden chest blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
