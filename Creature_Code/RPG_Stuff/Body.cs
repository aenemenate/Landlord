using System.Collections.Generic;

namespace Landlord
{
    class Body
    {
        private bool rightHanded;

        private Helmet helmet;
        private ChestPiece chestPiece;
        private Shirt shirt;
        private Gauntlets gauntlets;
        private Leggings leggings;
        private Boots boots;

        private Item rightHand;
        private Item leftHand;


        // CONSTRUCTOR

        public Body()
        {
            rightHanded = true;
            int rand = Program.RNG.Next(0, 10);
            if (rand == 0) // 1 in 10 chance
                rightHanded = false;

            helmet = null;
            chestPiece = null;
            shirt = null;
            gauntlets = null;
            leggings = null;
            boots = null;
            rightHand = null;
            leftHand = null;
        }

        public List<Armor> GetArmorList()
        {
            List<Armor> armorList = new List<Armor>();
            if (helmet != null)
                armorList.Add(helmet);
            if (chestPiece != null)
                armorList.Add(chestPiece);
            if (shirt != null)
                armorList.Add(shirt);
            if (gauntlets != null)
                armorList.Add(gauntlets);
            if (leggings != null)
                armorList.Add(leggings);
            if (boots != null)
                armorList.Add(boots);

            return armorList;
        }

        public List<Item> GetEquipmentList()
        {
            List<Item> equipList = new List<Item>();
            if (helmet != null)
                equipList.Add(helmet);
            if (chestPiece != null)
                equipList.Add(chestPiece);
            if (shirt != null)
                equipList.Add(shirt);
            if (gauntlets != null)
                equipList.Add(gauntlets);
            if (leggings != null)
                equipList.Add(leggings);
            if (boots != null)
                equipList.Add(boots);
            if (rightHand != null)
                equipList.Add(rightHand);
            if (leftHand != null)
                equipList.Add(leftHand);

            return equipList;
        }


        // PROPERTIES

        public bool RightHanded
        {
            get { return rightHanded; }
        }

        public Helmet Helmet
        {
            get { return helmet; }
            set { helmet = value; }
        }

        public ChestPiece ChestPiece
        {
            get { return chestPiece; }
            set { chestPiece = value; }
        }

        public Shirt Shirt
        {
            get { return shirt; }
            set { shirt = value; }
        }

        public Gauntlets Gauntlets
        {
            get { return gauntlets; }
            set { gauntlets = value; }
        }

        public Leggings Leggings
        {
            get { return leggings; }
            set { leggings = value; }
        }

        public Boots Boots
        {
            get { return boots; }
            set { boots = value; }
        }
        
        public Item MainHand
        {
            get { return rightHanded ? rightHand : leftHand; }
            set { if (rightHanded) rightHand = value; else leftHand = value; }

        }

        public Item OffHand
        {
            get { return rightHanded ? leftHand : rightHand; }
            set { if (rightHanded) leftHand = value; else rightHand = value; }

        }
    }
}