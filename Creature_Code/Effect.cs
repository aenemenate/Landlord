using System.IO;
using Polenter.Serialization;

namespace Landlord
{
    public enum StatusEffect
    {
        Frozen,
        Paralyzed,
        Confused,
        None
    }

    class Effect
    {
        private string name;
        private Creature user;

        private int[] victimResourceDamage = new int[3]; // HP, MP, SP
        private DamageType victimHPDamageType;

        private StatusEffect victimStatusEffect;

        private int[] victimResourceHealing = new int[3]; // HP, MP, SP
        private int[] userResourceHealing = new int[3]; // HP, MP, SP

        private int chance;
        private int totalTurns;
        private int turns;

        // CONSTRUCTORS //

        public Effect(string name, int[] victimResourceDamage, DamageType victimHPDamageType, StatusEffect victimStatusEffect, int[] victimResourceHealing, int[] userResourceHealing, int chance, int turns)
        {
            this.name = name.Replace('_', ' ');
            this.user = null;

            this.victimResourceDamage = victimResourceDamage;
            this.victimHPDamageType = victimHPDamageType;

            this.victimStatusEffect = victimStatusEffect;

            this.victimResourceHealing = victimResourceHealing;
            this.userResourceHealing = userResourceHealing;

            this.chance = chance;
            totalTurns = turns;
            this.turns = totalTurns;
        }
        
        public Effect()
        {
        }


        // FUNCTIONS //

        public void Apply(Creature victim)
        {
            string victimText = $"{victim.Name}";
            string userText = $"{user.Name}";

            if (victimHPDamageType != DamageType.None)
            {
                int damageTaken = victim.DefendAgainstDmg(victimHPDamageType, victimResourceDamage[0]);
                victimText += $" recieved {damageTaken} {victimHPDamageType.ToString()} damage,";
            }
            if (victimResourceDamage[1] > 0)
            {
                victim.ChangeResource(Resource.MP, - victimResourceDamage[1]);
                victimText += $" lost {victimResourceDamage[1]} magicka,";
            }
            if (victimResourceDamage[2] > 0)
            {
                victim.ChangeResource(Resource.SP, -victimResourceDamage[2]);
                victimText += $" lost {victimResourceDamage[2]} stamina,";
            }
            if (victimResourceHealing[0] > 0)
            {
                victim.ChangeResource(Resource.HP, victimResourceHealing[0]);
                victimText += $" had {victimResourceHealing[0]} health restored,";
            }
            if (victimResourceHealing[1] > 0)
            {
                victim.ChangeResource(Resource.MP, victimResourceHealing[1]);
                victimText += $" had {victimResourceHealing[1]} magicka restored,";
            }
            if (victimResourceHealing[2] > 0)
            {
                victim.ChangeResource(Resource.SP, victimResourceHealing[2]);
                victimText += $" had {victimResourceHealing[2]} stamina restored,";
            }
            if (userResourceHealing[0] > 0)
            {
                user.ChangeResource(Resource.SP, userResourceHealing[0]);
                userText += $" recieved {userResourceHealing[0]} health,";
            }
            if (userResourceHealing[1] > 0)
            {
                user.ChangeResource(Resource.SP, userResourceHealing[1]);
                userText += $" recieved {userResourceHealing[1]} magicka,";
            }
            if (userResourceHealing[2] > 0)
            {
                user.ChangeResource(Resource.SP, userResourceHealing[2]);
                userText += $" recieved {userResourceHealing[2]} stamina,";
            }

            if (!victimText.Equals($"{victim.Name}"))
            {
                victimText = victimText.Substring(0, victimText.LastIndexOf(','));
                if (victimText.Contains(","))
                {
                    int commaIndex = victimText.LastIndexOf(',');
                    victimText = victimText.Substring(0, commaIndex) + " and " + victimText.Substring(commaIndex + 2, victimText.Length - (commaIndex + 2));
                }
                victimText += $" from {name}.";
                Program.MsgConsole.WriteLine(victimText);
            }
            if (!userText.Equals($"{user.Name}"))
            {
                userText = userText.Substring(0, userText.LastIndexOf(','));
                if (userText.Contains(","))
                {
                    int commaIndex = userText.LastIndexOf(',');
                    userText = userText.Substring(0, commaIndex) + " and " + userText.Substring(commaIndex + 2, userText.Length - (commaIndex + 2));
                }
                userText += $" from {name}.";
                Program.MsgConsole.WriteLine(userText);
            }

            turns--;
            if (turns == 0)
                victim.Effects.Remove(this);
        }

        public Effect Copy()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                SharpSerializer s = new SharpSerializer(true);
                s.Serialize(this, ms);
                ms.Position = 0;
                return (Effect)s.Deserialize(ms);
            }
        }


        // PROPERTIES //

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Creature User
        {
            get { return user; }
            set { user = value; }
        }

        public int[] VictimResourceDamage
        {
            get { return victimResourceDamage; }
            set { victimResourceDamage = value; }
        }

        public DamageType VictimHPDamageType
        {
            get { return victimHPDamageType; }
            set { victimHPDamageType = value; }
        }

        public StatusEffect VictimStatusEffect
        {
            get { return victimStatusEffect; }
            set { victimStatusEffect = value; }
        }

        public int[] VictimResourceHealing
        {
            get { return victimResourceHealing; }
            set { victimResourceHealing = value; }
        }

        public int[] UserResourceHealing
        {
            get { return userResourceHealing; }
            set { userResourceHealing = value; }
        }

        public int Chance
        {
            get { return chance; }
            set { chance = value; }
        }

        public int Turns
        {
            get { return turns; }
            set { turns = value; }
        }

        public int TotalTurns
        {
            get { return totalTurns; }
            set { totalTurns = value; }
        }
    }
}