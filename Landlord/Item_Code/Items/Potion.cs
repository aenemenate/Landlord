using Microsoft.Xna.Framework;
namespace Landlord
{
    public enum PotionType
    {
        Healing,
        MajorHealing,
        Magicka,
        MajorMagicka,
        Energy,
        MajorEnergy
    }

    class Potion : Item
    {
        int effect;
        PotionType potionType;


        // CONSTRUCTORS

        public Potion(bool instantiating, byte graphic = 235, ItemType type = ItemType.Potion, double volume = 0.016, bool hollow = false, DamageType damageType = DamageType.Blunt)
               : base(graphic, type, volume, hollow, damageType)
        {
            potionType = DeterminePotionType();
            ForeColor = Program.Identification.PotionColors[potionType];
            effect = DetermineEffect();
            Rarity = DetermineRarity();
        }

        public Potion()
        {

        }


        // FUNCTIONS

        public override void Activate(Creature user)
        {
            bool identified = Identified;
            string text = "";
            Identify();
            switch (potionType)
            {
                case (PotionType.Healing):
                    user.ChangeResource(Resource.HP, effect);
                    text = "You feel a bit healthier! That must have been a potion of healing.";
                    break;
                case (PotionType.MajorHealing):
                    user.ChangeResource(Resource.HP, effect);
                    text = "You feel a lot healthier! That must have been a potion of major healing.";
                    break;
                case (PotionType.Magicka):
                    user.ChangeResource(Resource.MP, effect);
                    text = "Your connection to the aether has become a bit stronger! That must have been a potion of magicka.";
                    break;
                case (PotionType.MajorMagicka):
                    user.ChangeResource(Resource.MP, effect);
                    text = "Your connection to the aether has become a lot stronger! That must have been a potion of major magicka.";
                    break;
                case (PotionType.Energy):
                    user.ChangeResource(Resource.SP, effect);
                    text = "You feel less tired! That must have been a potion of energy.";
                    break;
                case (PotionType.MajorEnergy):
                    user.ChangeResource(Resource.SP, effect);
                    text = "You feel rejuvenated! That must have been a potion of major energy.";
                    break;
            }
            if (!identified)
                Menus.DisplayIdentified(text);
        }

        public override string DetermineName(bool identifying)
        {
            if (identifying || !Identified)
                return Program.Identification.ColorNames[ForeColor] + " potion";
            else
            {
                switch (potionType)
                {
                    case (PotionType.Healing):
                        return "potion of healing";
                    case (PotionType.MajorHealing):
                        return "potion of major healing";
                    case (PotionType.Magicka):
                        return "potion of magicka";
                    case (PotionType.MajorMagicka):
                        return "potion of major magicka";
                    case (PotionType.Energy):
                        return "potion of energy";
                    case (PotionType.MajorEnergy):
                        return "potion of major energy";
                }
            }
            return "";
        }

        public override string DetermineDescription()
        {
            return $"A{ReturnIdentifiedString()} drink made from the magical essences of certain plants.";
        }

        public override Material DetermineMaterial()
        {
            return Material.Glass;
        }

        private PotionType DeterminePotionType()
        {
            int rand = Program.RNG.Next(0, 101);

            if (rand <= 30)
                return PotionType.Healing;
            if (rand <= 30 + 25)
                return PotionType.Energy;
            if (rand <= 30 + 25 + 25)
                return PotionType.Magicka;
            if (rand <= 30 + 25 + 25 + 7)
                return PotionType.MajorHealing;
            if (rand <= 30 + 25 + 25 + 7 + 7)
                return PotionType.MajorEnergy;
            else
                return PotionType.MajorMagicka;
        }

        public override Rarity DetermineRarity()
        {
            switch (potionType)
            {
                case (PotionType.Healing):
                case (PotionType.Energy):
                case (PotionType.Magicka):
                    return Rarity.Common;
                case (PotionType.MajorHealing):
                case (PotionType.MajorEnergy):
                case (PotionType.MajorMagicka):
                    return Rarity.Uncommon;
                default:
                    return Rarity.Common;
            }
        }

        private int DetermineEffect()
        {
            switch (potionType)
            {
                case (PotionType.Healing):
                case (PotionType.Magicka):
                case (PotionType.Energy):
                    return Program.RNG.Next(20, 50);
                case (PotionType.MajorHealing):
                case (PotionType.MajorMagicka):
                case (PotionType.MajorEnergy):
                    return Program.RNG.Next(60, 150);
            }
            return 0;
        }


        // PARAMETERS

        public int Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public PotionType PotionType
        {
            get { return potionType; }
            set { potionType = value; }
        }
    }
}
