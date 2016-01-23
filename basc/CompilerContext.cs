using System.Collections.Generic;

namespace Basic
{
    public class CompilerContext
    {
        public CompilerContext()
        {
            SourceUnit = new SourceUnit();
            Report = new Report();
        }

        /// <summary>
        /// Gets the source unit containing all the source files.
        /// </summary>
        public SourceUnit SourceUnit { get; private set; }

        /// <summary>
        /// Gets the compilation report.
        /// </summary>
        public Report Report { get; private set; }

        /// <summary>
        /// Gets the list of paths to referenced assemblies.
        /// </summary>
        public List<string> References { get; private set; }
    }
}