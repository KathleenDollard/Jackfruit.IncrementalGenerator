namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class NamedItemModel
    {
        public string Name { get; }

        public NamedItemModel(string name)
        {
            Name = name;
        }

        // TODO: Work needed here on generics
        public static implicit operator NamedItemModel(string name) => new(name);

    }

    public class GenericNamedItemModel : NamedItemModel
    {
        public GenericNamedItemModel(string name) : base(name)
        {
        }

        public List<NamedItemModel> GenericTypes { get; } = new List<NamedItemModel>();
    }



    /*
        static member GenericsFromStrings(name: string, genericsAsStrings) =
            genericsAsStrings |> List.map(fun x -> SimpleNamedItem x)
        member this.WithoutGenerics() =
            match this with
            | SimpleNamedItem name -> name
            | GenericNamedItem (name, t) -> name
        static member Create(name: string, generics) =
            match generics with 
            | [] -> SimpleNamedItem name
            | _ -> GenericNamedItem(name, generics)
        static member Create(name: string) =
            let fMap(oldNode: TreeNodeType<string>) (newChildren: NamedItem list) =
                if newChildren.IsEmpty then
                    SimpleNamedItem oldNode.Data
                else
                    GenericNamedItem (oldNode.Data, newChildren)
            TreeFromDelimitedString '<' '>' ',' name
            |> MapTree fMap

        }*/
}