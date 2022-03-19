namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class ConstructorModel : IMember
    {
        public string ClassName { get; }

        public ConstructorModel(string className)
        {
            ClassName = className;
        }

        public BaseOrThis BaseOrThis { get; }
        public List<IExpression> BaseOrThisArguments { get; } = new List<IExpression> { };
        public Scope Scope { get; }
        public bool IsStatic { get; }
        public List<ParameterModel> Parameters { get; } = new List<ParameterModel> { };
        public List<IStatement> Statements { get; } = new List<IStatement> { };

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