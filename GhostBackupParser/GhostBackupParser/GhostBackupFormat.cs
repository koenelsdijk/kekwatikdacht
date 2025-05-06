using System.Text.Json.Serialization;

namespace GhostBackupParser;

public record GhostBackupFormat
{
  public Database[] Db { get; set; }
}


// Define a record for the Post data structure
public record Post
{
  [JsonPropertyName("slug")]
  public string Slug { get; init; }

  [JsonPropertyName("html")]
  public string Html { get; init; }

  [JsonPropertyName("status")]
  public string Status { get; init; }

  [JsonPropertyName("type")]
  public string Type { get; init; }
}

// Define a record for the resulting filename and content
public record FileContent
{
  [JsonPropertyName("filename")]
  public string Filename { get; init; }

  [JsonPropertyName("content")]
  public string Content { get; init; }

  [JsonPropertyName("type")]
  public string Type { get; init; }
}

// Define a record for the Database structure containing posts
public record Database
{
  [JsonPropertyName("data")]
  public DataContainer Data { get; init; }
}

// Define a record for the container that holds an array of posts
public record DataContainer
{
  [JsonPropertyName("posts")]
  public Post[] Posts { get; init; }
}