using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class Identification
    {
        private Dictionary<PotionType, Color> potionColors;
        private Dictionary<Color, string> colorNames;
        private Dictionary<string, bool> identificationTable;


        // CONSTRUCTORS

        public Identification(bool init)
        {
            Init();
        }

        public Identification()
        {

        }


        // FUNCTIONS

        private void Init()
        {
            List<Color> availableColors = new List<Color>()
            {
                Color.Aquamarine,
                Color.Black,
                Color.Blue,
                Color.Brown,
                Color.Crimson,
                Color.Cyan,
                Color.Gold,
                Color.Green,
                Color.Gray,
                Color.Magenta,
                Color.Orange,
                Color.Pink,
                Color.Purple,
                Color.Red,
                Color.Silver,
                Color.Tan,
                Color.Turquoise,
                Color.Violet,
                Color.White,
                Color.Yellow
            };
            potionColors = new Dictionary<PotionType, Color>();
            colorNames = new Dictionary<Color, string>()
            {
                { Color.Aquamarine, "aquamarine"},
                { Color.Black, "black" },
                { Color.Blue, "blue" },
                { Color.Brown, "brown" },
                { Color.Crimson, "crimson" },
                { Color.Cyan, "cyan" },
                { Color.Gold, "gold" },
                { Color.Green, "green" },
                { Color.Gray, "gray" },
                { Color.Magenta, "magenta" },
                { Color.Orange, "orange" },
                { Color.Pink, "pink" },
                { Color.Purple, "purple" },
                { Color.Red, "red" },
                { Color.Silver, "silver" },
                { Color.Tan, "tan" },
                { Color.Turquoise, "turquoise" },
                { Color.Violet, "violet" },
                { Color.White, "white" },
                { Color.Yellow, "yellow" }
            };
            identificationTable = new Dictionary<string, bool>();
            // assign the colors to potions
            foreach (PotionType potionType in Enum.GetValues(typeof(PotionType)))
            {
                Color next = availableColors[new Random().Next(0, availableColors.Count)];
                PotionColors.Add(potionType, next);
                availableColors.Remove(next);
            }
        }

        public bool IsThisIdentified(string name)
        {
            if (!identificationTable.ContainsKey(name))
                identificationTable.Add(name, false);

            return identificationTable[name];
        }

        public void IdentifyItem(string name)
        {
            identificationTable[name] = true;
        }


        // PARAMETERS

        public Dictionary<PotionType, Color> PotionColors
        {
            get { return potionColors; }
            set { potionColors = value; }
        }

        public Dictionary<Color, string> ColorNames
        {
            get { return colorNames; }
            set { colorNames = value; }
        }

        public Dictionary<string, bool> IdentificationTable
        {
            get { return identificationTable; }
            set { identificationTable = value; }
        }
    }
}
