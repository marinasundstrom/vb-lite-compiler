using System;
using System.Reflection.Emit;
using Basic.Ast;

namespace Basic
{
    internal static class RootContext
    {
        static RootContext()
        {
            Module = new AssemblyModule();
        }

        public static MethodDef EntryPoint { get; private set; }

        public static AssemblyModule Module { get; private set; }

        public static bool HasEntryPoint
        {
            get { return EntryPoint != null; }
        }

        public static void SetEntryPoint(MethodDef method)
        {
            if (EntryPoint != null)
                throw new Exception("EntryPoint is already set.");

            EntryPoint = method;
        }

        public static void InitializeForBuild(ModuleBuilder moduleBuilder)
        {
            Module.InitializeForBuild(moduleBuilder);
        }
    }
}