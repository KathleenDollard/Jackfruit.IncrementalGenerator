using Jackfruit.IncrementalGenerator.CodeModels;
using Jackfruit.IncrementalGenerator.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator
{
    public interface IOutput
    {
        string Output();
    }

    public abstract class LanguageOutput
    {
        private readonly IWriter writer;

        public string SpaceIfNotEmpty(string value)
            => string.IsNullOrWhiteSpace(value)
                ? ""
                : $"{value} ";

        public LanguageOutput(IWriter writer)
        {
            this.writer = writer;
        }

        public string UnknownKeyword { get; } = "<UNKNOWN>";

        public abstract string XmlDocPrefix { get; }
        public abstract string PrivateKeyword { get; }
        public abstract string PublicKeyword { get; }
        public abstract string InternalKeyword { get; }
        public abstract string ProtectedKeyword { get; }
        public abstract string ProtectedInternalKeyword { get; }
        public abstract string PrivateProtectedKeyword { get; }

        public abstract string StaticKeyword { get; }
        public abstract string NewSlotKeyword { get; }
        public abstract string AsyncKeyword { get; }
        public abstract string OverrideKeyword { get; }
        public abstract string PartialKeyword { get; }
        public abstract string AbstractKeyword { get; }
        public abstract string ReadonlyKeyword { get; }
        public abstract string SealedKeyword { get; }
        public abstract string HideByNameKeyword { get; }

        public abstract string TrueKeyword { get; }
        public abstract string FalseKeyword { get; }

        public abstract string UsingKeyword { get; }
        public abstract string NamespaceKeyword { get; }
        public abstract string ClassKeyword { get; }
        public abstract string ThrowKeyword { get; }
        public abstract string GetKeyword { get; }
        public abstract string SetKeyword { get; }
        public abstract string IfKeyword { get; }
        public abstract string ReturnKeyword { get; }
        public abstract string AwaitKeyword { get; }
        public abstract string NewKeyword { get; }
        public abstract string NullKeyword { get; }
        public abstract string ThisKeyword { get; }

        public abstract string EqualsOperator { get; }
        public abstract string NotEqualsOperator { get; }
        public abstract string BlockOpen { get; }
        public abstract string EndOfStatement { get; }
        public abstract string CommentPrefix { get; }

        public abstract string BaseOrThisCall(ConstructorModel model);

        //abstract member TypeAndName: typeName: NamedItem -> name: string -> string
        //abstract member BlockClose: blockType: string -> string

        //abstract member Generic: typeNames: NamedItem list -> string

        //abstract member IfOpen: IfModel -> string list
        //abstract member ElseIfOpen: ElseIfModel -> string list
        //abstract member ElseOpen: ElseModel -> string list
        //abstract member ForEachOpen: ForEachModel -> string list
        //abstract member ForEachClose : ForEachModel -> string list

        //abstract member AssignWithDeclare: AssignWithDeclareModel -> string list
        //abstract member CompilerDirective: CompilerDirectiveModel -> string list

        public abstract IEnumerable<string> Comments(IEnumerable<string> comments);
        public abstract IEnumerable<string> NamespaceClose(NamespaceModel? nspacce);
        public abstract IEnumerable<string> NamespaceOpen(NamespaceModel? nspace);
        public abstract IEnumerable<string> Usings(List<UsingModel> usings);
        public abstract IEnumerable<string> ClassOpen(ClassModel model);
        public abstract IEnumerable<string> ClassClose(ClassModel model);
        public abstract IEnumerable<string> MethodOpen(MethodModel model);
        public abstract IEnumerable<string> MethodClose(MethodModel model);
        public abstract IEnumerable<string> ConstructorOpen(ConstructorModel model);
        public abstract IEnumerable<string> ConstructorClose(ConstructorModel model);
        public abstract IEnumerable<string> Field(FieldModel model);
        public abstract IEnumerable<string> AutoProperty(PropertyModel model);
        public abstract IEnumerable<string> PropertyOpen(PropertyModel model);
        public abstract IEnumerable<string> PropertyClose(PropertyModel model);
        public abstract IEnumerable<string> GetterOpen(PropertyModel model);
        public abstract IEnumerable<string> GetterClose(PropertyModel model);
        public abstract IEnumerable<string> SetterOpen(PropertyModel model);
        public abstract IEnumerable<string> SetterClose(PropertyModel model);

        public abstract string NamedItem(NamedItemModel namedItem);
        public abstract string Operator(Operator op);

        // statements
        public abstract IEnumerable<string> IfOpen(ExpressionBase ifCondition);
        public abstract IEnumerable<string> ElseIfOpen(ExpressionBase condition);
        public abstract IEnumerable<string> ElseOpen();
        public abstract IEnumerable<string> IfClose();
        public abstract IEnumerable<string> ForEachOpen(string loopVar, ExpressionBase loopOver);
        public abstract IEnumerable<string> ForEachClose();
        public abstract IEnumerable<string> Assign(string variable, ExpressionBase value);
        public abstract IEnumerable<string> AssignWithDeclare(NamedItemModel? typeName, string variable, ExpressionBase value);
        public abstract IEnumerable<string> Return(ExpressionBase expression);
        public abstract IEnumerable<string> SimpleCall(ExpressionBase expression);
        public abstract IEnumerable<string> Comment(string text);
        public abstract IEnumerable<string> Throw(NamedItemModel exception, params ExpressionBase[] args);


        // expressions
        public abstract string Invoke(NamedItemModel instance, NamedItemModel methodName, IEnumerable<ExpressionBase> arguments);
        public abstract string Instantiate(NamedItemModel typeName, IEnumerable<ExpressionBase> arguments);
        public abstract string TypeOf(NamedItemModel typeName);
        public abstract string Cast(NamedItemModel typeName, ExpressionBase expression);
        public abstract string Compare(ExpressionBase left, Operator @operator, ExpressionBase right);
        public abstract string Not(ExpressionBase expression);


        public IWriter AddCodeFile(CodeFileModel codeFile)
        {
            if (codeFile is null) { return writer; }
            writer
                .AddLines(Comments(codeFile.HeaderComments))
                .AddLines(Usings(codeFile.Usings))
                .AddLines(NamespaceOpen(codeFile.Namespace))
                .IncreaseIndent();
            if (codeFile.Namespace is not null)
            {
                foreach (var classModel in codeFile.Namespace.Classes)
                {
                    AddClass(classModel);
                }
            }
            writer.DecreaseIndent()
                .AddLines(NamespaceClose(codeFile.Namespace))
                ;
            return writer;
        }

        public IWriter AddClass(ClassModel classModel)
        {
            if (! string.IsNullOrWhiteSpace(classModel.XmlDescription))
            {
                writer.AddLine($"{XmlDocPrefix}<summary>");
                writer.AddLine($"{XmlDocPrefix}{classModel.XmlDescription}");
                writer.AddLine($"{XmlDocPrefix}</summary>");
            }
            writer.AddLines(ClassOpen(classModel))
                .IncreaseIndent();
            foreach (var member in classModel.Members)
            {
                switch (member)
                {
                    case MethodModel method: AddMethod(method); break;
                    case PropertyModel property: AddProperty(property); break;
                    case FieldModel field: AddField(field); break;
                    case ConstructorModel constructor: AddConstructor(constructor); break;
                    case ClassModel nestedClass: AddClass(nestedClass); break;
                }
            }

            writer.DecreaseIndent()
                .AddLines(ClassClose(classModel));
            return writer;
        }

        public IWriter AddMethod(MethodModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.XmlDescription))
            {
                writer.AddLine($"{XmlDocPrefix}<summary>");
                writer.AddLine($"{XmlDocPrefix}{model.XmlDescription}");
                writer.AddLine($"{XmlDocPrefix}</summary>");
                foreach (var param in model.Parameters)
                {
                    writer.AddLine($"{XmlDocPrefix}<param name=\"{param.Name}\">{param.XmlDescription ?? ""}</param>");
                }
            }
            writer.AddLines(MethodOpen(model))
                .IncreaseIndent();

           Statements(writer,model.Statements);

            writer.DecreaseIndent()
                .AddLines(MethodClose(model));
            return writer;
        }

        public IWriter AddField(FieldModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.XmlDescription))
            {
                writer.AddLine($"{XmlDocPrefix}<summary>");
                writer.AddLine($"{XmlDocPrefix}{model.XmlDescription}");
                writer.AddLine($"{XmlDocPrefix}</summary>");
            }
            writer.AddLines(Field(model));
            return writer;
        }

        public IWriter AddConstructor(ConstructorModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.XmlDescription))
            {
                writer.AddLine($"{XmlDocPrefix}<summary>");
                writer.AddLine($"{XmlDocPrefix}{model.XmlDescription}");
                writer.AddLine($"{XmlDocPrefix}</summary>");
            }
            writer.AddLines(ConstructorOpen(model))
                .IncreaseIndent();

            Statements(writer, model.Statements);

            writer.DecreaseIndent()
                .AddLines(ConstructorClose(model));
            return writer;
        }

        public IWriter AddProperty(PropertyModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.XmlDescription))
            {
                writer.AddLine($"{XmlDocPrefix}<summary>");
                writer.AddLine($"{XmlDocPrefix}{model.XmlDescription}");
                writer.AddLine($"{XmlDocPrefix}</summary>");
            }
            if (model.Getter is null && model.Setter is null)
            {
                writer.AddLines(AutoProperty(model));
            }
            else
            {
                writer.AddLines(PropertyOpen(model))
                    .IncreaseIndent();
                if (model.Getter is not null)
                {
                    writer.AddLines(GetterOpen(model))
                        .IncreaseIndent();

                    Statements(writer, model.Getter.Statements);

                    writer.DecreaseIndent()
                    .AddLines(GetterClose(model));
                }
                if (model.Setter is not null)
                {
                    writer.AddLines(SetterOpen(model))
                        .IncreaseIndent();

                    Statements(writer, model.Setter.Statements);

                    writer.DecreaseIndent()
                    .AddLines(SetterClose(model));

                }

                writer.DecreaseIndent()
                .AddLines(PropertyClose(model));
            }
            return writer;
        }


        // ARGHHHH! this is broken because statements often need to indent. We need to pass the writer down.

        public IWriter Statements(IWriter writer, IEnumerable<IStatement> statements)
        {
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case IfModel ifModel:
                        AddIf(writer, ifModel);
                        break;
                    case ForEachModel forEach:
                        writer.AddLines(ForEachOpen(forEach.LoopVar, forEach.LoopOver));
                        writer.IncreaseIndent();
                        Statements(writer, forEach.Statements);
                        writer.DecreaseIndent();
                        writer.AddLines(ForEachClose());
                        break;
                    case AssignmentModel assign:
                        writer.AddLines(Assign(assign.Variable, assign.Value));
                        break;
                    case AssignWithDeclareModel assign:
                        writer.AddLines(AssignWithDeclare(assign.TypeName, assign.Variable, assign.Value));
                        break;
                    case ReturnModel returnModel:
                        writer.AddLines(Return(returnModel.Expression));
                        break;
                    case SimpleCallModel simpleCallModel:
                        writer.AddLines(SimpleCall(simpleCallModel.Expression));
                        break;
                    case CommentModel commentModel:
                        writer.AddLines(Comment(commentModel.Text));
                        break;
                    case ThrowModel throwModel:
                        writer.AddLines(Throw(throwModel.Exception, throwModel.Args));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return writer;
        }

        private IWriter AddIf(IWriter writer, IfModel ifModel)
        {
            writer.AddLines(IfOpen(ifModel.IfCondition));
            writer.IncreaseIndent();
            Statements(writer, ifModel.IfStatements);
            writer.DecreaseIndent();
            foreach (var elseIf in ifModel.ElseIfBlocks)
            {
                writer.AddLines(ElseIfOpen(elseIf.Condition));
                writer.IncreaseIndent();
                Statements(writer, elseIf.Statements);
                writer.DecreaseIndent();
            }
            if (ifModel.ElseStatements.Any())
            {
                writer.AddLines(ElseOpen());
                writer.IncreaseIndent();
                Statements(writer, ifModel.ElseStatements);
                writer.DecreaseIndent();
            }
            writer.AddLines(IfClose());
            return writer;
        }

        public string Expression(ExpressionBase expression) 
            => expression switch
            {
                InvocationModel invocationModel => Invoke(invocationModel.Instance, invocationModel.MethodName, invocationModel.Arguments),
                InstantiationModel instantiationModel => Instantiate(instantiationModel.TypeName, instantiationModel.Arguments),
                TypeOfModel typeOfModel => TypeOf(typeOfModel.TypeName),
                CastModel castModel => Cast(castModel.TypeName, castModel.Expression),
                ComparisonModel comparisonModel => Compare(comparisonModel.Left, comparisonModel.Operator, comparisonModel.Right),
                ListModel listModel => Instantiate("List<ExpressionBase>", listModel.Values ),
                StringLiteralModel literalModel => $@"""{literalModel.Value}""",
                LiteralModel literalModel => literalModel.Value ?? "",
                SymbolModel symbolModel => symbolModel.Name ?? "",
                NotModel notModel => Not(notModel.Expression),
                NullLiteralModel _ => NullKeyword,
                ThisLiteralModel _ => ThisKeyword,
                TrueLiteralModel _ => TrueKeyword,
                FalseLiteralModel _ => FalseKeyword,
                _ => throw new NotImplementedException(),
            };

        public string Output()
            => writer.Output();

    }

}
