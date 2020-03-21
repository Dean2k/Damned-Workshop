using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DamnedWorkshop
{
    public partial class DamnedPatcherForm : Form
    {
        [DllImport("user32.dll")]
        static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private static string DOWNLOAD_TEST_PATCH_STABLE_LINK = "https://github.com/Sweats/Damned/archive/master.zip";
        private static string DOWNLOAD_TEST_PATCH_TESTING_LINK = "https://github.com/Sweats/Damned/archive/testing.zip";

        private static int PATCH_TESTING = 0;
        private static int PATCH_STABLE = 1;

        private string directory;


        private static string TOOLTIP_TEST_PATCH_STABLE_TEXT = "Downloads and installs the latest public test patch from the stable banch from " + DOWNLOAD_TEST_PATCH_STABLE_LINK + " \n\nFiles will be downloaded into a temporary directory then extracted to where you set the directory to. After that, the temporary directory will be removed.";
        private static string TOOLTIP_TEST_PATCH_TESTING_TEXT = "Downloads and installs the latest public test patch from the testing banch from " + DOWNLOAD_TEST_PATCH_TESTING_LINK + ". \n\nFiles will be downloaded into a temporary directory then extracted to where you set the directory to. After that, the temporary directory will be removed.";

        private string tempDirectory = "";
        private string backupDirectory = "";
        private string publicTestPatchTestingSavedDirectory = "";
        private string publicTestPatchStableSavedDirectory = "";

        private bool validBackUpFolder = false;

        private DamnedFiles damnedFiles;
        private DamnedMainForm mainForm;

        public DamnedPatcherForm(DamnedFiles damnedFiles, DamnedMainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.damnedFiles = damnedFiles;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
            SetButtons();
            directory = damnedFiles.Directory;
            damnedDirectoryStringLabel.Text = damnedFiles.Directory;
            toolTipPublicTestPatchTesting.SetToolTip(publicTestPatchTestingButton, TOOLTIP_TEST_PATCH_TESTING_TEXT);
            toolTipPublicTestPatchStable.SetToolTip(publicTestPatchStableButton, TOOLTIP_TEST_PATCH_STABLE_TEXT);
        }


        private void PublicTestPatchStableButton_Click(object sender, EventArgs e)
        {
            if (publicTestPatchStablePathLabel.Text.Length == 0 && keepPublicTestPatchStableCheckbox.Checked)
            {
                MessageBox.Show("You did not select a folder to save a copy of the zip file from GitHub. Either uncheck the box if you don't want to save it or select a folder", "No folder selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            InstallPatch(PATCH_STABLE);

        }
        private void PublicTestPatchTestingButtion_Click(object sender, EventArgs e)
        {
            if (publicTestPatchTestingPathLabel.Text.Length == 0 && keepPublicTestPatchTestingCheckbox.Checked)
            {
                MessageBox.Show("You did not select a folder to save a copy of the zip file from GitHub. Either uncheck the box if you don't want to save it or select a folder", "No folder selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }

            InstallPatch(PATCH_TESTING);
        }


        private void InstallPatch(int patch)
        {
            if (!validBackUpFolder)
            {
                DialogResult result = MessageBox.Show("PLEASE READ:\n\nIt looks like you did not create a backup or the backup that you currently have is not valid. This backup is used to restore the game to the original state as if you were to re-install the game on Steam\n\nIf you continue, you understand that you will need to re-install the game from Steam to get back to the original state.\n\nDo you wish to continue?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
            }


            if (patch == PATCH_STABLE)
            {
                if (keepPublicTestPatchStableCheckbox.Checked)
                {
                    InstallPatchLocally(patch);

                }

                else
                {
                    InstallPatchFromGithub(patch);

                }

            }

            else if (patch == PATCH_TESTING)
            {
                if (keepPublicTestPatchTestingCheckbox.Checked)
                {
                    InstallPatchLocally(patch);
                }

                else
                {
                    InstallPatchFromGithub(patch);
                }

            }
        }

        private void InstallPatchLocally(int patch)
        {
            loggingTextBox.AppendText("Begin installation from local\n\n-----------------------------------------------------------------------------------------------------------\n\n");

            if (patch == PATCH_TESTING)
            {
                string directoryName = "Damned-testing";
                string damnedTempDirectory = Path.Combine(publicTestPatchTestingSavedDirectory, directoryName);

                if (!Directory.Exists(damnedTempDirectory))
                {
                    loggingTextBox.AppendText(String.Format("No existing local branch found in \"{0}\". Installing from GitHub. This will only have to happen one time.\n\n", publicTestPatchTestingSavedDirectory));
                    InstallPatchFromGithub(patch);
                    return;
                }

                loggingTextBox.AppendText(String.Format("Copying from \"{0}\" to \"{1}\"...\n\n", damnedTempDirectory, directory));

                if (!DamnedCopyFiles(damnedTempDirectory, directory))
                {
                    MessageBox.Show(String.Format("Failed to extract files and folders from {0}\n\n To:\n\n \"{1}\". Did you select the right local path for the testing branch?", publicTestPatchTestingSavedDirectory, directory), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }
            }


            else if (patch == PATCH_STABLE)
            {
                string directoryName = "Damned-master";
                string damnedTempDirectory = Path.Combine(publicTestPatchStableSavedDirectory, directoryName);

                if (!Directory.Exists(damnedTempDirectory))
                {
                    loggingTextBox.AppendText(String.Format("No existing local branch found in \"{0}\". Installing from GitHub. This will only have to happen one time.\n\n", publicTestPatchStableSavedDirectory));
                    InstallPatchFromGithub(patch);
                    return;
                }

                loggingTextBox.AppendText(String.Format("Copying from \"{0}\" to \"{1}\"...\n\n", damnedTempDirectory, directory));

                if (!DamnedCopyFiles(damnedTempDirectory, directory))
                {
                    MessageBox.Show(String.Format("Failed to extract files and folders from {0}\n\n To:\n\n \"{1}\". Did you select the right local path for the stable branch?", publicTestPatchStableSavedDirectory, directory), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }

            MessageBox.Show("Successfully installed the patch from the local directory!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Makess things look a little cleaner
        private bool DamnedCopyFiles(string source, string dest)
        {
            bool success = true;

            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(source, dest, true);
            }

            catch (IOException)
            {

                success = false;
            }


            return success;
        }

        private void InstallPatchFromGithub(int patch)
        {
            Cursor.Current = Cursors.WaitCursor;

            loggingTextBox.AppendText("Begin installing public test patch\n\n--------------------------------------------------------------------------------------------\n\n");
            tempDirectory = Path.Combine(directory, "tmp");
            string damnedTempDirectory = tempDirectory;

            if (Directory.Exists(tempDirectory))
            {
                loggingTextBox.AppendText("Deleted old temporary directory.\n\n");
                Directory.Delete(tempDirectory, true);
                Cursor.Current = Cursors.Default;
            }

            string extractedFolderName = "";

            Directory.CreateDirectory(tempDirectory);
            loggingTextBox.AppendText("Created a new temporary directory\n\n");
            Directory.SetCurrentDirectory(tempDirectory);
            loggingTextBox.AppendText("Moved into new temporary directory\n\n");

            if (patch == PATCH_STABLE)
            {
                loggingTextBox.AppendText(String.Format("Downloading the stable branch from \"{0}\" into  \"{1}\". Please wait...\n\n", DOWNLOAD_TEST_PATCH_STABLE_LINK, tempDirectory));
                DownloadPatch(DOWNLOAD_TEST_PATCH_STABLE_LINK);
                extractedFolderName = "Damned-master";
                damnedTempDirectory = Path.Combine(tempDirectory, extractedFolderName);
            }

            else if (patch == PATCH_TESTING)
            {

                loggingTextBox.AppendText(String.Format("Downloading the testing branch from \"{0}\" into  \"{1}\". Please wait...\n\n", DOWNLOAD_TEST_PATCH_TESTING_LINK, tempDirectory));
                DownloadPatch(DOWNLOAD_TEST_PATCH_TESTING_LINK);
                extractedFolderName = "Damned-testing";
                damnedTempDirectory = Path.Combine(tempDirectory, extractedFolderName);

            }

            try
            {
                ZipFile.ExtractToDirectory("DamnedPatch.zip", tempDirectory);

            }

            catch (IOException)
            {
                Directory.Delete(tempDirectory, true);
                MessageBox.Show("Failed to extract the zip file. Cleaned up what was created", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loggingTextBox.AppendText(String.Format("Deleted \"{0}\"", tempDirectory));
                Cursor.Current = Cursors.Default;
                return;

            }

            Directory.SetCurrentDirectory(directory);
            loggingTextBox.AppendText(String.Format("Copying from \"{0}\" to \"{1}\"...\n\n", damnedTempDirectory, directory));

            if (!DamnedCopyFiles(damnedTempDirectory, directory))
            {
                MessageBox.Show(String.Format("Failed to extract files and folders from {0}\n\n To:\n\n \"{1}\"", damnedTempDirectory, directory), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Directory.Delete(tempDirectory, true);
                loggingTextBox.AppendText(String.Format("Deleted \"{0}\"\n\n", tempDirectory));
                Cursor.Current = Cursors.Default;
                return;

            }

            loggingTextBox.AppendText(String.Format("Successfully installed the public test patch!\n\nDeleted \"{0}\"\n\n", tempDirectory));

            if (keepPublicTestPatchStableCheckbox.Checked && extractedFolderName == "Damned-master")
            {
                DamnedCopyFiles(tempDirectory, publicTestPatchStableSavedDirectory);
                loggingTextBox.AppendText(String.Format("Saved a local copy of the downloaded file to \"{0}\"...\n\n", publicTestPatchStableSavedDirectory));
            }

            else if (keepPublicTestPatchTestingCheckbox.Checked && extractedFolderName == "Damned-testing")
            {
                DamnedCopyFiles(tempDirectory, publicTestPatchTestingSavedDirectory);
                loggingTextBox.AppendText(String.Format("Saved a local copy of the downloaded file to \"{0}\"...\n\n", publicTestPatchStableSavedDirectory));
            }

            Directory.Delete(tempDirectory, true);
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Successfully installed the patch from GitHub!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void DownloadPatch(string link)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string zipName = "DamnedPatch.zip";
                    webClient.DownloadFile(new Uri(link), zipName);
                }

            }

            catch (WebException e)
            {
                string errorMessage = String.Format("Failed to download the patch. Do you have a valid internet connection? If so, then try downloading from this link manually:\n\n{0}\n\nError Code:\n\n{1}", link, e.ToString());
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                loggingTextBox.AppendText(errorMessage);
                FlashWindow(this.Handle, false);
            }
        }

        private void SetDamnedFolderButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() == DialogResult.OK)
            {
                directory = browser.SelectedPath;
            }


            string text = String.Format("Selected \"{0}\" as the folder to install the patches\n\n", directory);
            damnedDirectoryStringLabel.ForeColor = Color.Black;
            loggingTextBox.AppendText(text);
            DisablePatchButtonControls();
            damnedDirectoryStringLabel.Text = directory;
            buttonRestore.Enabled = false;
        }


        private void EnablePatchButtionControls()
        {
            publicTestPatchStableButton.Enabled = true;
            publicTestPatchTestingButton.Enabled = true;
            keepPublicTestPatchTestingCheckbox.Enabled = true;
            keepPublicTestPatchStableCheckbox.Enabled = true;

        }

        private void DisablePatchButtonControls()
        {
            publicTestPatchTestingButton.Enabled = false;
            publicTestPatchStableButton.Enabled = false;
            keepPublicTestPatchStableCheckbox.Enabled = false;
            keepPublicTestPatchTestingCheckbox.Enabled = false;
        }

        private void ButtonSelectBackupFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.damnedBackupFolderStringLabel.Text = dialog.SelectedPath;
                this.backupDirectory = dialog.SelectedPath;
                damnedBackupFolderStringLabel.ForeColor = Color.Black;
                buttonBackUp.Enabled = true;
                buttonOnlyCheck.Enabled = true;
                buttonRestore.Enabled = false;
                Properties.Settings.Default.damnedBackupFolderPath = dialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void KeepPublicTestPatchStableCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (keepPublicTestPatchStableCheckbox.Checked)
            {
                buttonSetPublicTestPatchStableLocation.Enabled = true;

            }

            else
            {
                buttonSetPublicTestPatchStableLocation.Enabled = false;

            }
        }

        private void KeepPublicTestPatchTestingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (keepPublicTestPatchTestingCheckbox.Checked)
            {
                buttonSetPublicTestPatchTestingLocation.Enabled = true;

            }

            else
            {
                buttonSetPublicTestPatchTestingLocation.Enabled = false;
            }
        }

        private void ButtonSetPublicTestPatchStableLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.publicTestPatchStableSavedDirectory = browser.SelectedPath;
            this.publicTestPatchStablePathLabel.Text = browser.SelectedPath;

            if (publicTestPatchStableSavedDirectory == publicTestPatchTestingSavedDirectory)
            {
                MessageBox.Show("This directory is already being for the testing branch. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                publicTestPatchStableSavedDirectory = String.Empty;
                publicTestPatchStablePathLabel.Text = String.Empty;
                return;

            }

            else if (publicTestPatchStableSavedDirectory == directory)
            {
                MessageBox.Show("This directory is used for the game folder. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                publicTestPatchStableSavedDirectory = String.Empty;
                publicTestPatchStablePathLabel.Text = String.Empty;
                return;
            }

            Properties.Settings.Default.damnedPublicTestPatchStablePath = publicTestPatchStableSavedDirectory;
            Properties.Settings.Default.Save();
        }

        private void ButtonSetPublicTestPatchTestingLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            this.publicTestPatchTestingSavedDirectory = browser.SelectedPath;
            this.publicTestPatchTestingPathLabel.Text = browser.SelectedPath;

            if (publicTestPatchTestingSavedDirectory == publicTestPatchStableSavedDirectory)
            {
                MessageBox.Show("This directory is already being for the stable branch. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                publicTestPatchTestingSavedDirectory = String.Empty;
                publicTestPatchTestingPathLabel.Text = String.Empty;
                return;
            }

            else if (publicTestPatchTestingSavedDirectory == directory)
            {
                MessageBox.Show("This directory is used for the game folder. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                publicTestPatchTestingSavedDirectory = String.Empty;
                publicTestPatchTestingPathLabel.Text = String.Empty;
                return;
            }

            Properties.Settings.Default.damnedPublicTestPatchTestingPath = publicTestPatchStableSavedDirectory;
            Properties.Settings.Default.Save();
        }

        private void ButtonBackUp_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string text = String.Format("Backing up \"{0}\" to \"{1}\"\n\n", directory, backupDirectory);
            loggingTextBox.AppendText(text);

            if (!DamnedCopyFiles(directory, backupDirectory))
            {
                MessageBox.Show("Failed to backup Damned Folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor.Current = Cursors.Default;
                return;

            }

            DamnedFiles damnedFiles = new DamnedFiles(backupDirectory); ;

            if (damnedFiles.Check())
            {
                loggingTextBox.AppendText("Looks like you have a good backup folder!\n\n");
                damnedBackupFolderStringLabel.ForeColor = Color.FromArgb(255, 138, 38);
                validBackUpFolder = true;
                buttonRestore.Enabled = true;
            }

            else
            {
                damnedBackupFolderStringLabel.ForeColor = Color.Red;
                MessageBox.Show("The folder you have selecetd is not a valid back up folder. Please select another.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor.Current = Cursors.Default;
        }


        private void ButtonRestore_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!validBackUpFolder)
            {
                MessageBox.Show("You have one or more invalid directories. Please select another", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Cursor.Current = Cursors.Default;
                return;
            }

            if (!DamnedCopyFiles(backupDirectory, directory))
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Failed to restore Damned to its original backup. Did the directories get changed by something else?\n\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DamnedFiles backupFiles = new DamnedFiles(backupDirectory);
            DamnedFiles.CleanUpNewFiles(backupFiles, damnedFiles);
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Restored the game back to its unpatched state!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonOnlyCheck_Click(object sender, EventArgs e)
        {
            DamnedFiles damnedFiles = new DamnedFiles(backupDirectory);

            if (damnedFiles.Check())
            {
                loggingTextBox.AppendText("Looks like you have a good backup folder!\n\n");
                damnedBackupFolderStringLabel.ForeColor = Color.FromArgb(255, 168, 38);
                validBackUpFolder = true;
                buttonRestore.Enabled = true;
            }

            else
            {
                damnedBackupFolderStringLabel.ForeColor = Color.Red;
                buttonRestore.Enabled = false;
            }
        }


        private void LoadSettings()
        {
            string setting = String.Empty;

            setting = Properties.Settings.Default.damnedBackupFolderPath;

            if (setting != String.Empty)
            {
                damnedBackupFolderStringLabel.Text = Properties.Settings.Default.damnedBackupFolderPath;
                buttonOnlyCheck.Enabled = true;
                buttonBackUp.Enabled = true;
                backupDirectory = setting;

                if (damnedFiles.Check())
                {
                    damnedBackupFolderStringLabel.ForeColor = Color.FromArgb(255, 168, 38);
                    buttonRestore.Enabled = true;
                    validBackUpFolder = true;
                }
            }

            setting = Properties.Settings.Default.damnedPublicTestPatchStablePath;

            if (setting != String.Empty)
            {
                publicTestPatchStableSavedDirectory = setting;
                keepPublicTestPatchStableCheckbox.Checked = true;
                buttonSetPublicTestPatchStableLocation.Enabled = true;
                publicTestPatchStablePathLabel.Text = setting;
            }

            setting = Properties.Settings.Default.damnedPublicTestPatchTestingPath;

            if (setting != String.Empty)
            {
                publicTestPatchStableSavedDirectory = setting;
                keepPublicTestPatchTestingCheckbox.Checked = true;
                buttonSetPublicTestPatchTestingLocation.Enabled = true;
                publicTestPatchTestingPathLabel.Text = setting;
            }
        }

        private void SetButtons()
        {
            buttonBackUp.MouseEnter += OnMouseEnterButton;
            buttonBackUp.MouseLeave += OnMouseLeaveButton;
            buttonSelectBackupFolder.MouseEnter += OnMouseEnterButton;
            buttonSelectBackupFolder.MouseLeave += OnMouseLeaveButton;
            buttonSetPublicTestPatchStableLocation.MouseEnter += OnMouseEnterButton;
            buttonSetPublicTestPatchStableLocation.MouseLeave += OnMouseLeaveButton;
            buttonSetPublicTestPatchTestingLocation.MouseEnter += OnMouseEnterButton;
            buttonSetPublicTestPatchTestingLocation.MouseLeave += OnMouseLeaveButton;
            publicTestPatchStableButton.MouseEnter += OnMouseEnterButton;
            publicTestPatchStableButton.MouseLeave += OnMouseLeaveButton;
            publicTestPatchTestingButton.MouseEnter += OnMouseEnterButton;
            publicTestPatchTestingButton.MouseLeave += OnMouseLeaveButton;
            buttonRestore.MouseEnter += OnMouseEnterButton;
            buttonRestore.MouseLeave += OnMouseLeaveButton;
            buttonOnlyCheck.MouseEnter += OnMouseEnterButton;
            buttonOnlyCheck.MouseLeave += OnMouseLeaveButton;

        }
        private void OnMouseEnterButton(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.ForeColor = Color.FromArgb(255, 168, 38);
        }

        private void OnMouseLeaveButton(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.ForeColor = Color.White;
        }

        private void DamnedPatcherForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Enabled = true;
            mainForm.Show();
        }
    }
}
