using System;
using System.Collections.Generic;

namespace Landlord
{
    class ProcessLogRecipe : CraftingRecipe
    {
        public ProcessLogRecipe(bool instantiating) : base (instantiating)
        {
        }

        public ProcessLogRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new Plank(true), new Plank(true), new Plank(true), new Plank(true) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 15;
            WorkbenchRequirement = BlockType.Empty;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 30 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to chop a log into planks.";
        }

        public override string DetermineName(bool identifying)
        {
            return "plank recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
