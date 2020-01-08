using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class ConstructTorchRecipe : CraftingRecipe
    {
        public ConstructTorchRecipe(bool instantiating) : base(instantiating)
        {
        }

        public ConstructTorchRecipe() : base()
        {

        }

        public override void DetermineCraftingTarget()
        {
            CraftingTarget = new List<Item>() { new Torch(false) };
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Handle, RecipeComponent.Hilt, RecipeComponent.Coal };
        }

        public override void DetermineCraftingRequirements()
        {
            MinCraftingSkill = 25;
            WorkbenchRequirement = BlockType.Empty;
        }

        public override void DetermineCraftTime()
        {
            CraftTime = 120 * 60;
        }

        public override string DetermineDescription()
        {
            return "A crafting recipe which details how to make a torch.";
        }

        public override string DetermineName(bool identifying)
        {
            return "create torch recipe";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
