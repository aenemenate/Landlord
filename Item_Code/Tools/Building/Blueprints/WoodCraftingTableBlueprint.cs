using System;
using System.Collections.Generic;

namespace Landlord
{
    class WoodCraftingTableBlueprint : Blueprint
    {
        public WoodCraftingTableBlueprint( bool instantiating, List<RecipeComponent> recipe = null) : base (instantiating, recipe)
        {
        }

        public WoodCraftingTableBlueprint() : base()
        {

        }

        public override void DetermineBlueprintTarget()
        {
            BlueprintTarget = new CraftingTable(Material.Wood);
        }

        public override void DetermineRecipe()
        {
            Recipe = new List<RecipeComponent>() { RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log, RecipeComponent.Log,
                                                   RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank, RecipeComponent.Plank,
                                                   RecipeComponent.Plank, RecipeComponent.Plank };
        }

        public override string DetermineDescription()
        {
            return "A blueprint to build a wooden crafting table. Crafting tables allow you to craft more complex contraptions.";
        }

        public override string DetermineName(bool identifying)
        {
            return "wooden crafting table blueprint";
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Uncommon;
        }
    }
}
