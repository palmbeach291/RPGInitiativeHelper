using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RPGInitiativeHelper.Configuration
{
    public class ConfigManager
    {
        public Brush MenuColor { get; set; }
		public Brush BackgroundColor { get; set; }
        public int FontSize { get; set; }
        public bool isBold { get; set; }
        private BrushConverter brushConverter = new BrushConverter();

        public ConfigManager()
        {
            LoadSettings();
        }

        // Methode zum Laden der Einstellungen
        public void LoadSettings()
        {
            try
            {
                MenuColor = (Brush)brushConverter.ConvertFromString(ConfigurationManager.AppSettings["MenuColor"]);
                BackgroundColor = (Brush)brushConverter.ConvertFromString(ConfigurationManager.AppSettings["BackgroundColor"]);
                FontSize = int.Parse(ConfigurationManager.AppSettings["FontSize"]);
                isBold = ConfigurationManager.AppSettings["Bold"].ToLower() == "true";
            }
            catch (Exception ex)
            {
                // Fallback auf Standardwerte, wenn etwas schiefgeht
                MenuColor = Brushes.Blue;
                BackgroundColor = Brushes.LightBlue;
                FontSize = 14;
                isBold = true;
                // Logging oder Error Handling könnte hier erfolgen
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
        }

        public void SaveSettings()
        {
            try
            {
                // Konfiguration abrufen
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Werte setzen
                config.AppSettings.Settings["MenuColor"].Value = MenuColor.ToString();
                config.AppSettings.Settings["BackgroundColor"].Value = BackgroundColor.ToString();
                config.AppSettings.Settings["FontSize"].Value = FontSize.ToString();
                config.AppSettings.Settings["Bold"].Value = isBold.ToString();

                // Änderungen speichern
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }
    }
}
