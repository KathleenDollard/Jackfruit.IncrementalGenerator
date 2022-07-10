// See https://aka.ms/new-console-template for more information
using DemoHandlers;
using Jackfruit;


var rootCommand = RootCommand.Create(new SubCommand[] { SubCommand.Create(RunHandlers.Voyager) });

