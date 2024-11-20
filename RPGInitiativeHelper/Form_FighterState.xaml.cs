using RPGInitiativeHelper.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaktionslogik für Form_FighterState.xaml
    /// </summary>
    public partial class Form_FighterState : Window
    {
        ConfigManager _configManager = new ConfigManager();
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

            PaintMe();
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

        private void PaintMe()
        {
            this.Background = _configManager.BackgroundColor;
            // Neuen dynamischen Label-Style erstellen
            Style newLabelStyle = new Style(typeof(Label));
            newLabelStyle.Setters.Add(new Setter(Label.FontWeightProperty, _configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newLabelStyle.Setters.Add(new Setter(Label.FontFamilyProperty, new FontFamily(_configManager.FontFamily)));
            newLabelStyle.Setters.Add(new Setter(Label.FontSizeProperty, (double)_configManager.FontSize));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["LabelStyle"] = newLabelStyle;

            // Neuen dynamischen Button-Style erstellen
            Style newButtonStyle = new Style(typeof(Button));
            newButtonStyle.Setters.Add(new Setter(Button.FontWeightProperty, _configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newButtonStyle.Setters.Add(new Setter(Button.FontFamilyProperty, new FontFamily(_configManager.FontFamily)));
            newButtonStyle.Setters.Add(new Setter(Button.FontSizeProperty, (double)_configManager.FontSize));
            newButtonStyle.Setters.Add(new Setter(Button.BackgroundProperty, _configManager.MenuColor));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["ButtonStyle"] = newButtonStyle;

            // Optional: Layout aktualisieren
            this.UpdateLayout();
        }
    }
}
