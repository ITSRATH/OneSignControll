using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OneSignControll.Models;
using OneSignControll.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace OneSignControll
{
    public partial class MainWindow : MetroWindow
    {
        #region Variables

        private readonly MainWindowViewModel viewModel;

        #endregion

        #region Constructor

        public MainWindow()
        {
            viewModel = new MainWindowViewModel(DialogCoordinator.Instance);

            InitializeComponent();

            this.DataContext = viewModel;
        }

        #endregion

        #region EventHandlers

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.InitializeAsync();
            viewModel.CurrentStatus = "Programm erfolgreich gestartet";
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageDialogResult result = await this.ShowMessageAsync("Warnung", "Ungespeicherte Änderungen gehen verloren.", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Application.Current.Shutdown();
            }
        }

        private async void CmdAddProgram_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Programm auswählen",
                Filter = "Anwendungsdateien (*.exe)|*.exe|" + "Installationsdateien (*.msi)|*.msi|" + "Batch- und Skriptdateien (*.bat;*.cmd;*.vbs;*.ps1)|*.bat;*.cmd;*.vbs;*.ps1|" + "Management Console Dateien (*.msc)|*.msc|" + "Systemsteuerungsdateien (*.cpl)|*.cpl|" + "Alle Dateien (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                string programName = System.IO.Path.GetFileNameWithoutExtension(selectedPath);

                MetroDialogSettings dialogSettings = new MetroDialogSettings()
                {
                    DefaultText = programName
                };

                var inputResult = await this.ShowInputAsync("Programm umbenennen", "Geben Sie einen neuen Namen ein:", dialogSettings);
                if (String.IsNullOrWhiteSpace(inputResult) == false)
                {
                    await viewModel.AddProgramAsync(inputResult, selectedPath);
                    viewModel.CurrentStatus = $"Programm '{inputResult}' hinzugefügt";
                }
            }
        }

        private void CmdAddProgram_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdRenameProgram_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            string oldName = viewModel.SelectedProgramEntry.Name;

            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                DefaultText = oldName
            };

            var inputResult = await this.ShowInputAsync("Programm umbenennen", "Geben Sie einen neuen Namen ein:", dialogSettings);
            if (String.IsNullOrWhiteSpace(inputResult) == false)
            {
                await viewModel.RenameProgramAsync(viewModel.SelectedProgramEntry, inputResult);
                viewModel.CurrentStatus = $"Programm '{oldName}' umbenannt zu '{viewModel.SelectedProgramEntry.Name}'";
            }
        }

        private void CmdRenameProgram_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null && ProgramListView.SelectedItems.Count == 1)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdRemoveProgram_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            var selectedPrograms = ProgramListView.SelectedItems;
            var programsToRemove = selectedPrograms.Cast<ProgramEntry>().ToList();

            foreach (var program in programsToRemove)
            {
                await viewModel.RemoveProgramAsync(program);
            }
        }

        private void CmdRemoveProgram_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null && viewModel.SelectedProgramEntry != null)
            {
                e.CanExecute = true;
            }
        }

        private async void Cmd_ReloadXML_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MessageDialogResult result = await this.ShowMessageAsync("Warnung", "Ungespeicherte Änderungen gehen verloren.", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                await viewModel.LoadXMLAsync(viewModel.XMLFileManager.CurrentXMLFilePath);
                viewModel.CurrentStatus = "Konfiguration erfolgreich geladen";
            }
        }

        private void Cmd_ReloadXML_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null && viewModel.XMLFileManager.CurrentXMLFilePath != null)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdSaveXML_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ja",
                NegativeButtonText = "Nein",
                FirstAuxiliaryButtonText = "Abbrechen",
            };

            MessageDialogResult result = await this.ShowMessageAsync("Warnung", "Möchten Sie die Standardwerte mit dieser Konfiguration überschreiben?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, metroDialogSettings);
            if (result == MessageDialogResult.Affirmative)
            {
                await viewModel.SaveXMLAsync(true);
                viewModel.CurrentStatus = "Standardkonfiguration erfolgreich überschrieben";
            }
            else if (result == MessageDialogResult.Negative)
            {
                await viewModel.SaveXMLAsync(false);
                viewModel.CurrentStatus = "Konfiguration erfolgreich gespeichert";
            }
            else
            {
                return;
            }
        }

        private void CmdSaveXML_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdImportXML_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
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
                    MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Ja",
                        NegativeButtonText = "Nein",
                        FirstAuxiliaryButtonText = "Abbrechen",
                    };

                    MessageDialogResult result = await this.ShowMessageAsync("Warnung", "Möchten Sie die Standardwerte mit dieser Konfiguration überschreiben?", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, metroDialogSettings);
                    if (result == MessageDialogResult.Affirmative)
                    {
                        await viewModel.ImportXMLAsync(openFileDialog.FileName, true);
                        viewModel.CurrentStatus = "Konfiguration erfolgreich geladen und Standardkonfiguration überschrieben";
                    }
                    else if (result == MessageDialogResult.Negative)
                    {
                        await viewModel.ImportXMLAsync(openFileDialog.FileName, false);
                        viewModel.CurrentStatus = "Konfiguration erfolgreich geladen";
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Warnung", $"Fehler beim Importieren der XML-Datei: {ex.Message}");
                }
            }
        }

        private void CmdImportXML_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdExportXML_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
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
                    await viewModel.SaveXMLAsync(false, saveFileDialog.FileName);
                    viewModel.CurrentStatus = "Konfiguration erfolgreich exportiert";
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Fehler", $"Fehler beim Exportieren der XML-Datei: {ex.Message}");
                }
            }
        }

        private void CmdExportXML_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null && viewModel.XMLFileManager.CurrentXMLFilePath != null)
            {
                e.CanExecute = true;
            }
        }

        private async void CmdClose_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            MessageDialogResult result = await this.ShowMessageAsync("Warnung", "Ungespeicherte Änderungen gehen verloren.", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Application.Current.Shutdown();
            }
        }

        private void CmdClose_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CmdOpenInfoWindow_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }

        private void CmdOpenInfoWindow_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private async void CmdStartProgram_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            try
            {
                viewModel.CurrentStatus = $"Programm wird gestartet...";
                viewModel.RunSelectedProgram();
                viewModel.CurrentStatus = "Programm erfolgreich gestartet";
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Warnung", $"Fehler beim Starten des Programms: {ex.Message}", MessageDialogStyle.AffirmativeAndNegative);
            }
        }

        private void CmdStartProgram_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (viewModel != null && ProgramListView != null && ProgramListView.SelectedItems.Count == 1)
            {
                e.CanExecute = true;
            }
        }

        #endregion
    }
}
