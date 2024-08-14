using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OneSignControll.Managers;
using OneSignControll.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace OneSignControll.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Variables

        #endregion

        #region Constructor

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
        {
            ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        #endregion

        #region Public Properties

        public ProgramEntry SelectedProgramEntry { get; set; }

        public string CurrentStatus { get; set; } = "";

        public string ProgramVersion { get; set; }

        public XMLFileManager XMLFileManager => XMLFileManager.Instance;

        #endregion

        #region Public Methods

        public async Task<bool> InitializeAsync()
        {
            await XMLFileManager.InitializeAsync();
            
            return await LoadXMLAsync(XMLFileManager.DefaultXMLFilePath);
        }

        public async Task<bool> LoadXMLAsync(string filePath)
        {
            try
            {
                return await XMLFileManager.ReadXMLAsync(filePath);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Laden der XML-Datei: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveXMLAsync(bool overwriteDefault, string overrideFileName = null)
        {
            try
            {
                return await XMLFileManager.WriteXMLAsync(overwriteDefault, overrideFileName);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Speichern der XML-Datei: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ImportXMLAsync(string filePath, bool overwriteDefault)
        {
            try
            {
                return await XMLFileManager.ImportXMLAsync(filePath, overwriteDefault);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Importieren der XML-Datei: {ex.Message}");
                return false;
            }            
        }

        public async Task AddProgramAsync(string name, string filePath)
        {
            try
            {
                XMLFileManager.AddProgram(name, filePath);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Hinzufügen des Programms: {ex.Message}");
            }
        }

        public async Task RenameProgramAsync(ProgramEntry entry, string newName)
        {
            try
            {
                XMLFileManager.RenameProgram(entry, newName);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Umbenennen des Programms: {ex.Message}");
            }
        }

        public async Task RemoveProgramAsync(ProgramEntry entry)
        {
            try
            {
                XMLFileManager.RemoveProgram(entry);
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Entfernen des Programms: {ex.Message}");
            }
        }

        public void RunSelectedProgram()
        { 
            Process.Start(new ProcessStartInfo
            {
                FileName = SelectedProgramEntry.Path,
                UseShellExecute = true,
                Verb = "runas" // Ensure it runs with administrator rights
            });
        }

        #endregion
    }
}
