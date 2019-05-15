using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodFloorBlueprint : Blueprint
    {
        public WoodFloorBlueprint(bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodFloorBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new Floor(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Plank, RecipeComponent.Plank };
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden floor.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden floor blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
