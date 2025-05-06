using System.Text.Json;
using GhostBackupParser;
using Spectre.Console;
using Spectre.Console.Cli;

public class StartCommand : AsyncCommand<StartCommand.Settings>
{
  public class Settings : CommandSettings
  {
    [CommandArgument(0, "<json>")]
    public string BackupJson { get; set; }
  }

  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
  {
    var parsedJson = JsonSerializer.Deserialize<GhostBackupFormat>(settings.BackupJson);
    
    
  }
}