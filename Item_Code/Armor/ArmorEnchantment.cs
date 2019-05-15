using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{

    class ArmorEnchantment
    {
        private int effect;
        private DamageType damageType;


        // CONSTRUCTORS

        public ArmorEnchantment(DamageType damageType, int effect)
        {
            this.damageType = damageType;
            this.effect = effect;
        }

        public ArmorEnchantment()
        {
        }


        // FUNCTIONS

        private string DetermineName()
        {
            switch (damageType)
            {
                case (DamageType.Blunt):
                    return "blunt defense";
                case (DamageType.Burn):
                    return "burn defense";
                case (DamageType.Frost):
                    return "frost defense";
                case (DamageType.Magic):
                    return "magic defense";
                case (DamageType.Shear):
                    return "shear defense";
                case (DamageType.Shock):
                    return "shock defense";
                default:
                    return "filthy hacker";
            }
        }


        // PROPERTIES

        public int Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public DamageType DamageType
        {
            get { return damageType; }
            set { damageType = value; }
        }

        public string Name
        {
            get { return DetermineName(); }
        }
    }
}
