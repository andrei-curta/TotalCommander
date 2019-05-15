using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCommander
{
    

    /// <summary>
    /// A view model for each directory item
    /// </summary>
    class DirectoryItemViewModel : BaseViewModel
    {
        public DirectoryItemType Type { get; set; }
        public string FullPath { get; set; }

        public string Name { get { return this.Type == DirectoryItemType.Drive ? this.FullPath : DirectoryStructure.GetFileFolderName(this.FullPath); } }

        public ObservableCollection<DirectoryItem> Items { get; set; }

        public DirectoryItemViewModel()
        {
            Items = new ObservableCollection<DirectoryItem>(DirectoryStructure.GetLogicalDrives());
        }
    }  
}
