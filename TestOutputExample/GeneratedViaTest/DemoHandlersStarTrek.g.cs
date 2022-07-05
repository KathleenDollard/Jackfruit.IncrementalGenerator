// This file is created by a generator.
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit_DemoHandlers
{
   /// <summary>
   /// The wrapper class for the StarTrek command.
   /// </summary>
   public partial class StarTrek : GeneratedCommandBase<StarTrek, StarTrek.Result, Jackfruit.RootCommand>, ICommandHandler
   {
      internal static StarTrek Build(Jackfruit.RootCommand parent)
      {
         var command = new StarTrek();
         command.Name = "star-trek";
         command.Parent = parent;
         command.KirkOption = new Option<bool>("--kirk");
         command.KirkOption.Description = "Whether to greet Captain Kirk";
         command.KirkOption.AddAlias("--kirk");
         command.Add(command.KirkOption);
         command.SpockOption = new Option<bool>("--spock");
         command.SpockOption.Description = "Whether to greet Spock";
         command.SpockOption.AddAlias("--spock");
         command.Add(command.SpockOption);
         command.UhuraOption = new Option<bool>("--uhura");
         command.UhuraOption.Description = "Whether to greet Lieutenant Uhura";
         command.UhuraOption.AddAlias("--uhura");
         command.Add(command.UhuraOption);
         command.NextGeneration = NextGeneration.Build(command);
         command.AddCommandToScl(command.NextGeneration);
         command.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the StarTrek command.
      /// </summary>
      public partial class Result : Jackfruit.RootCommand.Result
      {
         public bool Kirk {get; set;}
         public bool Spock {get; set;}
         public bool Uhura {get; set;}
         /// <summary>
         /// Get an instance of the Result class for the NextGeneration command.
         /// </summary>
         /// <param name="command">The command corresponding to the result</param>
         /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve.</param>
         internal static Result GetResult(StarTrek command, InvocationContext invocationContext)
         {
            return new Result(command, invocationContext.ParseResult.CommandResult);
         }
         
         private protected Result(StarTrek command, CommandResult commandResult)
         : base(command.Parent, commandResult)
         {
            Kirk = GetValueForSymbol(command.KirkOption, commandResult);
            Spock = GetValueForSymbol(command.SpockOption, commandResult);
            Uhura = GetValueForSymbol(command.UhuraOption, commandResult);
         }
         
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.StarTrek(result.Greeting, result.Kirk, result.Spock, result.Uhura);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> KirkOption {get; set;}
      public Option<bool> SpockOption {get; set;}
      public Option<bool> UhuraOption {get; set;}
      public NextGeneration NextGeneration {get; set;}
   }
   
}
