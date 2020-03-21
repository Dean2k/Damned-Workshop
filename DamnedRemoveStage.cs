using System;

// A simple class that is only supposed to be used in a List<T>
public class DamnedRemoveStage
{
    public string StagePath { get; set; }
    public string ScenePath { get; set; }

    public DamnedRemoveStage()
    {
        this.StagePath = String.Empty;
        this.ScenePath = String.Empty;
    }

    public DamnedRemoveStage(DamnedRemoveStage copy)
    {
        this.StagePath = copy.StagePath;
        this.ScenePath = copy.ScenePath;
    }

    public void Clear()
    {
        this.StagePath = String.Empty;
        this.ScenePath = String.Empty;
    }
}