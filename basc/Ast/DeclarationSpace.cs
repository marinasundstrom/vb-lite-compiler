using System.Collections.Generic;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public abstract class DeclarationSpace
    {
        public EnumBuilder EnumBuilder;
        public FieldBuilder FieldBuilder;
        public MethodBuilder MethodBuilder;
        public List<ParameterBuilder> ParameterBuilders;
        public TypeBuilder TypeBuilder;

        public AssemblyModule AssemblyModule
        {
            get { return RootContext.Module; }
        }

        public AssemblyBuilder AssemblyBuilder
        {
            get { return RootContext.Module.AssemblyBuilder; }
        }

        public ModuleBuilder ModuleBuilder
        {
            get { return RootContext.Module.ModuleBuilder; }
        }
    }
}