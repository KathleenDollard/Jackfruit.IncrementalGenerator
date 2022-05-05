using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class IfPair
    {
        public IfPair(ExpressionBase condition, IEnumerable<IStatement> statements)
        {
            Condition = condition;
            Statements = statements;
        }

        public ExpressionBase Condition { get; }
        public IEnumerable<IStatement> Statements { get; }
    }

    public class IfModel : IStatement
    {
        public IfModel(ExpressionBase ifCondition)
        {
            IfCondition = ifCondition;
        }
        public ExpressionBase IfCondition { get; set; }
        public IEnumerable<IStatement> IfStatements { get; set; } = new List<IStatement>();
        public IEnumerable<IStatement> ElseStatements { get; set; } = new List<IStatement>();
        public IEnumerable<IfPair> ElseIfBlocks { get; set; } = new List<IfPair>();
    }

    public class ForEachModel : IStatement
    {
        public ForEachModel(string loopVar, ExpressionBase loopOver)
        {
            LoopVar = loopVar;
            LoopOver = loopOver;
        }

        public string LoopVar { get; set; }
        public ExpressionBase LoopOver { get; set; }
        public IEnumerable<IStatement> Statements { get; set; } = new List<IStatement>();
    }

    public class AssignmentModel : IStatement
    {
        public AssignmentModel(string variableName, ExpressionBase value)
        {
            Value = value;
            Variable = variableName;
        }
        public string Variable { get; set; }
        public ExpressionBase Value { get; set; }
    }

    public class AssignWithDeclareModel : IStatement
    {
        public AssignWithDeclareModel(NamedItemModel? typeName, string variableName, ExpressionBase value)
        {
            TypeName = typeName;
            Variable = variableName;
            Value = value;
        }

        public string Variable { get; set; }
        public ExpressionBase Value { get; set; }
        public NamedItemModel? TypeName { get; set; }
    }

    public class ReturnModel : IStatement
    {
        public ReturnModel(ExpressionBase expression)
        {
            Expression = expression;
        }
        public ExpressionBase Expression { get; set; }
    }

    public class ThrowModel : IStatement
    {
        public ThrowModel(NamedItemModel exception, params ExpressionBase[] args)
        {
            Exception = exception;
            Args = args;
        }
        public NamedItemModel Exception { get; set; }
        public ExpressionBase[] Args { get; set; }
    }

    public class SimpleCallModel : IStatement
    {
        public SimpleCallModel(ExpressionBase expression)
        {
            Expression = expression;
        }
        public ExpressionBase Expression { get; set; }
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
        public static IfModel If(ExpressionBase ifCondition, params IStatement[] statements)
            => new(ifCondition) { IfStatements = statements };

        public static IfModel If(
                ExpressionBase ifCondition,
                IEnumerable<IStatement> ifStatements)
            => new(ifCondition)
            {
                IfStatements = ifStatements,
            };

        public static IfModel If(
                ExpressionBase ifCondition,
                IEnumerable<IStatement> ifStatements,
                IEnumerable<IStatement> elseStatements)
            => new(ifCondition)
            {
                IfStatements = ifStatements,
                ElseStatements = elseStatements
            };

        public static IfModel If(
                ExpressionBase ifCondition,
                IEnumerable<IStatement> ifStatements,
                params (ExpressionBase condition, IEnumerable<IStatement> statements)[] elseifs)
            => new(ifCondition)
            {
                IfStatements = ifStatements,
                ElseIfBlocks = elseifs.Select(x => new IfPair(x.condition, x.statements))
            };

        public static IfModel If(
                ExpressionBase ifCondition,
                IEnumerable<IStatement> ifStatements,
                IEnumerable<IStatement> elseStatements,
                params (ExpressionBase condition, IEnumerable<IStatement> statements)[] elseifs)
            => new(ifCondition)
            {
                IfStatements = ifStatements,
                ElseStatements = elseStatements,
                ElseIfBlocks = elseifs.Select(x => new IfPair(x.condition, x.statements))
            };

        public static ForEachModel ForEach(
                string loopVar,
                ExpressionBase loopOver,
                IEnumerable<IStatement> statements)
             => new(loopVar, loopOver)
             { Statements = statements };

        public static AssignmentModel Assign(string variableName, ExpressionBase value)
            => new(variableName, value);

        public static AssignWithDeclareModel AssignWithDeclare(
                NamedItemModel typeName,
                string variableName,
                ExpressionBase value)
            => new(typeName, variableName, value);

        public static AssignWithDeclareModel AssignWithDeclare(
                string variableName,
                ExpressionBase value)
            => new(null, variableName, value);

        public static ReturnModel Return(ExpressionBase expression)
            => new(expression);

        public static ThrowModel Throw(NamedItemModel exception, params ExpressionBase[] args)
             => new(exception, args);

        public static SimpleCallModel SimpleCall(ExpressionBase expression)
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

