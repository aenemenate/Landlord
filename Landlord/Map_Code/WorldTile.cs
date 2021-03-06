﻿using System;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class WorldTile : Element
    {
        public WorldTile(byte graphic, string name, Color? foreColor, Color? backColor) : base(graphic, name, foreColor, backColor)
        {
        }
    }
    
    class PlayerWorld : WorldTile
    {
        public PlayerWorld(byte graphic = 1, string name = "player", Color? foreColor = null, Color? backColor = null) : base(graphic, name, foreColor, backColor)
        {
            ForeColor = Color.AntiqueWhite;
            BackColor = Color.Black;
        }
    }
    class ForestTile : WorldTile
    {
        public ForestTile(Color? backColor = null, byte graphic = 24, string name = "forest", Color? foreColor = null) : base(graphic, name, foreColor, backColor)
        {
            ForeColor = new Color(34, 139, 34);
            BackColor = backColor == null ? new Color(67, 48, 30) : (Color)backColor;
        }
    }

    class Unexplored : WorldTile
    {
        public Unexplored(byte graphic = 1, string name = "unexplored", Color? foreColor = null, Color? backColor = null) : base(graphic, name, foreColor, backColor)
        {
            ForeColor = Color.Black;
            BackColor = Color.Black;
        }
    }
}
