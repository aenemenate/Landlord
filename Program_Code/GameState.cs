using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Landlord
{
    public enum PlayMode
    {
        Roguelike,
        BuildMode
    }

    abstract class GameState
    {
        public GameState()
        {
        }
        public abstract void Update();
        public abstract void Render();
        public abstract void ClientSizeChanged();
    }

}
