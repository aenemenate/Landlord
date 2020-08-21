using System.Collections.Generic;

namespace Landlord
{
    class CarveHiltRecipe : CraftingRecipe
    {
        public CarveHiltRecipe(bool instantiating) : base(instantiating)
        {
        }

        public CarveHiltRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new Hilt(true, Material.Wood) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 25;
            WorkbenchRequirement = BlockType.CraftingTable;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 120 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to carve a hilt from a log.";
        }

        public override string DetermineName(bool identifying)
        {
            return "carve hilt recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
