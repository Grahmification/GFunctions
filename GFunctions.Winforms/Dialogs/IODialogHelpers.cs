namespace GFunctions.Winforms.Dialogs
{
    /// <summary>
    /// Class providing a global entry point for displaying dialog boxes to save and load files/folders
    /// </summary>
    public static class IODialogHelpers
    {
        /// <summary>
        /// Gets the filter string for a <see cref="SaveFileDialog"/> or <see cref="OpenFileDialog"/>
        /// </summary>
        /// <param name="fileTypes">An array of filetypes for which to include in the filter string. If null returns the default all files filter.</param>
        /// <returns>The formatted filter string for the dialog. All files plus any extra specified</returns>
        public static string GetFileTypeFilter(IOFileTypes[]? fileTypes = null)
        {
            var fileStrings = new List<string>();

            //if not null we have extra file types to add
            if (fileTypes is not null)
            {
                //iterate through each additional file and add the required filter string
                foreach (IOFileTypes fileType in fileTypes)
                {
                    switch (fileType)
                    {
                        case IOFileTypes.CSV:
                            fileStrings.Add("CSV File (*.csv)|*.csv");
                            break;

                        case IOFileTypes.Sqlite:
                            fileStrings.Add("Sqlite Database (*.sqlite)|*.sqlite");
                            break;

                        case IOFileTypes.ImagePNG:
                            fileStrings.Add("PNG Image (*.png)|*.png");
                            break;

                        default:
                            break;
                    }
                }
            }

            //Add the default filter for all files. We always want this. Add it at the end so it is always last
            fileStrings.Add("All files(*.*) | *.*");


            //join the strings with the correct separator to create the output
            return String.Join("|", fileStrings.ToArray());
        }


        /// <summary>
        /// Displays a folder browser dialog for selecting a folder. Returns blank string on error. 
        /// </summary>
        /// <param name="folderDlg">Optional override for the default dialog formatting</param>
        /// <returns>The selected folder path or a blank string on error</returns>
        public static string DisplayFolderBrowserDialog(FolderBrowserDialog? folderDlg = null)
        {
            if (folderDlg is null)
            {
                folderDlg = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = true
                };
            }

            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                return folderDlg.SelectedPath;
            }

            return "";
        }


        /// <summary>
        /// Displays a <see cref="SaveFileDialog"/> for selecting a save file. Returns blank string on error. 
        /// </summary>
        /// <param name="saveDlg">Optional override for the default dialog formatting</param>
        /// <returns>The selected file path or a blank string on error</returns>
        public static string DisplaySaveFileDialog(SaveFileDialog? saveDlg = null)
        {
            if (saveDlg is null)
            {
                saveDlg = new SaveFileDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = GetFileTypeFilter(),
                    FilterIndex = 0,
                    RestoreDirectory = true,
                };
            }

            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                return saveDlg.FileName;
            }

            return ""; //return blank string if nothing 
        }

        /// <summary>
        /// Displays a <see cref="SaveFileDialog"/> for selecting a save file. Returns blank string on error. 
        /// </summary>
        /// <param name="fileFilters">Optional file type filters</param>
        /// <param name="fileNameSuggestion">Optional default starting filename</param>
        /// <returns>The selected file path or a blank string on error</returns>
        public static string DisplaySaveFileDialog(IOFileTypes[]? fileFilters = null, string fileNameSuggestion = "")
        {
            int filterIndex = 0;

            if (fileFilters is not null)
                filterIndex = fileFilters.Length; //select the first filter entry

            var saveDlg = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = GetFileTypeFilter(fileFilters),
                FilterIndex = filterIndex,
                RestoreDirectory = true,
                FileName = fileNameSuggestion,
            };

            return DisplaySaveFileDialog(saveDlg);
        }


        /// <summary>
        /// Displays a <see cref="OpenFileDialog"/> for selecting a file to open. Returns blank string on error. 
        /// </summary>
        /// <param name="openDlg">Optional override for the default dialog formatting</param>
        /// <returns>The selected file path or a blank string on error</returns>
        public static string DisplayOpenFileDialog(OpenFileDialog? openDlg = null)
        {
            if (openDlg is null)
            {
                openDlg = new OpenFileDialog()
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = GetFileTypeFilter(),
                    FilterIndex = 0,
                    RestoreDirectory = true,
                };
            }

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                return openDlg.FileName;
            }

            //an error has occurred
            return "";
        }

        /// <summary>
        /// Displays a <see cref="OpenFileDialog"/> for selecting a file to open. Returns blank string on error. 
        /// </summary>
        /// <param name="fileFilters">Optional file type filters</param>
        /// <param name="fileNameSuggestion">Optional default starting filename</param>
        /// <returns>The selected file path or a blank string on error</returns>
        public static string DisplayOpenFileDialog(IOFileTypes[]? fileFilters = null, string fileNameSuggestion = "")
        {
            int filterIndex = 0;

            if (fileFilters is not null)
                filterIndex = fileFilters.Length; //select the first filter entry

            var openDlg = new OpenFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = GetFileTypeFilter(fileFilters),
                FilterIndex = filterIndex,
                RestoreDirectory = true,
            };

            return DisplayOpenFileDialog(openDlg);
        }
    }


    /// <summary>
    /// Types of files we might want to save or load
    /// </summary>
    public enum IOFileTypes
    {
        CSV,
        Sqlite,
        ImagePNG
    }
}
