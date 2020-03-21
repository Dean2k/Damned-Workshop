using System;
using System.Collections.Generic;

public class DamnedNewStage
{
    public string LoadingImagePath { get; set; }
    public string LobbyImageButtonPath { get; set; }
    public string LobbyImageButtonHighlightedPath { get; set; }
    public string NewStagePath { get; set; }
    public string NewScenePath { get; set; }

    public List<string> NewObjectsPath { get; set; }

    public bool HasObjects { get; set; }
    public int Count { get; set; }

    public DamnedNewStage()
    {
        this.LobbyImageButtonPath = String.Empty;
        this.LoadingImagePath = String.Empty;
        this.NewStagePath = String.Empty;
        this.LobbyImageButtonHighlightedPath = String.Empty;
        this.NewScenePath = String.Empty;
        this.NewObjectsPath = new List<string>();
        this.HasObjects = false;
    }

    public DamnedNewStage(DamnedNewStage copy)
    {
        this.LoadingImagePath = copy.LoadingImagePath;
        this.LobbyImageButtonPath = copy.LobbyImageButtonPath;
        this.NewStagePath = copy.NewStagePath;
        this.LobbyImageButtonHighlightedPath = copy.LobbyImageButtonHighlightedPath;
        this.NewScenePath = copy.NewScenePath;
        this.NewObjectsPath = new List<string>(copy.NewObjectsPath);
        this.HasObjects = copy.HasObjects;
    }

    public void Clear()
    {
        LobbyImageButtonPath = String.Empty;
        LoadingImagePath = String.Empty;
        NewStagePath = String.Empty;
        LobbyImageButtonHighlightedPath = String.Empty;
        NewScenePath = String.Empty;
        NewObjectsPath.Clear();
        HasObjects = false;
        Count = 0;
    }
}