using GhostBackupParser.DI;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var registrar = TypeRegistrar.Create();
var app = new CommandApp<StartCommand>(registrar);