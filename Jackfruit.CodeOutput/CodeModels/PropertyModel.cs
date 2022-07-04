namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class PropertyModel : IMember, IHasScope
    {
        public PropertyModel(string name, NamedItemModel type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public NamedItemModel Type { get; }
        public Scope Scope { get; set; }
        public bool IsStatic { get; set; }
        public PropertyAccessorModel? Getter { get; set; }
        public PropertyAccessorModel? Setter { get; set; }
        public bool IsPartial { get; internal set; }
        public string? XmlDescription { get; set; }

    }

    //public static class PropertyModelExtensions
    //{
    //    public static PropertyModel Property(string name, NamedItemModel type)
    //        => new(name, type);

    //    public static PropertyModel Static(this PropertyModel model)
    //    {
    //        model.IsStatic = true;
    //        return model;
    //    }
    //}
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