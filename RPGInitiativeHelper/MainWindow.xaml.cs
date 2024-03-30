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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Combat ActiveCombat = new Combat();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            fighterListView.ItemsSource = ActiveCombat.Combatants;
            LTurnCounter.Content = 1;
            LPhaseCounter.Content = 0;
        }

        private void NewCombat()
        {
            ActiveCombat = new Combat();
            LTurnCounter.Content = 1;
        }

        private void NeueDatei_Click(object sender, RoutedEventArgs e)
        {
            NewCombat();
        }

        private void AddNewFighter_Click(object sender, RoutedEventArgs e)
        {
            ActiveCombat.Combatants.Add(new Fighter("Kämpfer",1,1));
            fighterListView.Items.Refresh();
        }
        private void RemoveFighter_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LastPhase_Click(object sender, RoutedEventArgs e)
        {
        }

        private void StartFight_Click(object sender, RoutedEventArgs e)
        {
        }

        private void NextPhase_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
