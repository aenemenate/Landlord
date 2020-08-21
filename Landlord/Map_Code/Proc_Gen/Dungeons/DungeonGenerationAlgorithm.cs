using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    abstract class DungeonGenerationAlgorithm
    {
        protected Random rng;
        protected Point firstRoomPos;
        protected DungeonFloor dungeonFloor;
        protected List<string> monsterTypes;

        protected DungeonGenerationAlgorithm(DungeonFloor dungeonFloor, List<string> monsterTypes)
        {
            this.dungeonFloor = dungeonFloor;
            this.monsterTypes = monsterTypes;
            rng = new Random();
        }

        public abstract void GenerateDungeon(int size, int floor, Point worldIndex);
    }
}
