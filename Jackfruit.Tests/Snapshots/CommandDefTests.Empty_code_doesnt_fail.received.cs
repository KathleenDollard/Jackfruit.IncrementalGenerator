//HintName: CliRoot.cs
// This file is created by a generator.
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit
{
    partial class CliRoot : GeneratedCommandBase<CliRoot, CliRoot.Result>
   {
      private CliRoot() : base("<EmptyCommand>", null)
      {
      }
      
      public static EmptyCommand Create(Delegate rootMethodToRun)
      {
         return new EmptyCommand();
      }
      
      public class Result
      {
      }
      
      public override Result GetResult(CommandResult CommandResult)
      {
         throw Jackfruit.IncrementalGenerator.CodeModels.NamedItemModel("Result not available");
      }
      
   }
   
}
