using System;
using System.Collections.Generic;

namespace Landlord
{
    class ShapeWheelRecipe : CraftingRecipe
    {
        public ShapeWheelRecipe( bool instantiating) : base (instantiating)
        {
        }

        public ShapeWheelRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new StoneWheel( true ) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Stone };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 20;
            WorkbenchRequirement = BlockType.StoneMill;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 240 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to shape a stone into a useable wheel.";
        }

        public override string DetermineName(bool identifying)
        {
            return "stone wheel recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
