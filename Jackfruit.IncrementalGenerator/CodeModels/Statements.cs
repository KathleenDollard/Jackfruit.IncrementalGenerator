using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class IfPair
    {
        public IfPair(IExpression condition, IEnumerable<IStatement> statements)
        {
            Condition = condition;
            Statements = statements;
        }

        public IExpression Condition { get; }
        public IEnumerable<IStatement> Statements { get; }
    }

    public class IfModel : IStatement
    {
        public IfModel(IExpression ifCondition)
        {
            IfCondition = ifCondition;
        }
        public IExpression IfCondition  { get; set; }
        public IEnumerable<IStatement> IfStatements { get; } = new List<IStatement>();
        public IEnumerable<IStatement> ElseStatements { get; } = new List<IStatement>();
        public IEnumerable<IfPair> ElseIfBlocks { get; set; } = new List<IfPair>();
    }

    public class ForEachModel : IStatement
    {
        public ForEachModel(string loopVar, IExpression loopOver)
        {
            LoopVar = loopVar;
            LoopOver = loopOver;
        }

        public string LoopVar { get; set; }
        public IExpression LoopOver { get; set; }
        public IEnumerable<IStatement> Statements { get; set; } = new List<IStatement>();
    }

    public class AssignmentModel : IStatement
    {
        public AssignmentModel(string variableName, IExpression value)
        {
            Value = value;
            Variable = variableName;
        }
        public string Variable { get; set; }
        public IExpression Value { get; set; }
    }

    public class AssignWithDeclareModel : IStatement
    {
        public AssignWithDeclareModel(NamedItemModel? typeName, string variableName, IExpression value)
        {
            TypeName = typeName;
            Variable = variableName;
            Value = value;
        }

        public string Variable { get; set; }
        public IExpression Value { get; set; }
        public NamedItemModel? TypeName { get; set; }
    }

    public class ReturnModel : IStatement
    {
        public ReturnModel(IExpression expression)
        {
            Expression = expression;
        }
        public IExpression Expression { get; set; }
    }

    public class SimpleCallModel : IStatement
    {
        public SimpleCallModel(IExpression expression)
        {
            Expression = expression;
        }
        public IExpression Expression { get; set; }
    }

    public class CommentModel : IStatement
    {
        public CommentModel(string text)
        {
            Text = text;
        }

        public string Text { get; set; }
    }

    public static class StatementHelpers
    {
        public static IfModel If(IExpression ifCondition)
            => new(ifCondition);

        public static ForEachModel ForEach(string loopVar, IExpression loopOver)
             => new(loopVar, loopOver);

        public static AssignmentModel Assign(string variableName, IExpression value)
            => new(variableName, value);

        public static AssignWithDeclareModel AssignWithDeclare(NamedItemModel typeName, string variableName, IExpression value)
            => new(typeName, variableName, value);

        public static ReturnModel Return(IExpression expression)
            => new(expression);

        public static SimpleCallModel SimpleCall(IExpression expression)
            => new(expression);

        public static CommentModel Comment(string text)
             => new(text);
    }
}

// TODO: Compiler directives
/*


type CompilerActionTriState =
    | Disable
    | Enable
    | Restore

type CompilerNullableAction =
    | All
    | Annotations
    | Warnings

// KAD: Make a single member DU for Symbole which will allow validation at hte right time
type CompilerDirectiveType =
    | IfDef of symbol: string
    | ElIfDef of symbol: string
    | ElseDef
    | EndIfDef
    | Nullable of action: CompilerActionTriState* setting: CompilerNullableAction
    | Define of symbol: string
    | UnDefine of symbol: string
    | Region of name: string
    | EndRegion
    | CompilerError of message: string
    | CompilerWarning of message: string
    | Line of lineNumer: int* fileNameOverride: string
    | PragmaWarning of action: CompilerActionTriState* warnings: int list
    // If people actually use the following, give some Guid and CheckSum help
    | PragmaCheckSum of filename: string* guid: string* checksumBytes: string


type CompilerDirectiveModel =
    { CompilerDirectiveType: CompilerDirectiveType }
    interface IStatement
    static member Create directive =
        { CompilerDirectiveType = directive }
    
*/

