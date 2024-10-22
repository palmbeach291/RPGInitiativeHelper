using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace YourNamespace
{
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
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
    }
}
