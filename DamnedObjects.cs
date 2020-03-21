using System;
using System.IO;

public class DamnedObjects
{
    private readonly string directory;
    public string ObjectsDirectory
    {
        get;
        private set;
    }

    public string[] Objects
    {
        get;
        private set;
    }

	public DamnedObjects(string rootDirectory)
	{
        this.directory = rootDirectory;
        ObjectsDirectory = FindObjectDirectory();

        if (ObjectsDirectory == String.Empty)
        {
            CreateObjectsDirectory();
        }

        SetObjects();
	}


    private void CreateObjectsDirectory()
    {
        ObjectsDirectory = Path.Combine(directory, "DamnedData", "Resources", "Objects");
        Directory.CreateDirectory(ObjectsDirectory);
    }

    private void SetObjects()
    {
        if (!Directory.Exists(ObjectsDirectory))
        {
            return;
        }

        FileInfo[] objectsList = new DirectoryInfo(ObjectsDirectory).GetFiles("*.object", SearchOption.TopDirectoryOnly);
        Objects = new string[objectsList.Length];

        for (int i = 0; i < Objects.Length; i++)
        {
            Objects[i] = objectsList[i].Name;
        }

    }

    private string FindObjectDirectory()
    {
        string returnDirectoryPath = String.Empty;
        DirectoryInfo[] info = new DirectoryInfo(directory).GetDirectories("*", SearchOption.AllDirectories);

        for (int i = 0; i < info.Length; i++)
        {
            string objectDirectory = info[i].Name;

            if (objectDirectory == "Objects")
            {
                returnDirectoryPath = info[i].FullName;
                break;
                
            }
        }

        return returnDirectoryPath;

    }

    public void CopyObjects(string[] sourceObjectsPath, string dest)
    {
        for (int i = 0; i < sourceObjectsPath.Length; i++)
        {
            CopyObject(sourceObjectsPath[i], dest);
        }

    }

    public void CopyObject(string sourcePath, string dest)
    {
        string objectName = Path.GetFileName(sourcePath);
        string newPath = Path.Combine(dest, objectName);

        if (File.Exists(newPath))
        {
            File.Delete(newPath);
        }

        File.Copy(sourcePath, newPath);
    }
}
