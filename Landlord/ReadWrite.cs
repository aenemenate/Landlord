using Polenter.Serialization;
using System;
using System.IO;
using System.Collections.Generic;

namespace Landlord
{
    static class ReadWrite
    {
        private static Player player;
        private static WorldMap worldMap;
        private static Identification identification;
        private static TimeHandler timeHandler;
        private static List<Faction> factions;
        public static event EventHandler<EventArgs> OnFinishedLoading;
        
        public static void LoadObjHoldersFromProgram()
        {
            player = Program.Player;
            worldMap = Program.WorldMap;
            identification = Program.Identification;
            timeHandler = Program.TimeHandler;
            factions = Program.Factions;
        }
        public static void SetObjHoldersToProgram()
        {
            Program.Player = player;
            Program.WorldMap = worldMap;
            Program.Identification = identification;
            Program.TimeHandler = timeHandler;
            Program.Factions = factions;
        }
        public static void SaveGame()
        {
            string text = worldMap.Name + ';';
            string fileName = player.Name;
            System.IO.File.WriteAllText($@"saves\{fileName}.lls", text);

            var serializer = new SharpSerializer(true);

            fileName = player.Name;
            serializer.Serialize(player, $@"saves\data\{fileName}.lls");

            fileName = worldMap.Name;
            serializer.Serialize(worldMap, $@"saves\data\{fileName}.lls");
            serializer.Serialize(identification, $@"saves\data\identification.lls");
            serializer.Serialize(timeHandler, $@"saves\data\time.lls");
            serializer.Serialize(factions, $@"saves\data\factions.lls");
            OnFinishedLoading(typeof(ReadWrite), EventArgs.Empty);
        }
        public static void LoadGame()
        {
            player = new Player();
            worldMap = new WorldMap(4, 4, 100, 100, "");
            identification = new Identification();
            DirectoryInfo d = new DirectoryInfo(@"saves\");
            FileInfo[] Files = d.GetFiles("*.lls");
            string filename = "";
            filename += Files[0].Name;
            string playerName = "";
            foreach (char c in filename) {
                if (c == '.')
                    break;
                else
                    playerName += c;
            }
            string text = System.IO.File.ReadAllText($@"saves\{playerName}.lls");
            string mapName;

            string temp = "";
            int i;
            for (i = 0; text[i] != ';'; i++)
                temp += text[i]; 
            mapName = temp;

            var serializer = new SharpSerializer(true);
            player = (Player)serializer.Deserialize($@"saves\data\{playerName}.lls");

            serializer = new SharpSerializer(true);
            worldMap = (WorldMap)serializer.Deserialize($@"saves\data\{mapName}.lls");
            serializer = new SharpSerializer(true);
            identification = (Identification)serializer.Deserialize($@"saves\data\identification.lls");
            serializer = new SharpSerializer(true);
            timeHandler = (TimeHandler)serializer.Deserialize($@"saves\data\time.lls");
            serializer = new SharpSerializer(true);
            factions = (List<Faction>)serializer.Deserialize($@"saves\data\factions.lls");
            OnFinishedLoading(typeof(ReadWrite), EventArgs.Empty);
        }
        public static void DeleteSave()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo($@"saves\");
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
            di = new DirectoryInfo($@"saves\data");
            foreach (FileInfo file in di.GetFiles())
                file.Delete();
        }
    }
}
