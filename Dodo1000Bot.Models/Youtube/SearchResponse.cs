namespace Dodo1000Bot.Models.Youtube;

public class SearchResponse
{
    public Video[] Items { get; set; }
}

public class Video
{
    public Id Id { get; set; }
    public Snippet Snippet { get; set; }
}

public class Id
{
    public string VideoId { get; set; }
}

public class Snippet
{
    public string PublishedAt { get; set; }
    public string LiveBroadcastContent { get; set; }
    public string PublishTime { get; set; }
    public string Title { get; set; }
}