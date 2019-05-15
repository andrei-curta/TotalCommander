using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SupremeCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ObservableCollection<DirectoryItem> directories = new ObservableCollection<DirectoryItem>(DirectoryStructure.GetLogicalDrives());
        private string currentPath1;
        private string currentPath2;
        private DataGrid inFocusDataGrid;

        public MainWindow()
        {
            InitializeComponent();
            inFocusDataGrid = DataGrid1;

            #region Initialize Directories Comboboxes

            //loop trough the 2 comboboxes
            foreach (var comboBox in new List<ComboBox>() { ComboBoxSelectDirectory1, ComboBoxSelectDirectory2 })
            {
                foreach (DirectoryItem it in directories)
                {
                    comboBox.Items.Add(it.Name);
                }
                comboBox.SelectedItem = comboBox.Items[0];
            }

            #endregion
        }


        internal ObservableCollection<DirectoryItems> Directories1 { get; set; }
        internal ObservableCollection<DirectoryItems> Directories2 { get; set; }

        #region Event handlers

        private void ComboBoxSelectDirectory1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DirectoryItem dir in directories)
            {
                if (ComboBoxSelectDirectory1.SelectedItem.ToString() == dir.Name)
                {
                    lblVolumeSize1.Content = dir.Size;
                }
            }

            //update the current path for the left side
            currentPath1 = ComboBoxSelectDirectory1.SelectedItem.ToString();

            //update the data grid
            UpdateDataGrid(DataGrid1, currentPath1, Directories1);
        }

        private void ComboBoxSelectDirectory2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (DirectoryItem dir in directories)
            {
                if (ComboBoxSelectDirectory2.SelectedItem.ToString() == dir.Name)
                {
                    lblVolumeSize2.Content = dir.Size;
                }
            }

            //update the current path for the right side
            currentPath2 = ComboBoxSelectDirectory2.SelectedItem.ToString();

            //update the data grid
            UpdateDataGrid(DataGrid2, currentPath2, Directories2);
        }

        /// <summary>
        /// opens selected itemn in notepad, if it is possible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNotepad_Click(object sender, RoutedEventArgs e)
        {
            //get the selected elements
            var selectedItems = inFocusDataGrid.SelectedItems;

            foreach (var item in selectedItems)
            {
                //ignore folders
                if (((DirectoryItems)item).Extension == "<DIR>")
                {
                    MessageBox.Show("Cannot ope folder in Text editor!");
                    continue;
                }

                System.Diagnostics.Process.Start(((DirectoryItems)item).FullPath);
            }
        }

        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            inFocusDataGrid = (DataGrid)sender;
        }

        /// <summary>
        /// event thet expands a folder upon double ckicking it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            var currentItem = (DirectoryItems)row.Item;

            //other elenents than a folder cannot be expanded
            if (currentItem.Extension != "<DIR>")
                return;

            if (row.Parent == DataGrid1)
            {
                currentPath1 = currentItem.FullPath;
                UpdateDataGrid(DataGrid1, currentPath1, Directories1);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //get the selected item
            var currentDataGrid = (DataGrid)sender;
            var currentItem = (DirectoryItems)currentDataGrid.SelectedItem;

            //other elenents than a folder cannot be expanded
            if (currentItem == null || currentItem.Extension != "<DIR>")
                return;

            if (currentDataGrid == DataGrid1)
            {
                currentPath1 = currentItem.FullPath;
                UpdateDataGrid(DataGrid1, currentPath1, Directories1);
            }
            else
            {
                currentPath2 = currentItem.FullPath;
                UpdateDataGrid(DataGrid2, currentPath2, Directories2);
            }
        }

        private void BtnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            string currentPath;
            var selectedDirectory = new ObservableCollection<DirectoryItems>();

            if (inFocusDataGrid == DataGrid1)
            {
                currentPath = currentPath1;
                selectedDirectory = Directories1;
            }
            else
                if (inFocusDataGrid == DataGrid2)
            {
                currentPath = currentPath2;
                selectedDirectory = Directories2;
            }
            else
            {
                MessageBox.Show("No location selected");
                return;
            }

            //TODO: treat case when deault folder name exists
            string newFolderName = "NewFolder";
            Directory.CreateDirectory(currentPath + "\\" + newFolderName);

            UpdateDataGrid(inFocusDataGrid, currentPath, selectedDirectory);
        }

        /// <summary>
        /// treats keypressed event. It is used to map key presses to behaviours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyPressedEvent(object sender, KeyEventArgs e)
        {
            var currentDataGrid = (DataGrid)sender;
            var currentItem = (DirectoryItems)currentDataGrid.SelectedItem;

            ///check which key what pressed and act accordingly
            //enter is used to open files and expand folders
            if (e.Key == Key.NumPad0)
            {
                if (currentItem == null)
                    return;

                //if selected item is a folder, expand it
                if (currentItem.Extension == "<DIR>")
                {
                    if (currentDataGrid == DataGrid1)
                    {
                        currentPath1 = currentItem.FullPath;
                        UpdateDataGrid(DataGrid1, currentPath1, Directories1);
                    }
                    else
                    {
                        currentPath2 = currentItem.FullPath;
                        UpdateDataGrid(DataGrid2, currentPath2, Directories2);
                    }
                }
            }
        }

        /// <summary>
        /// Event thet fires when the 'Move' Button is clicked
        /// Moves selected item to the other side
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            FileOperation(FileOperationTypes.Move);
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            FileOperation(FileOperationTypes.Copy);
        }

        /// <summary>
        /// navigates to parent directory in the left side
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGoToParent1_Click(object sender, RoutedEventArgs e)
        {
            //check if the current folder is not a root directory
            foreach (var dir in directories)
            {
                if (dir.FullPath == currentPath1)
                    return;
            }

            currentPath1 = Directory.GetParent(currentPath1).FullName;
            UpdateDataGrid(DataGrid1, currentPath1, Directories1);
        }

        /// <summary>
        /// navigates to parent directory in the right side
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGoToParent2_Click(object sender, RoutedEventArgs e)
        {
            //check if the current folder is not a root directory
            foreach (var dir in directories)
            {
                if (dir.FullPath == currentPath2)
                    return;
            }

            currentPath2 = Directory.GetParent(currentPath2).FullName;
            UpdateDataGrid(DataGrid2, currentPath2, Directories2);
        }

        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void BtnAddToArchive(object sender, RoutedEventArgs e)
        {
            FileOperation(FileOperationTypes.AddToArchive);
            //Todo: fix the bloody updates
            UpdateDataGrid(DataGrid1, currentPath1, Directories1);
            UpdateDataGrid(DataGrid2, currentPath2, Directories2);
        }

        private void BtnExtractArchive(object sender, RoutedEventArgs e)
        {
            FileOperation(FileOperationTypes.ExtractArchive);
            //Todo: fix the bloody updates
            UpdateDataGrid(DataGrid1, currentPath1, Directories1);
            UpdateDataGrid(DataGrid2, currentPath2, Directories2);
        }

        private void BtnCompare_Click(object sender, RoutedEventArgs e)
        {
            CompareByContent();
        }

        #endregion

        #region Helper Functions

        enum FileOperationTypes
        {
            Copy,
            Move,
            AddToArchive,
            ExtractArchive
        }

        private void UpdateDataGrid(DataGrid dataGrid, string path, ObservableCollection<DirectoryItems> selectedDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files;
            DirectoryInfo[] Dirs;

            //make sure the update is possible
            try
            {
                Dirs = d.GetDirectories();
                Files = d.GetFiles();
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("You do not have the rights to open that!");
                return;
            }

            //clear the current Directory itemn list
            selectedDirectory = new ObservableCollection<DirectoryItems>();

            //clear the now obsolete list of files
            dataGrid.Items.Clear();

            foreach (FileInfo file in Files)
            {
                selectedDirectory.Add(new DirectoryItems(System.IO.Path.GetFileNameWithoutExtension(file.Name), file.FullName,
                    file.LastWriteTimeUtc.ToShortDateString(), file.Extension, file.Length));
            }
            foreach (var dir in Dirs)
            {
                selectedDirectory.Add(new DirectoryItems(System.IO.Path.GetFileNameWithoutExtension(dir.Name), dir.FullName,
                    dir.LastWriteTime.ToLongTimeString(), "<DIR>"));
            }

            foreach (var element in selectedDirectory)
            {
                dataGrid.Items.Add(element);
            }
        }

        /// <summary>
        /// moves or copies files
        /// </summary>
        /// <param name="move">true for move, false for copy</param>
        private void FileOperation(FileOperationTypes operationType)
        {
            try
            {
                var filesToMove = inFocusDataGrid.SelectedItems;
                string destinationFolder;

                //set the destiation folder as the path to the folder opened in the other side to the selected item's side
                if (inFocusDataGrid == DataGrid1)
                {
                    destinationFolder = currentPath2;
                }
                else
                {
                    destinationFolder = currentPath1;
                }

                //if it is an Add to archive command, perform it and exit the function
                if (operationType == FileOperationTypes.AddToArchive)
                {
                    //Todo: fix this

                    DirectoryItems currentDirectoryItem = (DirectoryItems)filesToMove[0];
                    string destination = System.IO.Path.Combine(destinationFolder, currentDirectoryItem.Name);
                    
                    UpdateDataGrid(DataGrid1, currentPath1, Directories1);
                    UpdateDataGrid(DataGrid2, currentPath2, Directories2);

                    return;
                }

                //if it is an extract archive command, perform it and exit the function
                if (operationType == FileOperationTypes.ExtractArchive)
                {
                    //Todo: fix it

                    DirectoryItems currentDirectoryItem = (DirectoryItems)filesToMove[0];
                    string destination = System.IO.Path.Combine(destinationFolder, currentDirectoryItem.Name);
                   
                    UpdateDataGrid(DataGrid1, currentPath1, Directories1);
                    UpdateDataGrid(DataGrid2, currentPath2, Directories2);

                    return;
                }

                foreach (var file in filesToMove)
                {
                    DirectoryItems currentDirectoryItem = (DirectoryItems)file;
                    string destination = System.IO.Path.Combine(destinationFolder, currentDirectoryItem.Name);

                    //check if we move a file or a folder in order to use the appropriate command
                    if (currentDirectoryItem.Extension == "<DIR>")
                    {
                        switch (operationType)
                        {
                            case FileOperationTypes.Move:
                                {
                                    Directory.Move(currentDirectoryItem.FullPath, destination);
                                    break;
                                }
                            case FileOperationTypes.Copy:
                                {
                                    CopyDir(currentDirectoryItem.FullPath, destination);
                                    //CopyDirectoryCMD(currentDirectoryItem.FullPath, destination);
                                    break;
                                }

                        }
                    }
                    else
                    {
                        //if it is a file it must have a usefull extension, so add it to the destination
                        destination += currentDirectoryItem.Extension;

                        switch (operationType)
                        {
                            case FileOperationTypes.Move:
                                {
                                    File.Move(currentDirectoryItem.FullPath, destination);
                                    break;
                                }
                            case FileOperationTypes.Copy:
                                {
                                    File.Copy(currentDirectoryItem.FullPath, destination);
                                    break;
                                }
                        }
                    }
                }

                UpdateDataGrid(DataGrid1, currentPath1, Directories1);
                UpdateDataGrid(DataGrid2, currentPath2, Directories2);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region Copy Directory

        public static void CopyDir(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        void CopyDirectoryCMD(string src, string dest)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C Xcopy /E /I " + src + " " + dest;
            process.StartInfo = startInfo;
            process.Start();
        }

        #endregion


        void CompareByContent()
        {
            //get the selected items
            var selectedItems = inFocusDataGrid.SelectedItems;

            //make sure no more than 2 items are selected and that theese files are txt
            int count = 0;
            foreach(var item in selectedItems)
            {
                count++;
                if (count > 2)
                {
                    MessageBox.Show("Too many items selected!");
                    return;
                }

                //check if the current item is a txt file
                if( ((DirectoryItems)item).Extension != ".txt")
                {
                    MessageBox.Show("Invalid file type!");
                    return;
                }
            }

            DirectoryItems file1 = (DirectoryItems)selectedItems[0];
            DirectoryItems file2 = (DirectoryItems)selectedItems[1];

            
        }

        #endregion


    }
}
