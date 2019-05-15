using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCommander
{
    class ComboBoxViewModel : BaseViewModel
    {
        public ObservableCollection<DirectoryItem> Drives { get; set; }
        
    }
}
