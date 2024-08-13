using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace OneSignControll
{
    public partial class MainWindow : MetroWindow
    {
        private readonly string XmlFilePath;
        public ObservableCollection<ProgramEntry> Programs { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Initialisierung des XML-Dateipfads
            XmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneSignControll", "OneSignControllSave.xml");

            EnsureDirectoryExists();

            Programs = new ObservableCollection<ProgramEntry>();
            ProgramListView.ItemsSource = Programs;
            ProgramListView.SelectionChanged += ProgramListView_SelectionChanged;

            UpdateMenuItemState(); // Initialer Aufruf, um den Zustand der Menüpunkte festzulegen
            LoadProgramsFromXml(); // Laden der gespeicherten Programme beim Start
        }

        private void EnsureDirectoryExists()
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneSignControll");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        private void LoadProgramsFromXml()
        {
            if (File.Exists(XmlFilePath))
            {
                try
                {
                    var doc = XDocument.Load(XmlFilePath);
                    Programs.Clear();
                    foreach (var programElement in doc.Descendants("Program"))
                    {
                        Programs.Add(new ProgramEntry(programElement.Element("Path").Value)
                        {
                            Name = programElement.Element("Name")?.Value
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Laden der XML-Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ProgramListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMenuItemState();
        }

        private void UpdateMenuItemState()
        {
            int programCount = Programs.Count;
            int selectedCount = ProgramListView.SelectedItems.Count;

            // Deaktivieren von "Programm entfernen", wenn keine Programme vorhanden sind
            RemoveProgramMenuItem.IsEnabled = programCount > 0 && selectedCount > 0;

            // Deaktivieren von "Programm umbenennen", wenn kein oder mehrere Programme ausgewählt sind
            RenameProgramMenuItem.IsEnabled = selectedCount == 1;

            // Deaktivieren von "Programm hinzufügen", wenn mehrere Programme ausgewählt sind
            AddProgramMenuItem.IsEnabled = selectedCount <= 1;
        }

        private void AddProgram_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Sie können nur ein Programm auf einmal hinzufügen.", "Mehrfachauswahl", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var openFileDialog = new OpenFileDialog
            {
                Title = "Programm auswählen",
                Filter = "Anwendungsdateien (*.exe)|*.exe|" + "Installationsdateien (*.msi)|*.msi|" + "Batch- und Skriptdateien (*.bat;*.cmd;*.vbs;*.ps1)|*.bat;*.cmd;*.vbs;*.ps1|" + "Management Console Dateien (*.msc)|*.msc|" + "Systemsteuerungsdateien (*.cpl)|*.cpl|" + "Alle Dateien (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                string defaultName = System.IO.Path.GetFileNameWithoutExtension(selectedPath);

                var inputDialog = new InputDialog("Programm umbenennen", "Geben Sie einen neuen Namen ein:", defaultName);
                if (inputDialog.ShowDialog() == true)
                {
                    Programs.Add(new ProgramEntry(selectedPath) { Name = inputDialog.ResponseText });
                }
                else
                {
                    Programs.Add(new ProgramEntry(selectedPath));
                }

                UpdateMenuItemState(); // Aktualisieren des Zustands nach dem Hinzufügen eines Programms
            }
        }

        private void RenameProgram_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Sie können nur ein Programm auf einmal umbenennen.", "Mehrfachauswahl", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (ProgramListView.SelectedItem is ProgramEntry selectedProgram)
            {
                var inputDialog = new InputDialog("Programm umbenennen", "Geben Sie einen neuen Namen ein:", selectedProgram.Name);
                if (inputDialog.ShowDialog() == true)
                {
                    selectedProgram.Name = inputDialog.ResponseText;
                    ProgramListView.Items.Refresh(); // Aktualisieren der ListView
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein Programm zum Umbenennen aus.");
            }
        }

        private void RemoveProgram_Click(object sender, RoutedEventArgs e)
        {
            var selectedPrograms = ProgramListView.SelectedItems;
            if (selectedPrograms.Count > 0)
            {
                var programsToRemove = selectedPrograms.Cast<ProgramEntry>().ToList();

                foreach (var program in programsToRemove)
                {
                    Programs.Remove(program);
                }

                UpdateMenuItemState(); // Aktualisieren des Zustands nach dem Entfernen eines Programms
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein oder mehrere Programme zum Entfernen aus.");
            }
        }

        private void StartProgramButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPrograms = ProgramListView.SelectedItems;
            if (selectedPrograms.Count > 0)
            {
                foreach (ProgramEntry program in selectedPrograms)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = program.Path,
                            UseShellExecute = true,
                            Verb = "runas" // Ensure it runs with administrator rights
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Fehler beim Starten des Programms: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Bitte wählen Sie ein oder mehrere Programme zum Starten aus.");
            }
        }

        private void LoadXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = XDocument.Load(XmlFilePath);
                Programs.Clear();
                foreach (var programElement in doc.Descendants("Program"))
                {
                    Programs.Add(new ProgramEntry(programElement.Element("Path").Value)
                    {
                        Name = programElement.Element("Name")?.Value
                    });
                }

                UpdateMenuItemState(); // Aktualisieren des Zustands nach dem Laden der Konfiguration

                MessageBox.Show("Konfiguration erfolgreich geladen.", "Laden", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der XML-Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doc = new XDocument(new XElement("Programs",
                    Programs.Select(p => new XElement("Program",
                        new XElement("Path", p.Path),
                        new XElement("Name", p.Name)
                    ))
                ));

                doc.Save(XmlFilePath);
                MessageBox.Show("Konfiguration erfolgreich gespeichert.", "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern der XML-Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportXML_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Importiere XML-Konfigurationsdatei",
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var doc = XDocument.Load(openFileDialog.FileName);

                    Programs.Clear();
                    foreach (var programElement in doc.Descendants("Program"))
                    {
                        Programs.Add(new ProgramEntry(programElement.Element("Path").Value)
                        {
                            Name = programElement.Element("Name")?.Value
                        });
                    }

                    UpdateMenuItemState(); // Aktualisieren des Zustands nach dem Importieren der Konfiguration

                    MessageBox.Show("Konfiguration erfolgreich importiert.", "Importieren", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Abfrage, ob die importierte Konfiguration im AppData-Ordner gespeichert werden soll
                    var saveConfirmation = MessageBox.Show("Möchten Sie die importierte Konfiguration im AppData-Ordner speichern?", "Speichern bestätigen", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (saveConfirmation == MessageBoxResult.Yes)
                    {
                        // Speichern der importierten Konfiguration im AppData-Ordner
                        doc.Save(XmlFilePath);
                        MessageBox.Show("Konfiguration wurde im AppData-Ordner gespeichert.", "Gespeichert", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Importieren der XML-Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExportXML_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Exportiere XML-Konfigurationsdatei",
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                FileName = "OneSignControllSave_export.xml"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var doc = new XDocument(new XElement("Programs",
                        Programs.Select(p => new XElement("Program",
                            new XElement("Path", p.Path),
                            new XElement("Name", p.Name)
                        ))
                    ));

                    doc.Save(saveFileDialog.FileName);
                    MessageBox.Show("Konfiguration erfolgreich exportiert.", "Exportieren", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Exportieren der XML-Datei: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Schließt die Anwendung
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }
    }

    public class ProgramEntry
    {
        public string Path { get; set; }
        public string Name { get; set; }

        public ProgramEntry(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path); // Extrahiert den Anwendungsnamen ohne Erweiterung
        }
    }

    public class InputDialog : Window
    {
        private readonly TextBox inputBox;
        public string ResponseText { get; private set; }

        public InputDialog(string title, string message, string defaultText = "")
        {
            Title = title;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Width = 300;
            Height = 150;
            ResizeMode = ResizeMode.NoResize;
            WindowStyle = WindowStyle.ToolWindow;

            StackPanel panel = new StackPanel { Margin = new Thickness(10) };
            panel.Children.Add(new TextBlock { Text = message });

            inputBox = new TextBox { Text = defaultText, Margin = new Thickness(0, 10, 0, 10) };
            panel.Children.Add(inputBox);

            Button okButton = new Button { Content = "OK", Width = 80, IsDefault = true };
            okButton.Click += (s, e) => { ResponseText = inputBox.Text; DialogResult = true; Close(); };
            panel.Children.Add(okButton);

            Content = panel;
        }
    }
}
