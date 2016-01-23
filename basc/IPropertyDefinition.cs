using System.Reflection;
using Basic.Ast;

namespace Basic
{
    public interface IPropertyDefinition : IMemberDefinition, IVariable, IIndexableVariable
    {
        string FullName { get; }

        FullNamedExpression PropertyType { get; }

        bool IsAssignable { get; }
        bool IsIndexedProperty { get; }

        IMethodDefinition Accessor { get; }
        IMethodDefinition Assigner { get; }
        IParameterDefinition[] GetParameters();

        PropertyInfo GetPropertyInfo();
    }
}