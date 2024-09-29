using System.Windows;

namespace RPGInitiativeHelper
{
    public partial class Form_PlayerName : Window
    {
        public string PlayerName { get; private set; }

        public Form_PlayerName()
        {
            InitializeComponent();

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
    }
}
