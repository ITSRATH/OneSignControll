using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using OneSignControll.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using OneSignControll.Models;
using System.ComponentModel;

namespace OneSignControll
{
    public partial class InfoWindow : MetroWindow
    {
        #region Variables

        private readonly InfoWindowViewModel viewModel;

        #endregion

        #region Constructor

        public InfoWindow()
        {
            viewModel = new InfoWindowViewModel(DialogCoordinator.Instance);

            InitializeComponent();

            DataContext = viewModel;
        }

        #endregion

        #region EventHandlers

        private async void CmdDestroyData_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            //try
            //{
            //    string message = $"Sind Sie sicher, dass Sie den AppData-Ordner ({viewModel.XMLFileManager.DefaultXMLDirPath}) und die gespeicherte XML-Datei ({viewModel.XMLFileManager.DefaultXMLFilePath}) löschen möchten? Dies kann nicht rückgängig gemacht werden!";
            //    MessageDialogResult dialogResult = await this.ShowMessageAsync("Bestätigung erforderlich", message);

            //    if (dialogResult == MessageDialogResult.Affirmative)
            //    {
            //        await viewModel.ResetXMLAsnyc();
            //        await this.ShowMessageAsync("Löschen erfolgreich", "Der AppData-Ordner und die XML-Datei wurden erfolgreich gelöscht und auf die Standardwerte zurückgesetzt");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await this.ShowMessageAsync("Fehler", $"Fehler beim Löschen: {ex.Message}");
            //}

            MetroDialogSettings metroDialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Ja",
                NegativeButtonText = "Abbrechen",
            };

            MessageDialogResult result = await this.ShowMessageAsync(
                "Löschbestätigung",  // Titel des Dialogs
                "Sind Sie sicher, dass Sie den AppData-Ordner und die gespeicherte Datei löschen möchten? Dies kann nicht rückgängig gemacht werden!",  // Nachricht
                MessageDialogStyle.AffirmativeAndNegative,  // Dialogstil
                metroDialogSettings  // Dialogeinstellungen
            );

            if (result == MessageDialogResult.Affirmative)
            {
                try
                {
                    await viewModel.ResetXMLAsnyc();
                    await this.ShowMessageAsync("Löschen erfolgreich", "Der AppData-Ordner und die XML-Datei wurden erfolgreich gelöscht und auf die Standardwerte zurückgesetzt");
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Fehler", $"Fehler beim Löschen: {ex.Message}");
                }
            }
            else if (result == MessageDialogResult.Negative)
            {
                return;
            }
            else
            {
                return;
            }
        }

        private void CmdDestroyData_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion
    }
}
