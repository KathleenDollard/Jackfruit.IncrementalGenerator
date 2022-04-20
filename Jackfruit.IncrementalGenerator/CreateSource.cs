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
using System.Collections.Immutable;

namespace Jackfruit.IncrementalGenerator
{
    public static class CreateSource
    {
        // * ID of command seems to include "Handler"
        // * Generic for option is blank
        // * Greeting is appearing as an option, not an arg

        private const string addRootCommand = "AddRootCommand";
        private const string cliRoot = "CliRoot";
        private const string rootCommandHandler = "rootCommandHandler";
        private const string commandVar = "command";

        private static string CommandClassName(CommandDef commandDef)
            => $"{commandDef.Name}Command";
        private static string MemberPropertyName(MemberDef memberDef)
            => memberDef switch
            {
                OptionDef optionDef => $"{optionDef.Name}Option",
                ArgumentDef argDef => $"{argDef.Name}Argument",
                _ => $"{memberDef.Id}Service"
            };
        private static string MemberPropertyStyle(MemberDef memberDef)
            => memberDef switch
            {
                OptionDef optionDef => "Option",
                ArgumentDef argDef => "Argument",
                _ => "Service"
            };

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
                                Method(addRootCommand,Helpers. ConsoleAppClassName)
                                    .Public()
                                    .Static()
                                    .Parameters(new ParameterModel(rootCommandHandler,"Delegate"))
                            }
                        }
                    }
                }
            };
        }

        internal static CodeFileModel WrapClassesInCodefile(
            ImmutableArray<ClassModel> classModels,
            CommandDefBase rootCommandDef)
        {
            var nspace = rootCommandDef is CommandDef rootCommand
                ? rootCommand.Namespace
                : "RootNamespace";
            var codeFile = new CodeFileModel("Whereisthisused")
            {
                Usings = { "System", "System.CommandLine", "System.CommandLine.Invocation", "System.Threading.Tasks" },
                Namespace = new(nspace)  // not sure what nspace to put this in
                {
                    Classes = classModels.ToList()
                }
            };
            return codeFile;
        }

        public static CodeFileModel GetConsoleApp(CommandDefBase commandDefBase)
        {
            if (commandDefBase is not CommandDef commandDef)
            {
                return GetDefaultConsoleApp();
            }
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


        public static ClassModel GetCommandClass(
            CommandDef commandDef,
            CommandDefBase rootCommandDef)
        {
            var commandClassName = CommandClassName(commandDef);
            var isRoot = rootCommandDef == commandDef;
            var constructor =
                Constructor(commandClassName)
                    .Private();
            if (!isRoot)
            {
                constructor.Base(commandDef.Name);
            }
            var commandCode =
                Class(commandClassName)
                    .Public()
                    .Partial()
                    // TODO: Implement way to recognize RootCommand
                    .InheritedFrom(isRoot ? "RootCommand" : "Command")
                    .ImplementedInterfaces("ICommandHandler")
                    .Members(
                        constructor,
                        CreateMethod(commandDef),
                        InvokeHandlerMethod(commandDef));
            foreach (var member in commandDef.Members)
            {
                var name = MemberPropertyName(member);
                var style = MemberPropertyStyle(member);


                switch (member)
                {
                    case OptionDef:
                    case ArgumentDef:
                        commandCode.Members.Add(
                            Property(name, Generic(style, member.TypeName))
                                .Public());
                        commandCode.Members.Add(
                            Method($"{name}Result", member.TypeName)
                                .Public()
                                .Parameters(Parameter("context", "InvocationContext"))
                                .Statements(Return(Invoke($"context.ParseResult", Generic($"GetValueFor{style}", member.TypeName), Symbol(name)))));
                        break;
                    default:
                        break;
                }
            }

            foreach (var subCommand in commandDef.SubCommands)
            {
                if (subCommand is CommandDef subCommandDef)
                { commandCode.Members.Add(Property(subCommandDef.Name, CommandClassName(subCommandDef)).Public()); }
            }

            return commandCode;
        }

        private static ClassModel GetConsoleCode(ClassModel commandCode, string commandClassName)
        {
            var args = "args";
            var app = "app";
            var consoleCode = new ClassModel(Helpers.ConsoleAppClassName)
            {
                Members = new()
                {
                    Field($"_{cliRoot}",commandClassName).Private(),
                    Constructor(Helpers.ConsoleAppClassName).Private(),
                    Method(addRootCommand, Void())
                        .Public()
                        .Static()
                        .Parameters(new ParameterModel(rootCommandHandler,"Delegate")),
                    Method("Create", Helpers.ConsoleAppClassName)
                        .Public()
                        .Static()
                        .Statements(
                            AssignWithDeclare(app, New(Helpers.ConsoleAppClassName)),
                            Assign($"{app}._{cliRoot}", Invoke(commandClassName, "Create")),
                            Return(Symbol(app))),
                    Method("Run", "int")
                        .Public()
                        .Static()
                        .Parameters(new ParameterModel(args,"string[]"))
                        .Statements(
                            AssignWithDeclare(app, Invoke(Helpers.ConsoleAppClassName, "Create")),
                            Return(Invoke($"{app}.{cliRoot}","Invoke", Symbol(args)))),
                    Property(cliRoot,commandClassName)
                        .Public()
                        .Get( Return(Symbol( $"_{cliRoot}"))),
                    commandCode
                }
            };
            return consoleCode;
        }

        private static ClassModel GetNestedCommands(int i, CommandDef commandDef)
        {
            if (i == 10) throw new IOException("Runaway recursion suspected");
            var classCode =
                Class(commandDef.Name)
                    .Public()
                    .Static()
                    .Members(
                        Method(Helpers.AddSubCommand, Void())
                            .Public()
                            .Static()
                            .Parameters(Parameter("handler", "Delegate")));

            foreach (var subCommandDef in commandDef.SubCommands)
            {
                if (subCommandDef is CommandDef subCommand)
                { classCode.Members.Add(GetNestedCommands(i + 1, subCommand)); }
            }
            return classCode;
        }

        private static MethodModel InvokeHandlerMethod(CommandDef commandDef)
        {
            var arguments = new List<ExpressionBase>();
            foreach (var member in commandDef.Members)
            {
                //RestaurantArgumentResult(context)
                arguments.Add(Invoke("", $"{MemberPropertyName(member)}Result", Symbol("context")));
            }
            var method =
                Method("InvokeAsync", Generic("Task", "int"))
                    .Public()
                    .Parameters(Parameter("context", "InvocationContext"));
            if (commandDef.ReturnType == "void")
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol("context.ExitCode"))));
            }
            else
            {
                method.Statements.Add(Return(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
            }

            return method;
        }

        private static MethodModel CreateMethod(CommandDef commandDef)
        {
            var commandClassName = CommandClassName(commandDef);
            var method =
                    Method("Create", commandClassName)
                    .Public()
                    .Static()
                    .Statements(
                        AssignWithDeclare(commandVar, New(commandClassName)));
            foreach (var member in commandDef.Members)
            {
                switch (member)
                {
                    case OptionDef opt:
                        var optPropertyName = $"{commandVar}.{MemberPropertyName(opt)}";
                        method.Statements.Add(Assign(optPropertyName, New(Generic("Option", opt.TypeName), $"--{opt.Name}")));
                        if (!string.IsNullOrWhiteSpace(opt.Description))
                        { method.Statements.Add(Assign($"{optPropertyName}.Description", opt.Description)); }
                        if (opt.Aliases.Any())
                        { method.Statements.Add(Assign($"{optPropertyName}.Aliases", new ListLiteralModel(opt.Aliases))); }
                        if (!string.IsNullOrWhiteSpace(opt.ArgDisplayName))
                        { method.Statements.Add(Assign($"{optPropertyName}.ArgDisplayName", opt.ArgDisplayName)); }
                        if (opt.Required)
                        { method.Statements.Add(Assign($"{optPropertyName}.Required", opt.Required)); }
                        method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(optPropertyName))));
                        break;
                    case ArgumentDef arg:
                        var argPropertyName = $"{commandVar}.{MemberPropertyName(arg)}";
                        method.Statements.Add(Assign(argPropertyName, New(Generic("Argument", arg.TypeName), arg.Id)));
                        if (arg.Required)
                        { method.Statements.Add(Assign($"{argPropertyName}.Required", arg.Required)); }
                        method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(argPropertyName))));
                        break;
                    default:
                        break;
                }
            }
            foreach (var subCommandDef in commandDef.SubCommands)
            {
                if (subCommandDef is CommandDef subCommand)
                {
                    var toAdd = $"{commandVar}.{subCommand.Name}";
                    method.Statements.Add(Assign(toAdd, Invoke(CommandClassName(subCommand), "Create")));
                    method.Statements.Add(SimpleCall(Invoke(commandVar, "Add", Symbol(toAdd))));
                }
            }
            method.Statements.Add(Assign($"{commandVar}.Handler", Symbol(commandVar)));
            method.Statements.Add(Return(Symbol(commandVar)));
            return method;
        }

    }
}
