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
    public static class CreateSource
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

        public static CodeFileModel GetDefaultConsoleApp()
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

        public static CodeFileModel GetCustomApp(CommandDef commandDef)
        {

            var codeFile = new CodeFileModel(Helpers.ConsoleAppClassName)
            {
                Usings = { "System", "System.CommandLine", "System.CommandLine.Invocation", "System.Threading.Tasks" },
                Namespace = new(commandDef.Namespace)  // not sure what nspace to put this in
                {
                    Classes = new List<ClassModel> 
                    {
                        GetConsoleCode(GetNestedCommands(0, commandDef), CommandClassName(commandDef))
                    }
                }
            };
            return codeFile;

        }

        public static CodeFileModel GetCommandFile(CommandDef commandDef)
        {
            var codeFile = new CodeFileModel("Whereisthisused")
            {
                Usings = { "System", "System.CommandLine", "System.CommandLine.Invocation", "System.Threading.Tasks" },
                Namespace = new(commandDef.Namespace)  // not sure what nspace to put this in
                {
                    Classes = GetCommandClass( commandDef )
                }
            };
            return codeFile;

        }

        public static ClassModel GetCommandClass(CommandDef commandDef)
        {
            var commandClassName = CommandClassName(commandDef);
            var commandCode =
                Class(commandClassName)
                    .Public()
                    .Partial()
                    .Members(
                        Constructor(commandClassName)
                            .Private(),
                        CreateMethod(commandDef),
                        InvokeHandlerMethod(commandDef));
            foreach (var member in commandDef.Members)
            {
                switch (member)
                {
                    case OptionDef opt:
                        var optPropertyName = OptionPropertyName(opt);
                        commandCode.Members.Add(Property(optPropertyName, Generic("Option", opt.TypeName)).Public());
                        commandCode.Members.Add(
                            Method($"{optPropertyName}Result", opt.TypeName)
                                .Public()
                                .Parameters(Parameter("context", "InvocationContext"))
                                .Statements(Return(Invoke($"context.ParseResult", Generic("GetValueForOption", opt.TypeName), Symbol(optPropertyName)))));
                        break;
                    case ArgumentDef arg:
                        var argPropertyName = ArgumentPropertyName(arg);
                        commandCode.Members.Add(Property(argPropertyName, Generic("Argument", arg.TypeName)).Public());
                        commandCode.Members.Add(
                            Method($"{argPropertyName}Result", arg.TypeName)
                                .Public()
                                .Parameters(Parameter("context", "InvocationContext"))
                                .Statements(Return(Invoke($"context.ParseResult", Generic("GetValueForOption", arg.TypeName), Symbol(argPropertyName)))));
                        break;
                    default:
                        break;
                }
            }

            foreach (var subCommand in commandDef.SubCommands)
            {
                commandCode.Members.Add(Property(subCommand.Id, CommandClassName(subCommand)).Public());
            }

            return commandCode;

        }

        private static ClassModel GetConsoleCode(ClassModel commandCode, string commandClassName)
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

        private static ClassModel GetNestedCommands(int i, CommandDef commandDef)
        {
            if (i == 10) throw new IOException("Runaway recursion suspected");
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

        public static MethodModel InvokeHandlerMethod(CommandDef commandDef)
        {
            var arguments = new List<ExpressionBase>();
            var method =
                Method("InvokeAsync", Generic("Task", "int"))
                    .Public()
                    .Parameters(Parameter("context", "InvocationContext"));
            if (commandDef.ReturnType == "void")
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol("context.Exitcode"))));
            }
            else
            {
                method.Statements.Add(Return(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
            }

            return method;
        }

        public static MethodModel CreateMethod(CommandDef commandDef)
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
                        method.Statements.Add(Assign(optPropertyName, New(Generic("Option", opt.TypeName), opt.Id)));
                        if (!string.IsNullOrWhiteSpace(opt.Description))
                        { method.Statements.Add(Assign($"{optPropertyName}.Description", opt.Description)); }
                        if (opt.Aliases.Any())
                        { method.Statements.Add(Assign($"{optPropertyName}.Aliases", new ListLiteralModel( opt.Aliases))); }
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
