using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using GhostBackupParser;
using Spectre.Console.Cli;
using HtmlAgilityPack;

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
    try
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
        var prettyHtml = PrettifyXml(post.Html);
        await File.WriteAllTextAsync($"{dir}/{filename}", prettyHtml);

        Console.WriteLine($"Wrote to {dir}/{filename}");
      }
      return 0;
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
      throw;
    }
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

  private static string PrettifyXml(string contentHtml)
  {
    var doc = new HtmlDocument();
    contentHtml = $"<body>{contentHtml}</body>";
    doc.LoadHtml(contentHtml);

    var xDoc = new XDocument(new XElement("html", XElement.Parse(doc.DocumentNode.OuterHtml
      .Replace("<hr>", "<hr/>")
      .Replace("<br>", "<br/>"))));
    var settings = new XmlWriterSettings { 
      Indent = true, 
      NewLineOnAttributes = true, 
      OmitXmlDeclaration = true,
      NewLineChars = "\r\n" 
    };
    using var writer = new StringWriter();
    using var xmlWriter = XmlWriter.Create(writer, settings);
    xDoc.Save(xmlWriter);
    return xDoc.ToString();
  }
}