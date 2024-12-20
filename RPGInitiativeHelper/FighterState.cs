﻿using RPGInitiativeHelper.Configuration;
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

        // Kopierkonstruktor
        public FighterState(FighterState original)
        {
            this.name = original.name;
            this.rounds = original.rounds;
            this.description = original.description;
            this.isBonus = original.isBonus;
            this.isFresh = original.isFresh;
        }

        public string Display
        {
            get
            {
                return $"{name} Runden: {rounds}";
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
