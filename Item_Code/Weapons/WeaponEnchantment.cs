using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Landlord
{
    class WeaponEnchantment
    {
        private string name;
        private string partName;
        private int victimDamage;
        private DamageType damageType;
        private Effect appliedEffect;


        // CONSTRUCTORS //

        public WeaponEnchantment(string name, string partName, DamageType damageType, int victimDamage, Effect appliedEffect = null)
        {
            this.name = name.Replace('_', ' ');
            this.partName = partName.Replace('_', ' ');
            this.damageType = damageType;
            this.victimDamage = victimDamage;
            this.appliedEffect = appliedEffect;
        }

        public WeaponEnchantment()
        {

        }


        // FUNCTIONS //

        public void Apply(Creature user, Creature victim)
        {
            victim.DefendAgainstDmg(damageType, victimDamage);

            if (appliedEffect != null)
            {
                if (Program.RNG.Next(0, 100) < appliedEffect.Chance)
                {
                    Effect effectInstance = appliedEffect.Copy();
                    effectInstance.User = user;
                    Program.MsgConsole.WriteLine($"{user.Name} inflicted {victim.Name} with {effectInstance.Name}.");
                    victim.Effects.Add(effectInstance.Copy());
                }
            }
        }


        // PROPERTIES //


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string PartName
        {
            get { return partName; }
            set { partName = value; }
        }

        public int VictimDamage
        {
            get { return victimDamage; }
            set { victimDamage = value; }
        }

        public DamageType DamageType
        {
            get { return damageType; }
            set { damageType = value; }
        }

        public Effect AppliedEffect
        {
            get { return appliedEffect; }
            set { appliedEffect = value; }
        }
    }
}
