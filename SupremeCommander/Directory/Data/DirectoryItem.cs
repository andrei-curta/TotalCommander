using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupremeCommander
{
    /// <summary>
    /// Information about a directory item
    /// </summary>
    public class DirectoryItem
    {
        //TODO: a constructor maybe
        #region Constructors

        
        #endregion


        public DirectoryItemType Type{get; set;}
        public string FullPath { get; set; }
        public string Name { get { return this.Type == DirectoryItemType.Drive ? this.FullPath : DirectoryStructure.GetFileFolderName(this.FullPath); } set { Name = value; } }
        public string Size { get { return getSize(); }  }
        

        #region Helpers

        private string getSize()
        {
            switch (this.Type)
            {
                case DirectoryItemType.Folder:
                    return "<DIR>";
                case DirectoryItemType.File:
                    return new System.IO.FileInfo(FullPath).Length.ToString();
                case DirectoryItemType.Drive:
                    {
                        DriveInfo[] allDrives = DriveInfo.GetDrives();
                        foreach (DriveInfo d in allDrives)
                        {
                            if (d.Name[0] == Name[0] && d.IsReady == true)
                            {
                                return (d.TotalSize - d.TotalFreeSpace).ToString() + " k free of " + d.TotalSize.ToString() + " k";
                            }
                        }
                        return "";
                    }
            }
            return "";

        }

        #endregion
    }
}




















