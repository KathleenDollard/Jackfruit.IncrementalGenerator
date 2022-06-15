using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{

    public class ClassModel : IMember, IHasScope
    {
        public static List<ClassModel> Create(params ClassModel[] classes)
            => classes.ToList();

        public ClassModel(NamedItemModel name, NamedItemModel? inheritedFrom = null)
        {
            Name = name;
            InheritedFrom = inheritedFrom;
        }

        public NamedItemModel Name { get; }
        public Scope Scope { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAsync { get; set; }
        public bool IsPartial { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsSealed { get; set; }
        public NamedItemModel? InheritedFrom { get; set; }
        public List<NamedItemModel> ImplementedInterfaces { get; } = new List<NamedItemModel> { };
        public List<IMember> Members { get; set; } = new List<IMember> { };
        public string XmlDescription { get; set; }
    }

}
