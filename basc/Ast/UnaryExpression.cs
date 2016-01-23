namespace Basic.Ast
{
    internal class UnaryExpression : Expression
    {
        private Expression expression;
        private object p;

        public UnaryExpression(Statement statement, object p, Expression expression, SourceData sourceData)
            : base(statement, sourceData)
        {
            // TODO: Complete member initialization
            this.p = p;
            this.expression = expression;
        }
    }
}