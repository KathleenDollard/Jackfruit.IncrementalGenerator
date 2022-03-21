using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StructureHelpers;
using System.Xml.Schema;
using System.Runtime.CompilerServices;

namespace Jackfruit.IncrementalGenerator
{
    internal class CreateSource
    {

        private const string createWithRootCommand = "CreateWithRootCommand";
        private const string cliRoot = "CliRoot";
        private const string rootCommandHandler = "rootCommandHandler";
        private const string commandVar = "command";

        private static string CommandClassName(CommandDef commandDef)
            => $"{commandDef.Id}Command";
        private static string OptionPropertyName(OptionDef optionDef)
            => $"{commandVar}.{optionDef.Id}Option";
        private static string ArgumentPropertyName(ArgumentDef optionDef)
            => $"{commandVar}.{optionDef.Id}Argument";

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
            var commandClass = GetNestedCommands(0, commandDef);
            var consoleClassCode = GetConsoleCode(commandClass, CommandClassName(commandDef));
            var codeFile = new CodeFileModel(Helpers.ConsoleAppClassName)
            {
                Usings = { "System", "System.CommandLine", "System.CommandLine.Invocation", "System.Threading.Tasks" },
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

        public ClassModel GetCommandCode(CommandDef commandDef)
        {
            var commandClassName = CommandClassName(commandDef);
            var commandCode =
                Class(commandClassName)
                    .Public()
                    .Partial()
                    .Members(
                        Constructor(commandClassName)
                            .Private(),
                    CreateMethodModel(commandDef)
);

            return commandCode;

        }
 
        public MethodModel CreateMethodModel(CommandDef commandDef)
        {
            var commandClassName = CommandClassName(commandDef);
            var method =
                    Method(commandClassName, "Create")
                    .Public()
                    .Statements(
                        AssignWithDeclare(commandVar, New(commandClassName)));
            foreach (var member in commandDef.Members)
            {
                switch (member)
                {
                    case OptionDef opt:
                        var optPropertyName = OptionPropertyName(opt);
                        method.Statements.Add(Assign(optPropertyName, New(Generic("Option",opt.TypeName), opt.Id)));
                        if (!string.IsNullOrWhiteSpace(opt.Description))
                        { method.Statements.Add(Assign($"{optPropertyName}.Description", opt.Description)); }
                        if(opt.Aliases.Any())
                        { method.Statements.Add(Assign($"{optPropertyName}.Aliases", opt.Aliases)); }
                        if (!string.IsNullOrWhiteSpace(opt.ArgDisplayName))
                        { method.Statements.Add(Assign($"{optPropertyName}.ArgDisplayName", opt.ArgDisplayName)); }
                        if (opt.Required)
                        { method.Statements.Add(Assign($"{optPropertyName}.Required", opt.Required)); }
                        method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(optPropertyName))));
                        break;
                    case ArgumentDef arg:
                        var argPropertyName = ArgumentPropertyName(arg);
                        method.Statements.Add(Assign(argPropertyName, New(Generic("Argument", arg.TypeName), arg.Id)));
                        if (arg.Required)
                        { method.Statements.Add(Assign($"{argPropertyName}.Required", arg.Required)); }
                        method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(argPropertyName))));
                        break;
                    default:
                        break;
                }
            }
            foreach (var subCommand in commandDef.SubCommands)
            {
                var toAdd = $"{commandVar}.{subCommand.Id}";
                method.Statements.Add(Assign(toAdd, Invoke(CommandClassName(subCommand), "Create")));
                method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(toAdd))));
            }
            method.Statements.Add(Assign($"{commandVar}.Handler", Symbol(commandVar)));
            method.Statements.Add(Return(commandVar));
            return method;
        }

    }
}
