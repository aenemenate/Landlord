using System;
using System.Collections.Generic;

namespace Landlord
{
    class GlueWheelRecipe : CraftingRecipe
    {
        public GlueWheelRecipe( bool instantiating) : base (instantiating)
        {
        }

        public GlueWheelRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new WoodWheel( true ) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 15;
            WorkbenchRequirement = BlockType.CraftingTable;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 180 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to glue planks at the precise angle to form a wheel.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wood wheel recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
