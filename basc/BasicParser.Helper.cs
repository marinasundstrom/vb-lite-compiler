namespace Basic
{
    public partial class BasicParser
    {
        #region Nested type: Helpers

        /// <summary>
        /// Helper methods
        /// </summary>
        public static class Helpers
        {
            public static bool IsIdentifier(TokenInfo token)
            {
                return TokenInfo.IsIdentifier(token);
            }

            public static bool IsReservedWord(TokenInfo token)
            {
                return TokenInfo.IsReservedWord(token);
            }

            public static bool IsIdentifierOrReservedWord(TokenInfo token)
            {
                return TokenInfo.IsIdentifier(token) || TokenInfo.IsReservedWord(token);
            }

            public static bool IsNumber(TokenInfo token)
            {
                return TokenInfo.IsNumber(token);
            }

            public static bool IsOperator(TokenInfo token)
            {
                return IsUnaryOperator(token) || IsBinaryOperator(token) || IsArithmeticOperator(token) ||
                       IsLogicOperator(token) || IsComparisonOperator(token);
            }

            public static bool IsUnaryOperator(TokenInfo token)
            {
                switch (token.Token)
                {
                    case Token.Plus:
                    case Token.Minus:
                    case Token.Not:
                        return true;
                }

                return false;
            }

            public static bool IsBinaryOperator(TokenInfo token)
            {
                switch (token.Token)
                {
                    case Token.Plus:
                    case Token.Minus:
                    case Token.Slash:
                    case Token.Star:
                    case Token.Percent:
                    case Token.LeftAngleBracket:
                    case Token.RightAngleBracket:
                    case Token.Is:
                    case Token.IsNot:
                    case Token.Mod:
                    case Token.Equality:
                    case Token.Inequality:
                    case Token.Period:
                        return true;
                }

                return false;
            }

            public static bool IsArithmeticOperator(TokenInfo token)
            {
                switch (token.Token)
                {
                    case Token.Plus:
                    case Token.Minus:
                    case Token.Slash:
                    case Token.Star:
                    case Token.Percent:
                    case Token.Mod:
                        return true;
                }

                return false;
            }

            public static bool IsLogicOperator(TokenInfo token)
            {
                switch (token.Token)
                {
                    case Token.And:
                    case Token.Or:
                    case Token.Xor:
                        return true;
                }

                return false;
            }

            public static bool IsComparisonOperator(TokenInfo token)
            {
                switch (token.Token)
                {
                    case Token.Equality:
                    case Token.Inequality:
                    case Token.LeftAngleBracket:
                    case Token.RightAngleBracket:
                    case Token.Is:
                    case Token.IsNot:
                        return true;
                }

                return false;
            }
        }

        #endregion
    }
}