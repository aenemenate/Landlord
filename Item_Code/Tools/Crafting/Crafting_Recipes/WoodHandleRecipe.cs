using System.Collections.Generic;

namespace Landlord
{
    class WoodHandleRecipe : CraftingRecipe
    {
        public WoodHandleRecipe(bool instantiating) : base(instantiating)
        {
        }

        public WoodHandleRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new Handle(true, Material.Wood) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Stick, RecipeComponent.Stick, RecipeComponent.Stick, RecipeComponent.Stick, 
                RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf,
                RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf, RecipeComponent.Leaf 
            };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 25;
            WorkbenchRequirement = BlockType.CraftingTable;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 60 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to meld sticks together with leafs.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wood handle recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
