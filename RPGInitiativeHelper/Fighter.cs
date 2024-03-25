namespace RPGInitiativeHelper
{
    internal class Fighter
    {
        public string Name { set; get; }
        public string Note { set; get; }
        public int Initiative
        {
            set
            {
                Initiative += value;
                if (Initiative < 0)
                    Initiative = 0;
            }
            get
            {
                return Initiative;
            }
        }
        public int Life
        {
            set
            {
                Life += value;
                if (value < 0)
                    Alive = false;
                else if (Life > MaxLife)
                    Life = MaxLife;
            }
            get
            {
                return Life;
            }
        }
        public int MaxLife { set; get; }
        public int Mana
        {
            set
            {
                Mana += value;
                if (value < 0)
                    Mana = 0;
                else if (Mana > MaxMana)
                    Mana = MaxMana;
            }
            get
            {
                return Mana;
            }
        }
        public int MaxMana { set; get; }
        public int Karma
        {
            set
            {
                Karma += value;
                if (value < 0)
                    Karma = 0;
                else if (Karma > MaxKarma)
                    Karma = MaxKarma;
            }
            get
            {
                return Karma;
            }
        }
        public int MaxKarma { set; get; }
        public bool Alive = true;

        public Fighter(string name, int initiative, int maxLife, int maxMana, string note = "")
        {
            Name = name;
            Initiative = initiative;
            MaxLife = maxLife;
            Life = maxLife;
            MaxMana = maxMana;
            Mana = maxMana;
            Note = note;
        }
    }
}
