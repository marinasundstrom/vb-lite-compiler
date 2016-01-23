namespace Basic.Ast
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(Block block)
            : base(block)
        {
        }

        public Expression Expression { get; private set; }

        public override void Emit()
        {
            Expression.Emit();
        }

        public override void CheckAndResolve()
        {
            Expression.CheckAndResolve();
        }

        public void SetExpression(Expression expr)
        {
            Expression = expr;
        }
    }
}