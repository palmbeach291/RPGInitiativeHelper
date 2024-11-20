using RPGInitiativeHelper.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGInitiativeHelper
{
    public partial class Form_PlayerName : Window
    {
        public string PlayerName { get; private set; }

        private ConfigManager _configManager = new ConfigManager();

        public Form_PlayerName()
        {
            InitializeComponent();
            PaintMe();
            // Registriere das Loaded-Event des Fensters
            this.Loaded += Form_PlayerName_Loaded;
        }

        // Event-Handler für das Loaded-Event
        private void Form_PlayerName_Loaded(object sender, RoutedEventArgs e)
        {
            // Setze den Fokus auf das TextBox-Element
            TB_PlayerName.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = TB_PlayerName.Text;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PlayerName = null;
            this.Close();
        }

        private void PaintMe()
        {
            this.Background = _configManager.BackgroundColor;

            Style newLabelStyle = new Style(typeof(Label));
            Style newTextBoxStyle = new Style(typeof(TextBox));
            Style newListViewStyle = new Style(typeof(ListView));

            newLabelStyle.Setters.Add(new Setter(Label.FontWeightProperty, _configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newLabelStyle.Setters.Add(new Setter(Label.FontFamilyProperty, new FontFamily(_configManager.FontFamily)));
            newLabelStyle.Setters.Add(new Setter(Label.FontSizeProperty, (double)_configManager.FontSize));
            newLabelStyle.Setters.Add(new Setter(Label.BackgroundProperty, _configManager.MenuColor));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["LabelStyle"] = newLabelStyle;

            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontWeightProperty, _configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(_configManager.FontFamily)));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, (double)_configManager.FontSize));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, _configManager.BackgroundColor));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.MinWidthProperty, 25.0));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["TextBoxStyle"] = newTextBoxStyle;

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
