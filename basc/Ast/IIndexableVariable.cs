namespace Basic.Ast
{
    public interface IIndexableVariable : IVariable
    {
        void EmitLoad(Block block, params Expression[] expr);
        void EmitStore(Block block, params Expression[] expr);

        void EmitLoad(Block block, Expression expr);
        void EmitStore(Block block, Expression expr);
    }
}