namespace Basic.Ast
{
    public class CommentStatement : Statement
    {
        private TokenList list;

        public CommentStatement(Block block)
            : base(block)
        {
        }

        public void SetTokenList(TokenList list)
        {
            this.list = list;
        }

        public override void Emit()
        {
        }

        public override void CheckAndResolve()
        {
        }
    }
}