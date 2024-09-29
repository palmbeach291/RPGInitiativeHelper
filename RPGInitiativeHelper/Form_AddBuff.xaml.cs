using System.Windows;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaktionslogik für Form_AddBuff.xaml
    /// </summary>
    public partial class Form_AddBuff : Window
    {
        public Form_AddBuff()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfung der Eingaben
            if (string.IsNullOrWhiteSpace(TB_Name.Text))
            {
                MessageBox.Show("Bitte geben Sie einen Namen ein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int rounds;
            if (!int.TryParse(TB_Rounds.Text, out rounds))
            {
                MessageBox.Show("Die Runden müssen eine ganze Zahl sein.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
