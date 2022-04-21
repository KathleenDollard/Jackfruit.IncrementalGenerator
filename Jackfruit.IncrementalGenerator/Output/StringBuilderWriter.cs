using System;
using System.Collections.Generic;
using System.Text;

namespace Jackfruit.IncrementalGenerator.Output
{
    public class StringBuilderWriter : IWriter
    {
        private int currentIndent;
        private readonly StringBuilder sb=new ();

        public StringBuilderWriter(int indentSize)
        {
            IndentSize = indentSize;
        }

        public int IndentSize { get; }

        public IWriter AddLine(string line)
        {
            var space = new string(' ', currentIndent * IndentSize);
            sb.AppendLine(space + line);
            return this;
        }

        public IWriter AddLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                AddLine(line);
            }
            return this;
        }

        public IWriter DecreaseIndent()
        {
            currentIndent -= 1;
            return this;
        }

        public IWriter IncreaseIndent()
        {
            currentIndent += 1;
            return this;
        }

        public string Output()
        => sb.ToString();
    }

}
