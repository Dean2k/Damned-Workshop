using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.IO.Compression;

namespace DamnedWorkshop
{
    public partial class DamnedCommunityStagesForm : Form
    {
        private DamnedMainForm form;
        private DamnedMaps damnedStages;
        private DamnedFiles damnedFiles;
        private const string JSON_FILE_NAME = "CommunityStages.json";

        private List<DamnedNewStage> newStagesList;
        private List<DamnedRemoveStage> removeStagesList;
        private DataGridViewSelectedRowCollection selectedRows;

        private const int COLUMN_NAME = 0;
        private const int COLUMN_INSTALLED = 4;


        public DamnedCommunityStagesForm(DamnedMainForm form, DamnedFiles files)
        {
            this.form = form;
            damnedFiles = files;
            damnedStages = damnedFiles.DamnedMaps;
            InitializeComponent();
        }

        private void DamnedCommunityStagesForm_Load(object sender, EventArgs e)
        {
            SetButtons();
            damnedDataView.EnableHeadersVisualStyles = false;
            newStagesList = new List<DamnedNewStage>();
            removeStagesList = new List<DamnedRemoveStage>();
        }

        private void DamnedCommunityStagesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Enabled = true;
            form.Show();
            DamnedFiles.DeleteWorkshopTempDirectories();
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            this.Close();
            form.Enabled = true;
            form.Show();
            DamnedFiles.DeleteWorkshopTempDirectories();
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            RefreshDataView();
            labelNote.Text = @"Select the stages on the left by left clicking. 

To select multiple stages, hold down control while left clicking.

You can also hold down left click and drag your mouse up or down to select multiple stages.

If the selected stage is installed when clicking on ""Modify Stages"", it will be removed from the game.

If the selected stage is not installed when clicking on ""Modify Stages"", it will be added into the game.";
        }


        private void SetButtons()
        {
            buttonModifyStages.MouseEnter += OnMouseEnterButton;
            buttonModifyStages.MouseLeave += OnMouseLeaveButton;
            buttonBack.MouseEnter += OnMouseEnterButton;
            buttonBack.MouseLeave += OnMouseLeaveButton;
            buttonRefresh.MouseEnter += OnMouseEnterButton;
            buttonRefresh.MouseLeave += OnMouseLeaveButton;
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

        private void PrepareToModifyStages()
        {
            newStagesList.Clear();
            removeStagesList.Clear();

            selectedRows = damnedDataView.SelectedRows;

            for (int i = 0; i < selectedRows.Count; i++)
            {
                var cellsInRow = selectedRows[i].Cells;

                for (int j = 0; j < cellsInRow.Count; j++)
                {
                    var cell = cellsInRow[j];

                    if (cell.ColumnIndex != COLUMN_INSTALLED)
                    {
                        continue;
                    }

                    string cellValue = cell.Value.ToString();

                    int result = String.Compare(cellValue, "yes", true);

                    if (result != 0)
                    {
                        string githubLink = "https://github.com/Sweats/Damned-Community-Stages/raw/master/";

                        string archiveToDownload = $"{cellsInRow[COLUMN_NAME].Value.ToString()}.zip";
                        string downloadLink = $"{githubLink}{archiveToDownload}".Replace(" ", "%20");
                        string workshopTempPath = DamnedFiles.CreateTempWorkshopDirectory();
                        string archiveLocation = Path.Combine(workshopTempPath, archiveToDownload);

                        if (!DamnedFiles.DownloadFile(downloadLink, archiveLocation))
                        {
                            MessageBox.Show($"Failed to download the stage archive {archiveToDownload} from {downloadLink}. Do you have a valid internet connection? ", "Failed To Download", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Directory.Delete(workshopTempPath, true);
                            return;
                        }

                        DamnedPackage package = new DamnedPackage();

                        if (!package.Check(archiveLocation))
                        {
                            string reason = $"Failed to prepare the stage archive {archiveToDownload} for installation:\n\n{package.ReasonForFailedCheck}";
                            MessageBox.Show(reason, "Failed to prepare the stage archive.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Directory.Delete(workshopTempPath, true);
                            return;
                        }

                        package.Load();

                        DamnedNewStage newStage = new DamnedNewStage()
                        {
                            LoadingImagePath = package.LoadingImagePath,
                            LobbyImageButtonHighlightedPath = package.LobbyButtonImageHighlightedPath,
                            LobbyImageButtonPath = package.LobbyButtonImagePath,
                            NewScenePath = package.ScenePath,
                            NewStagePath = package.StagePath,
                            NewObjectsPath = package.objectsPath,
                            HasObjects = package.HasObjects
                        };

                        newStagesList.Add(newStage);
                    }


                    else
                    {
                        string stageToFind = $"{cellsInRow[COLUMN_NAME].Value.ToString()}.stage".Replace(" ", "_");
                        string sceneToFind = $"{cellsInRow[COLUMN_NAME].Value.ToString()}.scene".Replace(" ", "_");
                        DamnedRemoveStage removeStage = new DamnedRemoveStage();
                        string pathToStage = Path.Combine(damnedStages.StagesAndScenesDirectory, stageToFind);
                        string pathToScene = Path.Combine(damnedStages.StagesAndScenesDirectory, sceneToFind);
                        removeStage.StagePath = pathToStage;
                        removeStage.ScenePath = pathToScene;
                        removeStagesList.Add(new DamnedRemoveStage(removeStage));

                    }
                }
            }
        }
        

        private void ButtonModifyStages_Click(object sender, EventArgs e)
        {
            PrepareToModifyStages();

            if (selectedRows.Count < 1)
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            string tempDirectory = DamnedFiles.CreateTempWorkshopDirectory();
            string terrorZipFile = damnedFiles.DamnedImages.terrorZipFile;
            ZipFile.ExtractToDirectory(terrorZipFile, tempDirectory);
            string tempTerrorZipFileLocation = damnedFiles.DamnedImages.GetLayoutFileFromZip(tempDirectory);
            damnedFiles.DamnedImages.UpdateXmlFiles(tempTerrorZipFileLocation, removeStagesList.ToArray(), newStagesList.ToArray());
            string destination = Path.Combine(damnedFiles.DamnedImages.guiDirectory, "Terror.zip");
            File.Delete(destination);
            ZipFile.CreateFromDirectory(tempDirectory, destination);
            Directory.Delete(tempDirectory, true);
            RefreshDataView();
            Cursor.Current = Cursors.Default;
            MessageBox.Show("Successfully modified the stages in the game.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }


        private void RefreshDataView()
        {
            damnedDataView.Rows.Clear();
            string link = "https://raw.githubusercontent.com/Sweats/Damned-Community-Stages/master/CommunityStages.json";

            if (!(DamnedFiles.DownloadFile(link, JSON_FILE_NAME)))
            {
                MessageBox.Show("Failed to download the latest stages listing from the community repository. Do you have a valid internet connection?", "Failed To Update the Stage Listing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CommunityRepository repository = JsonConvert.DeserializeObject<CommunityRepository>(File.ReadAllText(JSON_FILE_NAME));

            for (int i = 0; i < repository.Stages.Count; i++)
            {
                Stage stage = repository.Stages[i];
                string stageToFind = String.Format("{0}.stage", stage.Name).Replace(" ", "_");

                if (String.IsNullOrEmpty(stage.Description))
                {
                    stage.Description = "No description provided.";
                }

                if (String.IsNullOrEmpty(stage.Date))
                {
                    stage.Date = "Unknown upload date.";
                }

                if (damnedStages.StageExists(stageToFind))
                {
                    string[] newRow = new string[] { stage.Name, stage.Author, stage.Date, stage.Description, "Yes" };
                    int newIndex = damnedDataView.Rows.Add(newRow);
                    damnedDataView.Rows[newIndex].ReadOnly = true;
                    damnedDataView.Rows[newIndex].DefaultCellStyle.ForeColor = Color.FromArgb(255, 168, 38);
                }

                else
                {
                    string[] newRow = new string[] { stage.Name, stage.Author, stage.Date, stage.Description, "No" };
                    int newIndex = damnedDataView.Rows.Add(newRow);
                    damnedDataView.Rows[newIndex].ReadOnly = true;
                    damnedDataView.Rows[newIndex].DefaultCellStyle.ForeColor = Color.White;
                }
            }


            File.Delete(JSON_FILE_NAME);
        }
    }
}
