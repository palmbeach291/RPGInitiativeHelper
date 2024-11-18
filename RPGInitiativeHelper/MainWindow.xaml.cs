﻿using Microsoft.Win32;
using Newtonsoft.Json;
using RPGInitiativeHelper.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using YourNamespace;

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
        private string? saveFile;
        private DispatcherTimer _timer;
        private int _secondsElapsed;
        private ConfigManager configManager = new ConfigManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            fighterListView.SelectionChanged += FighterListView_SelectionChanged;
            this.KeyDown += MainWindow_KeyDown;
            // Timer initialisieren
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1); // Intervall auf 1 Sekunde setzen
            _timer.Tick += Timer_Tick; // Event-Handler hinzufügen
            NewCombat();
            PaintMe();
        }

        private void NewCombat()
        {
            StopTimer();
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

            //Entferne alle Buffs von bestehen Kämpfern
            foreach (Fighter fighter in Combatants)
            {
                fighter.Buffs = new List<FighterState>();
            }

            fighterListView.SelectedItem = fighterListView.Items[0];
            Fighter CurrentFighter = (Fighter)fighterListView.Items[0];
            StartTimer(CurrentFighter);
            CombatStarted = true;
            RefreshState();
            CurrentFighter.State = Status.StatusValue.Active;
            RefreshTurnLabels();
        }

        private void RefreshState()
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
                RefreshPlayer(selectedFighter);
            }
            refreshInitiative();
        }

        private void RefreshPlayer(Fighter player)
        {

            if (player != null)
            {
                if (!player.isPlayer)
                {
                    player.PlayerName = "NPC";
                    B_Player.Background = Brushes.Chocolate;
                    B_Player.Foreground = Brushes.Black;
                }
                else
                {
                    B_Player.Background = Brushes.Green;
                    B_Player.Foreground = Brushes.White;
                }

                B_Player.Content = player.PlayerName;
            }
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog(); // Öffnet das Info-Fenster als modales Dialogfenster
        }

        private void NextPhase_Click(object sender, RoutedEventArgs e)
        {
            NextPlayer();
        }

        private void NextPlayer()
        {
            if (CombatStarted)
            {
                bool viableFighter = false;
                int iterationCount = 0; // Schutz gegen Endlosschleifen
                int maxIterations = Combatants.Count; // Maximal erlaubte Iterationen

                do
                {
                    StopTimer();
                    Fighter lastFighter = (Fighter)fighterListView.Items[CurrentFighterID];
                    foreach (FighterState Buff in lastFighter.Buffs)
                    {
                        if (!Buff.isFresh)
                        {
                            Buff.rounds--;
                        }
                        Buff.isFresh = false;
                    }

                    // Liste mit den zu entfernenden Buffs
                    List<FighterState> buffsToRemove = lastFighter.Buffs.Where(buff => buff.rounds < 1).ToList();
                    foreach (FighterState Buff in buffsToRemove)
                    {
                        lastFighter.Buffs.Remove(Buff);
                    }

                    if (lastFighter.State != Status.StatusValue.Downed)
                    {
                        lastFighter.State = Status.StatusValue.Done;
                    }

                    // Neuer Kämpfer
                    CurrentFighterID++;
                    if (CurrentFighterID >= Combatants.Count)
                    {
                        CurrentTurn++;
                        CurrentFighterID = 0;
                        RefreshState();
                    }

                    Fighter CurrentFighter = (Fighter)fighterListView.Items[CurrentFighterID];
                    if (CurrentFighter.State == Status.StatusValue.Standard)
                    {
                        CurrentFighter.State = Status.StatusValue.Active;
                        viableFighter = true;
                    }
                    StartTimer(CurrentFighter);

                    fighterListView.SelectedItem = fighterListView.Items[CurrentFighterID];
                    RefreshDetails();
                    RefreshTurnLabels();

                    iterationCount++;
                    if (iterationCount >= maxIterations)
                    {
                        // Abbruch, wenn kein passender Kämpfer gefunden wurde, um Endlosschleife zu verhindern
                        break;
                    }
                }
                while (!viableFighter);
            }
        }


        private void FighterListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshDetails();
        }

        private void RefreshDetails()
        {
            // Hier können Sie den ausgewählten Fighter abrufen und das andere UI-Element anpassen
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                TB_Name.Text = selectedFighter.Name;
                TB_Initiative.Text = selectedFighter.Initiative.ToString();
                TB_Max_Life.Text = selectedFighter.MaxLife.ToString();
                TB_Current_Life.Text = selectedFighter.Life.ToString();
                TB_Max_Mana.Text = selectedFighter.MaxMana.ToString();
                TB_Current_Mana.Text = selectedFighter.Mana.ToString();
                TB_Notes.Text = selectedFighter.Note;
                TB_Armor.Text = selectedFighter.Armor.ToString();
                TB_Defence.Text = selectedFighter.Defence.ToString();
                TB_Offence.Text = selectedFighter.Offence.ToString();
                TB_Damage.Text = selectedFighter.Damage;
                BuffListView.ItemsSource = selectedFighter.Buffs;
                BuffListView.Items.Refresh();

                fighterMenu.Visibility = Visibility.Visible;
                RefreshPlayer(selectedFighter);
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

        private void TB_Offence_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveOffence();
        }

        private void TB_Damage_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveDamage();
        }

        private void SaveDamage()
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.Damage = TB_Damage.Text;
            }
        }

        private bool SaveOffence()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Offence.Text, out int offence))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Offence = offence;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Angriff. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Offence.Text = selectedFighter.Offence.ToString();
                    retval = false;
                }
            }
            return retval;
        }

        private void TB_Defence_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveDefence();
        }

        private bool SaveDefence()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Defence.Text, out int defence))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Defence = defence;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültige Verteidigung. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Defence.Text = selectedFighter.Defence.ToString();
                    retval = false;
                }
            }
            return retval;
        }

        private void TB_Armor_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveArmor();
        }

        private bool SaveArmor()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Armor.Text, out int armor))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Armor = armor;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültige Ruestung. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Armor.Text = selectedFighter.Armor.ToString();
                    retval = false;
                }
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

        private void TB_Current_Mana_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCurrentMana();
        }

        private bool SaveCurrentMana()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Current_Mana.Text, out int mana))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.Mana = mana;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für Mana. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Current_Mana.Text = selectedFighter.Mana.ToString();
                    retval = false;

                }
            }
            return retval;
        }

        private void TB_Max_Mana_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveMaxMana();
        }

        private bool SaveMaxMana()
        {
            bool retval = false;
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                // Versuchen Sie, den Text in einen Integer zu konvertieren
                if (int.TryParse(TB_Max_Mana.Text, out int mana))
                {
                    // Wenn die Konvertierung erfolgreich ist, aktualisieren Sie die Initiative des ausgewählten Kämpfers
                    selectedFighter.MaxMana = mana;
                    retval = true;
                }
                else
                {
                    // Wenn die Konvertierung fehlschlägt, können Sie eine Fehlermeldung anzeigen oder eine alternative Behandlung durchführen
                    MessageBox.Show("Ungültiger Wert für das maximale Mana. Bitte geben Sie eine ganze Zahl ein.");
                    TB_Max_Mana.Text = selectedFighter.MaxMana.ToString();
                    retval = false;

                }
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
            Combatants.Sort((x, y) =>
            {
                int initiativeComparison = y.Initiative.CompareTo(x.Initiative); // Zuerst nach Initiative sortieren
                if (initiativeComparison == 0) // Wenn die Initiative gleich ist
                {
                    // "Nicht-NPC" Kämpfer sollen vor NPCs erscheinen
                    bool xIsNpc = x.PlayerName == "NPC";
                    bool yIsNpc = y.PlayerName == "NPC";

                    if (xIsNpc && !yIsNpc) return 1; // NPC nach hinten verschieben
                    if (!xIsNpc && yIsNpc) return -1; // Spieler nach vorne holen
                }
                return initiativeComparison; // Wenn die Initiative unterschiedlich ist, diese Sortierung beibehalten
            });
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

        private void GetOneMana_Click(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.GetMana();
                TB_Current_Mana.Text = selectedFighter.Mana.ToString();
            }
        }

        private void LooseOneMana_Click(object sender, RoutedEventArgs e)
        {

            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;
                selectedFighter.LooseMana();
                TB_Current_Mana.Text = selectedFighter.Mana.ToString();
            }
        }

        //SaveStats wird benötigt um den Datentransfer von UI in die Fighterklasse durchzuführen, da bei Speichervorgängen LostFocus nicht unbedingt durchgeführt wird.
        private bool SaveStats()
        {
            bool retval = false;
            bool ini = SaveInitiative();
            bool mLife = SaveMaxLife();
            bool cLife = SaveCurrentLife();
            bool mMana = SaveMaxMana();
            bool cMana = SaveCurrentMana();
            bool armor = SaveArmor();
            bool defence = SaveDefence();
            bool offence = SaveOffence();

            if (ini && mLife && cLife && mMana && cMana && armor && defence && offence)
            {
                SaveName();
                SaveNotes();
                SaveDamage();
                retval = true;
            }

            return retval;
        }

        private void SaveData(Func<List<Fighter>> dataSelector, string noDataMessage)
        {
            if (!SaveStats())
            {
                MessageBox.Show("Es wurde nicht gespeichert", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dataToSave = dataSelector.Invoke();

            if (dataToSave.Count == 0)
            {
                MessageBox.Show(noDataMessage, "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Gruppendateien (*.grp)|*.grp|Alle Dateien (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                saveFile = saveFileDialog.FileName;

                if (File.Exists(saveFile) && !saveFileDialog.OverwritePrompt)
                    return;

                SaveGroup(dataToSave, saveFile);
            }
        }

        private void SaveGroup_Click(object sender, RoutedEventArgs e)
        {
            SaveData(() => Combatants.Where(f => f.PlayerName != "NPC").ToList(), "Es existieren keine Spielercharaktere.");
        }

        private void SaveCombatants_Click(object sender, RoutedEventArgs e)
        {
            SaveData(() => Combatants, "Es existieren keine Kämpfer.");
        }

        private void SaveSelectedFighter()
        {
            SaveData(() => new List<Fighter> { (Fighter)fighterListView.SelectedItem }, "Kein Kämpfer ausgewählt.");
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Prüfen, ob STRG + D gedrückt wurde
            if (e.Key == Key.D && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                CloneSelectedFighter();
            }
            else if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SaveSelectedFighter(); // STRG + S ruft SaveSelectedFighter auf
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

        private void CloneSelectedFighter()
        {
            // Überprüfen, ob ein Kämpfer ausgewählt ist
            if (fighterListView.SelectedItem != null)
            {
                Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;

                // Klonen des ausgewählten Kämpfers
                Fighter clonedFighter = new Fighter(selectedFighter.Name + "_Klon",
                                                    selectedFighter.Initiative,
                                                    selectedFighter.Life)
                {
                    MaxLife = selectedFighter.MaxLife,
                    Armor = selectedFighter.Armor,
                    Defence = selectedFighter.Defence,
                    Offence = selectedFighter.Offence,
                    PlayerName = selectedFighter.PlayerName,
                    Note = selectedFighter.Note,
                    Damage = selectedFighter.Damage,
                    Mana = selectedFighter.Mana,
                    MaxMana = selectedFighter.MaxMana,
                    State = Status.StatusValue.Standard
                };

                // Füge den Klon zur Liste hinzu
                AddFighter(clonedFighter);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _secondsElapsed++; // Zeit erhöhen
            RefreshTimer(_secondsElapsed);
        }

        private void StartTimer(Fighter fighter)
        {

            if (fighter.isPlayer)
            {
                _secondsElapsed = 0;
                _timer.Start();
                RefreshTimer(_secondsElapsed);
            }
        }

        private void StopTimer()
        {
            _timer.Stop();
            LTimer.Content = "";
        }

        private void RefreshTimer(int seconds)
        {
            int rest = seconds % 60;
            int minutes = (seconds - rest) / 60;

            if (seconds >= 30)
            {
                LTimer.Foreground = Brushes.Red;
            }
            else
            {
                LTimer.Foreground = Brushes.Black;
            }

            LTimer.Content = $"Timer: {minutes}:{rest:d2}";
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            Form_Configuration ConfigDiag = new Form_Configuration(configManager);

            ConfigDiag.ShowDialog();
            PaintMe();
        }

        private void PaintMe()
        {
            this.Background = configManager.BackgroundColor;
            ToolBarMenu.Background = configManager.MenuColor;

            Style newLabelStyle = new Style(typeof(Label));
            Style newTextBoxStyle = new Style(typeof(TextBox));
            Style newListViewStyle = new Style(typeof(ListView));

            newLabelStyle.Setters.Add(new Setter(Label.FontWeightProperty, configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newLabelStyle.Setters.Add(new Setter(Label.FontFamilyProperty, new FontFamily(configManager.FontFamily)));
            newLabelStyle.Setters.Add(new Setter(Label.FontSizeProperty, (double)configManager.FontSize));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["LabelStyle"] = newLabelStyle;

            newListViewStyle.Setters.Add(new Setter(ListView.FontWeightProperty, configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newListViewStyle.Setters.Add(new Setter(ListView.FontFamilyProperty, new FontFamily(configManager.FontFamily)));
            newListViewStyle.Setters.Add(new Setter(ListView.FontSizeProperty, (double)configManager.FontSize));

            this.Resources["ListViewStyle"] = newListViewStyle;

            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontWeightProperty, configManager.IsBold ? FontWeights.Bold : FontWeights.Normal));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontFamilyProperty, new FontFamily(configManager.FontFamily)));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.FontSizeProperty, (double)configManager.FontSize));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, configManager.BackgroundColor));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.MinWidthProperty, 25.0));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center));
            newTextBoxStyle.Setters.Add(new Setter(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center));

            // Ersetze den alten Style in den Ressourcen
            this.Resources["TextBoxStyle"] = newTextBoxStyle;

            // Optional: Layout aktualisieren
            this.UpdateLayout();
        }

        private void AddBuff_Click(object sender, RoutedEventArgs e)
        {
            if (fighterListView.SelectedItem != null)
            {
                Fighter SelectedFighter = (Fighter)fighterListView.SelectedItem;
                Form_FighterState form = new Form_FighterState(SelectedFighter);
                form.ShowDialog();

                if (SelectedFighter.State == Status.StatusValue.Active)
                {
                    form.fighterState.isFresh = true;
                }
                SelectedFighter.Buffs.Add(form.fighterState);
            }
            BuffListView.Items.Refresh();
        }

        private void RemoveBuff_Click(object sender, RoutedEventArgs e)
        {
            if (BuffListView.SelectedItem != null && fighterListView.SelectedItem != null)
            {
                FighterState SelectedBuff = (FighterState)BuffListView.SelectedItem;
                Fighter SelectedFighter = (Fighter)fighterListView.SelectedItem;

                SelectedFighter.Buffs.Remove(SelectedBuff);
            }

            BuffListView.Items.Refresh();
        }

        private void TB_Buff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) // Überprüfen, ob es ein Doppelklick ist
            {
                TextBlock textBlock = sender as TextBlock;
                if (textBlock != null && textBlock.DataContext is FighterState item)
                {
                    if (fighterListView.SelectedItem != null)
                    {
                        Fighter selectedFighter = (Fighter)fighterListView.SelectedItem;

                        // Öffne das Formular zur Bearbeitung einer Kopie des Buffs
                        Form_FighterState form = new Form_FighterState(item);
                        form.ShowDialog();

                        if (form.DialogResult == true) // Überprüfen, ob der Benutzer Änderungen bestätigt hat
                        {
                            // Übernehme die bearbeiteten Werte in das Original-Objekt
                            item.name = form.fighterState.name;
                            item.rounds = form.fighterState.rounds;
                            item.description = form.fighterState.description;
                            item.isBonus = form.fighterState.isBonus;
                            item.isFresh = form.fighterState.isFresh;

                            BuffListView.Items.Refresh();
                        }
                    }
                }
            }
        }
    }
}

