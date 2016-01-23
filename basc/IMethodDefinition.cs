using System.Reflection;

namespace Basic
{
    public interface IMethodDefinition : IMemberDefinition
    {
        IParameterDefinition[] GetParameters();

        MethodInfo GetMethodInfo();
    }
}