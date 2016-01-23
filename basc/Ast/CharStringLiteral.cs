using System.Collections.Generic;

namespace Basic.Ast
{
    internal class CharStringLiteral : Expression
    {
        private List<TokenInfo> tokens;

        public CharStringLiteral(Statement statement, List<TokenInfo> tokens, SourceData sourceData)
            : base(statement, sourceData)
        {
            // TODO: Complete member initialization
            this.tokens = tokens;
        }
    }
}