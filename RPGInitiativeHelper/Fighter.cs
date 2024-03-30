using System.ComponentModel;
using System.Xml.Linq;

namespace RPGInitiativeHelper
{
    internal class Fighter
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Note { set; get; }
        public int Initiative { set; get; }
        public int Life { set; get; }
        public int MaxLife { set; get; }
        public int Mana { set; get; }
        public int MaxMana { set; get; }
        public int Karma { set; get; }
        public int MaxKarma { set; get; }
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
    }
}
