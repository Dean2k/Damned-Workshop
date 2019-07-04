﻿using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System;
using System.Drawing;
using System.Windows.Forms;

public class DamnedPackage
{

    public string tempDirectory { get; private set; }
    private string zipArchivePath;


    public string reasonForFailedCheck { get; private set; }
    public int objectsCount { get; private set; }

    private bool hasObjects;

    public DamnedPackage()
    {

    }

    private struct Cords
    {
        public int x;
        public int y;
    }


    public bool Check(string zipArchivePath)
    {
        this.zipArchivePath = zipArchivePath;
        hasObjects = false;

        if (Path.GetExtension(zipArchivePath) != ".zip")
        {
            string archiveName = Path.GetFileName(zipArchivePath);
            reasonForFailedCheck = String.Format("Check failed because \"{0}\" is not a .zip file. Please repackage your stage into a .zip file", archiveName);
            return false;
        }

        CreateTempDirectory();

        if (!CheckDirectories())
        {
            return false;
        }


        if (!CheckFiles())
        {
            return false;
        }

        return true;
    }


    private bool CheckFiles()
    {
        FileInfo[] files = new DirectoryInfo(tempDirectory).GetFiles("*", SearchOption.AllDirectories);
        bool success = true;

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Name;
            string filePath = files[i].FullName;
            string fileExtension = Path.GetExtension(fileName);

            if (fileExtension == ".jpg" || fileExtension == ".png")
            {
                if (!CheckImage(filePath))
                {
                    success = false;
                    break;
                }
            }

            else if (fileExtension == ".stage" || fileExtension == ".scene")
            {
                if (!CheckStageOrScene(filePath))
                {
                    success = false;
                    break;
                }
            }

            else if (fileExtension == ".object")
            {
                if (!CheckObject(filePath))
                {
                    success = false;
                    break;
                }
            }
        }

        return success;
    }

    private bool CheckDirectories()
    {
        DirectoryInfo[] info = new DirectoryInfo(tempDirectory).GetDirectories("*", SearchOption.AllDirectories);

        string[] directoriesToCheck = new string[] { "DamnedData", "GUI", "Resources", "TerrorImages", "Stages" };

        bool success = true;

        for (int i = 1; i < info.Length; i++)
        {
            bool found = false;
            string directory = info[i].Name;
            
            if (directory == "Objects")
            {
                hasObjects = true;
                continue;
            }

            for (int j = 0; j < directoriesToCheck.Length; j++)
            {
                if (directory == directoriesToCheck[j])
                {
                    found = true;
                }
            }

            if (!found)
            {
                success = false;
                reasonForFailedCheck = String.Format("Check failed because the directory \"{0}\" is not supposed to be in your zip archive", directory);
            }
        }

        return success;
    }



    // TODO: Check the stage or scene file itself and see if the name inside the file matches the filename!
    private bool CheckStageOrScene(string stagePath)
    {
        string directoryPath = Path.GetDirectoryName(stagePath);
        string directoryName = Path.GetFileName(directoryPath);

        if (directoryName != "Stages")
        {
            string stageName = Path.GetFileName(stagePath);
            reasonForFailedCheck = String.Format("Check failed because \"{0}\" does not reside in the Stages directory", stageName);
            return false;
        }

        FileInfo[] stages = new DirectoryInfo(directoryPath).GetFiles("*", SearchOption.TopDirectoryOnly);

        if (stages.Length > 2)
        {
            reasonForFailedCheck = "Check failed because the Stages directory has more than 2 files in it. Only 1 scene and 1 file is allowed";
            return false;
        }

        bool success = true;

        for (int i = 0; i < stages.Length; i++)
        {
            string stageName = stages[i].Name;

            if (!FindCorrespondingFile(stages, stageName))
            {
                success = false;
                break;
            }
        }

        return success;
    }


    private bool FindCorrespondingFile(FileInfo[] stages,  string stageOrScene)
    {
        string extension = Path.GetExtension(stageOrScene);
        bool success = false;

        for (int i = 0; i < stages.Length; i++)
        {
            string otherExtension = Path.GetExtension(stages[i].Name);

            if (otherExtension != extension)
            {
                string name = Path.GetFileNameWithoutExtension(stageOrScene);
                string otherName = Path.GetFileNameWithoutExtension(stages[i].Name);

                if (name == otherName)
                {
                    success = true;
                    break;
                }
            }
        }

        if (!success)
        {
            string otherExtension;

            if (extension == ".stage")
            {
                otherExtension = ".scene";

            }

            else
            {
                otherExtension = ".stage";
            }

            string stageOrSceneName = Path.GetFileName(stageOrScene);
            reasonForFailedCheck = String.Format("Check failed because \"{0}\" does not have its corresponding \"{1}{2}\" file in the same directory.", stageOrSceneName, stageOrScene, otherExtension);
            return false;
        }

        return success;

    }

    private bool CheckImage(string imagePath)
    {
        string directoryPath = Path.GetDirectoryName(imagePath);
        string directoryName = Path.GetFileName(directoryPath);

        if (directoryName != "GUI" && directoryName != "TerrorImages")
        {
            string imageName = Path.GetFileName(imagePath);
            reasonForFailedCheck = String.Format("Check failed because \"{0}\" does not reside in either the GUI directory or the TerrorImages directory.", imageName);
            return false;
        }

        string fileExtension = Path.GetExtension(imagePath);

        if (fileExtension == ".png")
        {
            Cords cords = GetDimensions(imagePath);

            if (cords.x != 300 && cords.y != 100 || cords.x != 900 && cords.y != 100)
            {
                string imageName = Path.GetFileName(imagePath);
                reasonForFailedCheck = String.Format("Check failed because the dimensions for the image \"{0}\" is not 300 X 100 or 900 X 100", imageName);
                return false;
            }
        }

        else if (fileExtension == ".jpg")
        {
            Cords cords = GetDimensions(imagePath);

            if (cords.x != 1920 && cords.y != 1080)
            {
                string imageName = Path.GetFileName(imagePath);
                reasonForFailedCheck = String.Format("Check failed because the dimensions for the image \"{0}\" is not 1920 X 1080", imageName);
                return false;
            }
        }

        return true;
    }


    private bool CheckObject(string objectPath)
    {
        string directoryNamePath = Path.GetDirectoryName(objectPath);
        string directoryName = Path.GetFileName(directoryNamePath);

        if (directoryName != "Objects")
        {
            string objectName = Path.GetFileName(objectPath);
            reasonForFailedCheck = String.Format("Check failed because object \"{0}\" does not reside in the Objects directory.", objectName);
            return false;
        }

        return true;
    }


    // Loads the variables from a zip file  into the DamnedMappingForm assuming that it is packaged correctly.
    public void Load(DamnedWorkshop.DamnedMappingForm form)
    {
        FileInfo[] info = new DirectoryInfo(tempDirectory).GetFiles("*", SearchOption.AllDirectories);
        objectsCount = 0;

        for (int i = 0; i < info.Length; i++ )
        {
            string fileNamePath = info[i].FullName;
            string fileExtension = Path.GetExtension(fileNamePath);
            
            if (fileExtension == ".png")
            {
                Cords cords = GetDimensions(fileNamePath);

                if (cords.x == 300 && cords.y == 100)
                {
                    form.damnedNewStage.lobbyImageButtonPath = fileNamePath;
                }

                else if (cords.x == 900 && cords.y == 100)
                {

                    form.damnedNewStage.lobbyImageButtonHighlightedPath = fileNamePath;
                }
            }

            else if (fileExtension == ".jpg")
            {
                Cords cords = GetDimensions(fileNamePath);

                if (cords.x == 1920 && cords.y == 1080)
                {
                    form.damnedNewStage.loadingImagePath = fileNamePath;
                }
            }


            else if (fileExtension == ".scene")
            {
                form.damnedNewStage.newScenePath = fileNamePath;
            }

            else if (fileExtension == ".stage")
            {
                form.damnedNewStage.newStagePath = fileNamePath;
            }


            else if (fileExtension == ".object")
            {
                form.damnedNewStage.hasObjects = true;
                form.damnedNewStage.newObjectsPath.Add(fileNamePath);
                objectsCount++;
            }
        }
    }

    private Cords GetDimensions(string fileName)
    {
        Cords cords = new Cords();

        using (var image = Image.FromFile(fileName))
        {
            cords.x = image.Width;
            cords.y = image.Height;
        }

        return cords;
    }

    private void CreateTempDirectory()
    {
        string tempPath = Path.GetTempPath();
        int randomNumber = new Random().Next();
        string tempStringNumber = String.Format("DamnedWorkshop_{0}", randomNumber);
        tempPath = Path.Combine(tempPath, tempStringNumber);

        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }

        Directory.CreateDirectory(tempPath);
        ZipFile.ExtractToDirectory(zipArchivePath, tempPath);
        tempDirectory = tempPath;
    }

    public void Package(DamnedNewStage[] newStages, string destination)
    {
        for (int i = 0; i < newStages.Length; i++)
        {
            Package(newStages[i], destination);
        }
    }

    // Too much work to write this. Probably a better way to do this.
    private void Package(DamnedNewStage newStage, string destination)
    {
        int randomNumber = new Random().Next();
        string tempDirectory = Path.GetTempPath();
        string directoryName = String.Format("DamnedWorkshop_{0}", randomNumber);
        tempDirectory = Path.Combine(tempDirectory, directoryName);
        this.tempDirectory = tempDirectory;

        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }


 
        CreateDirectories();
        DirectoryInfo[] info = new DirectoryInfo(tempDirectory).GetDirectories("*", SearchOption.AllDirectories);

        string newStageNamePath = newStage.newStagePath;
        string newStageName = Path.GetFileName(newStageNamePath);
        string newSceneNamePath = newStage.newScenePath;
        string newSceneName = Path.GetFileName(newStage.newScenePath);
        string newLoadingImageNamePath = newStage.loadingImagePath;
        string newLoadingImageName = Path.GetFileName(newLoadingImageNamePath);
        string newLobbyButtonImagePath = newStage.lobbyImageButtonPath;
        string newLobbyButtnImageName = Path.GetFileName(newStage.lobbyImageButtonPath);
        string newLobbyButtonImageHighlightedPath = newStage.lobbyImageButtonHighlightedPath;
        string newLobbyButtonImageHighlightedName = Path.GetFileName(newStage.lobbyImageButtonHighlightedPath);

        string stageAndScenePath = GetPath(info, "Stages");
        string guiPath = GetPath(info, "GUI");
        string terrorImagesPath = GetPath(info, "TerrorImages");

        


        string newZipArchiveName = Path.GetFileNameWithoutExtension(newStageName).Replace("_", " ");
        newZipArchiveName = String.Format("{0}.zip", newZipArchiveName);

        string newPath = Path.Combine(stageAndScenePath, newStageName);
        File.Copy(newStageNamePath, newPath);
        newPath = Path.Combine(stageAndScenePath, newSceneName);
        File.Copy(newSceneNamePath, newPath);
        newPath = Path.Combine(terrorImagesPath, newLoadingImageName);
        File.Copy(newLoadingImageNamePath, newPath);
        newPath = Path.Combine(guiPath, newLobbyButtnImageName);
        File.Copy(newLobbyButtonImagePath, newPath);
        newPath = Path.Combine(guiPath, newLobbyButtonImageHighlightedName);
        File.Copy(newLobbyButtonImageHighlightedPath, newPath);
        destination = Path.Combine(destination, newZipArchiveName);

        if (newStage.hasObjects)
        {
            CreateObjectsDirectory();
            string objectsPath = GetPath(info, "Objects");
            DamnedObjects damnedObjects = new DamnedObjects(tempDirectory);
            damnedObjects.CopyObjects(newStage.newObjectsPath.ToArray(), damnedObjects.objectsDirectory);
        }

        if (File.Exists(destination))
        {
            string message = String.Format("Package \"{0}\" already exists at this location. Do you wish to overwrite it?", newZipArchiveName);
            DialogResult result = MessageBox.Show(message, "Package already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                File.Delete(destination);
            }

            else
            {
                Directory.Delete(tempDirectory, true);
                return;
            }
        }

        ZipFile.CreateFromDirectory(tempDirectory, destination);
        Directory.Delete(tempDirectory, true);
    }

    private string GetPath(DirectoryInfo[] info, string folderToFind)
    {
        string returnString = String.Empty;

        for (int i = 0; i < info.Length; i++)
        {
            string folder = info[i].Name;

            if (folder == folderToFind)
            {
                returnString = info[i].FullName;
                break;
            }
        }

        return returnString;
    }

    private void CreateDirectories()
    {
        string directoryToMake = Path.Combine(tempDirectory, "DamnedData", "Resources", "Stages");

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }

        directoryToMake = Path.Combine(tempDirectory, "DamnedData", "GUI");

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }

        directoryToMake = Path.Combine(tempDirectory, "DamnedData", "GUI", "TerrorImages");

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }

        directoryToMake = Path.Combine(tempDirectory, "DamnedData", "Resources", "Objects");

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }
    }

    private void CreateObjectsDirectory()
    {
        string directoryToMake = Path.Combine(tempDirectory, "DamnedData", "Resources", "Objects");

        if (!Directory.Exists(directoryToMake))
        {
            Directory.CreateDirectory(directoryToMake);
        }
    }
}


   