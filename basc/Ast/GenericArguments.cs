using System.Collections.Generic;

namespace Basic.Ast
{
    public class GenericArguments
    {
        public GenericArguments()
        {
            Arguments = new List<Expression>();
        }

        public List<Expression> Arguments { get; private set; }

        public void Add(Expression expr)
        {
            Arguments.Add(expr);
        }
    }
}