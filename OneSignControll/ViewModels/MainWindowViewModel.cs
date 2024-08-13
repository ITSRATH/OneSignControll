using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OneSignControll.Manager;
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

        private readonly XMLFileManager xmlFileManager = new XMLFileManager();

        #endregion

        #region Constructor

        public MainWindowViewModel(IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
        {
            ProgramVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        #endregion

        #region Public Properties

        public ObservableCollection<ProgramEntry> Programs { get; set; } = new ObservableCollection<ProgramEntry>();

        public ProgramEntry SelectedProgramEntry { get; set; }

        public string CurrentStatus { get; set; } = "";

        public string ProgramVersion { get; set; }

        public XMLFileManager XMLFileManager => xmlFileManager;

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            await xmlFileManager.InitializeAsync();
            
            await LoadXMLAsync(xmlFileManager.DefaultXMLFilePath);
        }

        public async Task LoadXMLAsync(string filePath)
        {
            try
            {
                try
                {
                    var programs = await xmlFileManager.ReadXMLAsync(filePath);
                    Programs = new ObservableCollection<ProgramEntry>(programs);
                }
                catch (Exception ex)
                {
                    await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Laden der XML-Datei: {ex.Message}");
                }
            }
            catch (Exception)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", "Fehler beim Laden der XML-Datei. Die Datei existiert nicht.");
            }
        }

        public async Task<bool> SaveXMLAsync(bool overwriteDefault, string overrideFileName = null)
        {
            try
            {
                return await xmlFileManager.WriteXMLAsync(Programs.ToList(), overwriteDefault, overrideFileName);
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
                await LoadXMLAsync(filePath);
                if(overwriteDefault)
                {
                    return await SaveXMLAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                await DialogCoordinator.ShowMessageAsync(this, "Fehler", $"Fehler beim Importieren der XML-Datei: {ex.Message}");
                return false;
            }            
        }

        public void AddProgram(string name, string filePath)
        {
            Programs.Add(new ProgramEntry(filePath) { Name = name });
        }

        public void RenameProgram(ProgramEntry entry, string newName)
        {
            entry.Name = newName;
        }

        public void RemoveProgram(ProgramEntry entry)
        {
            Programs.Remove(entry);
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
