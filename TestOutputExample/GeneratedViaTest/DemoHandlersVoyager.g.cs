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
   /// The wrapper class for the Voyager command.
   /// </summary>
   public partial class Voyager : GeneratedCommandBase<Voyager, Voyager.Result, NextGeneration>, ICommandHandler
   {
      internal static Voyager Build(NextGeneration parent)
      {
         var command = new Voyager();
         command.Name = "voyager";
         command.Parent = parent;
         command.JanewayOption = new Option<bool>("--janeway");
         command.JanewayOption.AddAlias("--janeway");
         command.Add(command.JanewayOption);
         command.ChakotayOption = new Option<bool>("--chakotay");
         command.ChakotayOption.AddAlias("--chakotay");
         command.Add(command.ChakotayOption);
         command.TorresOption = new Option<bool>("--torres");
         command.TorresOption.AddAlias("--torres");
         command.Add(command.TorresOption);
         command.TuvokOption = new Option<bool>("--tuvok");
         command.TuvokOption.AddAlias("--tuvok");
         command.Add(command.TuvokOption);
         command.SevenOfNineOption = new Option<bool>("--seven-of-nine");
         command.SevenOfNineOption.AddAlias("--seven-of-nine");
         command.Add(command.SevenOfNineOption);
         command.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the Voyager command.
      /// </summary>
      public partial class Result : NextGeneration.Result
      {
         public System.CommandLine.IConsole Console {get; set;}
         public bool Janeway {get; set;}
         public bool Chakotay {get; set;}
         public bool Torres {get; set;}
         public bool Tuvok {get; set;}
         public bool SevenOfNine {get; set;}
         /// <summary>
         /// Get an instance of the Result class for the NextGeneration command.
         /// </summary>
         /// <param name="command">The command corresponding to the result</param>
         /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve.</param>
         internal static Result GetResult(Voyager command, InvocationContext invocationContext)
         {
            return new Result(command, invocationContext.ParseResult.CommandResult);
         }
         
         private protected Result(Voyager command, CommandResult commandResult)
         : base(command.Parent, commandResult)
         {
            Janeway = GetValueForSymbol(command.JanewayOption, commandResult);
            Chakotay = GetValueForSymbol(command.ChakotayOption, commandResult);
            Torres = GetValueForSymbol(command.TorresOption, commandResult);
            Tuvok = GetValueForSymbol(command.TuvokOption, commandResult);
            SevenOfNine = GetValueForSymbol(command.SevenOfNineOption, commandResult);
         }
         
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.Voyager(result.Console, result.Greeting, result.Janeway, result.Chakotay, result.Torres, result.Tuvok, result.SevenOfNine);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> JanewayOption {get; set;}
      public Option<bool> ChakotayOption {get; set;}
      public Option<bool> TorresOption {get; set;}
      public Option<bool> TuvokOption {get; set;}
      public Option<bool> SevenOfNineOption {get; set;}
   }
   
}
