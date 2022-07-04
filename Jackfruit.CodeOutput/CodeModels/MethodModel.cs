namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class MethodModel : IMember, IHasScope, IHasParameters, IHasStatements
    {
        public MethodModel(string name, NamedItemModel returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        public string Name { get; }
        public NamedItemModel ReturnType { get; }
        public Scope Scope { get; set; }
        public bool IsAsync { get; set; }
        public bool IsOverride { get; set; }
        public bool IsStatic { get; set; }
        public bool IsNewSlot { get; set; }
        public bool IsPartial { get; set; }
        public List<ParameterModel> Parameters { get; set; } = new List<ParameterModel>();
        public List<IStatement> Statements { get; set; } = new List<IStatement>();
        public string? XmlDescription { get; set; }

    }

}
