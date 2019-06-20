using Microsoft.Xna.Framework;

namespace Landlord
{
    class Food : Item
    {
        private DietType foodType;
        private string foodName;
        public Food( DietType foodType, string name, byte graphic, double volume, Color? foreColor = null, ItemType type = ItemType.Food, bool hollow = false, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            ForeColor = Physics.MaterialColors[foodType == DietType.Herbivore ? Material.Plant : Material.Meat];
            if (foreColor != null)
                ForeColor = (Color)foreColor;
            foodName = name;
            Identify();
        }

        public Food() : base()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
        }

        public override string DetermineDescription()
        {
            return $"A {foodName}.";
        }

        public override string DetermineName(bool identifying)
        {
            return foodName;
        }

        public override Material DetermineMaterial()
        {
            return foodType == DietType.Herbivore ? Material.Plant : Material.Meat;
        }

        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }

        public DietType FoodType {
            get { return foodType; }
            set { foodType = value; }
        }
        public string FoodName
        {
            get { return foodName; }
            set { foodName = value; }
        }
    }
}
