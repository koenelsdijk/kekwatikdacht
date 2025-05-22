using System.Text.Json;
using GhostBackupParser;
using Spectre.Console.Cli;

public class StartCommand : AsyncCommand<StartCommand.Settings>
{
  private static readonly JsonSerializerOptions SerializerOptions =
    new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
  
  public class Settings : CommandSettings
  {
    [CommandArgument(0, "<json>")]
    public string BackupJson { get; set; }
    [CommandOption("-c|--content-dir")]
    public string? ContentRoot { get; set; }
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    var isJson = IsJson(settings.BackupJson);
    var json = isJson 
      ? settings.BackupJson 
      : File.Exists(settings.BackupJson) 
        ? File.ReadAllText(settings.BackupJson) 
        : null;
    if (json == null) 
      return 1;
    
    var parsedJson = JsonSerializer.Deserialize<GhostBackupFormat>(json, SerializerOptions);

    if (parsedJson?.Db is not { Length: > 0 }) 
      return 1;
    
    foreach (var post in parsedJson.Db[0].Data.Posts)
    {
      var filename = post.Status == "draft" ? $"{post.Slug}_DRAFT.html" : $"{post.Slug}.html";
      var dir = $"{post.Type}s"; // Create plural by appending 's'
      if(!string.IsNullOrWhiteSpace(settings.ContentRoot)) dir = Path.Combine(settings.ContentRoot, dir);

      Directory.CreateDirectory(dir);
      await File.WriteAllTextAsync($"{dir}/{filename}", post.Html);

      Console.WriteLine($"Wrote to {dir}/{filename}");
    }
    return 0;
  }

  private static bool IsJson(string s)
  {
    try
    {
      JsonDocument.Parse(s);
      return true;
    }
    catch (JsonException)
    {
      return false;
    }
  }
}