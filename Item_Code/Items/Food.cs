using System;
using Microsoft.Xna.Framework;

namespace Landlord.Item_Code.Items
{
    class Food : Item
    {
        int effect;
        DietType foodType;

        // CONSTRUCTORS
        public Food(string name, byte graphic, double volume, int effect, DietType foodType, Color? color, ItemType type = ItemType.Food, bool hollow = false, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType, name)
        {
            this.foodType = foodType;
            ForeColor = (Color)color;
            this.effect = effect;
            Rarity = DetermineRarity();
        }
        public Food()
        {

        }

        // FUNCTIONS
        public override void Activate(Creature user)
        {
            user.Eat(this);
        }
        public override string DetermineName(bool identifying)
        {
            return "";
        }
        public override string DetermineDescription()
        {
            return foodType == DietType.Carnivore ? $"A piece of meat." : $"A {Name}.";
        }
        public override Material DetermineMaterial()
        {
            return foodType == DietType.Carnivore ? Material.Meat : Material.Plant;
        }
        public override Rarity DetermineRarity()
        {
            return Rarity.Common;
        }

        // PARAMETERS
        public int Effect {
            get { return effect; }
            set { effect = value; }
        }
        public DietType FoodType {
            get { return foodType; }
            set { foodType = value; }
        }
    }
}
