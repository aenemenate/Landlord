﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum RecipeComponent
    {
        Null,
        Log,
        Leaf,
        Stick,
        Stone,
        Coal,
        Plank,
        WoodWheel,
        StoneWheel,
        Handle,
        Hilt,
        Torch
    }

    static class RecipeHelper
    {
        public static int ToSkillValue(this RecipeComponent rc)
        {
            switch (rc)
            {
                case RecipeComponent.Log:
                    return 0;
                case RecipeComponent.Leaf:
                    return 0;
                case RecipeComponent.Stick:
                    return 0;
                case RecipeComponent.Stone:
                    return 0;
                case RecipeComponent.Coal:
                    return 0;
                case RecipeComponent.Plank:
                    return 10;
                case RecipeComponent.WoodWheel:
                    return 75;
                case RecipeComponent.StoneWheel:
                    return 150;
                case RecipeComponent.Handle:
                    return 100;
                case RecipeComponent.Hilt:
                    return 100;
                case RecipeComponent.Torch:
                    return 100;
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
                case RecipeComponent.Leaf:
                    return new Leaf(true, "tree", Color.ForestGreen);
                case RecipeComponent.Stick:
                    return new Stick(true);
                case RecipeComponent.Stone:
                    return new Stone( true );
                case RecipeComponent.Coal:
                    return new CoalOre( true );
                case RecipeComponent.Plank:
                    return new Plank( true );
                case RecipeComponent.WoodWheel:
                    return new WoodWheel( true );
                case RecipeComponent.StoneWheel:
                    return new Stone( true );
                case RecipeComponent.Handle:
                    return new Handle(true, Material.Wood);
                case RecipeComponent.Hilt:
                    return new Hilt(true, Material.Wood);
                case RecipeComponent.Torch:
                    return new Torch(false);
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
                case RecipeComponent.Leaf:
                    return "leaf";
                case RecipeComponent.Stick:
                    return "stick";
                case RecipeComponent.Stone:
                    return "stone";
                case RecipeComponent.Coal:
                    return "coal ore";
                case RecipeComponent.Plank:
                    return "plank";
                case RecipeComponent.WoodWheel:
                    return "wood wheel";
                case RecipeComponent.StoneWheel:
                    return "stone wheel";
                case RecipeComponent.Handle:
                    return "handle";
                case RecipeComponent.Hilt:
                    return "hilt";
                case RecipeComponent.Torch:
                    return "torch";
                default:
                    return "null";
            }
        }

        public static RecipeComponent ToComponent(this Item item)
        {
            if (item.ItemType == ItemType.Log)
                return RecipeComponent.Log;
            else if (item.ItemType == ItemType.Leaf)
                return RecipeComponent.Leaf;
            else if (item.ItemType == ItemType.Stick)
                return RecipeComponent.Stick;
            else if (item.ItemType == ItemType.Stone)
                return RecipeComponent.Stone;
            else if (item.ItemType == ItemType.Coal)
                return RecipeComponent.Coal;
            else if (item.ItemType == ItemType.Plank)
                return RecipeComponent.Plank;
            else if (item.ItemType == ItemType.Wheel && item.Material == Material.Wood)
                return RecipeComponent.WoodWheel;
            else if (item.ItemType == ItemType.Wheel && item.Material == Material.Stone)
                return RecipeComponent.StoneWheel;
            else if (item.ItemType == ItemType.Handle)
                return RecipeComponent.Handle;
            else if (item.ItemType == ItemType.Hilt)
                return RecipeComponent.Hilt;
            else if (item is Torch)
                return RecipeComponent.Torch;
            else
                return RecipeComponent.Null;
        }
    }
}
