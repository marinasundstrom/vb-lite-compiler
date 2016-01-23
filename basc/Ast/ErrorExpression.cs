namespace Basic.Ast
{
    public class ErrorExpression : Expression
    {
        public ErrorExpression(Statement statement, SourceData sourceData)
            : base(statement, sourceData)
        {
        }
    }
}