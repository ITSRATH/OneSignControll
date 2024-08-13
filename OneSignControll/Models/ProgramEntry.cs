using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSignControll.Models
{
    public class ProgramEntry : ModelBase
    {
        #region Constructor

        public ProgramEntry(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path); // Extrahiert den Anwendungsnamen ohne Erweiterung
        }

        #endregion

        #region Public Properties

        public string Path { get; set; }
        public string Name { get; set; }

        #endregion
    }
}
