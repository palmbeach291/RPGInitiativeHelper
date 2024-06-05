using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RPGInitiativeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int CurrentTurn = 0;
        private int CurrentFighterID = 0;
        private List<Fighter> Combatants = new List<Fighter>();
        private bool CombatStarted = false;
        private string exePath;
        private string exeDirectory;
        private string savePath;
        private string saveFile;

        public MainWindow()
        {
            InitializeComponent();
            exePath = Assembly.GetExecutingAssembly().Location;
            exeDirectory = Path.GetDirectoryName(exePath);
            savePath = exeDirectory;
            DataContext = this;
            fighterListView.SelectionChanged += FighterListView_SelectionChanged;
            NewCombat();
        }

        private void NewCombat()
        {
            Combatants = new List<Fighter>();
            fighterListView.ItemsSource = Combatants;
            fighterMenu.Visibility = Visibility.Hidden;
            RefreshTurnLabels();
            RefreshPhaseButtons();
            refreshInitiative();
            CombatStarted = false;
        }

        private void NeueDatei_Click(object sender, RoutedEventArgs e)
        {
            NewCombat();
        }

        private void AddNewFighter_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;
            string name = "Kämpfer";
            string newName = name;

            while (fighterContained(newName))
            {
                newName = name + "_" + count.ToString();
                count++;
            }

            AddFighter(new Fighter(newName, 1, 1));
        }

        private void AddFighter(Fighter fighter)
        {
            Combatants.Add(fighter);
            RefreshPhaseButtons();
            refreshInitiative();
        }

        public bool fighterContained(string name)
        {
            bool retval = false;

            foreach (Fighter f in Combatants)
                if (f.Name == name)
                    retval = true;

            return retval;
        }

        private void RemoveFighter_Click(object sender, RoutedEventArgs e)
        {
            // Überprüfen Sie, ob ein Element ausgewählt ist
            if (fighterListView.SelectedItem != null)
            {
                Fighter SelectedFighter = (Fighter)fighterListView.SelectedItem;
                // Entfernen Sie das ausgewählte Element aus Ihrer Datenquelle
                Combatants.Remove(SelectedFighter);
                if (CombatStarted && (SelectedFighter.State == Status.StatusValue.Active || SelectedFighter.State == Status.StatusValue.Done))
                    CurrentFighterID--;

                TB_Name.Text = "";
                TB_Initiative.Text = "";
                TB_Current_Life.Text = "";
                TB_Max_Life.Text = "";
                TB_Notes.Text = "";

                if (Combatants.Count == 0 && CombatStarted == true)
                    NewCombat();

                RefreshPhaseButtons();
                refreshInitiative();
            }
        }

        private void StartFight_Click(object sender, RoutedEventArgs e)
        {
            CurrentTurn = 1;
            fighterListView.SelectedItem = fighterListView.Items[0];
            Fighter CurrentFighter = (Fighter)fighterListView.Items[0];
            CombatStarted = true;
            refreshState();
            CurrentFighter.State = Status.StatusValue.Active;
            RefreshTurnLabels();
        }

        private void refreshState()
        {
            foreach (Fighter f in fighterListView.Items)
            {
                if (f.State != Status.StatusValue.Downed)
                    f.State = Status.StatusValue.Standard;
            }
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
            fighterListView.Items.Refresh();
        }

        private void ConvertPlayer_Click(object sender, RoutedEventArgs e)
        {
            Form_PlayerName form = new Form_PlayerName();
            form.ShowDialog();
            string playerName = form.PlayerName;
            Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;

            if (!string.IsNullOrEmpty(playerName))
            {
                selectedFighter.PlayerName = playerName;
                RefreshPlayer();
            }
        }

        private void RefreshPlayer()
        {
            Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;

            if (selectedFighter != null)
            {
                if (selectedFighter.PlayerName.ToLower() == "npc")
                {
                    B_Player.Content = "NPC";
                    B_Player.Background = Brushes.Chocolate;
                }
                else
                {
                    B_Player.Content = selectedFighter.PlayerName;
                    B_Player.Background = Brushes.LightGreen;
                }
            }
        }

        private void NextPhase_Click(object sender, RoutedEventArgs e)
        {
            if (CombatStarted)
            {
                Fighter lastFighter = (Fighter)fighterListView.Items[CurrentFighterID];
                lastFighter.State = Status.StatusValue.Done;
                CurrentFighterID++;

                if (CurrentFighterID >= Combatants.Count)
                {
                    CurrentTurn++;
                    CurrentFighterID = 0;
                    refreshState();
                }

                Fighter CurrentFighter = (Fighter)fighterListView.Items[CurrentFighterID];
                CurrentFighter.State = Status.StatusValue.Active;


                fighterListView.SelectedItem = fighterListView.Items[CurrentFighterID];
                RefreshTurnLabels();
            }
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

                fighterMenu.Visibility = Visibility.Visible;
                RefreshPlayer();
            }
            else
                fighterMenu.Visibility = Visibility.Hidden;
        }

        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveName();
        }

        private void SaveName()
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
            SaveInitiative();
        }

        private bool SaveInitiative()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Initiative.Text, out int initiative))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Initiative = initiative;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültige Initiative. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Initiative.Text = selectedFighter.Initiative.ToString();
                    retval = false;
                }
                refreshInitiative();
            }
            return retval;
        }

        private void TB_Current_Life_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCurrentLife();
        }

        private bool SaveCurrentLife()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Current_Life.Text, out int life))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Life = life;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für Leben. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Current_Life.Text = selectedFighter.Life.ToString();
                    retval = false;

                }
                fighterListView.Items.Refresh();
            }
            return retval;
        }

        private void TB_Max_Life_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveMaxLife();
        }

        private bool SaveMaxLife()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Max_Life.Text, out int life))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.MaxLife = life;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für das maximale Leben. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Max_Life.Text = selectedFighter.MaxLife.ToString();
                    retval = false;

                }
                fighterListView.Items.Refresh();
            }

            return retval;
        }

        private void TB_Notes_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveNotes();
        }

        private void SaveNotes()
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.Note = TB_Notes.Text;
            }
        }

        private void refreshInitiative()
        {
            Combatants.Sort((x, y) => y.Initiative.CompareTo(x.Initiative));
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
            bool fighterAvailable = Combatants.Count > 0;

            B_NextPhase.IsEnabled = fighterAvailable;
            B_StartFight.IsEnabled = fighterAvailable;

        }

        private bool SaveStats()
        {
            bool retval = true;

            if (!SaveInitiative() || !SaveMaxLife() || !SaveCurrentLife())
                retval = false;
            else
            {
                SaveName();
                SaveNotes();
            }

            return retval;
        }

        private void SaveGroup_Click(object sender, RoutedEventArgs e)
        {
            if (SaveStats())
            {
                if (Combatants.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Gruppendateien (*.grp)|*.grp|Alle Dateien (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;
                    List<Fighter> Group = new List<Fighter>();

                    foreach (Fighter f in Combatants)
                        if (f.PlayerName != "NPC")
                            Group.Add(f);

                    if (Group.Count > 0)
                    {


                        if (saveFileDialog.ShowDialog() == true)
                        {
                            saveFile = saveFileDialog.FileName;

                            if (File.Exists(saveFile) && !saveFileDialog.OverwritePrompt)
                                return;

                            SaveGroup(Group, saveFile);
                        }
                    }
                    else
                        MessageBox.Show("Es existieren keine Spielercharaktere.", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                    MessageBox.Show("Es existieren keine Kämpfer.", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Es wurde nicht gespeichert", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SaveCombatants_Click(object sender, RoutedEventArgs e)
        {
            if (SaveStats())
            {
                if (Combatants.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Gruppendateien (*.grp)|*.grp|Alle Dateien (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        saveFile = saveFileDialog.FileName;

                        if (File.Exists(saveFile) && !saveFileDialog.OverwritePrompt)
                            return;

                        SaveGroup(Combatants, saveFile);
                    }
                }
                else
                    MessageBox.Show("Es existieren keine Kämpfer.", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
                MessageBox.Show("Es wurde nicht gespeichert", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SaveGroup(List<Fighter> group, string filepath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(group);
                File.WriteAllText(filepath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der Gruppe: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                }
        }

        private void LoadGroup_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Gruppendateien (*.grp)|*.grp|Alle Dateien (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                try
                {
                    string json = File.ReadAllText(filePath);
                    List<Fighter> loadedGroup = JsonConvert.DeserializeObject<List<Fighter>>(json);

                    if (loadedGroup.Count > 0)
                    {
                        foreach (Fighter g in loadedGroup)
                            AddFighter(g);
                    }
                    else
                        MessageBox.Show($"{filePath} enthält keine valide Gruppe!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                catch (Exception ex)
                {
                    // Fehlermeldung anzeigen, falls ein Fehler auftritt
                    MessageBox.Show($"Fehler beim Laden der Gruppe: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
