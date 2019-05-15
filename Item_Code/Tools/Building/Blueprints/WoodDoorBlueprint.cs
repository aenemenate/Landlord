using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodDoorBlueprint : Blueprint
    {
        public WoodDoorBlueprint(bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodDoorBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new Door(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank};
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden door.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden door blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
