using RPGInitiativeHelper.Configuration;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace YourNamespace
{
    public partial class InfoWindow : Window
    {
        ConfigManager _configManager = new ConfigManager();
        public InfoWindow()
        {
            InitializeComponent();
            PaintMe();
            LoadAssemblyInfo();
        }

        private void LoadAssemblyInfo()
        {
            // Assembly-Informationen abrufen
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "Unbekannt";
            var copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute)))?.Copyright ?? "Copyright nicht verfügbar";

            // Textblöcke aktualisieren
            TB_Version.Text = $"Version: {version}";
            TB_Copyright.Text = copyright;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Öffne den Link im Standardbrowser
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void PaintMe()
        {
            this.Background = _configManager.BackgroundColor;
            
            Style newTextBlockStyle = new Style(typeof(TextBlock));
            newTextBlockStyle.Setters.Add(new Setter(TextBlock.FontWeightProperty, _configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newTextBlockStyle.Setters.Add(new Setter(TextBlock.FontFamilyProperty, new FontFamily(_configManager.FontFamily)));
            newTextBlockStyle.Setters.Add(new Setter(TextBlock.FontSizeProperty, (double)_configManager.FontSize));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["TextBlockStyle"] = newTextBlockStyle;

            this.UpdateLayout();
        }
    }
}
