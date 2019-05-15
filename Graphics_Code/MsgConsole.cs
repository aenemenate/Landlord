using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Console = SadConsole.Console;

namespace Landlord
{
    class MsgConsole
    {
        Console console;
        const int maxDisplayedLines = 3;
        int curDisplayedLines = 0;
        int memoryLen;
        List<Message> lines= new List<Message>();
        TimeSpan msgTime;


        // CONSTRUCTOR //

        public MsgConsole(int memoryLen, TimeSpan msgTime)
        {
            console = new Console(Program.Window.Width, Program.Window.Height);
            this.memoryLen = memoryLen;
            this.msgTime = msgTime;
        }


        // FUNCTIONS //

        public void WriteLine(string line, Color? color = null)
        {
            Color temp = (color == null) ? Color.AntiqueWhite : (Color)color;
            lines.Add(new Message(line, temp, DateTime.Now));
            if (curDisplayedLines < maxDisplayedLines)
                curDisplayedLines++;
        }

        public void Clear()
        {
            curDisplayedLines = 0;
        }

        public void Render()
        {
            Console.Clear();
            if (lines.Count == 0 || curDisplayedLines == 0)
                return;
            if (DateTime.Now - lines[lines.Count - curDisplayedLines].WriteTime > msgTime)
                curDisplayedLines--;
            for (int i = 0; i < curDisplayedLines; i++)
                Program.Window.PrintMessage(i, lines[lines.Count - (curDisplayedLines - i)].Text, GUI.MapWidth - 1, lines[lines.Count - 1 - i].Color);
        }


        // PROPERTIES //

        public int CurDisplayedLines
        {
            get { return curDisplayedLines; }
            set { curDisplayedLines = value; }
        }

        public Console Console
        {
            get { return console; }
            set { console = value; }
        }
    }

    struct Message
    {
        string text;
        Color color;
        DateTime writeTime;

        public Message(string text, Color color, DateTime writeTime)
        {
            this.text = text;
            this.color = color;
            this.writeTime = writeTime;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public DateTime WriteTime
        {
            get { return writeTime; }
            set { writeTime = value; }
        }
    }
}
