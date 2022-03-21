using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StructureHelpers;
using System.Xml.Schema;

namespace Jackfruit.IncrementalGenerator
{
    internal class CreateSource
    {

        private readonly string createWithRootCommand = "CreateWithRootCommand";
        private readonly string cliRoot = "CliRoot";
        private readonly string rootCommandHandler = "rootCommandHandler";
        private  string commandClassName(CommandDef commandDef)
            =>  $"{commandDef.Id}Command";

        public CodeFileModel GetDefaultConsoleApp()
        {
            return new CodeFileModel(Helpers.ConsoleAppClassName)
            {
                Usings = { new("System") },
                Namespace = new("System.CommandLine")  // not sure what nspace to put this in
                {
                    Classes = new()
                    {
                        new(Helpers.ConsoleAppClassName)
                        {
                            IsPartial = true, 
                            Members = new()
                            {
                                Constructor(Helpers.ConsoleAppClassName).Private(),
                                Method(createWithRootCommand,Helpers. ConsoleAppClassName)
                                    .Public()
                                    .Static()
                                    .Parameters(new ParameterModel(rootCommandHandler,"Delegate"))
                            }
                        }
                    }
                }
            };
        }

        public CodeFileModel GetCustomApp(CommandDef commandDef)
        {
            var commandClass = GetNestedCommands(0,commandDef);
            var consoleClassCode = GetConsoleCode(commandClass, commandClassName(commandDef));
            var codeFile = new CodeFileModel(Helpers.ConsoleAppClassName)
            {
                Usings = {"System", "System.CommandLine","System.CommandLine.Invocation","System.Threading.Tasks" },
                Namespace = new(commandDef.Namespace)  // not sure what nspace to put this in
                {
                    Classes = new()
                    {
                       consoleClassCode
                    }
                }
            };
            return codeFile;
            
        }

        private ClassModel GetConsoleCode(ClassModel commandCode, string commandClassName)
            {
            var consoleCode = new ClassModel(Helpers.ConsoleAppClassName)
            {
                Members = new()
                {
                    Field($"_{cliRoot}",commandClassName).Private(),
                    Constructor(Helpers. ConsoleAppClassName).Private(),
                    Method(createWithRootCommand, Helpers. ConsoleAppClassName)
                        .Public()
                        .Static()
                        .Parameters(new ParameterModel(rootCommandHandler,"Delegate"))
                        .Statements(Assign($"_{cliRoot}", New(commandClassName))),
                    Property(cliRoot,commandClassName)
                        .Public()
                        .Get( Return(Symbol( $"_{cliRoot}"))),
                }
            };
            return consoleCode;
        }

        private ClassModel GetNestedCommands(int i, CommandDef commandDef)
        {
            if (i == 0) throw new IOException("Runaway recursion suspected");
            var classCode =
                Class(commandDef.Id)
                    .Public()
                    .Static()
                    .Members(
                        Method(Helpers.AddSubCommand, Void())
                            .Parameters(Parameter("handler", "Delegate")));

            foreach (var subCommnand in commandDef.SubCommands)
            {
                classCode.Members.Add(GetNestedCommands(i + 1, subCommnand));
            }
            return classCode;
        }


        private ClassModel GetCommandCode(CommandDef commandDef)
        {
            var commandCode =
                Class(commandClassName(commandDef))
                    .Public()
                    .Partial();
                    //.Members(

                    //)
            return commandCode;
        }

    }
}
