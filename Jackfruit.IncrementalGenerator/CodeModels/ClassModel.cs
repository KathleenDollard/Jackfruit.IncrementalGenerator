using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{

    public class ClassModel
    {
        public static List<ClassModel> Create(params ClassModel[] classes)
            => classes.ToList();

        public ClassModel(NamedItemModel name, NamedItemModel? inheritedFrom = null)
        {
            Name = name;
            InheritedFrom = inheritedFrom;
        }

        public NamedItemModel Name { get; }
        public Scope Scope { get; }
        public bool IsStatic { get; }
        public bool IsAsync { get; }
        public bool IsPartial { get; }
        public bool IsAbstract { get; }
        public bool IsSealed { get; }
        public NamedItemModel? InheritedFrom { get; }
        public List<NamedItemModel> ImplementedInterfaces { get; } = new List<NamedItemModel> { };
        public List<IMember> Members { get; set; } = new List<IMember> { };

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