using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsExplorerExtensionDuplicateFile
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]
    public class DuplicateFileExt : SharpContextMenu
    {
        /// <summary>
        /// Determines whether this instance can show a shell context show menu, given the specified selected file list.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance should show a shell context menu for the specified file list; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanShowMenu()
        {           
            return true;  // replace with your customn logic to show or not show menu item
        }

        /// <summary>
        /// Creates the context menu. This can be a single menu item or a tree of them.
        /// </summary>
        /// <returns>
        /// The context menu for the shell context menu.
        /// </returns>
        protected override ContextMenuStrip CreateMenu()
        {
            try { 
            //  Create the menu strip.
            var menu = new ContextMenuStrip();

            //  Create a 'count lines' item.
            var itemCountLines = new ToolStripMenuItem
            {
                Text = Resources.AppStrings.ContextMenuCaption,
                Image = Resources.AppStrings.Duplicate3                
            };

            //  When we click, we'll count the lines.
            itemCountLines.Click += (sender, args) => DuplicateFiles();

            //  Add the item to the context menu.
            menu.Items.Add(itemCountLines);

            //  Return the menu.
            return menu;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Duplicate each file that has been selected.
        /// </summary>
        private void DuplicateFiles()
        {
            string existingFullFilename = string.Empty;

            try
            {
                //  Go through each selected item in explorer.
                foreach (var filePath in SelectedItemPaths)
                {
                    string existingFilenameNoExtension, existingFilenameExtension = string.Empty;
                    int copyCounter = 1;
                    bool isValidFilename = false;
                    string duplicatedFilename = Resources.AppStrings.DuplicateFilePostfix;
                    string newFilename = string.Empty;

                    // check that object is a file
                    if (File.Exists(filePath))
                    {
                        existingFullFilename = Path.GetFileName(filePath);
                        existingFilenameNoExtension = Path.GetFileNameWithoutExtension(filePath);
                        existingFilenameExtension = Path.GetExtension(filePath);                                                

                        while (!isValidFilename)
                        {
                            newFilename = string.Format(duplicatedFilename, existingFilenameNoExtension, copyCounter, existingFilenameExtension);

                            if (File.Exists(Path.GetDirectoryName(filePath) + "\\" + newFilename))
                            {
                                copyCounter++; // keep going until system finds a unique filename
                            }
                            else isValidFilename = true; // exit loop
                        }

                        string saveFileAs = Path.GetDirectoryName(filePath) + "\\" + newFilename;

                        File.Copy(filePath, saveFileAs);
                    }
                }

                //  Show the ouput (not necessary)
                MessageBox.Show(String.Format(Resources.AppStrings.DuplicateSuccessMessage, existingFullFilename), "Duplicate File", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format(Resources.AppStrings.DuplicateFailureMessage, existingFullFilename, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

