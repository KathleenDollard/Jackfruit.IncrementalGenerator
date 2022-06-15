using Jackfruit.IncrementalGenerator.CodeModels;
using static Jackfruit.IncrementalGenerator.CodeModels.StatementHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.ExpressionHelpers;
using static Jackfruit.IncrementalGenerator.CodeModels.StructureHelpers;

namespace Jackfruit.Common
{
    public static class CreateSource
    {
        private const string libName = "Jackfruit";
        private const string create = "Create";
        private const string command = "command";
        private const string result = "result";
        private const string parentResult = "parentResult";
        private const string commandVar = "command";
        private const string resultName = "Result";
        private const string getResultName = "GetResult";
        private const string commandResult = "CommandResult";
        private const string commandResultVar = "commandResult";
        private const string bindingContext = "bindingContext";
        private const string invocationContext = "invocationContext";

        private static string CommandClassName(CommandDef commandDef) => commandDef.Name;
        private static string CommandPropertyName(CommandDef commandDef) => commandDef.Name;
        private static string CommandFullClassName(IEnumerable<CommandDef> ancestors, CommandDef? parent, CommandDef commandDef)
        {
            // TODO: **** When it is clear we will not nest these types, probably remove this method and replace with direct call
            //if (commandDef.Name == CommonHelpers.CliRootName)
            //{ return CommonHelpers.CliRootName; }
            //var ancestorList =
            //        parent is null
            //        ? ancestors
            //        : new List<CommandDef> { parent }.Union(ancestors);
            //ancestorList = ancestorList
            //        .Reverse()
            //        .Where(a => a.Name != CommonHelpers.CliRootName);
            //var parentNames =
            //    ancestorList.Any()
            //        ? string.Join(".", ancestorList.Select(a => CommandClassName(a))) + "."
            //        : "";
            //return CommonHelpers.GetStyle(commandDef) == CommonHelpers.Cli
            //    ? $"{parentNames}{CommandClassName(commandDef)}"
            //    : $"Commands.{parentNames}{CommandClassName(commandDef)}";
            return CommandClassName(commandDef);
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
        private static NamedItemModel GeneratedCommandBase(string self, string parent)
            => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}", parent);
        private static NamedItemModel GeneratedCommandBase(string self)
           => new GenericNamedItemModel("GeneratedCommandBase", self, $"{self}.{resultName}");

        public static CodeFileModel? GetCliPartialCodeFile(IEnumerable<CommandDefBase> rootCommandDefs)
        {
            var roots = rootCommandDefs
                    .OfType<CommandDef>().ToList()
                    .Where(x => CommonHelpers.GetStyle(x) == CommonHelpers.Cli);
            return !roots.Any()
                ? null
                : new CodeFileModel(CommonHelpers.Cli)
                    .Usings(roots.Select(x => new UsingModel(x.Namespace)).Distinct().ToArray())
                    .Namespace("Jackfruit",
                        Class(CommonHelpers.Cli)
                            .Public()
                            .Partial()
                            .Members(
                                Constructor(CommonHelpers.Cli)
                                    .Static()
                                    .Statements(
                                        roots.Select(x => Assign(x.Name, Invoke(x.Name, create)))))
                            .Members(roots.Select(x => Property(x.Name, x.Name).Public().Static())));
        }

        public static CodeFileModel? GetCommandCodeFile(CommandDefBase rootCommandDef)
            => rootCommandDef is not CommandDef commandDef
                ? null
                : CodeFile("Commands")
                    .Usings("System",
                            "System.CommandLine",
                            "System.CommandLine.Invocation",
                            "System.CommandLine.Parsing",
                            "System.CommandLine.Binding",
                            "System.Threading.Tasks",
                            "Jackfruit.Internal",
                            libName)
                    .Namespace(commandDef.Namespace,
                        CommandClass(Enumerable.Empty<CommandDef>(),
                                     Enumerable.Empty<MemberDef>(),
                                     null,
                                     commandDef).ToArray());

        private static IEnumerable<ClassModel> CommandClass(IEnumerable<CommandDef> ancestors,
                                               IEnumerable<MemberDef> ancestorMembers,
                                               CommandDef? parent,
                                               CommandDef commandDef)
        {
            var commandClasses = new List<ClassModel>();
            // TODO: Consider moving Ancestors and ancestor members to CommandDef. These would be null until the tree is built.
            var ancestorsAndSelf = ancestors.ToList();
            if (parent is not null)
            { ancestorsAndSelf.Insert(0, parent); }
            // TODO: have a force option
            var myMembers = NonAncestorMembers(ancestorMembers, commandDef);
            var newAncestorMembers = ancestorMembers.Union(myMembers).ToList();

            string className = CommandClassName(commandDef);
            commandClasses.Add(
                Class(className)
                    .XmlDescription($"The wrapper class for the {className} command.")
                    .Public()
                    .Partial()
                    .ImplementedInterfaces(
                        string.IsNullOrWhiteSpace(commandDef.HandlerMethodName)
                            ? Array.Empty<NamedItemModel>()
                            : new NamedItemModel[] { "ICommandHandler" })
                    .InheritedFrom(parent is null
                        ? GeneratedCommandBase(className)
                        : GeneratedCommandBase(className, CommandClassName(parent)))
                    .Members(CommonClassMembers(ancestorsAndSelf, ancestorMembers, NonAncestorMembers(ancestorMembers, commandDef), commandDef)));

            commandClasses.AddRange(
                    commandDef.SubCommands
                                .OfType<CommandDef>()
                                .SelectMany(cmd => CommandClass(ancestorsAndSelf, newAncestorMembers, commandDef, cmd)));
            return commandClasses;

            static IEnumerable<MemberDef> NonAncestorMembers(IEnumerable<MemberDef> ancestorMembers, CommandDef commandDef)
                => commandDef.Members
                    .Where(m => !ancestorMembers.Any(a => a.Name.Equals(m.Name)))
                    .ToList();
        }

        private static IEnumerable<IMember> CommonClassMembers(IEnumerable<CommandDef> ancestors,
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
                GetResultMethodFromInvocationContext(commandDef),
                GetResultMethodFromCommandResult(commandDef),
                InvokeHandlerMethod(commandDef),
                InvokeAsyncHandlerMethod(commandDef)
            };

            if (commandDef.Validator is not null)
            { members.Add(ValidateMethod(commandDef.Validator)); }

            members.AddRange(myMembers
                .Where(m => m is OptionDef || m is ArgumentDef)
                .Select(m => Property(MemberPropertyName(m), Generic(MemberPropertyStyle(m), m.TypeName))
                                .Public()));

            members.AddRange(commandDef.SubCommands
                .OfType<CommandDef>()
                .Select(c => Property(CommandPropertyName(c), CommandFullClassName(ancestors, commandDef, c)).Public()));

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
            return AssignWithDeclare(result, Invoke("", getResultName, Symbol(invocationContext)));
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

        private static MethodModel CreateMethod(IEnumerable<CommandDef> ancestors, IEnumerable<MemberDef> myMembers, CommandDef commandDef)
        {
            var method =
                    Method(create, CommandClassName(commandDef))
                    .Internal()
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
                        foreach (var alias in opt.Aliases)
                        {
                            method.Statements.Add(SimpleCall(Invoke(optPropertyName, "AddAlias", alias)));
                        }
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
                    .XmlDescription($"The result class for the {CommandClassName(commandDef)} command.")
                    .Public()
                    .Members(ResultConstructors(ancestorMembers,myMembers,commandDef));
            resultClass.Members.AddRange(ancestorMembers
                    .Select(x => Property(x.Name, x.TypeName).Public()));
            resultClass.Members.AddRange(myMembers
                    .Select(x => Property(x.Name, x.TypeName).Public()));
            return resultClass;

            static ParameterModel[] ResultConstructorParameters(CommandDef commandDef)
            {
                var parameters = new List<ParameterModel>
                {
                    Parameter(command, CommandClassName(commandDef)),
                    Parameter(name: commandResultVar, "CommandResult")
                };
                if (commandDef.Parent is not null)
                {
                    parameters.Add(Parameter(parentResult, $"{CommandClassName(commandDef.Parent)}.Result"));
                }
                return parameters.ToArray();
            }

            static ConstructorModel[] ResultConstructors(
                IEnumerable<MemberDef> ancestorMembers,
                IEnumerable<MemberDef> myMembers,
                CommandDef commandDef)
            {
                var constructors = new List<MemberDef>();
                if (commandDef.Parent is not null)
                {
                    return new ConstructorModel[]
                    {
                    Constructor("Result")
                         .Internal()
                         .Parameters(
                                 Parameter(command, CommandClassName(commandDef)),
                                 Parameter(name: invocationContext, "InvocationContext"))
                         .This(Symbol(command), Symbol($"{invocationContext}.ParseResult.CommandResult"), Invoke($"{command}.Parent", "GetResult", Symbol(invocationContext)))
                         .Statements(ServiceConstructorStatements(myMembers, commandDef)),
                    Constructor("Result")
                        .Internal()
                        .Parameters(
                                Parameter(command, CommandClassName(commandDef)),
                                Parameter(name: result, "CommandResult"))
                        .This(Symbol(command), Symbol(result), Invoke($"{command}.Parent", "GetResult", Symbol(result))),
                    Constructor("Result")
                        .Private()
                        .Parameters(ResultConstructorParameters(commandDef))
                        .Statements(ResultConstructorStatements(ancestorMembers, myMembers, commandDef))
                    };
                }
                else
                {
                    return new ConstructorModel[]
                    {
                    Constructor("Result")
                         .Internal()
                         .Parameters(
                                 Parameter(command, CommandClassName(commandDef)),
                                 Parameter(name: invocationContext, "InvocationContext"))
                         .This(Symbol(command), Symbol($"{invocationContext}.ParseResult.CommandResult"))
                         .Statements(ServiceConstructorStatements(myMembers, commandDef)),
                    Constructor("Result")
                        .Internal()
                        .Parameters(ResultConstructorParameters(commandDef))
                        .Statements(ResultConstructorStatements(ancestorMembers, myMembers, commandDef))
                    };
                }
            }

            static IEnumerable<IStatement> ServiceConstructorStatements(
                    IEnumerable<MemberDef> myMembers,
                    CommandDef commandDef)
                => myMembers
                    .OfType<ServiceDef>()
                    .Select(service =>
                        Assign(service.Name, Invoke(null, $"GetService<{service.TypeName}>", Symbol(invocationContext))));

            static IStatement[] ResultConstructorStatements(
                    IEnumerable<MemberDef> ancestorOptionsAndArguments,
                    IEnumerable<MemberDef> myMembers,
                    CommandDef commandDef)
            {
                List<IStatement> statements = new();
                foreach (var member in ancestorOptionsAndArguments)
                {
                    statements.Add(Assign(member.Name, Symbol($"parentResult.{member.Name}")));
                }
                foreach (var member in myMembers.Where(x=>x is not ServiceDef))
                {

                    statements.Add(Assign(member.Name, Invoke(null, "GetValueForSymbol", Symbol($"command.{MemberPropertyName(member)}"), Symbol(commandResultVar))));
                }
                return statements.ToArray();
            }


            //internal Result(Voyager command, CommandResult result)
            //{
            //    Greeting = parentResult.Greeting;
            //    Kirk = parentResult.Kirk;
            //    Spock = parentResult.Spock;
            //    Uhura = parentResult.Uhura;
            //    Picard = parentResult.Picard;

            //    Janeway = GetValueForSymbol(command.JanewayOption, result);
            //    Chakotay = GetValueForSymbol(command.ChakotayOption, result);
            //    Torres = GetValueForSymbol(command.TorresOption, result);
            //    Tuvok = GetValueForSymbol(command.TuvokOption, result);
            //    SevenOfNine = GetValueForSymbol(command.SevenOfNineOption, result);
            //}

            //internal Result(Voyager command, InvocationContext invocationContext)
            //    : this(command, invocationContext.ParseResult.CommandResult)
            //{
            //    Console = GetService<System.CommandLine.IConsole>(invocationContext);
            //}


            //private Result(Voyager command, CommandResult result, NextGeneration.Result parentResult)
            //{
            //    Greeting = parentResult.Greeting;
            //    Kirk = parentResult.Kirk;
            //    Spock = parentResult.Spock;
            //    Uhura = parentResult.Uhura;
            //    Picard = parentResult.Picard;

            //    Janeway = GetValueForSymbol(command.JanewayOption, result);
            //    Chakotay = GetValueForSymbol(command.ChakotayOption, result);
            //    Torres = GetValueForSymbol(command.TorresOption, result);
            //    Tuvok = GetValueForSymbol(command.TuvokOption, result);
            //    SevenOfNine = GetValueForSymbol(command.SevenOfNineOption, result);
            //}

            //internal Result(Voyager command, InvocationContext invocationContext)
            //    : this(command, invocationContext.ParseResult.CommandResult, command.Parent.GetResult(invocationContext))
            //{
            //    Console = GetService<System.CommandLine.IConsole>(invocationContext);
            //}

            //internal Result(Voyager command, CommandResult result)
            //    : this(command, result, command.Parent.GetResult(result))
            //{ }
        }

        private static MethodModel GetResultMethodFromInvocationContext(CommandDef commandDef)
            => GetResultMethod(
                $"Get an instance of the Result class for the {CommandClassName(commandDef)} command.",
                invocationContext, "InvocationContext", "The System.CommandLine InvocationContext used to retrieve values.");

        private static MethodModel GetResultMethodFromCommandResult(CommandDef commandDef)
            => GetResultMethod(
                $"Get an instance of the Result class for the {CommandClassName(commandDef)} command that will not include any services.",
                result, "CommandResult", "The System.CommandLine CommandResult used to retrieve values.");

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
