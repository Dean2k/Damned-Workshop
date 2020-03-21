using System.IO;

public class DamnedSounds
{
    private readonly string directory;

    public string SoundsDirectory
    {
        get;
        private set;
    }

    public string[] Sounds
    {
        get;
        private set;
    }

    public DamnedSounds(string rootDirectory)
    {
        this.directory = rootDirectory;
        SoundsDirectory = FindSoundsDirectory();
        SetSounds();
    }

    private void SetSounds()
    {
        if (!Directory.Exists(SoundsDirectory))
        {
            return;
        }

        FileInfo[] soundsList = new DirectoryInfo(SoundsDirectory).GetFiles("*.ogg", SearchOption.AllDirectories);
        Sounds = new string[soundsList.Length];

        for (int i = 0; i < Sounds.Length; i++)
        {
            Sounds[i] = soundsList[i].Name;
        }
    }

    private string FindSoundsDirectory()
    {
        string returnSoundsDirectory = "";
        DirectoryInfo[] info = new DirectoryInfo(directory).GetDirectories("*", SearchOption.AllDirectories);

        for (int i = 0; i < info.Length; i++)
        {
            string soundsDirectory = info[i].Name;

            if (soundsDirectory == "Sounds")
            {
                returnSoundsDirectory = info[i].FullName;
                break;
            }
        }

        return returnSoundsDirectory;
    }
}