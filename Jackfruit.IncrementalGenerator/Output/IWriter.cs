using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.Output
{
    public interface IWriter
    {
        IWriter AddLine(string line);
        IWriter AddLines(IEnumerable<string> lines);
        IWriter IncreaseIndent();
        IWriter DecreaseIndent();
        string Output();
    }
}
