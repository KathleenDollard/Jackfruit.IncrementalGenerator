//using Jackfruit.IncrementalGenerator.CodeModels;
//using Jackfruit.Models;

//namespace Jackfruit.IncrementalGenerator
//{
//    internal class Playground
//    {
//        public CodeFileModel FirstFile(CommandDef commandDef)
//        => new(commandDef.UniqueId)
//        {
//            Usings = UsingModel.Create("System", "System.CommandLine"),
//            Namespace = new(commandDef.Namespace)
//            {
//                Classes = ClassModel.Create(
//                    AppClass(commandDef),
//                    RootCommand(commandDef))
//            }
//        };

//        private ClassModel RootCommand(CommandDef commandDef)
//        {
//            throw new NotImplementedException();
//        }

//        private ClassModel AppClass(CommandDef commandDef)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
