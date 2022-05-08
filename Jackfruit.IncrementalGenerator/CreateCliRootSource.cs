using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.Models;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StructureHelpers;
using System.Net;

namespace Jackfruit.IncrementalGenerator
{
    public static class CreateCliRootSource
    {
        private const string libName = "Jackfruit";
        private const string create = "Create";
        private const string command = "command";
        private const string result = "result";
        private const string parentResult = "parentResult";
        private const string cliRoot = "CliRoot";
        private const string commandVar = "command";
        private const string emptyCommand = "EmptyCommand";
        private const string resultName = "Result";
        private const string getResultName = "GetResult";
        private const string commandResult = "CommandResult";
        private const string methodToRun = "methodToRun";
        private const string rootMethodToRun = "rootMethodToRun";
        private const string invalidOpException = "InvalidOperationException";

        private static string CommandClassName(CommandDef commandDef) => commandDef.Name;
        private static string CommandPropertyName(CommandDef commandDef) => $"{commandDef.Name}Command";
        private static string CommandFullClassName(IEnumerable<CommandDef> ancestors, CommandDef? parent, CommandDef commandDef)
        {
            if (commandDef.Name == Helpers.CliRootName)
            { return Helpers.CliRootName; }
            var ancestorList =
                    parent is null
                    ? ancestors
                    : new List<CommandDef> { parent }.Union(ancestors);
            ancestorList = ancestorList
                    .Reverse()
                    .Where(a => a.Name != Helpers.CliRootName);
            var parentNames =
                ancestorList.Any()
                    ? string.Join(".", ancestorList.Select(a => CommandClassName(a))) + "."
                    : "";
            return Helpers.GetStyle(commandDef) == Helpers.Cli
                ? $"{parentNames}{CommandClassName(commandDef)}"
                : $"Commands.{parentNames}{CommandClassName(commandDef)}";
        }
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
        private static string SimpleName(MemberDef memberDef) => memberDef.Name;
        private static NamedItemModel GeneratedCommandBase(string self, string parent)
            => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}", parent);
        private static NamedItemModel GeneratedCommandBase(string self)
           => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}");
        private static NamedItemModel GeneratedRootCommandBase()
            => new GenericNamedItemModel("GeneratedCommandBase", cliRoot, $"{cliRoot}.{resultName}");

        public static CodeFileModel? GetCliPartialCodeFile(IEnumerable<CommandDefBase> rootCommandDefs)
        {
            var roots = rootCommandDefs
                    .OfType<CommandDef>().ToList()
                    .Where(x => Helpers.GetStyle(x) == Helpers.Cli);
            if (!roots.Any()) { return null; }

            var nspaces = roots.Select(x => new UsingModel(x.Namespace)).ToList();
            var ctorStatements = roots.Select(x => Assign(x.Name, Invoke(x.Name, create))).ToArray();
            var properties = roots.Select(x => Property(x.Name, x.Name)
                                                .Public()
                                                .Static());
            var classModel = Class(Helpers.Cli)
                                .Public()
                                .Partial()
                                .Members(
                                    Constructor(Helpers.Cli)
                                        .Static()
                                        .Statements(ctorStatements));
            classModel.Members.AddRange(properties);
            return new CodeFileModel(Helpers.Cli)
            {
                Usings = nspaces,
                Namespace = new("Jackfruit")
                {
                    Classes = new() { classModel }
                }
            };
        }

        public static CodeFileModel GetCommandCodeFile(CommandDefBase rootCommandDef)
            => rootCommandDef is CommandDef commandDef
                ? RootCommandCodeFile(commandDef)
                : DefaultRootCommand();

        internal static CodeFileModel DefaultRootCommand()
            => new CodeFileModel(Helpers.CliRootName)
            {
                Usings = { new("System.CommandLine.Parsing"), "Jackfruit.Internal" },
                Namespace = new("Jackfruit")  // not sure what nspace to put this in
                {
                    Classes = new()
                            {
                                Class(Helpers.CliRootName)
                                    .InheritedFrom (GeneratedRootCommandBase())
                                    .Partial ()
                                    .Members (
                                        Constructor(Helpers.CliRootName).Private()
                                            .Base("<EmptyCommand>", Null),
                                        Method(create ,emptyCommand)
                                            .Public()
                                            .Static()
                                            .Parameters(Parameter(rootMethodToRun,"Delegate"))
                                            .Statements(
                                                Return(New(emptyCommand))),
                                        Class(resultName)
                                            .Public(),
                                        Method(getResultName, resultName)
                                            .Public()
                                            .Override()
                                            .Parameters(Parameter(commandResult, commandResult))
                                            .Statements(
                                                Throw(invalidOpException, "Result not available"))
                                    )

                             }
                }
            };

        internal static CodeFileModel RootCommandCodeFile(CommandDef commandDef)
        {
            var style = Helpers.GetStyle(commandDef);
            if (style != Helpers.Cli)
            { commandDef.Name = "CliRoot"; }
            return new CodeFileModel("Commands")
            {
                Usings =
                {
                    "System",
                    "System.CommandLine",
                    "System.CommandLine.Invocation",
                    "System.CommandLine.Parsing",
                    "Jackfruit.Internal",
                    libName
                },
                Namespace = new(commandDef.Namespace)  // not sure what nspace to put this in
                {
                    Classes = style == Helpers.Cli
                        ? new List<ClassModel>
                        {
                            CommandClass(Enumerable.Empty<CommandDef>(),
                                         Enumerable.Empty<MemberDef>(),
                                         null,
                                         commandDef)
                        }
                        : new List<ClassModel>
                        {
                            CliRoot(commandDef),
                            SubCommands(commandDef)
                        }
                }
            };
        }

        private static ClassModel CliRoot(CommandDef commandDef)
        {
            var classModel =
                CommandClassDeclaration(commandDef)
                    .InheritedFrom(GeneratedRootCommandBase())
                    .Members(RootCommandMembers(commandDef));
            return classModel;
        }

        private static ClassModel SubCommands(CommandDef commandDef)
             => Class("Commands")
                .Public()
                .Members(SubCommandClasses(Array.Empty<CommandDef>(), commandDef.Members, commandDef));

        private static IMember[] RootCommandMembers(CommandDef commandDef)
        {
            var members = new List<IMember>();
            members.Add(
                Method(create, Helpers.CliRootName)
                    .Public()
                    .Static()
                    .Parameters(Parameter(methodToRun, "Delegate"))
                    .Statements(
                        Return(Invoke("", create))));
            members.AddRange(CommonClassMembers(Enumerable.Empty<CommandDef>(), Enumerable.Empty<MemberDef>(), commandDef.Members, commandDef));
            return members.ToArray();
        }

        private static IMember[] SubCommandClasses(IEnumerable<CommandDef> ancestors,
                                                   IEnumerable<MemberDef> ancestorMembers,
                                                   CommandDef commandDef)
            => commandDef.SubCommands
                    .OfType<CommandDef>()
                    .Select(cmd => CommandClass(ancestors, ancestorMembers, commandDef, cmd))
                    .ToArray();

        private static ClassModel CommandClass(IEnumerable<CommandDef> ancestors,
                                               IEnumerable<MemberDef> ancestorMembers,
                                               CommandDef? parent,
                                               CommandDef commandDef)
        {
            // TODO: Consider moving Ancestors and ancestor members to CommandDef. These would be null until the tree is built.
            var myAncestors = ancestors.ToList();
            if (parent is not null)
            { myAncestors.Insert(0, parent); }
            // TODO: have a force option
            var myMembers = NonAncestorMembers(ancestorMembers, commandDef);
            var newAncestorMembers = ancestorMembers.Union(myMembers).ToList();

            var commandClass = CommandClassDeclaration(commandDef)
                                    .InheritedFrom(parent is null
                                        ? GeneratedCommandBase(CommandClassName(commandDef))
                                        : GeneratedCommandBase(CommandClassName(commandDef), CommandClassName(parent)))
                                    .Members(CommonClassMembers(myAncestors, ancestorMembers, NonAncestorMembers(ancestorMembers, commandDef), commandDef)
                                        .Union(SubCommandClasses(myAncestors, newAncestorMembers, commandDef)).ToArray());
            return commandClass;

            static IEnumerable<MemberDef> NonAncestorMembers(IEnumerable<MemberDef> ancestorMembers, CommandDef commandDef)
                => commandDef.Members
                    .Where(m => !ancestorMembers.Any(a => a.Name.Equals(m.Name)))
                    .ToList();
        }

        private static ClassModel CommandClassDeclaration(CommandDef commandDef)
           => Class(CommandClassName(commandDef))
                .Public()
                .Partial()
                .ImplementedInterfaces(
                    string.IsNullOrWhiteSpace(commandDef.HandlerMethodName)
                        ? Array.Empty<NamedItemModel>()
                        : new NamedItemModel[] { "ICommandHandler" });

        private static IMember[] CommonClassMembers(IEnumerable<CommandDef> ancestors,
                                                    IEnumerable<MemberDef> ancestorMembers,
                                                    IEnumerable<MemberDef> myMembers,
                                                    CommandDef commandDef)
        {

            List<IMember> members = new()
            {
                ancestors.Any()
                    ? Constructor(CommandClassName(commandDef))
                        .Private()
                        .Parameters(Parameter("parent", CommandFullClassName(ancestors.Skip(1), null, ancestors.First())))
                        .Base(commandDef.Name, Symbol("parent"))
                    : Constructor(CommandClassName(commandDef))
                        .Private()
                        .Base(commandDef.Name),
                CreateMethod(ancestors, myMembers, commandDef),
                ResultClass(ancestorMembers, myMembers, commandDef),
                GetResultMethod(commandDef),
                InvokeHandlerMethod(commandDef),
                InvokeAsyncHandlerMethod(commandDef)
            };
            members.AddRange(myMembers
                .Where(m => m is OptionDef || m is ArgumentDef)
                .Select(m => Property(MemberPropertyName(m), Generic(MemberPropertyStyle(m), m.TypeName))
                                .Public()));

            members.AddRange(commandDef.SubCommands
                .OfType<CommandDef>()
                .Select(c => Property(CommandPropertyName(c), CommandFullClassName(ancestors, commandDef, c)).Public()));

            return members.ToArray();
        }

        private static ClassModel GetNestedCommands(int i, CommandDef commandDef)
        {
            if (i == 10) throw new IOException("Runaway recursion suspected");
            var classCode =
                Class(commandDef.Name)
                    .Public()
                    .Static()
                    .Members(
                        Method(Helpers.AddCommandName, Void())
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

        private static MethodModel InvokeAsyncHandlerMethod(CommandDef commandDef)
        {
            // This should be commandDef.Members, not myMembers because it should include parent members as requested
            var arguments = commandDef.Members
                    .Select(m => Symbol($"{result}.{m.Name}"));
            var method =
                Method("InvokeAsync", Generic("Task", "int"))
                    .Public()
                    .Parameters(Parameter("context", "InvocationContext"))
                    .Statements(
                        AssignWithDeclare(result, Invoke("", getResultName, Symbol("context"))));

            if (commandDef.ReturnType == "int")
            {
                method.Statements.Add(AssignWithDeclare("ret", Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol("ret"))));
            }
            else
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol("context.ExitCode"))));
            }
            return method;
        }

        private static MethodModel InvokeHandlerMethod(CommandDef commandDef)
        {
            // This should be commandDef.Members, not myMembers because it should include parent members as requested
            var arguments = commandDef.Members
                    .Select(m => Symbol($"{result}.{m.Name}"));
            var method =
                Method("Invoke", "int")
                    .Public()
                    .Parameters(Parameter("context", "InvocationContext"))
                    .Statements(
                        AssignWithDeclare(result, Invoke("", getResultName, Symbol("context"))));
            if (commandDef.ReturnType == "int")
            {
                method.Statements.Add(Return(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
            }
            else
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Symbol("context.ExitCode")));
            }
            return method;
        }

        private static MethodModel CreateMethod(IEnumerable<CommandDef> ancestors, IEnumerable<MemberDef> myMembers, CommandDef commandDef)
        {
            var method =
                    Method(create, CommandClassName(commandDef))
                    .Public()
                    .Static()
                    .Statements(
                        );
            if (ancestors.Any())
            {
                method.Parameters.Add(Parameter("parent", CommandFullClassName(ancestors.Skip(1), null, ancestors.First())));
                method.Statements.Add(AssignWithDeclare(commandVar, New(CommandFullClassName(ancestors, null, commandDef), Symbol("parent"))));
            }
            else
            {
                method.Statements.Add(AssignWithDeclare(commandVar, New(CommandFullClassName(ancestors, null, commandDef))));
            }
            foreach (var member in myMembers)
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
                    var toAdd = $"{commandVar}.{CommandPropertyName(subCommand)}";
                    method.Statements.Add(Assign(toAdd, Invoke(CommandFullClassName(ancestors, commandDef, subCommand), "Create", Symbol(commandVar))));
                    method.Statements.Add(SimpleCall(Invoke(commandVar, "AddCommandToScl", Symbol(toAdd))));
                }
            }
            method.Statements.Add(SimpleCall(Invoke("command.SystemCommandLineCommand", "AddValidator", Symbol("command.Validate"))));
            method.Statements.Add(Assign($"{commandVar}.Handler", Symbol(commandVar)));
            method.Statements.Add(Return(Symbol(commandVar)));
            return method;
        }

        private static ClassModel ResultClass(
                IEnumerable<MemberDef> ancestorMembers,
                IEnumerable<MemberDef> myMembers,
                CommandDef commandDef)
        {
            var resultClass =
                Class("Result")
                    .Public()
                    .Members(
                        Constructor("Result")
                            .Internal()
                            .Parameters(
                                    Parameter(command, CommandClassName(commandDef)),
                                    Parameter(name: "result", commandResult))
                            .Statements(ResultConstructorStatements(ancestorMembers, myMembers, commandDef)));
            resultClass.Members.AddRange(ancestorMembers
                    .Select(x => Property(x.Name, x.TypeName).Public()));
            resultClass.Members.AddRange(myMembers
                    .Select(x => Property(x.Name, x.TypeName).Public()));
            return resultClass;

            static IStatement[] ResultConstructorStatements(
                    IEnumerable<MemberDef> ancestorOptionsAndArguments,
                    IEnumerable<MemberDef> myMembers,
                    CommandDef commandDef)
            {
                List<IStatement> statements = new();
                if (ancestorOptionsAndArguments.Any())
                {
                    statements.Add(AssignWithDeclare(parentResult, Invoke($"{command}.Parent", "GetResult", Symbol(result))));
                }
                foreach (var member in ancestorOptionsAndArguments)
                {
                    statements.Add(Assign(member.Name, Symbol($"parentResult.{member.Name}")));
                }
                foreach (var member in myMembers)

                {
                    var methodName = member is OptionDef
                            ? "GetValueForOption"
                            : "GetValueForArgument";
                    statements.Add(Assign(member.Name, Invoke(result, methodName, Symbol($"command.{MemberPropertyName(member)}"))));
                }
                return statements.ToArray();
            }

        }

        private static MethodModel GetResultMethod(CommandDef commandDef)
            => Method(getResultName, "Result")
                .Public()
                .Override()
                .Parameters(Parameter(result, commandResult))
                .Statements(
                    Return(New("Result", This, Symbol(result))));

    }
}
