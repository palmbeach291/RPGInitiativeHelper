using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGInitiativeHelper.Configuration
{
    public partial class Form_Configuration : Window
    {
        private ConfigManager _configManager;
        BrushConverter brushConverter = new BrushConverter();

        public Form_Configuration(ConfigManager configManager)
        {
            InitializeComponent();
            _configManager = configManager;
            LoadConfig();
        }

        private void LoadConfig()
        {
            BoldCheckBox.IsChecked = _configManager.IsBold;

            // Hole die aktuellen Farben aus dem Config-Manager als Color-Objekt
            var currentMenuColor = ((SolidColorBrush)_configManager.MenuColor).Color;
            var currentBackgroundColor = ((SolidColorBrush)_configManager.BackgroundColor).Color;

            // Prüfen, ob die ComboBox-Werte existieren
            foreach (ComboBoxItem item in MenuColorComboBox.Items)
            {
                // Vergleiche die Farbe durch Zugriff auf den Inhalt des ComboBox-Items
                if (item.Content is string colorName)
                {
                    // Konvertiere den Farbnamen in eine Color-Instanz
                    var colorFromName = (Color)ColorConverter.ConvertFromString(colorName);

                    // Vergleiche die Farben
                    if (currentMenuColor == colorFromName)
                    {
                        MenuColorComboBox.SelectedItem = item;
                    }
                }
            }

            foreach (ComboBoxItem item in BackgroundColorComboBox.Items)
            {
                // Vergleiche die Farbe durch Zugriff auf den Inhalt des ComboBox-Items
                if (item.Content is string colorName)
                {                 
                    // Konvertiere den Farbnamen in eine Color-Instanz
                    var colorFromName = (Color)ColorConverter.ConvertFromString(colorName);

                    // Vergleiche die Farben
                    if (currentBackgroundColor == colorFromName)
                    {
                        BackgroundColorComboBox.SelectedItem = item;
                    }
                }
            }

            FontSizeNumericUpDown.Value = _configManager.FontSize;

            PaintMe();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            // Werte aus den UI-Elementen holen und in Brushes konvertieren
            Brush selectedMenuColor = brushConverter.ConvertFromString((MenuColorComboBox.SelectedItem as ComboBoxItem).Content.ToString()) as Brush;
            Brush selectedBackgroundColor = brushConverter.ConvertFromString((BackgroundColorComboBox.SelectedItem as ComboBoxItem).Content.ToString()) as Brush;

            int selectedFontSize = FontSizeNumericUpDown.Value ?? 14;  // Standardwert als Fallback
            bool isBoldChecked = BoldCheckBox.IsChecked ?? false;

            // Werte in den ConfigManager setzen
            _configManager.SetSetting(selectedMenuColor, selectedBackgroundColor, selectedFontSize, isBoldChecked);

            // Einstellungen speichern
            _configManager.SaveSettings();
            this.Close();
        }


        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            _configManager.LoadDefault();
            LoadConfig();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void FontSizeNumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ValuesChanged();
        }

        private void ValuesChanged()
        {
            if (_configManager != null)
            {
                if(BackgroundColorComboBox.SelectedItem != null)
                {
                    _configManager.BackgroundColor = brushConverter.ConvertFromString((BackgroundColorComboBox.SelectedItem as ComboBoxItem).Content.ToString()) as Brush;
                }

                if (MenuColorComboBox.SelectedItem != null)
                {
                    _configManager.MenuColor = brushConverter.ConvertFromString((MenuColorComboBox.SelectedItem as ComboBoxItem).Content.ToString()) as Brush;
                }

                _configManager.FontSize = (int)FontSizeNumericUpDown.Value;

                _configManager.IsBold = (bool)BoldCheckBox.IsChecked;
                
                PaintMe();  
            }
        }

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ValuesChanged();
        }

        private void BoldCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ValuesChanged();
        }
    }
}
