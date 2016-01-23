using System.Reflection;

namespace Basic
{
    public interface IFieldDefinition : IMemberDefinition
    {
        ITypeDefinition FieldType { get; }
        FieldInfo GetFieldInfo();
    }
}