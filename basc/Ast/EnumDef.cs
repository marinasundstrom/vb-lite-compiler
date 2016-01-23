using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class EnumDef : TypeDef
    {
        protected EnumDef(ClassOrStructDef parent, string name, MemberName baseClass, MemberName[] inherits,
                          Modifier modifiers)
            : base(parent, name, baseClass, inherits, modifiers)
        {
            parent.AddNestedType(parent);
        }

        public EnumDef(Namespace ns, string name, MemberName baseClass, MemberName[] inherits, Modifier modifiers)
            : base(ns, name, baseClass, inherits, modifiers)
        {
        }

        [Obsolete("Use CreateEnumBuilder() instead.")]
        public override TypeBuilder DefineTypeBuilder()
        {
            CreateEnumBuilder();
            return null;
        }

        public EnumBuilder CreateEnumBuilder()
        {
            if (Parent == null)
            {
                EnumBuilder = ModuleBuilder.DefineEnum(FullName, TypeAttributes.Class, typeof (int));
            }
            //else //Add nested
            //{
            //    this.EnumBuilder = this.TypeBuilder.nestedDefineEnum(this.Name, System.Reflection.TypeAttributes.Class, typeof(int));
            //}

            return EnumBuilder;
        }

        public void AddConstant(Constant constant)
        {
            constants.Add(constant);
        }
    }
}