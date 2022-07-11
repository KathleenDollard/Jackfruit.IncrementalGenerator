using Jackfruit.Internal;
using Microsoft.CodeAnalysis;

namespace Jackfruit.IncrementalGenerator
{
    // TODO: What is the easiest way to ensure value equality of arrays, lists and dictionaries
    public record Detail
    {
        private string description = "";

        public Detail(string id, string name, string? typeName = null)
        {
            Id = id;
            Name = char.ToUpperInvariant(name[0]) + name.Substring(1);
            TypeName = typeName;
        }

        public static Detail Empty => new("", "");

        public string Id { get; }
        public string Name { get; set; }
        public string Description
        {
            get => description;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    description = value;
                }
            }
        }
        public string[] Aliases { get; set; } = new string[] { };
        public MemberKind MemberKind { get; set; }
        public string? TypeName { get; }
        public string? ArgDisplayName { get; set; }
        public bool Required { get; set; }
    }

    public record CommandDetail : Detail
    {
        public CommandDetail(string id, string name, string typeName, string nspace, params Detail[] memberDetails)
            : base(id, name, typeName)
        {
            Namespace = nspace;
            MemberDetails = memberDetails.ToList();
        }
        public static CommandDetail Empty
            => new CommandDetail("", "", "", "", Detail.Empty);

        public string Namespace { get; }
        public List<Detail> MemberDetails { get; set; }
        public string? XmlDocs { get; set; }
        public List<Detail> SubCommandDetails { get; } = new List<Detail>();
        public List<Error> Errors { get; private set; } = new();
    }
}
