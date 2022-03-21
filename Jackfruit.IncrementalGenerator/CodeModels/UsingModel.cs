namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class UsingModel
    {
        public static List<UsingModel> Create(params string[] usings)
            => usings.Select(x => new UsingModel(x)).ToList();

        public UsingModel(string name, string? alias = null)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; }

        public string? Alias { get; }

        public static implicit operator UsingModel(string name) => new(name);

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