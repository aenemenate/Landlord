using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Landlord
{
    class GeneratingWorldMap : GameState
    {
        private Class uClass;
        private string gender, name;
        public GeneratingWorldMap(Class uClass, string gender, string name) : base()
        {
            this.uClass = uClass;
            this.gender = gender;
            this.name = name;
        }

        public override void Update()
        {
            Menus.LoadSave.GenerateWorldMapScreen();
        }

        public override void Render(ref SadConsole.Console console, ref Window window)
        {
        }

        public override void ClientSizeChanged()
        {

        }

        //PROPERTIES//

        public Class UClass { get { return uClass; } set { uClass = value; } }
        public string Gender {
            get { return gender; }
            set { gender = value; }
        }
        public string Name {
            get { return name; }
            set { name = value; }
        }
    }
}
