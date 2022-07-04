using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;

namespace Jackfruit.IncrementalGenerator
{
    public class LanguageCSharp : LanguageOutput
    {
        public LanguageCSharp(IWriter writer) : base(writer) { }

        public override string XmlDocPrefix => "/// ";
        public override string PrivateKeyword => "private";
        public override string PublicKeyword => "public";
        public override string InternalKeyword => "internal";
        public override string ProtectedKeyword => "protected";
        public override string ProtectedInternalKeyword => "protected internal";
        public override string PrivateProtectedKeyword => "private protected";

        public override string StaticKeyword => "static";
        public override string NewSlotKeyword => "new";
        public override string OverrideKeyword => "override";
        public override string AsyncKeyword => "async";
        public override string PartialKeyword => "partial";
        public override string AbstractKeyword => "abstract";
        public override string ReadonlyKeyword => "readonly";
        public override string SealedKeyword => "sealed";
        public override string HideByNameKeyword => "new";

        public override string UsingKeyword => "using";
        public override string NamespaceKeyword => "namespace";
        public override string ClassKeyword => "class";
        public override string ThrowKeyword => "throw";
        public override string GetKeyword => "get";
        public override string SetKeyword => "set";
        public override string IfKeyword => "if";
        public override string ReturnKeyword => "return";
        public override string AwaitKeyword => "await";
        public override string NewKeyword => "new";
        public override string NullKeyword => "null";
        public override string ThisKeyword => "this";
        public override string TrueKeyword => "true";
        public override string FalseKeyword => "false";

        public override string EqualsOperator => "==";
        public override string NotEqualsOperator => "!=";
        public override string BlockOpen => "{";
        public override string EndOfStatement => ";";
        public override string CommentPrefix => "//";

        public override string NamedItem(NamedItemModel namedItem)
            => namedItem switch
            {
                GenericNamedItemModel generic => $"{generic.Name}<{string.Join(", ", generic.GenericTypes.Select(x => NamedItem(x)))}>",
                VoidNamedItemModel => "void",
                ArrayNamedItemModel array => $"{array.Name}[]",
                null => "",
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
                ? $"using {x.Name};"
                : $"using {x.Alias} = {x.Name};");

        public override IEnumerable<string> ClassOpen(ClassModel model)
        {
            var keywords = OptionalKeyword(model.IsStatic, StaticKeyword) +
                           OptionalKeyword(model.IsAsync, AsyncKeyword) +
                           OptionalKeyword(model.IsSealed, SealedKeyword) +
                           OptionalKeyword(model.IsPartial, PartialKeyword);

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
            var keywords = OptionalKeyword(model.IsStatic, StaticKeyword) +
                           OptionalKeyword(model.IsNewSlot, NewSlotKeyword) +
                           OptionalKeyword(model.IsOverride, OverrideKeyword) +
                           OptionalKeyword(model.IsAsync, AsyncKeyword) +
                           OptionalKeyword(model.IsPartial, PartialKeyword);
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
                BaseOrThis.Base => $": base({string.Join(", ", model.BaseOrThisArguments.Select(x => Expression(x)))})",
                BaseOrThis.This => $": this({string.Join(", ", model.BaseOrThisArguments.Select(x => Expression(x)))})",
                _ => ""
            };

        public override IEnumerable<string> ConstructorOpen(ConstructorModel model)
        {
            var keywords = $"{OptionalKeyword(model.IsStatic, StaticKeyword)}";
            var ret = new List<string>
                {
                    $"{Scope(model.Scope)} {keywords}{NamedItem(model.ClassName)}({string.Join(",",Parameters(model.Parameters))})",
                };
            var baseOrThiscall = BaseOrThisCall(model);
            if(!string.IsNullOrWhiteSpace(baseOrThiscall))
            { ret.Add(baseOrThiscall); }
            ret.Add("{");
            return ret;
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
                    $"{Scope( model.Scope)}{keywords} {NamedItem( model.Type)} {model.Name};"
                };
        }

        private string PropertyKeywords(PropertyModel model)
            => $"{SpaceIfNotEmpty(OptionalKeyword(model.IsStatic, StaticKeyword))}" +
                $"{SpaceIfNotEmpty(OptionalKeyword(model.IsPartial, PartialKeyword))}";

        public override IEnumerable<string> AutoProperty(PropertyModel model)
            => new List<string>
            {
                $"{SpaceIfNotEmpty(Scope(model.Scope))}{PropertyKeywords(model)}{NamedItem(model.Type)} {model.Name} {{get; set;}}"
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

        public override IEnumerable<string> Throw(NamedItemModel exception, params ExpressionBase[] args)
            => new List<string>
                {
                    $"{ThrowKeyword} {Instantiate(exception, args)};"
                };


        // Expressions
        public override string Invoke(NamedItemModel? instance, NamedItemModel methodName, IEnumerable<ExpressionBase> arguments)
        {
            var namedItem = instance is null
                            ? null
                            : NamedItem(instance);
            var target = string.IsNullOrEmpty(namedItem)
                            ? NamedItem(methodName)
                            : $"{namedItem}.{NamedItem(methodName)}";
            return $"{target}({string.Join(", ", arguments.Select(x => Expression(x)))})";
        }

        public override string Instantiate(NamedItemModel typeName, IEnumerable<ExpressionBase> arguments)
            => $"new {NamedItem(typeName)}({string.Join(", ", arguments.Select(x => Expression(x)))})";

        public override string TypeOf(NamedItemModel typeName)
            => $"typeof({NamedItem(typeName)})";

        public override string Cast(NamedItemModel typeName,ExpressionBase expression)
            => $"({NamedItem(typeName)}){Expression(expression)}";

        public override string Compare(ExpressionBase left, Operator op, ExpressionBase right)
            => $"{Expression(left)} {Operator(op)} {Expression(right)}";

        public override string Not(ExpressionBase expression)
            => $"!({Expression(expression)})";

    }
}
