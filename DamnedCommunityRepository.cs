using System.Collections.Generic;

public class Stage
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string Date { get; set; }
}

public class CommunityRepository
{
    public List<Stage> Stages { get; set; }
}