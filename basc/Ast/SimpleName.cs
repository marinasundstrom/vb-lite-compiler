namespace Basic.Ast
{
    public class SimpleName : FullNamedExpression
    {
        public SimpleName(Statement statement, FullNamedExpression left, string value, SourceData sourceData)
            : base(statement, left, sourceData)
        {
            Value = value;
        }


        public SimpleName(Statement statement, string value, SourceData sourceData)
            : this(statement, null, value, sourceData)
        {
        }

        public SimpleName(Statement statement, string value, bool isKeyword, SourceData sourceData)
            : this(statement, null, value, sourceData)
        {
            IsKeyword = isKeyword;
        }

        public string Value { get; private set; }

        public override string GetSimpleName()
        {
            return Value;
        }
    }
}