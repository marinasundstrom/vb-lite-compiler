using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public abstract class TypeDef : MemberDef, ITypeDefinition
    {
        protected List<Constant> constants;
        protected List<FieldDef> fields;
        protected List<MethodDef> methods;
        protected List<TypeDef> nestedTypes;

        protected TypeResolveContext resolveContext;

        public TypeDef(TypeDef parent, string name, MemberName baseClass, MemberName[] inplements, Modifier modifiers)
            : base(parent, new MemberName(parent.MemberName, Separators.Slash, name), modifiers)
        {
            Initialize(baseClass, inplements);
        }


        public TypeDef(Namespace ns, string name, MemberName baseClass, MemberName[] inplements, Modifier modifiers)
            : base(
                null,
                (ns is GlobalRootNamespace ? new MemberName(name) : new MemberName(ns.MemberName, Separators.Dot, name)),
                modifiers)
        {
            Initialize(baseClass, inplements);

            Namespace = ns;
            ns.AddType(this);
        }

        public Namespace Namespace { get; private set; }

        public MemberName BaseClass { get; private set; }

        public MemberName[] Implements { get; private set; }

        public IEnumerable<FieldDef> Fields
        {
            get { return fields; }
        }

        public IEnumerable<MethodDef> Methods
        {
            get { return methods; }
        }

        public IEnumerable<TypeDef> NestedTypes
        {
            get { return nestedTypes; }
        }

        //public bool IsStruct
        //{
        //    get { return this is StructDef; }
        //}

        public bool IsClass
        {
            get { return this is ClassDef; }
        }

        public bool IsEnum
        {
            get { return this is EnumDef; }
        }

        #region ITypeDefinition Members

        string ITypeDefinition.Namespace
        {
            get { return Namespace.MemberName.BaseName; }
        }


        public bool IsNestedType
        {
            get { return Parent != null; }
        }

        public Type GetTypeInfo()
        {
            throw new NotImplementedException();
        }


        public bool IsAssignableFrom(ITypeDefinition type)
        {
            return IsExtending(type) | ConvertibleTo(type);
        }

        public bool IsExtending(ITypeDefinition type)
        {
            //foreach(var interfac in this.Implements)
            //{
            //    TypeManager.
            //}

            throw new NotImplementedException();
        }

        public bool ConvertibleTo(ITypeDefinition type)
        {
            throw new NotImplementedException();
        }

        ITypeDefinition ITypeDefinition.BaseType
        {
            get { return TypeManager.GetType(BaseClass.BaseName); }
        }

        public IMethodDefinition[] GetMethods()
        {
            return methods.ToArray();
        }

        public IFieldDefinition[] GetFields()
        {
            return (IFieldDefinition[]) fields.ToArray();
        }

        public ITypeDefinition[] GetNestedTypes()
        {
            return nestedTypes.ToArray();
        }

        public virtual IMethodDefinition GetTypeInitializer()
        {
            return null;
        }

        public ITypeDefinition MakeArrayType()
        {
            return MakeArrayType(1);
        }

        public ITypeDefinition MakeArrayType(int rank)
        {
            return new ArrayTypeDefinition(this, rank);
        }


        public bool IsArrayType
        {
            get { throw new NotImplementedException(); }
        }

        public int Rank
        {
            get { return 0; }
        }

        public IMethodDefinition[] GetOperatorsOverloads()
        {
            return methods.Where(x => x.Name.Contains("op_")).ToArray();
        }

        public IMethodDefinition[] GetConversionMethods()
        {
            return methods.Where(x => x.Name.Contains("implicit_") || x.Name.Contains("explicit_")).ToArray();
        }

        public bool IsGenericType
        {
            get { throw new NotImplementedException(); }
        }

        public IMethodDefinition[] GetConstructors()
        {
            return methods.Where(x => x.Name == ".ctor").ToArray();
        }

        public bool HasElementType
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeDefinition ElementType
        {
            get { throw new NotImplementedException(); }
        }


        public IPropertyDefinition[] GetIndexableProperties()
        {
            throw new NotImplementedException();
        }

        public bool IsIndexable
        {
            get { throw new NotImplementedException(); }
        }

        public override ITypeDefinition[] GetAttributeTypes(bool inherit)
        {
            if (!inherit)
            {
                return base.GetAttributeTypes(inherit);
            }

            var types = new List<ITypeDefinition>();

            attributes.Select(x => x.AttributeType).ToArray();

            ITypeDefinition[] r = TypeManager.ResolveType(BaseClass.BaseName).GetAttributeTypes(inherit);
            types.AddRange(r);

            return r.ToArray();
        }

        public override AttributeInstance[] GetAttributes(bool inherit)
        {
            if (!inherit)
            {
                return base.GetAttributes(inherit);
            }

            var types = new List<AttributeInstance>();

            attributes.Select(x => x.AttributeType).ToArray();

            AttributeInstance[] r = TypeManager.ResolveType(BaseClass.BaseName).GetAttributes(inherit);
            types.AddRange(r);

            return r.ToArray();
        }

        #endregion

        private void Initialize(MemberName baseClass, MemberName[] inplements)
        {
            BaseClass = baseClass;
            Implements = inplements;

            fields = new List<FieldDef>();
            constants = new List<Constant>();
            methods = new List<MethodDef>();
            nestedTypes = new List<TypeDef>();
        }

        public override void Define()
        {
            DefineNestedTypes();

            base.Define();
        }

        public void DefineNestedTypes()
        {
            TypeManager.AddUserDefinedType(this);

            foreach (TypeDef type in nestedTypes)
            {
                type.Define();
            }
        }

        public void DefineMembers()
        {
            foreach (FieldDef field in fields)
            {
                field.Define();
            }

            foreach (MethodDef method in methods)
            {
                method.Define();
            }
        }

        public abstract TypeBuilder DefineTypeBuilder();

        public void DefineMemberBuilders()
        {
            foreach (FieldDef method in fields)
            {
                method.DefineFieldBuilder();
            }

            foreach (MethodDef method in methods)
            {
                method.DefineMethodBuilder();
                method.DefineParameterBuilders();
            }
        }

        public override void Resolve()
        {
            //Resolves the current entity
            resolveContext = new TypeResolveContext();

            resolveTypeDef();

            ResolveNestedTypes();

            base.Resolve();
        }

        private void resolveTypeDef()
        {
        }

        public void ResolveMembers()
        {
            foreach (FieldDef field in fields)
            {
                field.Resolve();
            }

            foreach (MethodDef method in methods)
            {
                method.Resolve();
            }
        }

        public void ResolveNestedTypes()
        {
            foreach (TypeDef nt in nestedTypes)
            {
                nt.Resolve();
            }
        }

        public override void Emit()
        {
            foreach (MethodDef method in methods)
            {
                method.Emit();
            }

            foreach (TypeDef type in nestedTypes)
            {
                type.Emit();
            }

            base.Emit();
        }

        public void SetFileNamespaceEntry(FileNamespaceEntry fileNamespaceEntry)
        {
            this.fileNamespaceEntry = fileNamespaceEntry;
        }

        public void CloseType()
        {
        }

        public void CloseBuilder()
        {
            foreach (TypeDef nt in nestedTypes)
            {
                nt.CloseBuilder();
            }

            TypeBuilder.CreateType();
        }

        public virtual IMethodDefinition GetDefaultConstructor()
        {
            return null;
        }

        #region Nested type: TypeResolveContext

        public struct TypeResolveContext
        {
            public ITypeDefinition BaseClass;
            public List<ITypeDefinition> Implements;
        }

        #endregion

        public IEnumerable<ITypeDefinition> ResolveType(string name)
        {
            var types = new List<ITypeDefinition>();

            var t = InternalTypeAliases.GetTypeByAlias(name, true);

            if (t != null)
            {
                types.Add(t);
                return types;
            }

            t = TypeManager.ResolveType(this.fileNamespaceEntry.NamespaceEntry.Namespace.MemberName + "." + name);
            
            if (t != null)
            {
                types.Add(t);
            }
            
            types.AddRange(ResolveType(this.fileNamespaceEntry, name));

            return types;
        }

        public IEnumerable<ITypeDefinition> ResolveType(FileNamespaceEntry ns, string name)
        {
            var types = new List<ITypeDefinition>();

            foreach (var td in ns.Imports)
            {
                var t = TypeManager.ResolveType(td + "." + name);
                types.Add(t);
            }
            
            if(ns.Parent != null)
            {
                var types2 = ResolveType(ns.Parent, name);
                types.AddRange(types2);
            }

            return types;
        }
    }
}