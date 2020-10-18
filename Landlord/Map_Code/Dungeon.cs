using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{

    class Dungeon
    {
        public event EventHandler<EventArgs> OnFinishedGenerating;

        private string name;
        private DungeonType dungeonType;
        private int startLvl;
        private List<String> monsterTypes;
        private DungeonFloor[] floors;


        // CONSTRUCTORS //

        public Dungeon(string name, DungeonType dungeonType, List<String> monsterTypes, int floors)
        {
            this.name = name;
            this.dungeonType = dungeonType;
            this.monsterTypes = monsterTypes;
            this.floors = new DungeonFloor[floors];
            startLvl = Program.Player.Stats.Level.Lvl;
        }

        public Dungeon() { }


        // FUNCTIONS //

        public void Init()
        {
            Point firstRoomPos = Program.Player.Position;
            Point worldIndex = Program.Player.WorldIndex;

            for (int i = 0; i < floors.GetLength(0); i++) {
                floors[i] = new DungeonFloor(new Point(Program.WorldMap.TileWidth, Program.WorldMap.TileHeight));
                floors[i].Init(dungeonType, monsterTypes, worldIndex, i);
            }

            OnFinishedGenerating(this, EventArgs.Empty);
        }

        // PROPERTIES //

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DungeonType DungeonType
        {
            get { return dungeonType; }
            set { dungeonType = value; }
        }

        public int StartLvl
        {
            get { return startLvl; }
            set { startLvl = value; }
        }

        public List<string> MonsterTypes
        {
            get { return monsterTypes; }
            set { monsterTypes = value; }
        }

        public DungeonFloor[] Floors
        {
            get { return floors; }
            set { floors = value; }
        }
    }
}
