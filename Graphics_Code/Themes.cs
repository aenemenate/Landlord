using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Threading.Tasks;

namespace Landlord
{
    static class Themes
    {
        static private Cell defaultButtonNormal = new SadConsole.Cell(Color.AliceBlue, Color.DarkSlateGray * 1.1F);
        static private Cell defaultButtonDisabled = new SadConsole.Cell(Color.AliceBlue, Color.DarkSlateGray * 1.1F);
        static private Cell defaultButtonMouseOver = new SadConsole.Cell(Color.AliceBlue, Color.Gray);
        static private Cell defaultButtonMouseClicking = new SadConsole.Cell(Color.Gray, Color.AliceBlue);
        static private Cell defaultButtonFocused = new SadConsole.Cell(Color.AliceBlue, Color.DarkSlateGray * 1.1F);

        static private Cell defaultInputBoxNormal = new SadConsole.Cell(Color.AliceBlue, Color.DarkSlateGray);
        static private Cell defaultInputBoxDisabled = new SadConsole.Cell(Color.AliceBlue, Color.DarkSlateGray);
        static private Cell defaultInputBoxMouseOver = new SadConsole.Cell(Color.AliceBlue, Color.LightSlateGray * 1.1F);
        static private Cell defaultInputBoxFocused = new SadConsole.Cell(Color.AliceBlue, Color.SlateGray * 1.1F);
        

        // INIT

        static public void InitializeThemes()
        {
            SadConsole.Themes.Library.Default.ButtonTheme.Normal = defaultButtonNormal;
            SadConsole.Themes.Library.Default.ButtonTheme.Disabled = defaultButtonDisabled;
            SadConsole.Themes.Library.Default.ButtonTheme.MouseOver = defaultButtonMouseOver;
            SadConsole.Themes.Library.Default.ButtonTheme.MouseClicking = defaultButtonMouseClicking;
            SadConsole.Themes.Library.Default.ButtonTheme.Focused = defaultButtonFocused;

            SadConsole.Themes.Library.Default.InputBoxTheme.Normal = defaultInputBoxNormal;
            SadConsole.Themes.Library.Default.InputBoxTheme.Disabled = defaultInputBoxDisabled;
            SadConsole.Themes.Library.Default.InputBoxTheme.MouseOver = defaultInputBoxMouseOver;
            SadConsole.Themes.Library.Default.InputBoxTheme.Focused = defaultInputBoxFocused;
        }


        // PARAMETERS

        static public Cell DefaultButtonNormal
        {
            get { return defaultButtonNormal; }
            set { defaultButtonNormal = value; }
        }
        static public Cell DefaultButtonDisabled
        {
            get { return defaultButtonDisabled; }
            set { defaultButtonDisabled = value; }
        }
        static public Cell DefaultButtonMouseOver
        {
            get { return defaultButtonMouseOver; }
            set { defaultButtonMouseOver = value; }
        }
        static public Cell DefaultButtonMouseClicking
        {
            get { return defaultButtonMouseClicking; }
            set { defaultButtonMouseClicking = value; }
        }
        static public Cell DefaultButtonFocused
        {
            get { return defaultButtonFocused; }
            set { defaultButtonFocused = value; }
        }
    }
}
