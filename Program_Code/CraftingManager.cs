using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class CraftingManager
    {
        private static Time timeStarted = null;
        private static int currentOutput = 0;
        private static CraftingRecipe craftingRecipe;
        private static bool crafting = false;


        // FUNCTIONS //

        public static void StartCrafting(CraftingRecipe craftingRecipe)
        {
            bool canCraft = DetermineIfCanCraft(craftingRecipe);

            if (canCraft)
            {
                crafting = true;
                timeStarted = Program.TimeHandler.CurrentTime;
                ConsumeCraftingComponents(craftingRecipe);
                CraftingRecipe = craftingRecipe;
            }
            else
                Menus.DisplayIncorrectUsage("I don't have the required components.");
        }

        public static void ContinueCrafting()
        {
            Item i = craftingRecipe.CraftingTarget[currentOutput];
            Scheduler.HandleCraftingScheduling( ( craftingRecipe.CraftTime ) / craftingRecipe.CraftingTarget.Count );
            Program.Player.Inventory.Add( i );
            Program.Player.Stats.LvlSkill(Skill.Crafting, craftingRecipe.CraftingTarget[currentOutput].ToComponent().ToSkillValue(), Program.Player);
            currentOutput++;
            if (currentOutput >= craftingRecipe.CraftingTarget.Count)
            {
                currentOutput = 0;
                crafting = false;
            }
        }

        public static bool DetermineIfCanCraft(CraftingRecipe craftingRecipe)
        {
            // does the player have enough skill?
            if (craftingRecipe.MinCraftingSkill > Program.Player.Stats.Skills[Skill.Crafting])
                return false;

            List<RecipeComponent> recipe = new List<RecipeComponent>();
            // copy recipe to variable
            foreach (RecipeComponent r in craftingRecipe.Recipe)
                recipe.Add(r);

            // determine if the player has the required components

            for (int i = Program.Player.Inventory.Count - 1; i >= 0; i--)
            {
                Item item = Program.Player.Inventory[i];
                if ( recipe.Exists(rc => rc == item.ToComponent() ))
                    recipe.RemoveAt( recipe.FindLastIndex( rc => rc == item.ToComponent() ) );
            }
            
            if (recipe.Count > 0)
                return false;

            return true;
        }
        
        private static void ConsumeCraftingComponents(CraftingRecipe craftingRecipe)
        {
            foreach (RecipeComponent r in craftingRecipe.Recipe)
                for (int i = Program.Player.Inventory.Count - 1; i >= 0; i--)
                    if (Program.Player.Inventory[i].Name.Equals(r.ToItem().Name))
                    {
                        Program.Player.Inventory.RemoveAt(i);
                        break;
                    }
        }


        // PROPERTIES //

        public static Time TimeStarted
        {
            get { return timeStarted; }
            set { timeStarted = value; }
        }

        public static bool Crafting
        {
            get { return crafting; }
            set { crafting = value; }
        }

        public static CraftingRecipe CraftingRecipe
        {
            get { return craftingRecipe; }
            set { craftingRecipe = value; }
        }
    }
}
