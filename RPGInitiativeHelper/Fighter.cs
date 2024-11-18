using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;

namespace RPGInitiativeHelper
{
    public class Fighter
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
        public int Offence { set; get; }
        public int Defence { set; get; }
        public int Armor { set; get; }
        public string Damage { set; get; }
        public bool isDowned {
            get
            {
                return Life < 0;
            }
        }
        public Status.StatusValue State { set; get; }
        public string PlayerName { set; get; }
        public List<FighterState> Buffs { set; get; }
        public bool isPlayer
        {
            get
            {
                if (PlayerName.ToLower() == "npc")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string Display
        {
            get
            {
                return $"{Initiative} {Name} {Life}/{MaxLife}";
            }
        }

        public Brush Color
        {
            get
            {
                SolidColorBrush retVal;
                if (isDowned)
                {
                    retVal = Brushes.Salmon;
                }
                else
                {
                    if (State == Status.StatusValue.Active)
                        retVal = Brushes.LightGreen;
                    else if (State == Status.StatusValue.Done)
                        retVal = Brushes.LightGray;
                    else
                        retVal = Brushes.White;
                }
                return retVal;
            }
        }

        public Fighter(string name, int initiative, int maxLife, string playerName = "NPC", int maxMana = 0, int maxKarma = 0, string note = "", int offence = 1, int defence = 1, int armor = 1, string damage = "1w6")
        {
            Name = name;
            Initiative = initiative;
            MaxLife = maxLife;
            Life = maxLife;
            MaxMana = maxMana;
            Mana = maxMana;
            MaxKarma = maxKarma;
            Note = note;
            PlayerName = playerName;
            State = Status.StatusValue.Standard;
            Buffs = new List<FighterState>();
            Offence = offence;
            Defence = defence;
            Armor = armor;
            Damage = damage;
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

            if (!isDowned)
                State = Status.StatusValue.Done;
        }

        public void GetMana(int diffMana = 1)
        {
            if (MaxMana < Mana + diffMana)
                Mana = MaxMana;
            else
                Mana += diffMana;
        }

        public void GetDamage(int damage = 1)
        {
            Life -= damage;
        }

        public void LooseMana(int diffMana = 1)
        {
            if (Mana > diffMana)
                Mana -= diffMana;
            else
                Mana = 0;
        }

        public void DecreaseBuff()
        {
            foreach (FighterState Buff in Buffs)
            {
                if (!Buff.isFresh)
                {
                    Buff.rounds--;
                }
                Buff.isFresh = false;
            }

            // Liste mit den zu entfernenden Buffs
            List<FighterState> buffsToRemove = Buffs.Where(buff => buff.rounds < 1).ToList();
            foreach (FighterState Buff in buffsToRemove)
            {
                Buffs.Remove(Buff);
            }
        }
    }

    public struct Status
    {
        public enum StatusValue
        {
            Active,
            Done,
            Standard
        }

        public StatusValue Value { get; }

        public Status(StatusValue value)
        {
            Value = value;
        }
    }
}
