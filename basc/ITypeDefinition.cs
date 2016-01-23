using System;

namespace Basic
{
    public interface ITypeDefinition : IMemberDefinition
    {
        string FullName { get; }
        string Namespace { get; }

        bool IsGenericType { get; }

        bool IsNestedType { get; }
        bool IsArrayType { get; }
        bool IsIndexable { get; }

        int Rank { get; }

        bool HasElementType { get; }

        ITypeDefinition ElementType { get; }

        ITypeDefinition BaseType { get; }

        IMethodDefinition GetTypeInitializer();
        IMethodDefinition[] GetConstructors();

        IMethodDefinition[] GetMethods();
        IFieldDefinition[] GetFields();
        ITypeDefinition[] GetNestedTypes();

        IPropertyDefinition[] GetIndexableProperties();

        IMethodDefinition[] GetOperatorsOverloads();
        IMethodDefinition[] GetConversionMethods();

        Type GetTypeInfo();

        bool IsAssignableFrom(ITypeDefinition type);
        bool IsExtending(ITypeDefinition type);
        bool ConvertibleTo(ITypeDefinition type);

        ITypeDefinition MakeArrayType();
        ITypeDefinition MakeArrayType(int rank);
    }
}