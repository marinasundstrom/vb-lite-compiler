namespace Basic.Ast
{
    public abstract class Statement
    {
        public Statement(Block block)
        {
            Block = block;
            Method = Block.Method;
        }

        public Statement(MethodDef method, Block block)
        {
            Block = block;
            Method = method;
        }

        public SourceData SourceData { get; private set; }

        public Block Block { get; private set; }

        public MethodDef Method { get; private set; }

        public abstract void CheckAndResolve();

        public abstract void Emit();

        public void SetSourceData(SourceData sourceData)
        {
            SourceData = sourceData;
        }
    }
}