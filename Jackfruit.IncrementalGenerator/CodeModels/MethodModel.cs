namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class MethodModel : IMember
    {
        public MethodModel(string name, NamedItemModel returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        public string Name { get; }
        public NamedItemModel ReturnType { get; }
        public Scope Scope { get; }
        public bool IsAsync { get; }
        public bool IsStatic { get; }
        public bool IsPartial { get; set; }
        public List<ParameterModel> Parameters { get; set; } = new List<ParameterModel>();
        public List<IStatement> Statements { get; set; } = new List<IStatement>();
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