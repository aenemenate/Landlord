using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Landlord
{
    abstract class CraftingRecipe : Item
    {
        private List<Item> craftingTarget;
        private List<RecipeComponent> recipe;
        private int minCraftingSkill;
        private int craftTime;
        private BlockType workbenchRequirement;

        public CraftingRecipe(bool instantiating, byte graphic = 237, ItemType type = ItemType.Bottle, double volume = 0.02, bool hollow = true, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Color.Beige;
            Init();
        }

        public CraftingRecipe() : base()
        {

        }


        // FUNCTIONS

        public void Init()
        {
            Identify();
            DetermineCraftingTarget();
            DetermineRecipe();
            DetermineCraftingRequirements();
            DetermineCraftTime();
        }

        public abstract void DetermineCraftingTarget();

        public abstract void DetermineRecipe();

        public abstract void DetermineCraftingRequirements();

        public abstract void DetermineCraftTime();

        public override void Activate(Creature user)
        {

        }

        public override Material DetermineMaterial()
        {
            return Material.Cloth;
        }


        // PROPERTIES //

        public List<Item> CraftingTarget
        {
            get { return craftingTarget; }
            set { craftingTarget = value; }
        }

        public List<RecipeComponent> Recipe
        {
            get { return recipe; }
            set { recipe = value; }
        }

        public int MinCraftingSkill
        {
            get { return minCraftingSkill; }
            set { minCraftingSkill = value; }
        }

        public int CraftTime
        {
            get { return craftTime; }
            set { craftTime = value; }
        }
        public BlockType WorkbenchRequirement
        {
            get { return workbenchRequirement; }
            set { workbenchRequirement = value; }
        }
    }
}
