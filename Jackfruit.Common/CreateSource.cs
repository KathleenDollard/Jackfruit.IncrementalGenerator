using Jackfruit.IncrementalGenerator.CodeModels;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StructureHelpers;

namespace Jackfruit.Common
{
    public static class CreateSource
    {
        private const string command = "command";
        private const string result = "result";
        private const string commandVar = "command";
        private const string resultName = "Result";
        private const string getResultName = "GetResult";
        private const string commandResultVar = "commandResult";
        private const string invocationContext = "invocationContext";

        private static bool IsRoot(CommandDef commandDef)
            => string.IsNullOrWhiteSpace(commandDef.Parent);
        private static string CommandClassName(CommandDef commandDef)
            => IsRoot(commandDef)
                    ? "RootCommand"
                    : commandDef.Name;
        private static string CommandPropertyName(CommandDef commandDef) => commandDef.Name;
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


        private static NamedItemModel GeneratedCommandBase(string self, string parent)
            => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}", parent);
        private static NamedItemModel GeneratedCommandBase(string self)
           => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}");
        private static UsingModel[] usings = new UsingModel[]
            {
                "System",
                "System.Threading.Tasks",
                "System.CommandLine",
                "System.CommandLine.Invocation",
                "System.CommandLine.Parsing",
                "Jackfruit.Internal",
            };

        public static CodeFileModel? GetRootCommandPartialCodeFile(CommandDef rootCommandDef)
        {
            // TODO: Work this out for the generic version, particularly the name
            return rootCommandDef is null
                ? null
                : new CodeFileModel($"{CommonHelpers.RootCommand}.g.cs")
                    .Usings(new UsingModel(rootCommandDef.Namespace))
                    .Namespace("Jackfruit",
                        Class(CommonHelpers.RootCommand)
                            .Public()
                            .Partial()
                            .InheritedFrom(new GenericNamedItemModel(CommonHelpers.RootCommand, $"{CommonHelpers.RootCommand}.Result"))
                            .Members(
                                Method("Create", CommonHelpers.RootCommand)
                                    .Static()
                                    .NewSlot()
                                    .Parameters(
                                        Parameter("rootNode", "CommandNode"))
                                    .Statements(
                                        Return(Invoke("RootCommand<RootCommand, RootCommand.Result>",
                                             "Create",
                                             "rootNode")))));
        }

        public static CodeFileModel? GetCommandCodeFile(CommandDefBase commandDefBase)
        {
            if (commandDefBase is not CommandDef commandDef)
            { return null; }

            var isRoot = commandDef.Parent is null;
            return commandDef.Parent is null
                ? GetRootCommandCode(commandDef)
                : GetNonRootCommandCode(commandDef);
        }

        private static CodeFileModel? GetRootCommandCode(CommandDef commandDef)
        {
            // TODO: This needs to include the generic to be unique
            var fileName = "RootCommand";
            // TODO: The rootcommand name should include any generic - might move this to the commandDef
            var className = CommandClassName(commandDef);

            return CodeFile(fileName)
                .Usings(usings)
                .Usings($"Jackfruit.{commandDef.Namespace}")
                .Namespace("Jackfruit",
                    Class(className)
                        .Public().Partial()
                        .ImplementedInterfaces("ICommandHandler")
                        .Members(RootConstructor(commandDef))
                        .Members(CommonClassMembers(commandDef)));
        }

        public static CodeFileModel? GetNonRootCommandCode(CommandDef commandDef)
        {
            // TODO: This needs to be unique within the project, like fully qualified
            var fileName = commandDef.Name;
            var className = CommandClassName(commandDef);

            return CodeFile(fileName)
                .Usings(usings)
                .Namespace((string?)commandDef.Namespace,
                    Class(className)
                        .Public().Partial()
                        .XmlDescription((string?)$"The wrapper class for the {className} command.")
                        .ImplementedInterfaces(
                            string.IsNullOrWhiteSpace(commandDef.HandlerMethodName)
                                ? Array.Empty<NamedItemModel>()
                                : new NamedItemModel[] { "ICommandHandler" })
                        .InheritedFrom((NamedItemModel?)GeneratedCommandBase(className, commandDef.Parent!))
                        .Members(BuildMethod(commandDef))
                        .Members(CommonClassMembers(commandDef)));
        }

        private static IEnumerable<IMember> CommonClassMembers(CommandDef commandDef)
        {
            List<IMember> members = new()
            {
                ResultClass(commandDef),
                InvokeHandlerMethod(commandDef),
                InvokeAsyncHandlerMethod(commandDef)
            };

            if (commandDef.Validator is not null)
            { members.Add(ValidateMethod(commandDef.Validator)); }

            members.AddRange(commandDef.MyMembers
                .Where(m => m is OptionDef || m is ArgumentDef)
                .Select(m => Property(MemberPropertyName(m), Generic(MemberPropertyStyle(m), m.TypeName))
                                .Public()));

            members.AddRange(commandDef.SubCommandNames
                .Select(c => Property(c, c).Public()));

            return members;
        }

        private static MethodModel InvokeAsyncHandlerMethod(CommandDef commandDef)
        {
            // This should be commandDef.Members, not myMembers because it should include parent members as requested
            var arguments = commandDef.Members
                    .Select(m => Symbol($"{result}.{m.Name}"));
            var method =
                Method("InvokeAsync", Generic("Task", "int"))
                    .XmlDescription("The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.")
                    .Public()
                    .Parameters(
                        Parameter(invocationContext, "InvocationContext")
                            .XmlDescription("The System.CommandLine Invocation context used to retrieve values."))
                    .Statements(GetResultFromInvocationContext());

            if (commandDef.ReturnType == "int")
            {
                method.Statements.Add(AssignWithDeclare("ret", Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol("ret"))));
            }
            else
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Invoke("Task", "FromResult", Symbol($"{invocationContext}.ExitCode"))));
            }
            return method;
        }

        private static AssignWithDeclareModel GetResultFromInvocationContext()
        {
            return AssignWithDeclare(result,
                Invoke("Result", getResultName, This, Symbol(invocationContext)));
        }

        private static MethodModel InvokeHandlerMethod(CommandDef commandDef)
        {
            // This should be commandDef.Members, not myMembers because it should include parent members as requested
            var arguments = commandDef.Members
                    .Select(m => Symbol($"{result}.{m.Name}"));
            var method =
                Method("Invoke", "int")
                    .XmlDescription("The handler invoked by System.CommandLine. This will not be public when generated is more sophisticated.")
                    .Public()
                    .Parameters(
                        Parameter(invocationContext, "InvocationContext")
                            .XmlDescription("The System.CommandLine Invocation context used to retrieve values."))
                    .Statements(GetResultFromInvocationContext());
            if (commandDef.ReturnType == "int")
            {
                method.Statements.Add(Return(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
            }
            else
            {
                method.Statements.Add(SimpleCall(Invoke("", commandDef.HandlerMethodName, arguments.ToArray())));
                method.Statements.Add(Return(Symbol($"{invocationContext}.ExitCode")));
            }
            return method;
        }

        private static MethodModel ValidateMethod(ValidatorDef validatorDef)
        {
            var arguments = validatorDef.Members.Select(x => Symbol($"{result}.{x.Name}")).ToArray();
            return
                Method("Validate", "void")
                    .XmlDescription("The validate method invoked by System.CommandLine.")
                    .Public()
                    .Override()
                    .Parameters(
                        Parameter(commandResultVar, "CommandResult")
                            .XmlDescription("The System.CommandLine CommandResult used to retrieve values for validation and it will hold any errors."))
                    .Statements(
                        // TODO: Fix invoke to take base in langugae neutral way
                        SimpleCall(Invoke("base", "Validate", Symbol(commandResultVar))),
                        AssignWithDeclare(result, Invoke("", getResultName, Symbol(commandResultVar))),
                        AssignWithDeclare("err", Invoke("string", "Join",
                                    Symbol("Environment.NewLine"),
                                    Invoke("", validatorDef.MethodName, arguments))),
                        If(Not(Invoke("string", "IsNullOrWhiteSpace", Symbol("err"))),
                                Assign($"{commandResultVar}.ErrorMessage", Symbol("err"))));

        }

        private static ConstructorModel RootConstructor(CommandDef commandDef) 
            => Constructor("RootCommand")
                .Public()
                .Statements(BuildStatements(commandDef).ToArray());

        private static MethodModel BuildMethod(CommandDef commandDef)
        {
            var method =
                    Method("Build", CommandClassName(commandDef))
                        .Internal().Static();

            method.Parameters.Add(Parameter("parent", commandDef.Parent ?? ""));
            method.Statements.Add(AssignWithDeclare(commandVar, New(CommandClassName(commandDef), Symbol("parent"))));

            method.Statements.AddRange(BuildStatements(commandDef).ToArray());
            return method;
        }

        private static IEnumerable<IStatement> BuildStatements(CommandDef commandDef)
        {
            var statements = new List<IStatement>();
            var targetVar = IsRoot(commandDef)
                ? ""
                : commandVar;
            var target = IsRoot(commandDef) ? "" :  $"{targetVar}.";
            var thisOrCommand = IsRoot(commandDef)
                ? (ExpressionBase)This
                : Symbol(commandVar);
            statements.Add(Assign($"{target}Name", commandDef.Name));

            foreach (var member in commandDef.MyMembers)
            {
                switch (member)
                {
                    case OptionDef opt:
                        var optPropertyName = $"{target}{MemberPropertyName(opt)}";
                        statements.Add(Assign(optPropertyName, New(Generic("Option", opt.TypeName), $"--{opt.Name}")));
                        if (!string.IsNullOrWhiteSpace(opt.Description))
                        { statements.Add(Assign($"{optPropertyName}.Description", opt.Description)); }
                        foreach (var alias in opt.Aliases)
                        {
                            statements.Add(SimpleCall(Invoke(optPropertyName, "AddAlias", alias)));
                        }
                        if (!string.IsNullOrWhiteSpace(opt.ArgDisplayName))
                        { statements.Add(Assign($"{optPropertyName}.ArgDisplayName", opt.ArgDisplayName)); }
                        if (opt.Required)
                        { statements.Add(Assign($"{optPropertyName}.Required", opt.Required)); }
                        statements.Add(SimpleCall(Invoke(targetVar, "Add", Symbol(optPropertyName))));
                        break;
                    case ArgumentDef arg:
                        var argPropertyName = $"{target}{MemberPropertyName(arg)}";
                        statements.Add(Assign(argPropertyName, New(Generic("Argument", arg.TypeName), arg.Id)));
                        if (arg.Required)
                        { statements.Add(Assign($"{argPropertyName}.Required", arg.Required)); }
                        statements.Add(SimpleCall(Invoke(targetVar, "Add", Symbol(argPropertyName))));
                        break;
                    default:
                        break;
                }
            }
            foreach (var subCommandName in commandDef.SubCommandNames)
            {
                if (!string.IsNullOrWhiteSpace(subCommandName))
                {
                    var toAdd = $"{target}{subCommandName}";
                    statements.Add(Assign(toAdd, Invoke(subCommandName, "Build",thisOrCommand)));
                    statements.Add(SimpleCall(Invoke(targetVar, "AddCommandToScl", Symbol(toAdd))));
                }
            }
            statements.Add(SimpleCall(Invoke(target.Replace(".", ""),
                                             "AddValidator",
                                             Symbol($"{target}Validate"))));
            statements.Add(Assign($"{target}Handler", thisOrCommand));
            if (!IsRoot(commandDef))
            { statements.Add(Return(Symbol(commandVar))); }

            return statements;
        }

        private static ClassModel ResultClass(CommandDef commandDef)
        {
            var resultClass =
                Class("Result")
                    .XmlDescription($"The result class for the {commandDef.Name} command.")
                    .Public()
                    .Members(
                        DataMembers(commandDef).ToArray())
                    .Members(
                        GetResultMethod(commandDef),
                        ResultCommandResultConstructor( commandDef));

            if (commandDef.Parent is null)
            { resultClass.Members.Add(ResultInvocationConstructor( commandDef)); }

            return resultClass;

            static MethodModel GetResultMethod(CommandDef commandDef)
                => Method("GetResult", "Result")
                    .XmlDescription("Get an instance of the Result class for the NextGeneration command.")
                    .Internal().Static()
                    .Parameters(
                        Parameter("command", commandDef.Name)
                            .XmlDescription("The command corresponding to the result"),
                        Parameter("invocationContext", "InvocationContext")
                            .XmlDescription("The System.CommandLine InvocationContext used to retrieve."))
                    .Statements(
                        Return(New("Result", Symbol("command"), Symbol("invocationContext.ParseResult.CommandResult"))));

            static ConstructorModel ResultInvocationConstructor(CommandDef commandDef)
            {
                var args = new List<ExpressionBase>
                {
                    Symbol(command),
                    Symbol($"{invocationContext}.ParseResult.CommandResult"),
                };
                if (!IsRoot(commandDef))
                { args.Add(Invoke($"{command}.Parent", "GetResult", Symbol(invocationContext))); }
                return Constructor("Result")
                               .PrivateProtected()
                               .Parameters(
                                       Parameter(command, CommandClassName(commandDef)),
                                       Parameter(name: invocationContext, "InvocationContext"))
                               .This(args.ToArray())
                               .Statements(ServiceConstructorStatements( commandDef));
            }

            static ConstructorModel ResultCommandResultConstructor( CommandDef commandDef)
            {
                var ctor = Constructor("Result")
                    .PrivateProtected()
                    .Parameters(
                        Parameter(command, CommandClassName(commandDef)),
                        Parameter(name: commandResultVar, "CommandResult"))
                    .Statements(ResultConstructorStatements( commandDef));
                if (commandDef.Parent is not null)
                { ctor.Base(command, commandResultVar); }
                return ctor;
            }

            static IEnumerable<IStatement> ServiceConstructorStatements(   CommandDef commandDef)
                => commandDef.MyMembers
                    .OfType<ServiceDef>()
                    .Select(service =>
                        Assign(service.Name, Invoke(null, $"GetService<{service.TypeName}>", Symbol(invocationContext))));

            static IStatement[] ResultConstructorStatements(CommandDef commandDef)
            {
                List<IStatement> statements = new();
                foreach (var member in commandDef.MyMembers.Where(x => x is not ServiceDef))
                {
                    statements.Add(Assign(member.Name, Invoke(null, "GetValueForSymbol", Symbol($"command.{MemberPropertyName(member)}"), Symbol(commandResultVar))));
                }
                return statements.ToArray();
            }

            static IMember[] DataMembers(CommandDef commandDef)
            {
                List<IMember> members = new();
                foreach (var member in commandDef.MyMembers)
                {
                    members.Add(
                        Property(member.Name, member.TypeName)
                            .Public());
                }
                return members.ToArray();
            }
        }

        private static MethodModel GetResultMethod(string desc, string varName, NamedItemModel typeName, string paramDesc)
            => Method(getResultName, "Result")
                    .XmlDescription(desc)
                    .Public()
                    .Override()
                    .Parameters(
                        Parameter(varName, typeName)
                        .XmlDescription(paramDesc))
                    .Statements(
                        Return(New("Result", This, Symbol(varName))));
    }
}
