using MahApps.Metro.Controls.Dialogs;
using OneSignControll.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OneSignControll.ViewModels
{
    public class InfoWindowViewModel : ViewModelBase
    {
        #region Variables

        #endregion

        #region Constructor

        public InfoWindowViewModel(IDialogCoordinator dialogCoordinator) : base(dialogCoordinator)
        {
            
        }

        #endregion

        #region Public Properties

        public XMLFileManager XMLFileManager => XMLFileManager.Instance;

        #endregion

        #region Public Methods

        public async Task ResetXMLAsnyc()
        {
            await XMLFileManager.ResetXMLFileAsync();
        }

        #endregion
    }
}
