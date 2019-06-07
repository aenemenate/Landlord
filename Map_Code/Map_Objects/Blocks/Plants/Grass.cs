using Microsoft.Xna.Framework;

namespace Landlord
{
    class Grass : Plant
    {
        public Grass(bool instantiating, byte graphic = 34, string name = "grass") : base(graphic, name)
        {
            ForeColor = Color.LawnGreen;
            BackColor = new Color(87, 59, 12);
        }
        public Grass() : base()
        {
        }
    }
}
