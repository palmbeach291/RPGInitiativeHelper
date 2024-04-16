using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaktionslogik für Form_PlayerName.xaml
    /// </summary>
    public partial class Form_PlayerName : Window
    {
        public string PlayerName { get; private set; }
        public Form_PlayerName()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Setze den Spielername auf den Wert des TextBoxes
            PlayerName = TB_PlayerName.Text;

            // Schließe das Fenster
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Setze den Spielername auf null oder einen leeren String
            PlayerName = null;

            // Schließe das Fenster
            this.Close();
        }
    }
}
