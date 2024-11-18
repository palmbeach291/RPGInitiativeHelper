﻿using System.Windows;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaktionslogik für Form_FighterState.xaml
    /// </summary>
    public partial class Form_FighterState : Window
    {
        public FighterState fighterState { get; set; }
        // Konstruktor, der eine Kopie des Original-Objekts verwendet
        public Form_FighterState(FighterState fs)
        {
            InitializeComponent();
            fighterState = new FighterState(fs); // Kopie des Objekts erstellen
            InitialConfig();
        }

        public Form_FighterState(Fighter parent)
        {
            InitializeComponent();
            fighterState = new FighterState(parent, "", 1);
            InitialConfig();
        }

        public void InitialConfig()
        {
            TB_Name.Text = fighterState.name;
            TB_Description.Text = fighterState.description;
            DurationNumericUpDown.Value = fighterState.rounds;

            if (fighterState.isBonus)
            {
                this.RB_Boni.IsChecked = true;
            }
            else
            {
                this.RB_Mali.IsChecked = true;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TB_Name.Text) && DurationNumericUpDown.Value != null)
            {
                fighterState.name = TB_Name.Text;
                fighterState.description = TB_Description.Text;
                fighterState.rounds = (int)DurationNumericUpDown.Value;
                fighterState.isBonus = (bool)this.RB_Boni.IsChecked;

                this.DialogResult = true; // DialogResult setzen
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // DialogResult setzen
            this.Close();
        }
    }
}
