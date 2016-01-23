namespace Basic.Ast
{
    public class EmitContext
    {
        public EmitContext(CompilerContext compCntxt)
        {
            CompilerContext = compCntxt;
        }

        public CompilerContext CompilerContext { get; private set; }
    }
}