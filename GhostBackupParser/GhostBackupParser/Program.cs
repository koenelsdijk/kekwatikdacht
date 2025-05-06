using GhostBackupParser.DI;
using Spectre.Console.Cli;

var registrar = TypeRegistrar.Create();
var app = new CommandApp<StartCommand>(registrar);
return await app.RunAsync(args);