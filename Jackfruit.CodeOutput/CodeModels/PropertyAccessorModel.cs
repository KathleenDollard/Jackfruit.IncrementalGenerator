namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public class PropertyAccessorModel : IHasScope
    {
        public Scope Scope { get; set; }
        public List<IStatement> Statements { get; set; } = new List<IStatement>();
    }

}
