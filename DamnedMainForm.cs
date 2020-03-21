using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DamnedWorkshop
{
    public partial class DamnedMainForm : Form
    {
        private DamnedFiles damnedFiles;
        private string directory = @"C:\Program Files (x86)\Steam\steamapps\common\Damned";

        private string GetSteamPath()
        {
            RegistryView registryView = RegistryView.Registry64;
            if (!Environment.Is64BitOperatingSystem)
            {
                registryView = RegistryView.Registry32;
            }

            using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey regKey = hklm.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 251170");

                if (regKey != null)
                {
                    object val = regKey.GetValue("InstallLocation");

                    if (val != null)
                    {
                        return Path.Combine(val.ToString());
                    }
                }
            }

            return null;
        }

        public DamnedMainForm()
        {
            InitializeComponent();
            string installLocation = GetSteamPath();

            if (installLocation != null)
            {
                directory = installLocation;
            }
            txtInstallLocation.Text = directory;
            LoadSettings();
        }

        private void ButtonPatcherForm_Click(object sender, EventArgs e)
        {
            DamnedPatcherForm form = new DamnedPatcherForm(damnedFiles, this);
            this.Enabled = false;
            this.Hide();
            form.Show();
        }

        private void ButtonMappingForm_Click(object sender, EventArgs e)
        {
            damnedFiles.Refresh();
            DamnedMappingForm form = new DamnedMappingForm(damnedFiles.DamnedMaps, damnedFiles.DamnedImages, this);
            this.Enabled = false;
            this.Hide();
            form.Show();
        }

        private void ButtonSelectDamnedDirectory_Click(object sender, EventArgs e)
        {

        }

        private void EnableControls()
        {
            buttonMappingForm.Enabled = true;
            buttonPatcherForm.Enabled = true;
            browseStagesButton.Enabled = true;
        }

        private void DisableControls()
        {
            buttonMappingForm.Enabled = false;
            buttonPatcherForm.Enabled = false;
            browseStagesButton.Enabled = false;
        }

        private void ButtonCheckPath_Click(object sender, EventArgs e)
        {

        }

        private void LoadSettings()
        {
            string setting = Properties.Settings.Default.damnedGamePath;
            txtInstallLocation.Text = directory;

            if (setting != String.Empty)
            {
                txtInstallLocation.Text = setting;
                directory = setting;

                try
                {
                    damnedFiles = new DamnedFiles(directory);
                }
                catch (IOException)
                {
                    ResetSettings();
                    string message = String.Format("The directory \"{0}\" seems to no longer exist. Your settings have been reset.", setting);
                    MessageBox.Show(message, "Directory No Longer Exists", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtInstallLocation.Text = "Your Damned directory path will appear here.";
                    directory = String.Empty;
                    return;
                }

                if (damnedFiles.Check())
                {
                    lblDamnedValid.ForeColor = Color.LimeGreen;
                    lblDamnedValid.Text = $"Check successful! The specified directory is a valid Damned location!";
                    lblDamnedValid.Visible = true;
                    txtInstallLocation.ForeColor = Color.FromArgb(255, 168, 38);
                    EnableControls();
                }
            }
        }

        private void ResetSettings()
        {
            Properties.Settings.Default.damnedGamePath = String.Empty;
            Properties.Settings.Default.damnedBackupFolderPath = String.Empty;
            Properties.Settings.Default.damnedPublicTestPatchStablePath = String.Empty;
            Properties.Settings.Default.damnedPublicTestPatchTestingPath = String.Empty;
            Properties.Settings.Default.Save();
        }

        private void SetButtons()
        {
            btnCheck.MouseEnter += OnMouseEnterButton;
            btnCheck.MouseLeave += OnMouseLeaveButton;
            buttonMappingForm.MouseEnter += OnMouseEnterButton;
            buttonMappingForm.MouseLeave += OnMouseLeaveButton;
            buttonPatcherForm.MouseEnter += OnMouseEnterButton;
            buttonPatcherForm.MouseLeave += OnMouseLeaveButton;
            btnBrowse.MouseEnter += OnMouseEnterButton;
            btnBrowse.MouseLeave += OnMouseLeaveButton;
            browseStagesButton.MouseEnter += OnMouseEnterButton;
            browseStagesButton.MouseLeave += OnMouseLeaveButton;
        }

        private void OnMouseEnterButton(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.ForeColor = Color.FromArgb(255, 168, 38);
        }

        private void OnMouseLeaveButton(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.ForeColor = Color.Black;
        }

        private void DamnedMainForm_Load(object sender, EventArgs e)
        {
            SetButtons();
        }

        private void BrowseStagesButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Hide();
            damnedFiles.Refresh();
            DamnedCommunityStagesForm form = new DamnedCommunityStagesForm(this, damnedFiles);
            form.Show();
        }

        private void DamnedWelcomeTextbox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                directory = dialog.SelectedPath;
            }
            else
            {
                return;
            }

            txtInstallLocation.Text = directory;
            DisableControls();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (directory == String.Empty)
            {
                MessageBox.Show("You did not select a directory", "No directory selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                damnedFiles = new DamnedFiles(directory);
            }
            catch (IOException)
            {
                MessageBox.Show("This directory does not exist. Either the location in the registory for Damned does not exist on your system, or the directory that you selected was moved or deleted by something else. Please select a new directory where Damned is installed.", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                directory = string.Empty;
                txtInstallLocation.Text = "";
                lblDamnedValid.ForeColor = Color.Red;
                lblDamnedValid.Text = $"Check failed! The specified directory is not a valid Damned location!";
                lblDamnedValid.Visible = true;
                return;
            }

            if (damnedFiles.Check())
            {
                txtInstallLocation.ForeColor = Color.FromArgb(255, 168, 38);
                lblDamnedValid.ForeColor = Color.LimeGreen;
                lblDamnedValid.Text = $"Check successful! The specified directory is a valid Damned location!";
                lblDamnedValid.Visible = true;
                EnableControls();
                Properties.Settings.Default.damnedGamePath = directory;
                Properties.Settings.Default.Save();
                damnedFiles.Load();
            }
            else
            {
                lblDamnedValid.ForeColor = Color.Red;
                lblDamnedValid.Text = $"Check failed! The specified directory is not a valid Damned location!";
                lblDamnedValid.Visible = true;
                txtInstallLocation.ForeColor = Color.Red;
                DisableControls();
            }
        }

        private void txtInstallLocation_TextChanged(object sender, EventArgs e)
        {
            directory = txtInstallLocation.Text;
        }
    }
}