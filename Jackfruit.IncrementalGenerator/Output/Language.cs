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

        public LanguageOutput(IWriter writer)
        {
            this.writer = writer;
        }

        public string UnknownKeyword { get; } = "<UNKNOWN>";

        public abstract string PrivateKeyword { get; }
        public abstract string PublicKeyword { get; }
        public abstract string InternalKeyword { get; }
        public abstract string ProtectedKeyword { get; }
        public abstract string ProtectedInternalKeyword { get; }
        public abstract string PrivateProtectedKeyword { get; }

        public abstract string StaticKeyword { get; }
        public abstract string AsyncKeyword { get; }
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

        protected abstract IEnumerable<string> IfOpen(IExpression ifCondition);
        protected abstract void ElseIfOpen(IExpression condition);
        protected abstract void ElseOpen();
        protected abstract void IfClose();
        protected abstract void ForEachOpen(string loopVar, IExpression loopOver);
        protected abstract void ForEachClose();
        protected abstract void Assign(string variable, IExpression value);
        protected abstract void AssignWithDeclare(NamedItemModel? typeName, string variable, IExpression value);
        protected abstract void Return(IExpression expression);
        protected abstract void SimpleCall(IExpression expression);
        protected abstract void Comment(string text);


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
                }
            }

            writer.DecreaseIndent()
                .AddLines(ClassClose(classModel));
            return writer;
        }

        public IWriter AddMethod(MethodModel model)
        {
            writer.AddLines(MethodOpen(model))
                .IncreaseIndent(); 

            writer.AddLines(Statements(model.Statements));

            writer.DecreaseIndent()
                .AddLines(MethodClose(model));
            return writer;
        }

        public IWriter AddField(FieldModel model)
        {
            writer.AddLines(Field(model));
            return writer;
        }
        public IWriter AddConstructor(ConstructorModel model)
        {
            writer.AddLines(ConstructorOpen(model))
                .IncreaseIndent();
            foreach (var statement in model.Statements)
            {
                //switch (statement)
                //{
                //}
            }

            writer.DecreaseIndent()
                .AddLines(ConstructorClose(model));
            return writer;
        }

        public IWriter AddProperty(PropertyModel model)
        {
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
                    foreach (var statement in model.Getter.Statements)
                    {
                        //switch (statement)
                        //{
                        //}
                    }
                    writer.DecreaseIndent()
                    .AddLines(GetterClose(model));
                }
                if (model.Setter is not null)
                {
                    writer.AddLines(SetterOpen(model))
                        .IncreaseIndent();
                    foreach (var statement in model.Setter.Statements)
                    {
                        //switch (statement)
                        //{
                        //}
                    }
                    writer.DecreaseIndent()
                    .AddLines(SetterClose(model));

                }

                writer.DecreaseIndent()
                .AddLines(PropertyClose(model));
            }
            return writer;
        }

        public IEnumerable<string> Statements(IEnumerable<IStatement> statements)
        {
            var ret = new List<string>();
            foreach (var statement in statements)
            {
                switch (statement)
                {
                    case IfModel ifModel:
                        AddIf(ret, ifModel);
                        break;
                    case ForEachModel forEach:
                        ForEachOpen(forEach.LoopVar, forEach.LoopOver);
                        ret.AddRange(Statements(forEach.Statements));
                        ForEachClose();
                        break;
                    case AssignmentModel assign:
                        Assign(assign.Variable, assign.Value);
                        break;
                    case AssignWithDeclareModel assign:
                        AssignWithDeclare(assign.TypeName, assign.Variable, assign.Value);
                        break;
                    case ReturnModel returnModel:
                        Return(returnModel.Expression);
                        break;
                    case SimpleCallModel simpleCallModel:
                        SimpleCall(simpleCallModel.Expression);
                        break;
                    case CommentModel commentModel:
                        Comment(commentModel.Text);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return ret;
        }

        private void AddIf(List<string> ret, IfModel ifModel)
        {
            ret.AddRange(IfOpen(ifModel.IfCondition));
            ret.AddRange(Statements(ifModel.IfStatements));
            foreach (var elseIf in ifModel.ElseIfBlocks)
            {
                ElseIfOpen(elseIf.Condition);
                ret.AddRange(Statements(elseIf.Statements));
            }
            if (ifModel.ElseStatements.Any())
            {
                ElseOpen();
                ret.AddRange(Statements(ifModel.ElseStatements));
            }
            IfClose();

        }

        public IEnumerable<string> Expression(IExpression expression)
        {
            var ret = new List<string>();
            switch (expression)
            {
                case IfModel ifModel:
                    AddIf(ret, ifModel);
                    break;
                case ForEachModel forEach:
                    ForEachOpen(forEach.LoopVar, forEach.LoopOver);
                    ret.AddRange(Statements(forEach.Statements));
                    ForEachClose();
                    break;
                case AssignmentModel assign:
                    Assign(assign.Variable, assign.Value);
                    break;
                case AssignWithDeclareModel assign:
                    AssignWithDeclare(assign.TypeName, assign.Variable, assign.Value);
                    break;
                case ReturnModel returnModel:
                    Return(returnModel.Expression);
                    break;
                case SimpleCallModel simpleCallModel:
                    SimpleCall(simpleCallModel.Expression);
                    break;
                case CommentModel commentModel:
                    Comment(commentModel.Text);
                    break;
                default:
                    throw new NotImplementedException();
                }
            return ret;
        }

        public string Output()
            => writer.Output();

    }

}
