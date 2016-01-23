using Basic.Ast;

namespace Basic
{
    public interface IVariable
    {
        string Name { get; }
        FullNamedExpression Type { get; }
        bool IsIndexable { get; }
        void EmitLoad(Block block);
        void EmitStore(Block block);
    }
}