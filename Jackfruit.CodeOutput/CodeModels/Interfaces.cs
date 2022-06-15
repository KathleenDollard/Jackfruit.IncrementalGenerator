using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.CodeModels
{
    public interface IHasScope
    {
        Scope Scope { get; set; }
    }

    public interface IHasParameters
    {
        List<ParameterModel> Parameters { get; set; }
    }
    public interface IHasStatements
    {
        List<IStatement> Statements { get; set; }
    }
}
