using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;

namespace Jackfruit.IncrementalGenerator
{
    public class LanguageCSharp : LanguageOutput
    {
        public LanguageCSharp(IWriter writer) : base(writer) { }

        public override string PrivateKeyword { get; } = "private";
        public override string PublicKeyword { get; } = "public";
        public override string InternalKeyword { get; } = "internal";
        public override string ProtectedKeyword { get; } = "protected";
        public override string ProtectedInternalKeyword { get; } = "protected internal";
        public override string PrivateProtectedKeyword { get; } = "private protected";

        public override string StaticKeyword { get; } = "static";
        public override string AsyncKeyword { get; } = "async";
        public override string PartialKeyword { get; } = "partial";
        public override string AbstractKeyword { get; } = "abstract";
        public override string ReadonlyKeyword { get; } = "readonly";
        public override string SealedKeyword { get; } = "sealed";
        public override string HideByNameKeyword { get; } = "new";

        public override string UsingKeyword { get; } = "using";
        public override string NamespaceKeyword { get; } = "namespace";
        public override string ClassKeyword { get; } = "class";
        public override string GetKeyword { get; } = "get";
        public override string SetKeyword { get; } = "set";
        public override string IfKeyword { get; } = "if";
        public override string ReturnKeyword { get; } = "return";
        public override string AwaitKeyword { get; } = "await";
        public override string NewKeyword { get; } = "new";
        public override string NullKeyword { get; } = "null";
        public override string ThisKeyword { get; } = "this";
        public override string TrueKeyword { get; } = "true";
        public override string FalseKeyword { get; } = "false";

        public override string EqualsOperator { get; } = "==";
        public override string NotEqualsOperator { get; } = "!=";
        public override string BlockOpen { get; } = "{";
        public override string EndOfStatement { get; } = ";";
        public override string CommentPrefix { get; } = "//";

        public override string NamedItem(NamedItemModel namedItem)
            => namedItem switch
            {
                GenericNamedItemModel generic => $"{generic.Name}<{string.Join(", ", generic.GenericTypes.Select(x => NamedItem(x)))}>",
                _ => namedItem.Name
            };


        public override string Operator(Operator op)
            => op switch
            {
                CodeModels.Operator.Equals => "==",
                CodeModels.Operator.NotEquals => "!=",
                CodeModels.Operator.GreaterThan => ">",
                CodeModels.Operator.LessThan => "<",
                CodeModels.Operator.GreaterThanOrEqualTo => ">=",
                CodeModels.Operator.LessThanOrEqualTo => "<=",
                _ => throw new InvalidOperationException()
            };

        public override IEnumerable<string> Comments(IEnumerable<string> comments)
           => comments.Select(x => $"// {x}");

        public override IEnumerable<string> NamespaceOpen(NamespaceModel? nspace)
            => nspace is null
                ? new List<string>()
                : new List<string>
                    {
                        "",
                        $"namespace {nspace.Name}",
                        "{"
                    };

        public override IEnumerable<string> NamespaceClose(NamespaceModel? nspace)
            => nspace is null
                ? new List<string>()
                : new List<string>
                    {
                        "}",
                    };

        public override IEnumerable<string> Usings(List<UsingModel> usings)
            => usings.Select(x =>
                x.Alias is null
                ? $"using {x.Name}"
                : $"using {x.Alias} = {x.Name}");

        public override IEnumerable<string> ClassOpen(ClassModel model)
        {
            var keywords = $"{OptionalKeyword(model.IsStatic, StaticKeyword)}" +
                $"{OptionalKeyword(model.IsAsync, AsyncKeyword)}" +
                $"{OptionalKeyword(model.IsSealed, SealedKeyword)}" +
                $"{OptionalKeyword(model.IsPartial, PartialKeyword)}";

            var baseAndInterfacesList = model.ImplementedInterfaces.Select(x => NamedItem(x));
            baseAndInterfacesList =
                model.InheritedFrom is null
                    ? baseAndInterfacesList
                    : new List<string> { NamedItem(model.InheritedFrom) }.Union(baseAndInterfacesList);
            var baseAndInterfaces =
                baseAndInterfacesList.Any()
                    ? $" : {string.Join(", ", baseAndInterfacesList)}"
                    : $"";

            return new List<string>
                {
                    $"{Scope(model.Scope)} {keywords}class {NamedItem(model.Name)}{baseAndInterfaces}",
                    "{"
                };
        }

        public override IEnumerable<string> ClassClose(ClassModel model)
            => new List<string>
                {
                    "}",
                    ""
                };

        public override IEnumerable<string> MethodOpen(MethodModel model)
        {
            var keywords = $"{OptionalKeyword(model.IsStatic, StaticKeyword)}" +
                $"{OptionalKeyword(model.IsAsync, AsyncKeyword)}" +
                $"{OptionalKeyword(model.IsPartial, PartialKeyword)}";
            return new List<string>
                {
                    $"{Scope(model.Scope)} {keywords}{NamedItem(model.ReturnType)} {NamedItem(model.Name)}({Parameters(model.Parameters)})",
                    "{"
                };
        }

        private string Parameters(IEnumerable<ParameterModel> parameters)
            => string.Join(", ", parameters.Select(param =>
                param.Style switch
                {
                    ParameterStyle.Normal => $"{NamedItem(param.Type)} {param.Name}",
                    ParameterStyle.DefaultValue => $"{NamedItem(param.Type)} {param.Name} = {param.DefaultValue}",
                    ParameterStyle.IsParamArray => $"params {NamedItem(param.Type)}[] {param.Name}",
                    _ => "",
                }));

        public override IEnumerable<string> MethodClose(MethodModel model)
            => new List<string>
                {
                    "}",
                    ""
                };

        public override string BaseOrThisCall(ConstructorModel model)
            => model.BaseOrThis switch
            {
                BaseOrThis.None => "",
                BaseOrThis.Base => $" : base({string.Join(", ", model.BaseOrThisArguments.Select(x => Expression(x)))})",
                BaseOrThis.This => $" : base({string.Join(", ", model.BaseOrThisArguments.Select(x => Expression(x)))})",
                _ => ""
            };

        public override IEnumerable<string> ConstructorOpen(ConstructorModel model)
        {
            var keywords = $"{OptionalKeyword(model.IsStatic, StaticKeyword)}";
            return new List<string>
                {
                    $"{Scope(model.Scope)} {keywords}{NamedItem(model.ClassName)}({string.Join(",",Parameters(model.Parameters))}){BaseOrThisCall(model)}",
                    "{"
                };

        }

        public override IEnumerable<string> ConstructorClose(ConstructorModel model)
            => new List<string>
                {
                    "}",
                    ""
                };

        public override IEnumerable<string> Field(FieldModel model)
        {
            var keywords = $"{OptionalKeyword(model.IsStatic, StaticKeyword)}" +
                $"{OptionalKeyword(model.IsStatic, StaticKeyword)}" +
                $"{OptionalKeyword(model.IsReadonly, ReadonlyKeyword)}";

            return new List<string>
                {
                    $"{Scope( model.Scope)}{keywords} {NamedItem( model.Type)} {model.Name}"
                };
        }

        private string PropertyKeywords(PropertyModel model)
            => $"{OptionalKeyword(model.IsStatic, StaticKeyword)}" +
                $"{OptionalKeyword(model.IsPartial, PartialKeyword)}";

        public override IEnumerable<string> AutoProperty(PropertyModel model)
            => new List<string>
            {
                $"{Scope( model.Scope)}{PropertyKeywords(model)} {NamedItem( model.Type)} {model.Name} {{get; set;}}"
            };

        public override IEnumerable<string> PropertyOpen(PropertyModel model)
            => new List<string>
            {
                $"{Scope(model.Scope)}{PropertyKeywords(model)} {NamedItem(model.Type)} {model.Name}",
                "{"
            };

        public override IEnumerable<string> PropertyClose(PropertyModel model)
            => new List<string>
                {
                    "}",
                };

        public override IEnumerable<string> GetterOpen(PropertyModel model)
            => new List<string>
            {
                "get",
                "{"
            };

        public override IEnumerable<string> GetterClose(PropertyModel model)
            => new List<string>
                {
                    "}",
                };

        public override IEnumerable<string> SetterOpen(PropertyModel model)
            => new List<string>
            {
                "set",
                "{"
            };

        public override IEnumerable<string> SetterClose(PropertyModel model)
            => new List<string>
                {
                    "}",
                };





        public static string OptionalKeyword(bool shouldOutputIt, string it)
            => shouldOutputIt
                ? $"{it} "
                : "";

        public string Scope(Scope scope)
        {
            return scope switch
            {
                CodeModels.Scope.Unknown => "",
                CodeModels.Scope.Public => PublicKeyword,
                CodeModels.Scope.Internal => InternalKeyword,
                CodeModels.Scope.Protected => ProtectedKeyword,
                CodeModels.Scope.Private => PrivateKeyword,
                CodeModels.Scope.ProtectedInternal => ProtectedInternalKeyword,
                CodeModels.Scope.PrivateProtected => PrivateProtectedKeyword,
                _ => throw new NotImplementedException()
            };
        }


        // Statements
        public override IEnumerable<string> IfOpen(ExpressionBase condition)
            => new List<string>
                {
                    $"if ({Expression(condition)})",
                    "{"
                };

        public override IEnumerable<string> ElseIfOpen(ExpressionBase condition)
            => new List<string>
                {
                    "}",
                    $"else if ({Expression(condition)})",
                    "{"
                };

        public override IEnumerable<string> ElseOpen()
            => new List<string>
                {
                    "}",
                    $"else",
                    "{"
                };

        public override IEnumerable<string> IfClose()
            => new List<string>
                {
                    "}",
                };

        public override IEnumerable<string> ForEachOpen(string loopVar, ExpressionBase loopOver)
            => new List<string>
                {
                    "{",
                    $"foreach (var {loopVar} in {Expression(loopOver)})",
                };


        public override IEnumerable<string> ForEachClose()
            => new List<string>
                {
                    "}",
                };

        public override IEnumerable<string> Assign(string variable, ExpressionBase value)
            => new List<string>
                {
                    $"{variable} = {Expression(value)};"
                };

        public override IEnumerable<string> AssignWithDeclare(NamedItemModel? typeName, string variable, ExpressionBase value)
        {
            var useType = typeName is null
                    ? "var"
                    : NamedItem(typeName);
            return new List<string>
                {
                    $"{useType} {variable} = {Expression(value)};"
                };
        }

        public override IEnumerable<string> Return(ExpressionBase expression)
            => new List<string>
                {
                    $"return {Expression(expression)};"
                };


        public override IEnumerable<string> SimpleCall(ExpressionBase expression)
            => new List<string>
                {
                    $"{Expression(expression)};"
                };

        public override IEnumerable<string> Comment(string text)
            => new List<string>
                {
                    $"// {text}"
                };

        // Expressions
        public override string Invoke(NamedItemModel instance, NamedItemModel methodName, IEnumerable<ExpressionBase> arguments)
            => $"{NamedItem(instance)}.{NamedItem(methodName)}({string.Join(", ", arguments.Select(x=>Expression(x)))})";

        public override string Instantiate(NamedItemModel typeName, IEnumerable<ExpressionBase> arguments)
            => $"new {NamedItem(typeName)}({string.Join(", ", arguments.Select(x => Expression(x)))})";


        public override string Compare(ExpressionBase left, Operator op, ExpressionBase right)
            => $"{Expression(left)} {Operator(op)} {Expression(right)}";

    }
}
