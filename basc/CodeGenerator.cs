using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Basic.Ast;

namespace Basic
{
    /// <summary>
    /// A code generator.
    /// </summary>
    public class CodeGenerator
    {
        private readonly PEFileKinds PEFileKind;
        private readonly bool isDLL;
        private readonly string outputFilename;
        private readonly string outputPath;

        private AssemblyBuilder assemblyBuilder;
        private CompilerContext context;

        private AssemblyModule module;

        /// <summary>
        /// Creates and initializes an CodeGenerator instance.
        /// </summary>
        /// <param name="parser">A Parser</param>
        /// <param name="outputPath">The path of the output file</param>
        /// <param name="isDLL">A value indicating whether the output should be a DLL or not.</param>
        public CodeGenerator(CompilerContext context, string outputPath, bool isDLL)
        {
            this.context = context;

            this.outputPath = outputPath;
            outputFilename = Path.GetFileName(this.outputPath);
            this.isDLL = isDLL;

            if (isDLL)
            {
                PEFileKind = PEFileKinds.Dll;
                this.outputPath += ".dll";
            }
            else
            {
                PEFileKind = PEFileKinds.ConsoleApplication;
                this.outputPath += ".exe";
            }

            Initialize();
        }

        private void Initialize()
        {
            var name = new AssemblyName(outputFilename);

            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyBuilder.GetName().Name, outputPath,
                                                                              true);

            RootContext.InitializeForBuild(moduleBuilder);
        }

        private void DefineAssembly()
        {
        }

        /// <summary>
        /// Generate code.
        /// </summary>
        public void Generate()
        {
            module = RootContext.Module;

            GenerateAssembly();
            Finish();
        }

        private void Finish()
        {
            module.AssemblyBuilder.Save(outputPath);
        }

        private void GenerateAssembly()
        {
            module.DefineTypeBuilders();
            ;
            module.DefineMemberBuilders();

            emitMethodBodies();

            if (!isDLL)
            {
                if (!RootContext.HasEntryPoint)
                {
                    //this.context.Report.Add(
                    //Throw exception.
                }
                else
                {
                    SetEntryPoint();
                }
            }

            module.CloseBuilders();
        }

        private void emitMethodBodies()
        {
#if DEBUG

            foreach (MethodDef m in GetMethodDefs(module.RootNamespace))
            {
                m.Emit();
            }
#else 

            emitMethodBodiesParallel();
#endif
        }

        private void emitMethodBodiesParallel()
        {
            var tasks = new List<Task>();

            foreach (MethodDef m in GetMethodDefs(module.RootNamespace))
            {
                var t = new Task(m.Emit);
                tasks.Add(t);
            }

            tasks.ForEach(x => x.Start());

            Task.WaitAll(tasks.ToArray());
        }

        private IEnumerable<MethodDef> GetMethodDefs(Namespace ns)
        {
            var methods = new List<MethodDef>();

            foreach (TypeDef t in ns.Types)
            {
                foreach (MethodDef m in t.Methods)
                {
                    methods.Add(m);
                }

                //Properties
            }

            foreach (Namespace t in ns.ChildNamespaces)
            {
                methods.AddRange(GetMethodDefs(t));
            }

            return methods;
        }

        private void SetEntryPoint()
        {
            var entryPoint = (MethodInfo) RootContext.EntryPoint.GetMemberInfo();
            module.AssemblyBuilder.SetEntryPoint(entryPoint, PEFileKind);
        }
    }
}