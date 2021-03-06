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
   /// The wrapper class for the DeepSpaceNine command.
   /// </summary>
   public partial class DeepSpaceNine : GeneratedCommandBase<DeepSpaceNine, DeepSpaceNine.Result, NextGeneration>, ICommandHandler
   {
      internal static DeepSpaceNine Build(NextGeneration parent)
      {
         var command = new DeepSpaceNine();
         command.Name = "deep-space-nine";
         command.Parent = parent;
         command.SiskoOption = new Option<bool>("--sisko");
         command.Add(command.SiskoOption);
         command.OdoOption = new Option<bool>("--odo");
         command.Add(command.OdoOption);
         command.DaxOption = new Option<bool>("--dax");
         command.Add(command.DaxOption);
         command.WorfOption = new Option<bool>("--worf");
         command.Add(command.WorfOption);
         command.OBrienOption = new Option<bool>("--obrien");
         command.Add(command.OBrienOption);
         command.AddValidator(command.Validate);
         command.Handler = command;
         return command;
      }
      
      /// <summary>
      /// The result class for the DeepSpaceNine command.
      /// </summary>
      public partial class Result : NextGeneration.Result
      {
         public bool Sisko {get; set;}
         public bool Odo {get; set;}
         public bool Dax {get; set;}
         public bool Worf {get; set;}
         public bool OBrien {get; set;}
         /// <summary>
         /// Get an instance of the Result class for the NextGeneration command.
         /// </summary>
         /// <param name="command">The command corresponding to the result</param>
         /// <param name="invocationContext">The System.CommandLine InvocationContext used to retrieve.</param>
         internal static Result GetResult(DeepSpaceNine command, InvocationContext invocationContext)
         {
            return new Result(command, invocationContext.ParseResult.CommandResult);
         }
         
         private protected Result(DeepSpaceNine command, CommandResult commandResult)
         : base(command.Parent, commandResult)
         {
            Sisko = GetValueForSymbol(command.SiskoOption, commandResult);
            Odo = GetValueForSymbol(command.OdoOption, commandResult);
            Dax = GetValueForSymbol(command.DaxOption, commandResult);
            Worf = GetValueForSymbol(command.WorfOption, commandResult);
            OBrien = GetValueForSymbol(command.OBrienOption, commandResult);
         }
         
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public int Invoke(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
         return invocationContext.ExitCode;
      }
      
      /// <summary>
      /// The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.
      /// </summary>
      /// <param name="invocationContext">The System.CommandLine Invocation context used to retrieve values.</param>
      public Task<int> InvokeAsync(InvocationContext invocationContext)
      {
         var result = Result.GetResult(this, invocationContext);
         DemoHandlers.RunHandlers.DeepSpaceNine(result.Greeting, result.Sisko, result.Odo, result.Dax, result.Worf, result.OBrien);
         return Task.FromResult(invocationContext.ExitCode);
      }
      
      public Option<bool> SiskoOption {get; set;}
      public Option<bool> OdoOption {get; set;}
      public Option<bool> DaxOption {get; set;}
      public Option<bool> WorfOption {get; set;}
      public Option<bool> OBrienOption {get; set;}
   }
   
}
