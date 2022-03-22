using Jackfruit.IncrementalGenerator.CodeModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class InvocationModel : ExpressionBase
    {
        public InvocationModel(NamedItemModel instance, NamedItemModel methodToCall, ExpressionBase[] args)
        {
            Instance = instance;
            MethodName = methodToCall;
            Arguments = args;
        }

        public NamedItemModel Instance { get; set; }
        public NamedItemModel MethodName { get; set; }
        public bool ShouldAwait { get; set; }
        public IEnumerable<ExpressionBase> Arguments { get; set; } = Enumerable.Empty<ExpressionBase>();

    }

    public class InstantiationModel : ExpressionBase
    {
        public InstantiationModel(NamedItemModel typeName, ExpressionBase[] args)
        {
            TypeName = typeName;
            Arguments = args;
        }

        public NamedItemModel TypeName { get; set; }
        public IEnumerable<ExpressionBase> Arguments { get; set; } = Enumerable.Empty<ExpressionBase>();

    }

    public class ComparisonModel : ExpressionBase
    {
        public ComparisonModel(ExpressionBase left, Operator op, ExpressionBase right)
        {
            Left = left;
            Right = right;
            Operator = op;
        }

        public ExpressionBase Left { get; set; }
        public ExpressionBase Right { get; set; }
        public Operator Operator { get; set; }

    }

    public class StringLiteralModel : ExpressionBase
    {
        public StringLiteralModel(string value) 
        {
            Value = value;
        }
        public string Value { get; set; }
    }

    public class ListLiteralModel : ExpressionBase
    {
        public ListLiteralModel(IEnumerable<string> values)
        {
            Values = values;
        }
        public IEnumerable<string> Values { get; set; }
    }



    public class LiteralModel : ExpressionBase
    {
        public LiteralModel(string value)
        {
            Value = value;
        }
        public string Value { get; set; }


    }

    public class SymbolModel : ExpressionBase
    {
        public SymbolModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class NullLiteralModel : ExpressionBase
    {
    }
    public class ThisLiteralModel : ExpressionBase
    {
    }
    public class TrueLiteralModel : ExpressionBase
    {
    }
    public class FalseLiteralModel : ExpressionBase
    {
    }

    public static class ExpressionHelpers
    {
        public static InvocationModel Invoke(NamedItemModel instance, NamedItemModel methodToCall, params ExpressionBase[] args)
            => new(instance, methodToCall, args);

        public static InstantiationModel New(NamedItemModel typeName, params ExpressionBase[] args)
            => new(typeName, args);

        public static ComparisonModel Compare(ExpressionBase left, Operator op, ExpressionBase right)
            => new(left, op, right);

        public static SymbolModel Symbol(string name)
            => new(name);

        public static NullLiteralModel Null
            => new();

        public static ThisLiteralModel This
            => new();


    }
}


