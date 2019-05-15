using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SupremeCommander
{
    class DirectoryItems
    {
        private string name;
        private string lastWriteTime;
        private string extension;
        private string size;
        private Icon thumbNail;
        private string fullPath;
        private string attributes;

        public string Extension { get => extension; set => extension = value; }
        public string Size { get => size; set => size = value; }
        public string LastWriteTime { get => lastWriteTime; set => lastWriteTime = value; }
        public string Name { get => name; set => name = value; }
        public Icon ThumbNail { get => thumbNail; set => thumbNail = value; }
        public string FullPath { get => fullPath; set => fullPath = value; }
        public string Attributes { get => attributes; set => attributes = value; }

        #region Constructors

        public DirectoryItems(string name, string path,string lastWriteTime, string extension, long size = 0)
        {
            Name = name;
            LastWriteTime = lastWriteTime;
            Extension = extension;
            FullPath = path;
            Attributes = getFileAttributes();

            if(size != 0)
                Size = size.ToString();

            if (fullPath != null)
                ThumbNail = System.Drawing.Icon.ExtractAssociatedIcon(fullPath);
            
        }

        public DirectoryItems(string name, string path, string lastWriteTime, string extension)
        {
            Name = name;
            LastWriteTime = lastWriteTime;
            Extension = extension;
            FullPath = path;
            //ThumbNail = System.Drawing.Icon.ExtractAssociatedIcon(fullPath);
        }

        #endregion

        #region Helper Methods

        private string getFileAttributes()
        {
            string permissionShort = string.Empty;
            try
            {
                DirectorySecurity dSecurity = System.IO.Directory.GetAccessControl(FullPath);
                foreach (FileSystemAccessRule rule in dSecurity.GetAccessRules(true, true, typeof(NTAccount)))
                {
                    //permissionShort += rule.FileSystemRights.ToString() + " : ";
                    permissionShort += ((rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write) ? "w" : "-";
                    permissionShort += ((rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read) ? "r" : "-";
                    permissionShort += ((rule.FileSystemRights & FileSystemRights.AppendData) == FileSystemRights.AppendData) ? "a" : "-";
                    permissionShort += ((rule.FileSystemRights & FileSystemRights.Modify) == FileSystemRights.Modify) ? "m" : "-";
                    permissionShort += ((rule.FileSystemRights & FileSystemRights.ExecuteFile) == FileSystemRights.ExecuteFile) ? "e" : "-";
                    return permissionShort;
                }
            }
            catch { }

            return permissionShort;
        }

        #endregion
    }
}
