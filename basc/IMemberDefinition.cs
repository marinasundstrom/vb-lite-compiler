using System.Reflection;

namespace Basic
{
    public interface IMemberDefinition
    {
        string Name { get; }

        ITypeDefinition DeclaringType { get; }

        bool IsShared { get; }
        bool IsPrivate { get; }
        bool IsPublic { get; }
        bool IsFamily { get; }
        bool IsAssembly { get; }

        AttributeInstance[] GetAttributes(bool inherit);
        ITypeDefinition[] GetAttributeTypes(bool inherit);

        MemberInfo GetMemberInfo();
    }
}