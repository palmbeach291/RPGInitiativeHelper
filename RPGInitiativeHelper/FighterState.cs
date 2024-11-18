using RPGInitiativeHelper.Configuration;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace RPGInitiativeHelper
{
    public class FighterState
    {
        public string name { get; set; }
        public int rounds { get; set; }
        public string description { get; set; }
        public bool isBonus { get; set; }
        public bool isFresh = false;

        public FighterState(Fighter parent, string name, int rounds, string description = "")
        {
            this.name = name;
            this.rounds = rounds;
            this.description = description;
            this.isBonus = true;
        }

        public string Display
        {
            get
            {
                return $"{name} {rounds}";
            }
        }

        public Brush Color
        {
            get
            {
                SolidColorBrush retVal;

                if (isBonus)
                    retVal = Brushes.LightGreen;
                else
                    retVal = Brushes.LightSalmon;

                return retVal;
            }
        }
    }
}
