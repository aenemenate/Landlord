using System;
using System.Collections.Generic;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;

namespace Landlord
{

    /* Landlord is an open world roguelike with town building elements.
    Copyright (C) 2019  Tristan S. Williams

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be ..[entertaining],
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
    */

    class Program
    { 
        // * means it's a SadConsole construct
        static private Window window; // contains code for manipulating window properties during gameplay
        static private Console console; // * buffer that characters are drawn to, displayed on window
        static private ControlsConsole controlsConsole; // * for menus that have buttons
        static private MsgConsole msgConsole; // for displaying messages
        static private WorldMap worldMap; // contains all block and character data
        static private Identification identification; // keeps track of what's been identified in current playthrough
        static private TimeHandler timeHandler; // keeps track of time

        static private Player player;
        static private GameState currentState;
        static private List<Animation> animations; // running animations go here
        static private List<Animation> finishedAnims; // animations are placed here when done
        static private Random rng;
        static private AudioPlaybackEngine audioEngine; // uses NAudio mixer code I found in their tutorials on their website
        static private MusicHandler musicHandler;


        // FUNCTIONS //

        static void Main(string[] args)
        {
            window = new Window("res/graphics/fonts/Haowan_16x16.font", 85, 45);

           /*  ABOUT WHY THERE IS A WINDOW AND WHY ITS SIZE IS MEASURED WITH IN-GAME TILES
            *  The window's size can be changed by the program user. Default is 80 x 40.
            *  I decided to make the size of the console (where the map, gui, virtually everything is drawn to) 
            *  vary dynamically to match the window size. The window handles this resizing.
            *  
            *  As a result, all code that reads/writes from/to the console needs to be aware that the size of the console isn't static
            */


    HookEvents(); // Hook render and update to SadConsole

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //
            
            SadConsole.Game.Instance.Dispose();
        }
        private static void HookEvents ()
        {
            SadConsole.Game.OnInitialize = Init;
            SadConsole.Game.OnUpdate = Update;
        }
        private static void Init()
        {
            currentState = new MainMenu();

            console = new Console(window.Width, window.Height);
            msgConsole = new MsgConsole(0, new TimeSpan(0, 0, 5));
            GUI.Console = new SadConsole.Console( Program.Window.Width, Program.Window.Height );

            animations = new List<Animation>();
            finishedAnims = new List<Animation>();
            
            rng = new Random();

            audioEngine = new AudioPlaybackEngine();
            musicHandler = new MusicHandler();
            // play main menu theme
            Program.AudioEngine.PlayMusic( "Main_Menu" );

            timeHandler = new TimeHandler(8, 0, 0, 1);

            // initialize the color schemes and material properties
            Physics.InitializePhysics();
            Themes.InitializeThemes();

            SadConsole.Global.CurrentScreen = console;
            ((SadConsole.Game)SadConsole.Game.Instance).WindowResized += window.ClientSizeChanged;
        }
        private static void Update(GameTime time)
        {
            Render();
            currentState.Update();
            UserInterfaceInput.HandleKeys();
            musicHandler.Update();
        }
        private static void Render()
        {
            if (currentState is Play && 
                !(Program.Console.Children.Contains(msgConsole.Console) && Program.Console.Children.Contains( GUI.Console ) ) ) {
                Program.Console.Children.Remove( msgConsole.Console );
                Program.Console.Children.Remove( GUI.Console );
                Program.Console.Children.Add( msgConsole.Console );
                Program.Console.Children.Add( GUI.Console );
            }

            currentState.Render();
            if ((currentState is DialogWindow) == false) {
                foreach (Animation anim in animations)
                    anim.Play();
                foreach (Animation anim in finishedAnims)
                    animations.Remove(anim);
                finishedAnims = new List<Animation>();
            }
        }


        // PROPERTIES //
        
        public static Console Console {
            get { return console; }
            set { console = value; }
        }
        public static ControlsConsole ControlsConsole {
            get { return controlsConsole; }
            set { controlsConsole = value; }
        }
        public static Window Window {
            get { return window; }
            set { window = value; }
        }
        public static MsgConsole MsgConsole {
            get { return msgConsole; }
            set { msgConsole = value; }
        }
        public static GameState CurrentState {
            get { return currentState; }
            set { currentState = value; }
        }
        public static WorldMap WorldMap {
            get { return worldMap; }
            set { worldMap = value; }
        }
        public static Player Player {
            get { return player; }
            set { player = value; }
        }
        public static List<Animation> Animations {
            get { return animations; }
            set { animations = value; }
        }
        public static List<Animation> FinishedAnims {
            get { return finishedAnims; }
            set { finishedAnims = value; }
        }
        public static Identification Identification {
            get { return identification; }
            set { identification = value; }
        }
        public static TimeHandler TimeHandler {
            get { return timeHandler; }
            set { timeHandler = value; }
        }
        public static AudioPlaybackEngine AudioEngine {
            get { return audioEngine; }
            set { audioEngine = value; }
        }
        public static Random RNG {
            get { return rng; }
            set { rng = value; }
        }
    }
}
