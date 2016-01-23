using System;
using System.Reflection;

namespace Basic
{
    public class ArrayTypeDefinition : ITypeDefinition
    {
        public ArrayTypeDefinition(ITypeDefinition elementType, int rank)
        {
            ElementType = elementType;
            Rank = rank;
        }

        public bool IsJaggedArray
        {
            get { return ElementType is ArrayTypeDefinition; }
        }

        #region ITypeDefinition Members

        public ITypeDefinition ElementType { get; private set; }

        public int Rank { get; private set; }

        public string FullName
        {
            get { return ElementType.FullName + args(Rank); }
        }

        public string Namespace
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsNestedType
        {
            get { return false; }
        }

        public ITypeDefinition BaseType
        {
            get { return TypeManager.ResolveType("System.Array"); }
        }

        public IMethodDefinition[] GetMethods()
        {
            return BaseType.GetMethods();
        }

        public IFieldDefinition[] GetFields()
        {
            return BaseType.GetFields();
        }

        public ITypeDefinition[] GetNestedTypes()
        {
            return BaseType.GetNestedTypes();
        }

        public Type GetTypeInfo()
        {
            return ElementType.GetTypeInfo().MakeArrayType(Rank);
        }

        public bool IsAssignableFrom(ITypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public bool IsExtending(ITypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public bool ConvertibleTo(ITypeDefinition type)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return ElementType.Name + args(Rank); }
        }

        public MemberInfo GetMemberInfo()
        {
            return GetTypeInfo();
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
            get { return true; }
        }

        public bool IsIndexable
        {
            get { return true; }
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
            get { throw new NotImplementedException(); }
        }

        public IMethodDefinition GetTypeInitializer()
        {
            throw new NotImplementedException();
        }

        public IMethodDefinition[] GetConstructors()
        {
            throw new NotImplementedException();
        }

        public bool HasElementType
        {
            get { return true; }
        }


        public IPropertyDefinition[] GetIndexableProperties()
        {
            throw new NotImplementedException();
        }


        public bool IsShared
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPrivate
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPublic
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsFamily
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsAssembly
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeDefinition[] GetAttributeTypes(bool inherit)
        {
            return BaseType.GetAttributeTypes(inherit);
        }

        public AttributeInstance[] GetAttributes(bool inherit)
        {
            return BaseType.GetAttributes(inherit);
        }

        public ITypeDefinition DeclaringType
        {
            get { return BaseType.DeclaringType; }
        }

        #endregion

        private static string args(int rank)
        {
            return "[" + "]";
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}