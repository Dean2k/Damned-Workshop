﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace DamnedWorkshop
{

    public partial class DamnedMappingForm : Form
    {
        private List<DamnedNewStage> damnedNewStagesList;
        private DamnedNewStage damnedNewStage;
        private DamnedRemoveStage damnedRemoveStage;
        private List<DamnedRemoveStage> damnedRemoveStagesList;
        private bool changesMade;

        private DamnedMaps damnedMaps;
        private DamnedImages damnedImages;

        public DamnedMappingForm(DamnedMaps damnedMaps, DamnedImages damnedImages)
        {
            InitializeComponent();
            this.damnedMaps = damnedMaps;
            this.damnedImages = damnedImages;
            damnedRemoveStage = new DamnedRemoveStage();
            damnedNewStage = new DamnedNewStage();
            damnedNewStagesList = new List<DamnedNewStage>();
            damnedRemoveStagesList = new List<DamnedRemoveStage>();
            RefreshDamnedStagesList();

        }

        private void ButtonAddMapIntoDamned_Click(object sender, EventArgs e)
        {
            SelectStage(false);
        }
        private void ButtonRemoveMap_Click(object sender, EventArgs e)
        {
            SelectStage(true);

        }

    
        private void ButtonSelectMapLoadingScreen_Click(object sender, EventArgs e)
        {
            string loadingImage = String.Empty;

            FileDialog dialog = new OpenFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg"
            };

            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            loadingImage = dialog.FileName;

            if (Path.GetExtension(loadingImage) != ".jpg")
            {
                MessageBox.Show("You did not pick a jpg file. Please select one that is a .jpg file", "Please select a different file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (damnedNewStage.loadingImagePath == String.Empty)
            {
                damnedNewStage.count++;
            }

            damnedNewStage.loadingImagePath = loadingImage;
            buttonModifyStages.Enabled = true;
            labelLoadingScreenImage.Text = Path.GetFileName(damnedNewStage.loadingImagePath);
            labelLoadingScreenImage.ForeColor = Color.Green;
            buttonAddStageToList.Enabled = true;
            changesMade = true;

        }


        private void ButtonSelectLobbyButtonPicture_Click(object sender, EventArgs e)
        {
            string buttonImage = String.Empty;

            FileDialog dialog = new OpenFileDialog
            {
                Filter = ("PNG Files (*.png)|*.png")
            };

            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            buttonImage = dialog.FileName;

            if (Path.GetExtension(buttonImage) != ".png")
            {
                MessageBox.Show("You did not pick a png file. Please select one that is a .png file", "Please select a different file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (damnedNewStage.lobbyImageButtonPath == String.Empty)
            {
                damnedNewStage.count++;
            }

            damnedNewStage.lobbyImageButtonPath = buttonImage;
            labelLobbyButtonPicture.Text = Path.GetFileName(damnedNewStage.lobbyImageButtonPath);
            labelLobbyButtonPicture.ForeColor = Color.Green;
            changesMade = true;
            buttonSelectHighlightedLobbyButtons.Enabled = true;
            pictureDamnedButtonLobbyPicture.ImageLocation = buttonImage;
        }

        private void RefreshDamnedStagesList(bool afterRemoving = false)
        {
            damnedStagesTextBox.Clear();
            List<string> sortedStages = new List<string>(damnedMaps.stages);
            List<string> newSortedStages = new List<string>(damnedNewStagesList.Count);
            List<string> stagesToBeRemoved = new List<string>(damnedRemoveStagesList.Count);

            for (int i = 0; i < sortedStages.Count; i++)
            {
                string stage = Path.GetFileNameWithoutExtension(sortedStages[i]);
                string modifiedStageName = stage.Replace("_", " ");
                sortedStages[i] = modifiedStageName;
            }

            for (int i = 0; i < damnedNewStagesList.Count; i++)
            {
                string stage = Path.GetFileNameWithoutExtension(damnedNewStagesList[i].newStagePath);
                string modifiedStageName = stage.Replace("_", " ");
                sortedStages.Add(modifiedStageName);
                newSortedStages.Add(modifiedStageName);
            }

            for (int i = 0; i < damnedRemoveStagesList.Count; i++)
            {
                string stage = Path.GetFileNameWithoutExtension(damnedRemoveStagesList[i].stagePath);
                string modifiedStageName = stage.Replace("_", " ");
                stagesToBeRemoved.Add(modifiedStageName);
            }

            sortedStages.Sort();

            for (int i = 0; i < sortedStages.Count; i++)
            {
                damnedStagesTextBox.AppendText(String.Format("{0}\n", sortedStages[i]));
            }

            MarkStages(newSortedStages, Color.Green);

            if (!afterRemoving)
            {
                MarkStages(stagesToBeRemoved, Color.Red);
            }
        }

        private void MarkStages(List<string> stagesToBeMarked, Color color)
        {
            for (int i = 0; i < stagesToBeMarked.Count; i++)
            {
                MarkStage(stagesToBeMarked[i], color);
            }
        }

        private void MarkStage(string stage, Color color)
        {
            int index = damnedStagesTextBox.Find(stage);
            damnedStagesTextBox.Select(index, stage.Length);
            damnedStagesTextBox.SelectionColor = color;
        }

 

        private void SelectStage(bool remove)
        {
            FileDialog dialog = new OpenFileDialog();

            if (remove)
            {
                dialog.InitialDirectory = damnedMaps.stagesAndScenesDirectory;
            }

            dialog.Filter = "Stage Files (*.stage)|*.stage";
            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            string stagePath = dialog.FileName;
            string stage = Path.GetFileName(stagePath);
        
            if (Path.GetExtension(stage) != ".stage")
            {
                MessageBox.Show("You did not pick a stage file. Please select one that is a .stage file", "Please select a different file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (stage.Contains("menu_background"))
            {
                MessageBox.Show("Either you picked the stage that is for the main menu or you decided to name your stage along the lines of \"menu_background\". Either pick a different one or rename your stage.", "Pick a different stage", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (remove)
            {
                if (Path.GetFileName(Path.GetDirectoryName(stagePath)) != "Stages")
                {
                    MessageBox.Show("It seems that the stage that you have selected does not reside inside the Damned directory. Please select a different stage", "Please select a different file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                damnedRemoveStage.stagePath = stagePath;
                string newLabelText = Path.GetFileNameWithoutExtension(stage).Replace("_", " ");
                labelMapToRemove.Text = newLabelText;
                labelMapToRemove.ForeColor = Color.Green;
                buttonSelectSceneToRemove.Enabled = true;
                changesMade = true;

            }

            else
            {
                if (damnedMaps.StageExists(stage))
                {
                    MessageBox.Show("This stage is already installed in the game. Please pick another.", "Stage Already Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (damnedNewStage.newStagePath == String.Empty)
                {
                    damnedNewStage.count++;
                }

                damnedNewStage.newStagePath = stagePath;
                string newLabelText = stage.Replace("_", " ").Remove(stage.IndexOf("."), 6);
                labelMapToAdd.Text = newLabelText;
                labelMapToAdd.ForeColor = Color.Green;
                buttonSelectSceneFile.Enabled = true;
                changesMade = true;

            }
        }

        private void SelectScene(bool remove)
        {
            FileDialog dialog = new OpenFileDialog
            {
                Filter = "Scene Files(*.scene)|*.scene"
            };

            if (remove)
            {
                dialog.InitialDirectory = damnedMaps.stagesAndScenesDirectory;

            }

            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            string scenePath = dialog.FileName;
            string sceneName = Path.GetFileName(scenePath);

            if (Path.GetExtension(scenePath) != ".scene")
            {
                MessageBox.Show("You did not pick a .scene file. Please select one that is a .scene file", "Please select a different file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sceneName.Contains("menu_background"))
            {
                MessageBox.Show("Either you picked the scene that is for the main menu or you decided to name your scene along the lines of \"menu_background\". Either pick a different one or rename your scene.", "Pick a different scene", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            if (remove)
            {
                sceneName = sceneName.Replace("_", " ").Remove(sceneName.IndexOf("."), 6);

                if (sceneName != labelMapToRemove.Text)
                {
                    MessageBox.Show("It looks like you seleted a scene file that does not go with the stage file you selected already. Please pick the right one", "Wrong Scene File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                damnedRemoveStage.scenePath = scenePath;
                damnedRemoveStagesList.Add(new DamnedRemoveStage(damnedRemoveStage));
                buttonSelectSceneToRemove.Enabled = false;
                labelSelectSceneFileToRemove.Text = "Choose another scene file to remove with a stage:";
                labelMapToRemove.Text = "Choose another stage file to remove from the game.";
                labelMapToRemove.ForeColor = Color.Black;
                buttonModifyStages.Enabled = true;
                string stageToMark = Path.GetFileNameWithoutExtension(damnedRemoveStage.stagePath).Replace("_", " ");
                MarkStage(stageToMark, Color.Red);
            }

            else
            {

                if (damnedNewStage.newScenePath == String.Empty)
                {
                    damnedNewStage.count++;
                }

                damnedNewStage.newScenePath = scenePath;
                labelScene.Text = sceneName;
                labelScene.ForeColor = Color.Green;
                changesMade = true;
                buttonSelectLobbyButtonPicture.Enabled = true;
            }
        }

        private void ModifyStages()
        {
            string terrorImagesZipFile = damnedImages.terrorZipFile;
            string tempDirectory = Path.GetTempPath();
            tempDirectory = Path.Combine(tempDirectory, "Terror");

            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }

            Directory.CreateDirectory(tempDirectory);
            ZipFile.ExtractToDirectory(terrorImagesZipFile, tempDirectory);
            string layoutFilePath = damnedImages.GetLayoutFileFromZip(tempDirectory);

            if (!File.Exists(layoutFilePath))
            {
                MessageBox.Show("Failed to extract the zip file into the users temporary directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            damnedImages.UpdateXmlFiles(layoutFilePath, damnedRemoveStagesList.ToArray(), damnedNewStagesList.ToArray());

            string destination = Path.Combine(damnedImages.guiDirectory, "Terror.zip");
            File.Delete(destination);
            ZipFile.CreateFromDirectory(tempDirectory, destination);
            Directory.Delete(tempDirectory, true);
            Reset();
            MessageBox.Show("Successfully modified the stages in the game.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void ButtonResetPendingChanges_Click(object sender, EventArgs e)
        {
            if (changesMade)
            {
                Reset();
            }
        }

        private void Reset()
        {
            damnedNewStagesList.Clear();
            damnedRemoveStagesList.Clear();
            damnedStagesTextBox.Clear();
            damnedNewStage.Clear();
            damnedRemoveStage.Clear();
            labelMapToRemove.Text = "Choose a stage to remove:";
            labelMapToRemove.ForeColor = Color.Black;
            labelMapToAdd.Text = "Choose a new stage to add:";
            labelMapToAdd.ForeColor = Color.Black;
            labelLoadingScreenImage.Text = "Choose a loading screen picture:";
            labelLoadingScreenImage.ForeColor = Color.Black;
            labelLobbyButtonPicture.Text = "Choose a lobby button picture:";
            labelLobbyButtonPicture.ForeColor = Color.Black;
            labelSelectedHighlightedButton.ForeColor = Color.Black;
            labelSelectedHighlightedButton.Text = "Choose an enabled, highlighted, and disabled lobby button picture:";
            RefreshDamnedStagesList();
            changesMade = false;
            buttonSelectLobbyButtonPicture.Enabled = false;
            buttonSelectMapLoadingScreen.Enabled = false;
            buttonAddStageToList.Enabled = false;
            buttonModifyStages.Enabled = false;
            buttonSelectHighlightedLobbyButtons.Enabled = false;
            buttonSelectSceneFile.Enabled = false;
            pictureDamnedButtonLobbyPicture.Image = Properties.Resources.lobbyButtonImageExample;
            pictureLobbyButtonHighlightedExample.Image = Properties.Resources.example_lobbyButtonImage;
            labelScene.Text = "Choose a scene file that goes with your stage";
            labelScene.ForeColor = Color.Black;
            labelSelectSceneFileToRemove.Text = "Choose a scene file to remove with the stage:";
            labelSelectSceneFileToRemove.ForeColor = Color.Black;
        }

        private void ButtonModifyStages_Click(object sender, EventArgs e)
        {
            if (damnedNewStage.count > 0)
            {
                if (damnedNewStage.count != 5)
                {
                    MessageBox.Show("You did not select a loading screen image or a lobby button image for your new stages(s). Finish selecting those first before adding them into the game.", "Finish Selecting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            ModifyStages();
        }

        private void ButtonAddStageToList_Click(object sender, EventArgs e)
        {
            damnedNewStagesList.Add(new DamnedNewStage(damnedNewStage));
            RefreshDamnedStagesList();
            damnedNewStage.Clear();
            buttonSelectLobbyButtonPicture.Enabled = false;
            buttonSelectMapLoadingScreen.Enabled = false;
            buttonAddStageToList.Enabled = false;
            buttonSelectHighlightedLobbyButtons.Enabled = false;
            pictureDamnedButtonLobbyPicture.Image = Properties.Resources.lobbyButtonImageExample;
            pictureLobbyButtonHighlightedExample.Image = Properties.Resources.example_lobbyButtonImage;
            labelMapToAdd.Text = "Choose another stages to add:";
            labelMapToAdd.ForeColor = Color.Black;
            labelLoadingScreenImage.Text = "Choose another loading screen picture:";
            labelLoadingScreenImage.ForeColor = Color.Black;
            labelLobbyButtonPicture.Text = "Choose another lobby button picture:";
            labelLobbyButtonPicture.ForeColor = Color.Black;
            labelSelectedHighlightedButton.Text = "Choose another enabled, highlighted, and disabled lobby button picture:";
            labelSelectedHighlightedButton.ForeColor = Color.Black;
            labelScene.Text = "Choose another scene file that goes with your stages:";
            labelScene.ForeColor = Color.Black;

        }

        private void ButtonSelectHighlightedLobbyButtons_Click(object sender, EventArgs e)
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = ("PNG Files (*.png)|*.png");

            DialogResult result = dialog.ShowDialog();

            if (result != DialogResult.OK)
            {
                return;
            }

            if (damnedNewStage.lobbyImageButtonHighlightedPath == String.Empty)
            {
                damnedNewStage.count++;
            }

            damnedNewStage.lobbyImageButtonHighlightedPath = dialog.FileName;
            pictureLobbyButtonHighlightedExample.ImageLocation = dialog.FileName;
            labelSelectedHighlightedButton.Text = Path.GetFileName(dialog.FileName);
            labelSelectedHighlightedButton.ForeColor = Color.Green;
            changesMade = true;
            buttonSelectMapLoadingScreen.Enabled = true;
        }

        private void ButtonSelectSceneFile_Click(object sender, EventArgs e)
        {
            SelectScene(false);

        }

        private void ButtonSelectSceneToRemove_Click(object sender, EventArgs e)
        {
            SelectScene(true);
        }
    }
}
