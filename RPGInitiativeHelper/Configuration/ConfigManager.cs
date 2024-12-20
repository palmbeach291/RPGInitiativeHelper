﻿using System;
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
        public bool IsBold { get; set; }
        public string FontFamily { get; set; }
        public BrushConverter brushConverter = new BrushConverter();

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
                IsBold = ConfigurationManager.AppSettings["Bold"].ToLower() == "true";
                FontFamily = ConfigurationManager.AppSettings["FontFamily"];
            }
            catch (Exception ex)
            {
                // Fallback auf Standardwerte, wenn etwas schiefgeht
                LoadDefault();
                // Logging oder Error Handling könnte hier erfolgen
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
        }

        public void LoadDefault()
        {
            MenuColor = Brushes.Red;
            BackgroundColor = Brushes.LightCoral;
            FontSize = 14;
            IsBold = true;
            FontFamily = "Roboto";
        }

        public void SetSetting(Brush menuColor, Brush backGroundColor, int fontSize, bool isBold)
        {
            try
            {
                MenuColor = menuColor;
                BackgroundColor = backGroundColor;
                FontSize = fontSize;
                IsBold = isBold;
            }
            catch (Exception ex)
            {
                // Fallback auf Standardwerte, wenn etwas schiefgeht
                LoadDefault();
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
                config.AppSettings.Settings["Bold"].Value = IsBold.ToString();

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
