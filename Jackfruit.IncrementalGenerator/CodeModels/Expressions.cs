using Jackfruit.IncrementalGenerator.CodeModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class InvocationModel : IExpression
    {
        public InvocationModel(NamedItemModel instance, NamedItemModel methodToCall, IExpression[] args)
        {
            Instance = instance;
            MethodName = methodToCall;
            Arguments = args;
        }

        public NamedItemModel Instance { get; set; }
        public NamedItemModel MethodName { get; set; }
        public bool ShouldAwait { get; set; }
        public IEnumerable<IExpression> Arguments { get; set; } = Enumerable.Empty<IExpression>();

    }

    public class InstantiationModel : IExpression
    {
        public InstantiationModel(NamedItemModel typeName, IExpression[] args)
        {
            TypeName = typeName;
            Arguments = args;
        }

        public NamedItemModel TypeName { get; set; }
        public IEnumerable<IExpression> Arguments { get; set; } = Enumerable.Empty<IExpression>();

    }

    public class ComparisonModel : IExpression
    {
        public ComparisonModel(IExpression left, Operator op, IExpression right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public IExpression Left { get; set; }
        public IExpression Right { get; set; }
        public Operator Operator { get; set; }

    }

    public class StringLiteralModel : IExpression
    {
        public string? Value { get; set; }
    }

    public class LiteralModel : IExpression
    {
        public string? Value { get; set; }
    }

    public class SymbolModel : IExpression
    {
        public string? Name { get; set; }
    }

    public class NullLiteralModel : IExpression
    {
    }
    public class ThisLiteralModel : IExpression
    {
    }
    public class TrueLiteralModel : IExpression
    {
    }
    public class FalseLiteralModel : IExpression
    {
    }

    public static class ExpressionHelpers
    {
        public static InvocationModel Invoke(NamedItemModel instance, NamedItemModel methodToCall, params IExpression[] args)
            => new(instance, methodToCall, args);

        public static InstantiationModel New(NamedItemModel typeName, params IExpression[] args)
            => new(typeName, args);

        public static ComparisonModel Compare(IExpression left, Operator op, IExpression right)
            => new(left, op, right);

    }
}


