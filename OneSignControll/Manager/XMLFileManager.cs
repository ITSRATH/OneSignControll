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

namespace OneSignControll.Manager
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

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            EnsureDirectoryExists(CurrentXMLFilePath);
            await EnsureDefaultFileExistsAsync(CurrentXMLFilePath);
        }

        public async Task<List<ProgramEntry>> ReadXMLAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                var programs = new List<ProgramEntry>();

                await Task.Run(() =>
                {
                    var doc = XDocument.Load(filePath);
                    foreach (var programElement in doc.Descendants("Program"))
                    {
                        programs.Add(new ProgramEntry(programElement.Element("Path").Value)
                        {
                            Name = programElement.Element("Name")?.Value
                        });
                    }
                });

                return programs;
            }
            else
            {
                throw new Exception("Die Datei existiert nicht.");
            }
        }

        public async Task<bool> WriteXMLAsync(List<ProgramEntry> programs, bool overwriteDefault, string overrideFileName = null)
        {
            string filePath = overwriteDefault ? DefaultXMLFilePath : CurrentXMLFilePath;

            await Task.Run(() =>
            {
                var doc = new XDocument(new XElement("Programs",
                    programs.Select(p => new XElement("Program",
                        new XElement("Path", p.Path),
                        new XElement("Name", p.Name)
                    ))
                ));

                EnsureDirectoryExists(overrideFileName ?? filePath);
                doc.Save(overrideFileName ?? filePath);
            });

            CurrentXMLFilePath = filePath;
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
        }

        public async Task<bool> ImportXMLAsync(string filePath, bool overwriteDefault)
        {
            var programs = await ReadXMLAsync(filePath);
            CurrentXMLFilePath = filePath;
            if (overwriteDefault)
            {
                return await WriteXMLAsync(programs, true);
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
                await WriteXMLAsync(new List<ProgramEntry>(), true);
            }
        }

        #endregion
    }
}
