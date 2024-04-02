using System.Windows;
using System.Windows.Controls;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Combat ActiveCombat = new Combat();
        private int HighestInitiative = 0;
        private int CurrentInitiative = 0;
        private int CurrentTurn = 0;
        private int CurrentFighterID = 0;
        private int FighterCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            fighterListView.ItemsSource = ActiveCombat.Combatants;
            fighterListView.SelectionChanged += FighterListView_SelectionChanged;
            RefreshTurnLabels();
            RefreshPhaseButtons();
        }

        private void NewCombat()
        {
            ActiveCombat = new Combat();
            RefreshTurnLabels();
            RefreshPhaseButtons();
        }

        private void NeueDatei_Click(object sender, RoutedEventArgs e)
        {
            NewCombat();
        }

        private void AddNewFighter_Click(object sender, RoutedEventArgs e)
        {
            ActiveCombat.addFighter();
            RefreshPhaseButtons();
            refreshInitiative();
        }

        private void RemoveFighter_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen Sie, ob ein Element ausgewählt ist
            if (fighterListView.SelectedItem != null)
            {
                // Entfernen Sie das ausgewählte Element aus Ihrer Datenquelle
                ActiveCombat.Combatants.Remove((Fighter)fighterListView.SelectedItem);
                TB_Name.Text = "";
                TB_Initiative.Text = "";
                TB_Current_Life.Text = "";
                TB_Max_Life.Text = "";
                TB_Notes.Text = "";
                RefreshPhaseButtons();
                refreshInitiative();
            }
        }

        private void LastPhase_Click(object sender, RoutedEventArgs e)
        {
        }

        private void StartFight_Click(object sender, RoutedEventArgs e)
        {
            HighestInitiative = ActiveCombat.Combatants[0].Initiative;
            CurrentInitiative = HighestInitiative;
            CurrentTurn = 1;
            FighterCount = ActiveCombat.Combatants.Count;
            fighterListView.SelectedItem = fighterListView.Items[0];
            RefreshTurnLabels();
        }

        private void RefreshTurnLabels()
        {
            if (fighterListView.Items.Count > 0)
            {
                Fighter CurrentFighter = (Fighter)fighterListView.Items[CurrentFighterID];
                LPhaseCounter.Content = CurrentFighter.Initiative.ToString();
                LCurrentFighter.Content = CurrentFighter.Name;
            }
            else
                LCurrentFighter.Content = "";

            LTurnCounter.Content = CurrentTurn;
        }

        private void NextPhase_Click(object sender, RoutedEventArgs e)
        {
            CurrentFighterID++;

            if (CurrentFighterID >= FighterCount)
            {
                CurrentTurn++;
                CurrentFighterID = 0;
            }

            fighterListView.SelectedItem = fighterListView.Items[CurrentFighterID];
            RefreshTurnLabels();
        }

        private void FighterListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Hier können Sie den ausgewählten Fighter abrufen und das andere UI-Element anpassen
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                TB_Name.Text = selectedFighter.Name;
                TB_Initiative.Text = selectedFighter.Initiative.ToString();
                TB_Max_Life.Text = selectedFighter.Life.ToString();
                TB_Current_Life.Text = selectedFighter.MaxLife.ToString();
                TB_Notes.Text = selectedFighter.Note;
            }
        }

        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.Name = TB_Name.Text;
                fighterListView.Items.Refresh();
            }
        }

        private void TB_Initiative_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Initiative.Text, out int initiative))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Initiative = initiative;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültige Initiative. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Initiative.Text = selectedFighter.Initiative.ToString();

                }
                refreshInitiative();
            }
        }

        private void TB_Current_Life_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Current_Life.Text, out int life))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Life = life;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für Leben. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Current_Life.Text = selectedFighter.Life.ToString();

                }
                fighterListView.Items.Refresh();
            }
        }

        private void TB_Max_Life_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Max_Life.Text, out int life))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.MaxLife = life;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für das maximale Leben. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Max_Life.Text = selectedFighter.MaxLife.ToString();

                }
                fighterListView.Items.Refresh();
            }
        }

        private void TB_Notes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.Note = TB_Notes.Text;
            }
        }

        private void refreshInitiative()
        {
            ActiveCombat.Combatants.Sort((x, y) => y.Initiative.CompareTo(x.Initiative));
            fighterListView.Items.Refresh();
        }

        private void HealOneLife_Click(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.GetHeal();
                TB_Current_Life.Text = selectedFighter.Life.ToString();
                fighterListView.Items.Refresh();
            }
        }

        private void GetOneDamage_Click(object sender, RoutedEventArgs e)
        {

            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.GetDamage();
                TB_Current_Life.Text = selectedFighter.Life.ToString();
                fighterListView.Items.Refresh();
            }
        }

        private void RefreshPhaseButtons()
        {
            bool fighterAvailable = ActiveCombat.Combatants.Count > 0;

            B_LastPhase.IsEnabled = fighterAvailable;
            B_NextPhase.IsEnabled = fighterAvailable;
            B_StartFight.IsEnabled = fighterAvailable;

        }
    }
}
