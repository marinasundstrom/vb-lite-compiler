using System;

namespace Basic.Ast
{
    public class GenericNameExpression : FullNamedExpression
    {
        public GenericNameExpression(Statement statement, SimpleName left, GenericArguments arguments,
                                     SourceData sourceData)
            : base(statement, left, sourceData)
        {
            Arguments = arguments;
        }

        public GenericArguments Arguments { get; private set; }

        public override string GetSimpleName()
        {
            throw new NotImplementedException();
        }
    }
}