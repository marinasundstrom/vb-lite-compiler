using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class ConstructorDef : MethodDef
    {
        private ConstructorBuilder ConstructorBuilder;

        protected ConstructorDef(ClassStructOrModuleDef parent, string name, ParameterList param, Modifier modifiers)
            : base(parent, name, new SimpleName(null, "Void", null), param, modifiers)
        {
            if (IsShared)
            {
                IsConstructor = true;
            }
            else
            {
                IsTypeInitializer = true;
            }
        }

        public static ConstructorDef CreateTypeInitializer(ClassStructOrModuleDef parent)
        {
            return new ConstructorDef(parent, ".cctor", new ParameterList(), Modifier.Private | Modifier.Shared);
        }

        public static ConstructorDef CreateConstructor(ClassStructOrModuleDef parent, ParameterList parameters,
                                                       Modifier modifiers)
        {
            return new ConstructorDef(parent, ".ctor", parameters, modifiers);
        }

        public override MethodBuilder DefineMethodBuilder()
        {
            DefineConstructorBuilder();
            return null;
        }

        public ConstructorBuilder DefineConstructorBuilder()
        {
            if (IsConstructor)
            {
                ConstructorBuilder = Parent.TypeBuilder.DefineConstructor(
                    GetMethodModifiers(),
                    CallingConventions.Standard,
                    GetArgumentTypes());
            }
            else
            {
                ConstructorBuilder = Parent.TypeBuilder.DefineTypeInitializer();
            }

            return ConstructorBuilder;
        }

        public override void Emit()
        {
            emit();
        }

        private void emit()
        {
            Generator = ConstructorBuilder.GetILGenerator();

            Body.Emit();

            if (Body.Statements.Count() == 0)
            {
                Generator.Emit(OpCodes.Nop);
            }

            Generator.Emit(OpCodes.Ret);
        }


        public ConstructorInfo GetConstructorInfo()
        {
            return ConstructorBuilder;
        }
    }
}