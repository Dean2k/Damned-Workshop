using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class DamnedMaps
{

    private readonly string directory;
    public string StagesAndScenesDirectory
    {
        get;
        private set;
    }

    public string[] Stages
    {
        get;
        private set;

    }

    public string[] Scenes
    {
        get;
        private set;
    }


    public DamnedMaps(string rootDirectory)
    {
        this.directory = rootDirectory;
        SetStagesAndScenesDirectory();
        SetStages();
        SetScenes();
    }


    private void SetStages()
    {
        if (!Directory.Exists(StagesAndScenesDirectory))
        {
            return;
        }

        FileInfo[] stagesList = new DirectoryInfo(StagesAndScenesDirectory).GetFiles("*.stage", SearchOption.TopDirectoryOnly);
        List<string> stages = new List<string>();

        for (int i = 0; i < stagesList.Length; i++)
        {
            if (stagesList[i].Name == "menu_background.stage")
            {
                continue;
            }

            stages.Add(stagesList[i].FullName);
        }

        this.Stages = stages.ToArray();
    }

    public void Refresh()
    {
        SetStages();
        SetScenes();
    }

    private void SetScenes()
    {
        if (!Directory.Exists(StagesAndScenesDirectory))
        {
            return;
        }

        FileInfo[] scenesList = new DirectoryInfo(StagesAndScenesDirectory).GetFiles("*.scene", SearchOption.TopDirectoryOnly);
        List<string> scenes = new List<string>();

        for (int i = 0; i < scenesList.Length; i++)
        {
            if (scenesList[i].Name == "menu_background.scene")
            {
                continue;
            }

            scenes.Add(scenesList[i].FullName);
        }

        this.Scenes = scenes.ToArray();
    }

    // This can be written with an enum instead
    public string GetPath(string stageName, bool stageFile = true )
    {
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(stageName);
        string returnString = String.Empty;

        for (int i = 0; i < Stages.Length; i++)
        {
            string foundStage = Path.GetFileNameWithoutExtension(Stages[i]);

            int result = String.Compare(nameWithoutExtension, foundStage, true);

            if (result > 0)
            {
                returnString = Stages[i];
                break;
            }
        }

        return returnString;
    }

    public void RemoveStage(string stageName)
    {
        string stageToFind = Path.GetFileName(stageName);

        for (int i = 0; i < Stages.Length; i++)
        {
            string stageToRemove = Path.GetFileName(Stages[i]);

            if (stageToRemove == stageToFind)
            {
                File.Delete(Stages[i]);
                break;
            }
        }
    }


    public void RemoveScene(string sceneName)
    {
        string sceneToFind = Path.GetFileName(sceneName);

        for (int i = 0; i < Scenes.Length; i++)
        {
            string sceneToRemove = Path.GetFileName(Scenes[i]);

            if (sceneToRemove == sceneToFind)
            {
                File.Delete(Scenes[i]);
                break;
            }
        }

    }

    public bool StageExists(string stageName)
    {
        bool found = false;

        for (int i = 0; i < Stages.Length; i++)
        {
            string stageToFind = Path.GetFileName(Stages[i]);

            if (stageName == stageToFind)
            {
                found = true;
                break;
            }
        }

        return found;
    }

    private void SetStagesAndScenesDirectory()
    {
        DirectoryInfo[] info = new DirectoryInfo(directory).GetDirectories("*", SearchOption.AllDirectories);

        for (int i = 0; i < info.Length; i++)
        {
            string stagesAndScenesDirectory = info[i].Name;

            if (stagesAndScenesDirectory == "Stages")
            {
                this.StagesAndScenesDirectory = info[i].FullName;
                break;
            }

        }
    }

    public static bool CheckInnerStageFile(string stagePath, ref string failedReason)
    {
        string nameToMatch = Path.GetFileNameWithoutExtension(stagePath);
        string stageName = Path.GetFileName(stagePath);

        using (StreamReader reader = new StreamReader(stagePath))
        {
            string contents = reader.ReadToEnd();
            string stageLineToFind = String.Format("stage {0}", nameToMatch);
            string sceneLineToFind = String.Format("scene {0}", nameToMatch);

            Match match = Regex.Match(contents, stageLineToFind);

            if (!match.Success)
            {
                failedReason = String.Format("Check failed because the stage section in \"{0}\" does not match the file name.", stageName);
                return false;
            }

            match = Regex.Match(contents, sceneLineToFind);

            if (!match.Success)
            {
                failedReason = String.Format("Check failed because the scene section in \"{0}\" does not match the scene name", stageName);
                return false;
            }
        }

        return true;


    }
    private static bool CheckSceneForLights(string sceneFileContents, string scenePath, ref string failedReason)
    {
        MatchCollection collection = Regex.Matches(sceneFileContents, "light light.[0-9]+");

        if (collection.Count < 1)
        {
            string name = Path.GetFileName(scenePath);
            failedReason = String.Format("Check failed because \"{0}\" does not have any light points.", name);
            return false;
        }

        return true;
    }

    private static bool CheckSceneForSpawnPoints(string sceneFileContents, string scenePath, ref string failedReason)
    {
        MatchCollection collection = Regex.Matches(sceneFileContents, "spawn_point [0-9]+");
        string name = Path.GetFileName(scenePath);

        if (collection.Count < 1)
        {
            failedReason = String.Format("Check failed because \"{0}\" does not have any spawn points", name);
            return false;
        }

        int matchCount = collection.Count;

        if (matchCount < 7)
        {
            failedReason = String.Format("Check failed because \"{0}\" does not have enough spawn points. Found spawn point count: {1}. Required count: 7.", name, matchCount);
            return false;
        }

        return true;
    }


    private static bool CheckSceneForProperSceneName(string sceneFileContents, string scenePath, ref string failedReason)
    {
        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string pattern = String.Format("scene {0}", sceneName);
        Match match = Regex.Match(sceneFileContents, pattern);

        if (!match.Success)
        {
            failedReason = String.Format("Check failed because the scene section in {0} does not match the actual file name.", sceneName);
            return false;
        }

        return true;
    }

    public static bool CheckInnerSceneFile(string scenePath, ref string failedReason)
    {
        using (StreamReader reader = new StreamReader(scenePath))
        {
            string contents = reader.ReadToEnd();

            if (!CheckSceneForProperSceneName(contents, scenePath, ref failedReason))
            {
                return false;
            }

            if (!CheckSceneForSpawnPoints(contents, scenePath, ref failedReason))
            {
                return false;
            }

            if (!CheckSceneForLights(contents, scenePath, ref failedReason))
            {
                return false;
            }

        }

        return true;
    }


    // Code that I have wrote twice should go in here.
    public static void ModifyStages()
    {

    }
}
