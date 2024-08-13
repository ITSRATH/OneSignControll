using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneSignControll.Models
{
    public class ModelBase : INotifyPropertyChanged
    {
#pragma warning disable CS0067 // Das Ereignis "ModelBase.PropertyChanged" wird nie verwendet.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067 // Das Ereignis "ModelBase.PropertyChanged" wird nie verwendet.
    }
}
