using System.ComponentModel;
using System.Windows.Media;

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
        public Status.StatusValue State { set; get; }
        public string Display
        {
            get
            {
                string retVal = "";
                retVal = Initiative.ToString() + " " + Name + " " + Life.ToString() + "/" + MaxLife.ToString();
                return retVal;
            }
        }

        public Brush Color
        {
            get { 
                SolidColorBrush retVal = Brushes.White;

                if (State == Status.StatusValue.Active)
                    retVal = Brushes.LightGreen;
                else if (State == Status.StatusValue.Done)
                    retVal = Brushes.LightGray;
                else if (State == Status.StatusValue.Downed)
                    retVal = Brushes.Salmon;
                else 
                    retVal = Brushes.White;

                return retVal;            
            }
        }

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
            State = Status.StatusValue.Standard;
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

            if(State == Status.StatusValue.Downed && Life >= 0)
                State = Status.StatusValue.Done;
        }

        public void GetDamage(int damage = 1)
        {
            Life -= damage;

            if (State != Status.StatusValue.Downed && Life < 0)
                State = Status.StatusValue.Downed;
        }
    }

    public struct Status
    {
        public enum StatusValue
        {
            Active,
            Done,
            Standard,
            Downed
        }

        public StatusValue Value { get; }

        public Status(StatusValue value)
        {
            Value = value;
        }
    }
}
