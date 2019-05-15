using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    public enum RecipeComponent
    {
        Null,
        Log,
        Stone,
        Plank,
        WoodWheel,
        StoneWheel
    }

    static class RecipeHelper
    {
        public static int ToSkillValue(this RecipeComponent rc)
        {
            switch (rc)
            {
                case RecipeComponent.Plank:
                    return 5;
                case RecipeComponent.WoodWheel:
                    return 75;
                case RecipeComponent.StoneWheel:
                    return 125;
                default:
                    return 0;
            }
        }

        public static Item ToItem(this RecipeComponent rc)
        {
            switch (rc)
            {
                case RecipeComponent.Log:
                    return new Log( true );
                case RecipeComponent.Stone:
                    return new Stone( true );
                case RecipeComponent.Plank:
                    return new Plank( true );
                case RecipeComponent.WoodWheel:
                    return new WoodWheel( true );
                case RecipeComponent.StoneWheel:
                    return new Stone( true );
                default:
                    return new EmptyBottle( true );
            }
        }

        public static string ToString(this RecipeComponent rc)
        {
            switch (rc)
            {
                case RecipeComponent.Log:
                    return "log";
                case RecipeComponent.Stone:
                    return "stone";
                case RecipeComponent.Plank:
                    return "plank";
                case RecipeComponent.WoodWheel:
                    return "wood wheel";
                case RecipeComponent.StoneWheel:
                    return "stone wheel";
                default:
                    return "null";
            }
        }

        public static RecipeComponent ToComponent(this Item item)
        {
            if (item.ItemType == ItemType.Log)
                return RecipeComponent.Log;
            else if (item.ItemType == ItemType.Stone)
                return RecipeComponent.Stone;
            else if (item.ItemType == ItemType.Plank)
                return RecipeComponent.Plank;
            else if (item.ItemType == ItemType.Wheel && item.Material == Material.Wood)
                return RecipeComponent.WoodWheel;
            else if (item.ItemType == ItemType.Wheel && item.Material == Material.Stone)
                return RecipeComponent.StoneWheel;
            else
                return RecipeComponent.Null;
        }
    }
}
