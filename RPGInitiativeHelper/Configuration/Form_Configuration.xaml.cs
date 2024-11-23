using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGInitiativeHelper.Configuration
{
    public partial class Form_Configuration : Window
    {
        public ConfigManager _configManager =new ConfigManager();
        public ConfigManager oldConfig;
        private BrushConverter brushConverter = new BrushConverter();
        private bool isInitialized = false;
        private bool inUse = false;

        public Form_Configuration(ConfigManager configManager)
        {
            InitializeComponent();
            isInitialized =true;
            LoadColorsIntoComboBoxes();
            oldConfig= configManager;
            setInitialConfig();
        }

        private void setInitialConfig()
        {
            _configManager.MenuColor = oldConfig.MenuColor;
            _configManager.BackgroundColor = oldConfig.BackgroundColor;
            _configManager.FontFamily = oldConfig.FontFamily;
            _configManager.FontSize = oldConfig.FontSize;
            _configManager.IsBold = oldConfig.IsBold;
            LoadConfig();
        }

        private void LoadColorsIntoComboBoxes()
        {
            // Hole alle vordefinierten Farben aus der Colors-Klasse
            var colors = typeof(Colors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                                       .Select(p => new { Name = p.Name, Color = (Color)p.GetValue(null) })
                                       .Where(c => c.Color != Colors.Black &&
                                                c.Color != Colors.MidnightBlue &&
                                                c.Color != Colors.Navy)
                                       .OrderBy(c => c.Name);

            foreach (var color in colors)
            {
                // Item für MenuColorComboBox
                ComboBoxItem menuColorItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Background = new SolidColorBrush(color.Color),
                    Foreground = new SolidColorBrush(Colors.Black) // Sicherstellen, dass der Text sichtbar ist
                };
                MenuColorComboBox.Items.Add(menuColorItem);

                // Item für BackgroundColorComboBox
                ComboBoxItem backgroundColorItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Background = new SolidColorBrush(color.Color),
                    Foreground = new SolidColorBrush(Colors.Black)
                };
                BackgroundColorComboBox.Items.Add(backgroundColorItem);
            }
        }
        private void LoadConfig()
        {
            inUse = true;
            BoldCheckBox.IsChecked = _configManager.IsBold;
            FontSizeNumericUpDown.Value= _configManager.FontSize;

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

            inUse= false;
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
            setInitialConfig();
        }

        private void DefaultButton_Click(object sender, RoutedEventArgs e)
        {
            _configManager.LoadDefault();
            LoadConfig();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

            setInitialConfig();
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
            if (_configManager != null && isInitialized && !inUse)
            {
                if (BackgroundColorComboBox.SelectedItem != null)
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
