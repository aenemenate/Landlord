using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    static class DungeonHandler
    {
        public static void CreateDungeon(MapTile map)
        {
            map.Dungeon = DataReader.GetNextDungeon(Program.Player.Stats.Level.Lvl);
        }
    }
}
