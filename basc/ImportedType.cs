using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Basic
{
    public class ImportedType : ImportedMember, ITypeDefinition
    {
        private List<IFieldDefinition> fields;
        private List<IMethodDefinition> methods;
        private List<ITypeDefinition> types;

        public ImportedType(Type type)
            : base(type, null)
        {
            Initialize();
        }

        public ImportedType(Type type, ImportedType parent)
            : base(type, parent)
        {
            Initialize();
        }

        #region ITypeDefinition Members

        public string Namespace
        {
            get { return ((Type) member).Namespace; }
        }

        public bool IsNestedType
        {
            get { return DeclaringType != null; }
        }

        public Type GetTypeInfo()
        {
            return (Type) member;
        }

        public string FullName
        {
            get { return ((Type) member).FullName; }
        }

        public bool IsAssignableFrom(ITypeDefinition type)
        {
            return IsExtending(type) | ConvertibleTo(type);
        }

        public bool IsExtending(ITypeDefinition type)
        {
            return false;
        }

        public bool ConvertibleTo(ITypeDefinition type)
        {
            return false;
        }

        public ITypeDefinition BaseType
        {
            get { return TypeManager.GetType(((Type) member).BaseType.FullName); }
        }

        public IMethodDefinition[] GetMethods()
        {
            if (methods == null)
            {
                methods = new List<IMethodDefinition>();

                foreach (MethodInfo m in GetTypeInfo().GetMethods())
                {
                    IMethodDefinition method = new ImportedMethod(m, this);
                    methods.Add(method);
                }
            }

            return methods.ToArray();
        }

        public IFieldDefinition[] GetFields()
        {
            if (fields == null)
            {
                fields = new List<IFieldDefinition>();

                foreach (FieldInfo f in GetTypeInfo().GetFields())
                {
                    IFieldDefinition field = new ImportedField(f, this);
                    fields.Add(field);
                }
            }

            return fields.ToArray();
        }

        public ITypeDefinition[] GetNestedTypes()
        {
            if (types == null)
            {
                types = new List<ITypeDefinition>();

                foreach (Type t in GetTypeInfo().GetNestedTypes())
                {
                    ITypeDefinition type = new ImportedType(t, this);
                    types.Add(type);
                }
            }

            return types.ToArray();
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
            get { return false; }
        }

        public bool IsIndexable
        {
            get { return true; }
        }

        public int Rank
        {
            get { return GetTypeInfo().GetArrayRank(); }
        }

        public IMethodDefinition[] GetOperatorsOverloads()
        {
            throw new NotImplementedException();
        }

        public IMethodDefinition[] GetConversionMethods()
        {
            throw new NotImplementedException();
        }

        public bool IsGenericType
        {
            get { return GetTypeInfo().IsGenericType; }
        }

        public IMethodDefinition GetTypeInitializer()
        {
            return GetMethods().First(x => x.GetMethodInfo() == GetTypeInfo().TypeInitializer);
        }

        public IMethodDefinition[] GetConstructors()
        {
            return GetMethods().Where(x => x.Name == ".ctor").ToArray();
        }

        public bool HasElementType
        {
            get { return GetTypeInfo().HasElementType; }
        }

        public ITypeDefinition ElementType
        {
            get { return TypeManager.ResolveType(GetTypeInfo().GetElementType().FullName); }
        }

        public IPropertyDefinition[] GetIndexableProperties()
        {
            throw new NotImplementedException();
        }

        public override bool IsShared
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsPrivate
        {
            get { return !GetTypeInfo().IsPublic; }
        }

        public override bool IsPublic
        {
            get { return GetTypeInfo().IsPublic; }
        }

        public override bool IsFamily
        {
            get { return GetTypeInfo().IsNestedFamily; }
        }

        public override bool IsAssembly
        {
            get { return GetTypeInfo().IsNestedAssembly || GetTypeInfo().IsNotPublic; }
        }

        #endregion

        private void Initialize()
        {
            methods = new List<IMethodDefinition>();
            fields = new List<IFieldDefinition>();
            types = new List<ITypeDefinition>();
        }
    }
}