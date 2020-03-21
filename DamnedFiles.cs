using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

// A wrapper class that makes dealing with the Damned filesystem easy. This will become very useful.
// Not everything is used as of now but when we need them, we have them! :)

public class DamnedFiles
{
    public string Directory
    {
        get;
        private set;
    }

    public DamnedMaps DamnedMaps
    {
        get;
        private set;
    }

    public DamnedSounds DamnedSounds
    {
        get;
        private set;
    }

    public DamnedObjects DamnedObjects
    {
        get;
        private set;
    }

    public DamnedImages DamnedImages
    {
        get;
        private set;
    }

    public string[] DamnedDirectories
    {
        get;
        private set;
    }

    public string[] DamnedDirectoriesPath
    {
        get;
        private set;
    }

    public DamnedFiles(string rootDirectory)
    {
        this.Directory = rootDirectory;
        SetDirectories();
    }

    public void Load()
    {
        DamnedMaps = new DamnedMaps(Directory);
        DamnedObjects = new DamnedObjects(Directory);
        DamnedSounds = new DamnedSounds(Directory);
        DamnedImages = new DamnedImages(Directory, DamnedMaps, DamnedObjects);
    }

    private void SetDirectories()
    {
        DirectoryInfo[] directories = new DirectoryInfo(Directory).GetDirectories("*", SearchOption.AllDirectories);
        DamnedDirectoriesPath = new string[directories.Length];
        DamnedDirectories = new string[directories.Length];

        for (int i = 0; i < directories.Length; i++)
        {
            DamnedDirectories[i] = directories[i].Name;
            DamnedDirectoriesPath[i] = directories[i].FullName;
        }
    }

    public bool Check()
    {
        if (!CheckForDamnedExecutable())
        {
            return false;
        }

        string[] foldersToLookFor = new string[] { "DamnedData", "GUI", "Resources", "EditorImages", "TerrorImages", "Sounds", "Stages", "Redist", "Docs", "Ambience", "Traps" };
        int count = 0;
        int goal = foldersToLookFor.Length;
        bool success = false;

        for (int i = 0; i < foldersToLookFor.Length; i++)
        {
            string currentFolderLookingFor = foldersToLookFor[i];

            for (int k = 0; k < DamnedDirectories.Length; k++)
            {
                string currentFolderFound = DamnedDirectories[k];

                if (currentFolderLookingFor == currentFolderFound)
                {
                    count++;
                    break;
                }
            }

            if (count == goal)
            {
                success = true;
                break;
            }
        }

        return success;
    }

    public void Refresh()
    {
        SetDirectories();
        DamnedMaps = new DamnedMaps(Directory);
        DamnedObjects = new DamnedObjects(Directory);
        DamnedSounds = new DamnedSounds(Directory);
        DamnedImages = new DamnedImages(Directory, DamnedMaps, DamnedObjects);
    }

    private bool CheckForDamnedExecutable()
    {
        FileInfo[] files = new DirectoryInfo(Directory).GetFiles("*.exe", SearchOption.TopDirectoryOnly);
        bool found = false;

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name == "Damned.exe")
            {
                found = true;
                break;
            }
        }

        return found;
    }

    public static void CleanUpNewFiles(DamnedFiles oldFiles, DamnedFiles newFiles)
    {
        CleanUpAddedDirectories(oldFiles, newFiles);
        CleanUpAddedFiles(oldFiles, newFiles);
    }

    private static void CleanUpAddedDirectories(DamnedFiles oldFiles, DamnedFiles newFiles)
    {
        DirectoryInfo[] oldInfo = new DirectoryInfo(oldFiles.Directory).GetDirectories("*", SearchOption.AllDirectories);
        DirectoryInfo[] newInfo = new DirectoryInfo(newFiles.Directory).GetDirectories("*", SearchOption.AllDirectories);
        List<string> foldersToDelete = new List<string>();
        bool foundMatchingDirectory;

        for (int i = 0; i < newInfo.Length; i++)
        {
            foundMatchingDirectory = false;

            string newCurrentDirectoryName = newInfo[i].Name;

            for (int k = 0; k < oldInfo.Length; k++)
            {
                string oldCurrentDirectoryName = oldInfo[k].Name;

                if (newCurrentDirectoryName == oldCurrentDirectoryName)
                {
                    foundMatchingDirectory = true;
                    break;
                }
            }

            if (!foundMatchingDirectory)
            {
                foldersToDelete.Add(newInfo[i].FullName);
            }
        }

        for (int i = 0; i < foldersToDelete.Count; i++)
        {
            System.IO.Directory.Delete(foldersToDelete[i], true);
        }
    }

    private static void CleanUpAddedFiles(DamnedFiles oldFiles, DamnedFiles newFiles)
    {
        FileInfo[] oldInfo = new DirectoryInfo(oldFiles.Directory).GetFiles("*", SearchOption.AllDirectories);
        FileInfo[] newInfo = new DirectoryInfo(newFiles.Directory).GetFiles("*", SearchOption.AllDirectories);
        bool foundMatchingFile;
        List<string> filesToDelete = new List<string>();

        for (int i = 0; i < newInfo.Length; i++)
        {
            foundMatchingFile = false;
            string newCurrentFileName = newInfo[i].Name;

            for (int k = 0; k < oldInfo.Length; k++)
            {
                string oldCurrentFileName = oldInfo[k].Name;

                if (newCurrentFileName == oldCurrentFileName)
                {
                    foundMatchingFile = true;
                    break;
                }
            }

            if (!foundMatchingFile)
            {
                filesToDelete.Add(newInfo[i].FullName);
            }
        }

        for (int i = 0; i < filesToDelete.Count; i++)
        {
            File.Delete(filesToDelete[i]);
        }
    }

    // Creates a temp directory in the temp folder and returns a path to that temp folder
    public static string CreateTempWorkshopDirectory()
    {
        int randomNumber = new Random().Next();
        string tempFolderName = $"DamnedWorkshop_{randomNumber}";
        string tempPath = Path.GetTempPath();
        string workshopTempPath = Path.Combine(tempPath, tempFolderName);

        if (System.IO.Directory.Exists(workshopTempPath))
        {
            System.IO.Directory.Delete(workshopTempPath, true);
        }

        System.IO.Directory.CreateDirectory(workshopTempPath);

        return workshopTempPath;
    }

    public static void DeleteWorkshopTempDirectories()
    {
        string tempPath = Path.GetTempPath();
        DirectoryInfo[] info = new DirectoryInfo(tempPath).GetDirectories("DamnedWorkshop_*");

        for (int i = 0; i < info.Length; i++)
        {
            System.IO.Directory.Delete(info[i].FullName, true);
        }
    }

    public static string CreateTempFileInTempDirectory(string sourceFilePath)
    {
        string filePath = DamnedFiles.CreateTempWorkshopDirectory();
        string fileName = Path.GetFileName(sourceFilePath);
        string destFilePath = Path.Combine(filePath, fileName);
        File.Copy(sourceFilePath, destFilePath);
        return destFilePath;
    }

    public static bool DownloadFile(string link, string fileName)
    {
        try
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(link, fileName);
            }
        }
        catch (WebException)
        {
            return false;
        }

        return true;
    }
}