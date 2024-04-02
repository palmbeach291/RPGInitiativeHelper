using System.ComponentModel;

namespace RPGInitiativeHelper
{
    internal class Fighter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { set; get; }
        public string Note { set; get; }
        public int Initiative { set; get; }
        public int Life { set; get; }
        public int MaxLife { set; get; }
        public int Mana { set; get; }
        public int MaxMana { set; get; }
        public int Karma { set; get; }
        public int MaxKarma { set; get; }
        public string Display
        {
            get
            {
                string retVal = "";
                retVal = Initiative.ToString() + " " + Name + " " + Life.ToString() + "/" + MaxLife.ToString();
                return retVal;
            }
        }
        public bool Alive = true;

        public Fighter(string name, int initiative, int maxLife, int maxMana = 0, int maxKarma = 0, string note = "")
        {
            Name = name;
            Initiative = initiative;
            MaxLife = maxLife;
            Life = maxLife;
            MaxMana = maxMana;
            Mana = maxMana;
            MaxKarma = maxKarma;
            Note = note;
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GetHeal(int healing = 1)
        {
            if (MaxLife < Life + healing)
                Life = MaxLife;
            else
                Life += healing;

            if(!Alive && Life >= 0)
                Alive= true;
        }

        public void GetDamage(int damage = 1)
        {
            Life -= damage;

            if (Alive && Life < 0)
                Alive = false;
        }
    }
}
