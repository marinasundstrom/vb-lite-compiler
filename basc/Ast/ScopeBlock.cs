namespace Basic.Ast
{
    public class ScopeBlock : Block
    {
        public ScopeBlock(MethodDef method, Block parent)
            : base(method, parent)
        {
        }
    }
}