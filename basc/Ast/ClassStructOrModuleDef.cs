using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public abstract class ClassStructOrModuleDef : TypeDef
    {
        protected ClassStructOrModuleDef(ClassStructOrModuleDef parent, string name, MemberName baseClass,
                                         MemberName[] inherits, Modifier modifiers)
            : base(parent, name, baseClass, inherits, modifiers)
        {
        }

        public ClassStructOrModuleDef(Namespace ns, string name, MemberName baseClass, MemberName[] inherits,
                                      Modifier modifiers)
            : base(ns, name, baseClass, inherits, modifiers)
        {
        }

        public void CreateMemberBuilders()
        {
            foreach (TypeDef type in nestedTypes)
            {
                type.DefineTypeBuilder();
            }

            foreach (FieldDef field in fields)
            {
                field.DefineFieldBuilder();
            }

            foreach (MethodDef method in methods)
            {
                method.DefineMethodBuilder();
                method.DefineParameterBuilders();
            }
        }

        public void AddMethod(MethodDef method)
        {
            methods.Add(method);
        }

        public void AddField(FieldDef field)
        {
            fields.Add(field);
        }

        public override TypeBuilder DefineTypeBuilder()
        {
            if (Parent == null)
            {
                TypeBuilder = ModuleBuilder.DefineType(FullName, GetTypeAttributes());
            }
            else
            {
                TypeBuilder = TypeBuilder.DefineNestedType(Name, GetTypeAttributes());
            }

            return TypeBuilder;
        }

        protected TypeAttributes GetTypeAttributes()
        {
            TypeAttributes attr = TypeAttributes.Class | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout |
                                  TypeAttributes.AutoClass;

            if (this is ModuleDef)
            {
                attr = attr | TypeAttributes.Sealed;
            }

            if (!IsNestedType)
            {
                attr = attr | TypeAttributes.NotPublic;

                if (IsPublic)
                {
                    attr = attr ^ TypeAttributes.NotPublic;
                    attr = attr | TypeAttributes.Public;
                }

                //if (this.IsStatic)
                //{
                //    attr = attr | TypeAttributes.neste;
                //}

                //if (this.IsInternal)
                //{
                //    attr = attr | TypeAttributes.;
                //}
            }
            else
            {
                attr = attr | TypeAttributes.NotPublic;

                if (IsPublic)
                {
                    attr = attr ^ TypeAttributes.NotPublic;
                    attr = attr | TypeAttributes.NestedPublic;
                }

                //if (this.IsStatic)
                //{
                //    attr = attr | TypeAttributes.;
                //}

                if (IsAssembly)
                {
                    attr = attr | TypeAttributes.NestedAssembly;
                }
            }

            return attr;
        }

        public override void Emit()
        {
            base.Emit();
        }

        public override IMethodDefinition GetTypeInitializer()
        {
            MethodDef cctor = methods.Find(x => x.Name == ".cctor" && x.IsShared);

            if (cctor == null)
            {
                cctor = ConstructorDef.CreateTypeInitializer(this);
                cctor.SetBody(new ScopeBlock(cctor, null));
                //cctor.Resolve();
            }

            return cctor;
        }

        public override IMethodDefinition GetDefaultConstructor()
        {
            MethodDef ctor = methods.Find(x => x.Name == ".ctor" && !x.IsShared && x.Parameters.Count() == 0);

            if (ctor == null)
            {
                ctor = ConstructorDef.CreateConstructor(this, new ParameterList(), Modifier.Public);
                ctor.SetBody(new ScopeBlock(ctor, null));
                //cctor.Resolve();
            }

            return ctor;
        }
    }
}