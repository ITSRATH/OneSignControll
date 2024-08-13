using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSignControll.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Constructor

        public ViewModelBase(IDialogCoordinator dialogCoordinator)
        {
            DialogCoordinator = dialogCoordinator;
        }

        #endregion

        #region Public Properties

        public IDialogCoordinator DialogCoordinator { get; set; }

        #endregion

        #region Events

#pragma warning disable CS0067 // Das Ereignis "ViewModelBase.PropertyChanged" wird nie verwendet.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067 // Das Ereignis "ViewModelBase.PropertyChanged" wird nie verwendet.

        #endregion
    }
}
