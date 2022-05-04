//HintName: CliRoot.cs
// This file is created by a generator.
using System.CommandLine.Parsing;

namespace Jackfruit
{
    partial class CliRoot : GeneratedCommandBase<CliRoot, CliRoot.Result>
    {
        private CliRoot() : base("<EmptyCommand>", null)
        {
        }

        public static EmptyCommand Create(Delegate MethodToRun)
        {
            return new EmptyCommand();
        }

        public class Result
        {
        }

        Result GetResult(CommandResult CommandResult)
        {
            throw Jackfruit.IncrementalGenerator.CodeModels.NamedItemModel("Result not available");
        }

    }

}
