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

namespace DamnedWorkshop
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern bool FlashWindow(IntPtr hwnd, bool bInvert);



        private static string DOWNLOAD_TEST_PATCH_STABLE_LINK =  "https://github.com/Sweats/Damned/archive/master.zip";
        private static string DOWNLOAD_TEST_PATCH_TESTING_LINK = "https://github.com/Sweats/Damned/archive/testing.zip";
        private static string GITHUB_LINK = "TO BE DETERMINED";

        private static int PATCH_TESTING = 0;
        private static int PATCH_STABLE = 1;


        private static string TOOLTIP_TEST_PATCH_STABLE_TEXT = "Downloads and installs the latest public test patch from the stable banch from " + DOWNLOAD_TEST_PATCH_STABLE_LINK + " \n\nFiles will be downloaded into a temporary directory then extracted to where you set the directory to. After that, the temporary directory will be removed.";
        private static string TOOLTIP_TEST_PATCH_TESTING_TEXT = "Downloads and installs the latest public test patch from the testing banch from " + DOWNLOAD_TEST_PATCH_TESTING_LINK + ". \n\nFiles will be downloaded into a temporary directory then extracted to where you set the directory to. After that, the temporary directory will be removed.";
        private static string TOOLTIP_SET_DAMNED_FOLDER_TEXT = "Opens up the file explorer where you can select a location where Damned is installed.";
        private static string TOOLTIP_CHECK_BUTTON_TEXT = "Checks the listed directory to see if the location that you picked is a valid Damend directory.\n\nIf the result is red, it means it failed. If the result is green, it mean it was successful";

        private string directory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Damned";
        private string tempDirectory = "";
        private string backupDirectory = "";
        private string publicTestPatchTestingSavedDirectory = "";
        private string publicTestPatchStableSavedDirectory = "";

        private bool validBackUpFolder = false;

        private DamnedFiles damnedFiles;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: Figure out how to do settings so the user does not have to re do everything every time.
            this.damnedDirectoryStringLabel.Text = directory;
            this.damnedBackupFolderStringLabel.Text = backupDirectory;
            this.publicTestPatchTestingPathLabel.Text = this.publicTestPatchTestingSavedDirectory;
            this.publicTestPatchStablePathLabel.Text = this.publicTestPatchStableSavedDirectory;
            
            toolTipPublicTestPatchTesting.SetToolTip(publicTestPatchTestingButton, TOOLTIP_TEST_PATCH_TESTING_TEXT);
            toolTipPublicTestPatchStable.SetToolTip(publicTestPatchStableButton, TOOLTIP_TEST_PATCH_STABLE_TEXT);
            toolTipSetDamnedFolder.SetToolTip(setDamnedFolderButton, TOOLTIP_SET_DAMNED_FOLDER_TEXT);
            toolTipCheckButton.SetToolTip(checkPathButton, TOOLTIP_CHECK_BUTTON_TEXT);
            loggingTextBox.AppendText(String.Format("Welcome\n\nBefore using this tool, please check its github at {0}\n\nDamned directory has been set to \"{1}\". If you have installed Damned in a non traditonal location, you will have to change it. For more information on what the buttons do, hover your mouse over them.\n\n", GITHUB_LINK, directory));

        }

        private void LoadKeepButtons()
        {
        }


        private void HandleControls()
        {

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

            Cursor.Current = Cursors.WaitCursor;
            Application.UseWaitCursor = true;

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

            Application.UseWaitCursor = false;

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
            loggingTextBox.AppendText("Begin installing public test patch\n\n--------------------------------------------------------------------------------------------\n\n");
            tempDirectory = Path.Combine(directory, "tmp");
            string damnedTempDirectory = tempDirectory;

            if (Directory.Exists(tempDirectory))
            {
                loggingTextBox.AppendText("Deleted old temporary directory.\n\n");
                Directory.Delete(tempDirectory, true);
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
                return;

            }

            Directory.SetCurrentDirectory(directory);
            loggingTextBox.AppendText(String.Format("Copying from \"{0}\" to \"{1}\"...\n\n", damnedTempDirectory, directory));

            if (!DamnedCopyFiles(damnedTempDirectory, directory))
            {
                MessageBox.Show(String.Format("Failed to extract files and folders from {0}\n\n To:\n\n \"{1}\"", damnedTempDirectory, directory), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Directory.Delete(tempDirectory, true);
                loggingTextBox.AppendText(String.Format("Deleted \"{0}\"\n\n", tempDirectory));
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
            MessageBox.Show("Successfully installed the patch from GitHub!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void DownloadPatch(string link)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string zipName = "DamnedPatch.zip";
                    webClient.DownloadFile(link, zipName);
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

        private void ButtonCheckPath_Click(object sender, EventArgs e)
        {
            DamnedFiles gameFiles = new DamnedFiles(directory);

            if (gameFiles.Check())
            {

                EnablePatchButtionControls();
                damnedDirectoryStringLabel.Text = directory;
                damnedDirectoryStringLabel.ForeColor = Color.Green;
                loggingTextBox.AppendText("Successfully checked your directory. This seems to be the correct Damned folder.\n\n");
            }

            else
            {
                DisablePatchButtonControls();
                damnedDirectoryStringLabel.Text = directory;
                damnedDirectoryStringLabel.ForeColor = Color.Red;
                loggingTextBox.AppendText(String.Format("Directory \"{0}\" is not a vaild directory. Either you picked the wrong directory or you have missing game files.\n\n", directory));
            }

            damnedFiles = gameFiles;

        }

        

        private void ButtonSelectBackupFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.damnedBackupFolderStringLabel.Text = dialog.SelectedPath;
                this.backupDirectory = dialog.SelectedPath;
                buttonBackUp.Enabled = true;
                buttonOnlyCheck.Enabled = true;
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


        private void HandleControlsAfterLoadingSettings()
        {

        }

        private void ButtonSetPublicTestPatchStableLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() == DialogResult.OK)
            {
                this.publicTestPatchStableSavedDirectory = browser.SelectedPath;
                this.publicTestPatchStablePathLabel.Text = browser.SelectedPath;

                if (publicTestPatchStableSavedDirectory == publicTestPatchTestingSavedDirectory)
                {
                    MessageBox.Show("This directory is already being for the testing branch. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    publicTestPatchStableSavedDirectory = "";
                    publicTestPatchStablePathLabel.Text = "";
                }

                else if (publicTestPatchStableSavedDirectory == directory)
                {
                    MessageBox.Show("This directory is used for the game folder. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    publicTestPatchStableSavedDirectory = "";
                    publicTestPatchStablePathLabel.Text = "";

                }
            }

        }

        private void ButtonSetPublicTestPatchTestingLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();

            if (browser.ShowDialog() == DialogResult.OK)
            {
                this.publicTestPatchTestingSavedDirectory = browser.SelectedPath;
                this.publicTestPatchTestingPathLabel.Text = browser.SelectedPath;

                if (publicTestPatchTestingSavedDirectory == publicTestPatchStableSavedDirectory)
                {

                    MessageBox.Show("This directory is already being for the stable branch. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    publicTestPatchTestingSavedDirectory = "";
                    publicTestPatchTestingPathLabel.Text = "";
                }


                else if (publicTestPatchTestingSavedDirectory == directory)
                {
                    MessageBox.Show("This directory is used for the game folder. Please pick another.", "Already being used", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    publicTestPatchTestingSavedDirectory = "";
                    publicTestPatchTestingPathLabel.Text = "";
                }
            }

        }

        private void ButtonBackUp_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;
            string text = String.Format("Backing up \"{0}\" to \"{1}\"\n\n", directory, backupDirectory);
            loggingTextBox.AppendText(text);

            if (!DamnedCopyFiles(directory, backupDirectory))
            {
                MessageBox.Show("Failed to backup Damned Folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            Application.UseWaitCursor = false;
            DamnedFiles damnedFiles = new DamnedFiles(backupDirectory);

            if (damnedFiles.Check())
            {
                loggingTextBox.AppendText("Looks like you have a good backup folder!\n\n");
                damnedBackupFolderStringLabel.ForeColor = Color.Green;
                validBackUpFolder = true;
                buttonRestore.Enabled = true;
            }

            else
            {
                damnedBackupFolderStringLabel.ForeColor = Color.Red;
            }
        }


        private void ButtonRestore_Click(object sender, EventArgs e)
        {
            Application.UseWaitCursor = true;

            if (!DamnedCopyFiles(backupDirectory, directory))
            {
                MessageBox.Show("Failed to restore Damned to its original backup. Did the directories get changed by something else?\n\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            DamnedFiles backupFiles = new DamnedFiles(backupDirectory);
            DamnedFiles.CleanUpNewFiles(backupFiles, damnedFiles);
            Application.UseWaitCursor = false;
            MessageBox.Show("Restored the game back to its unpatched state!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonOnlyCheck_Click(object sender, EventArgs e)
        {
            DamnedFiles damnedFiles = new DamnedFiles(backupDirectory);

            if (damnedFiles.Check())
            {
                loggingTextBox.AppendText("Looks like you have a good backup folder!\n\n");
                damnedBackupFolderStringLabel.ForeColor = Color.Green;
                validBackUpFolder = true;
                buttonRestore.Enabled = true;
            }

            else
            {
                damnedBackupFolderStringLabel.ForeColor = Color.Red;
            }
        }
    }
}
