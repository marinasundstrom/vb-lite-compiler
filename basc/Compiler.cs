using System.Collections.Generic;
using System.Reflection;
using Basic.Ast;

namespace Basic
{
    /// <summary>
    /// Visual Basic compiler.
    /// </summary>
    public class Compiler
    {
        private BasicParser parser;

        /// <summary>
        /// Creates and initializes a Compiler instance.
        /// </summary>
        public Compiler()
        {
            References = new List<string>();
        }

        /// <summary>
        /// Gets the CompilerContext.
        /// </summary>
        public CompilerContext Context { get; private set; }

        /// <summary>
        /// Gets the parser.
        /// </summary>
        public BasicParser Parser
        {
            get { return parser; }
        }

        /// <summary>
        /// Gets a list of the referenced assemblies.
        /// </summary>
        public List<string> References { get; private set; }

        /// <summary>
        /// Compiles and builds an assembly.
        /// </summary>
        /// <param name="context">The CompilerContext.</param>
        /// <param name="outputPath">The output path. A full filepath.</param>
        /// <param name="isDLL">A value indicating whether the target should be a DLL or not.</param>
        public void Build(CompilerContext context, string outputPath, bool isDLL)
        {
            Context = context;

            LoadReferences();

            Compile(context);

#if !DEBUG
            if(!this.Context.Report.HasErrors)
            {
                var codeGen = new CodeGenerator(Context, outputPath, isDLL);
                codeGen.Generate();
            }
#else
            var codeGen = new CodeGenerator(Context, outputPath, isDLL);
            codeGen.Generate();
#endif
        }

        private void LoadReferences()
        {
            TypeManager.AddAssembly(typeof (int).Assembly);

            foreach (string path in References)
            {
                Assembly assembly = Assembly.LoadFile(path);
                TypeManager.AddAssembly(assembly);
            }
        }

        /// <summary>
        /// Compiles source code into intermediate representation that is accessible through the parser.
        /// </summary>
        /// <param name="context"></param>
        public void Compile(CompilerContext context)
        {
            parser = new BasicParser(context);
            parser.Parse();

#if DEBUG
            //Just for debugging
            GlobalRootNamespace root = parser.GlobalNamespace;
#endif
        }
    }
}