namespace Basic
{
    public interface IParameterDefinition
    {
        string Name { get; }
        ITypeDefinition ParameterType { get; }
        IMethodDefinition Method { get; }
    }
}