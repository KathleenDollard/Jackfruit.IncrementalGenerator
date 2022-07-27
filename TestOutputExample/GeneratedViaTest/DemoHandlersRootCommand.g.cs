// This file is created by a generator.
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;
using Jackfruit_DemoHandlers;

namespace Jackfruit
{
   public partial class RootCommand : ICommandHandler
   {
      public RootCommand()
      {
         Name = "root-command";
         GreetingArgument = new Argument<string>("GREETING");
         Add(GreetingArgument);
         StarTrek = StarTrek.Build(this);
         AddCommandToScl(StarTrek);
         AddValidator(Validate);
         Handler = this;
      }
      
      /// <summary>
      /// The result class for the RootCommand command.
      /// </summary>
      public partial class Result
      {
         public string Greeting {get; set;}
         /// <summary>
         /// Get an instance of the Result class for the NextGeneration command.
         /// </summary>
         /// <param name="command">The command corresponding to the result</param>
         /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve.</param>
         internal static Result GetResult(RootCommand command, InvocationContext invocationContext)
         {
            return new Result(command, invocationContext.ParseResult.CommandResult);
         }
         
         private protected Result(RootCommand command, CommandResult commandResult)
         {
            Greeting = GetValueForSymbol(command.GreetingArgument, commandResult);
         }
         
         private protected Result(RootCommand command, InvocationContext invocationContext)
         : this(command, invocationContext.ParseResult.CommandResult)
         {
         }
         
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.Franchise(result.Greeting);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.Franchise(result.Greeting);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Argument<string> GreetingArgument {get; set;}
      public StarTrek StarTrek {get; set;}
   }
   
}
