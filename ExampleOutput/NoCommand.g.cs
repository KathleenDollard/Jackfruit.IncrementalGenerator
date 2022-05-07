using ExampleOutput;
using System.CommandLine.Parsing;
using Jackfruit.Internal;

namespace Jackfruit
{

    public partial class Cli
    {
        public static void Create(CliNode cliRoot)
        {}
    }

    //// Generate
    //public partial class Cli
    //{
    //    public static FranchiseRoot Franchise { get; }
    //}


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

        public override Result GetResult(CommandResult CommandResult)
        {
            throw Jackfruit.IncrementalGenerator.CodeModels.NamedItemModel("Result not available");
        }

    }

}
//using System.CommandLine.Parsing;

//namespace Jackfruit
//{
//    public partial class CliRoot : GeneratedCommandBase<CliRoot, CliRoot.Result>
//    {
//        private CliRoot() : base("<EmptyCommand>", null)
//        { }

//        public static EmptyCommand Create(Delegate methodToRun)
//        {
//            var command = new EmptyCommand();
//            return command;
//        }

//        public struct Result
//        { }

//        public override Result GetResult(CommandResult commandResult)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}


