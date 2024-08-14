using MahApps.Metro.Controls.Dialogs;
using OneSignControll.Managers;
using OneSignControll.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneSignControll.Managers
{
    public class XMLFileManager : ManagerBase
    {
        #region Constructor

        public XMLFileManager()
        {
            DefaultXMLDirPath = Path.GetDirectoryName(DefaultXMLFilePath);
            CurrentXMLFilePath = DefaultXMLFilePath;
        }       

        #endregion
                    
        #region Public Properties

        public string DefaultXMLFilePath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OneSignControll", "OneSignControllSave.xml");

        public string DefaultXMLDirPath { get; set; }

        public string CurrentXMLFilePath { get; set; }

        public ObservableCollection<ProgramEntry> Programs { get; set; } = new ObservableCollection<ProgramEntry>();

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            EnsureDirectoryExists(CurrentXMLFilePath);
            await EnsureDefaultFileExistsAsync(CurrentXMLFilePath);
        }

        public async Task<bool> ReadXMLAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var doc = XDocument.Load(filePath);
                Programs.Clear();
                foreach (var programElement in doc.Descendants("Program"))
                {
                    Programs.Add(new ProgramEntry(programElement.Element("Path").Value)
                    {
                        Name = programElement.Element("Name")?.Value
                    });
                }
                OnConfigLoaded();
                return true;
            }
            else
            {
                throw new Exception("Die Datei existiert nicht.");
            }
        }

        public async Task<bool> WriteXMLAsync(bool overwriteDefault, string overrideFileName = null)
        {
            string filePath = overwriteDefault ? DefaultXMLFilePath : CurrentXMLFilePath;

            await Task.Run(() =>
            {
                var doc = new XDocument(new XElement("Programs",
                    Programs.Select(p => new XElement("Program",
                        new XElement("Path", p.Path),
                        new XElement("Name", p.Name)
                    ))
                ));

                EnsureDirectoryExists(overrideFileName ?? filePath);
                doc.Save(overrideFileName ?? filePath);
            });

            CurrentXMLFilePath = filePath;
            OnConfigSaved();
            return true;
        }
        
        public async Task ResetXMLFileAsync()
        {
            string defaultDirPath = Path.GetDirectoryName(DefaultXMLFilePath);

            if (File.Exists(DefaultXMLFilePath))
            {
                File.Delete(DefaultXMLFilePath);
            }

            if (Directory.Exists(defaultDirPath))
            {
                Directory.Delete(defaultDirPath, true);
            }

            EnsureDirectoryExists(DefaultXMLFilePath);
            await EnsureDefaultFileExistsAsync(DefaultXMLFilePath);

            Programs.Clear();

            OnConfigReseted();
        }

        public async Task<bool> ImportXMLAsync(string filePath, bool overwriteDefault)
        {
            await ReadXMLAsync(filePath);
            CurrentXMLFilePath = filePath;
            if (overwriteDefault)
            {
                return await WriteXMLAsync(true);
            }
            return true;
        }

        private void EnsureDirectoryExists(string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (Directory.Exists(dirPath) == false)
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        private async Task EnsureDefaultFileExistsAsync(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                await WriteXMLAsync(true);
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

        #endregion

        #region Events

        public event EventHandler ConfigSaved;
        protected void OnConfigSaved()
        {
            ConfigSaved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ConfigLoaded;
        protected void OnConfigLoaded()
        {
            ConfigLoaded?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ConfigReseted;
        protected void OnConfigReseted()
        {
            ConfigReseted?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Singleton

        // Intermediate solution until DI-Containers are implemented
        private static XMLFileManager _instance;
        public static XMLFileManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new XMLFileManager();
                }
                return _instance;
            }
        }

        #endregion
    }
}
