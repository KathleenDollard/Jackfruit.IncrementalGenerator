namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public interface IExpression { }
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