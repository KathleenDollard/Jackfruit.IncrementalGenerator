// This file is created by a generator.
using System;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit
{
   public partial class RootCommand : ICommandHandler
   {
      public RootCommand()
      {
         Name = "hello";
         ToOption = new Option<string>("--to");
         ToOption.AddAlias("--to");
         Add(ToOption);
         AddValidator(Validate);
         Handler = this;
      }
      
      /// <summary>
      /// The result class for the Hello command.
      /// </summary>
      public partial class Result
      {
         public string To {get; set;}
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
            To = GetValueForSymbol(command.ToOption, commandResult);
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
         return Temp.Class1.Hello(result.To);
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         var ret = Temp.Class1.Hello(result.To);
         return Task.FromResult(ret);
      }
      
      public Option<string> ToOption {get; set;}
   }
   
}
