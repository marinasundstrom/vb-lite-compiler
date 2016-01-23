using System.Collections;
using System.Collections.Generic;

namespace Basic
{
    public class TokenList : IEnumerable<TokenInfo>
    {
        private readonly List<TokenInfo> tokens;

        public TokenList()
        {
            tokens = new List<TokenInfo>();
        }

        #region IEnumerable<TokenInfo> Members

        public IEnumerator<TokenInfo> GetEnumerator()
        {
            return tokens.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public void Add(TokenInfo token)
        {
            tokens.Add(token);
        }
    }
}