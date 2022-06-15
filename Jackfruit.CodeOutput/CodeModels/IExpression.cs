namespace Jackfruit.IncrementalGenerator.CodeModels
{
    // This is a base class not an interface, because I _really_ want the implicit conversions
    public abstract class ExpressionBase
    {
        public static implicit operator ExpressionBase(string value)
            => new StringLiteralModel(value);
        public static implicit operator ExpressionBase(int value)
            => new LiteralModel(value.ToString());
        public static implicit operator ExpressionBase(double value)
            => new LiteralModel(value.ToString());
        public static implicit operator ExpressionBase(bool value)
            => value
                ? new TrueLiteralModel()
                : new FalseLiteralModel();
        public static implicit operator ExpressionBase(List<ExpressionBase> values)
            => new ListModel(values);
        // TODO: Make datetime, Guid, and decimal literals
        //public static implicit operator ExpressionBase(DateTime value)
        //    => new LiteralModel(value.ToString());
        //public static implicit operator ExpressionBase(Guid value)
        //    => new LiteralModel(value.ToString());
        //public static implicit operator ExpressionBase(decimal value)
        //    => new LiteralModel(value.ToString());
    }
}
/* Hmmm. Will we need these?

type ICompareExpression = 
inherit IExpression


type ReturnType =
| ReturnTypeVoid
| ReturnTypeUnknown
| ReturnType of t: NamedItem
//interface IStatement
static member Create typeName =
    match typeName with 
        | "void" -> ReturnTypeVoid
        | _ -> ReturnType(NamedItem.Create typeName)
static member op_Implicit(typeName: string) : ReturnType = 
    ReturnType.Create typeName

type InheritedFrom =
| SomeBase of BaseClass: NamedItem
| NoBase
//interface IMember

//type ImplementedInterface =
//    | ImplementedInterface of Name: NamedItem
//    //interface IMember
}
*/