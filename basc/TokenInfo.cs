namespace Basic
{
    public struct TokenInfo
    {
        #region Fields

        private readonly int col;
        private readonly int ln;
        private readonly SourceFile sourceFile;
        private readonly Token token;
        private readonly string value;

        #endregion

        public TokenInfo(Token token, string value, SourceFile sourceFile, int ln, int col)
        {
            this.token = token;
            this.value = value;
            this.ln = ln;
            this.col = col;
            this.sourceFile = sourceFile;
        }

        #region Properties

        public Token Token
        {
            get { return token; }
        }

        public string Value
        {
            get { return value; }
        }

        public int Ln
        {
            get { return ln; }
        }

        public int Col
        {
            get { return col; }
        }

        #endregion

        #region Methods

        public SourceFile SourceFile
        {
            get { return sourceFile; }
        }

        public SourceLocation GetSourceLocation()
        {
            return new SourceLocation(ln, col);
        }

        public bool GetAsInt32(out int value)
        {
            return int.TryParse(this.value, out value);
        }

        public bool GetAsDouble(out double value)
        {
            return double.TryParse(this.value, out value);
        }

        public int GetAsInt32()
        {
            int nr;
            GetAsInt32(out nr);
            return nr;
        }

        public double GetAsDouble()
        {
            double nr;
            GetAsDouble(out nr);
            return nr;
        }

        public string GetString()
        {
            return value;
        }

        public override string ToString()
        {
            return GetString();
        }

        #region Comparison methods

        public bool Is(Token token)
        {
            return this.token == token;
        }

        public bool HasValue(string value)
        {
            return string.Compare(Value, value) == 0;
        }

        public bool HasValue(string value, bool ignoreCase)
        {
            return string.Compare(this.value, value, ignoreCase) == 0;
        }

        #endregion

        #endregion

        #region Operator overloads

        public static implicit operator Token(TokenInfo token)
        {
            return token.token;
        }

        public static implicit operator string(TokenInfo token)
        {
            return token.value;
        }

        #endregion

        #region Static methods

        public static bool HasValue(TokenInfo token, string value, bool ignoreCase)
        {
            return string.Compare(token.value, value, ignoreCase) == 0;
        }

        public static bool IsIdentifierOrKeyword(TokenInfo token)
        {
            return IsIdentifierOrKeyword(token.Token);
        }

        public static bool IsIdentifierOrKeyword(Token token)
        {
            return IsIdentifier(token) || IsReservedWord(token);
        }

        public static bool IsNumber(TokenInfo token)
        {
            return IsNumber(token.Token);
        }

        private static bool IsNumber(Token token)
        {
            return (token == Token.Number);
        }

        public static bool IsIdentifier(TokenInfo token)
        {
            return IsIdentifier(token.Token);
        }

        private static bool IsIdentifier(Token token)
        {
            return token == Token.Identifier;
        }

        public static bool IsOperator(TokenInfo token)
        {
            return IsReservedWord(token.Token);
        }

        public static bool IsOperator(Token token)
        {
            switch (token)
            {
                case Token.Plus:
                case Token.Minus:
                case Token.Star:
                case Token.Slash:
                case Token.Backslash:
                case Token.Percent:
                case Token.Ampersand:
                case Token.Caret:
                case Token.Mod:
                    return true;
            }

            return false;
        }

        public static bool IsReservedWord(TokenInfo token)
        {
            return IsReservedWord(token.Token);
        }

        public static bool IsReservedWord(Token token)
        {
            switch (token)
            {
                case Token.Imports:
                case Token.Namespace:
                case Token.Module:
                case Token.Sub:
                case Token.Function:
                case Token.End:
                case Token.Public:
                case Token.Private:
                case Token.Internal:
                case Token.Shared:
                case Token.Static:
                case Token.Const:
                case Token.Me:
                case Token.My:
                case Token.True:
                case Token.False:
                case Token.Dim:
                case Token.ByVal:
                case Token.ByRef:
                case Token.Call:
                case Token.Handles:
                case Token.If:
                case Token.Then:
                case Token.Else:
                case Token.ElseIf:
                case Token.While:
                case Token.When:
                case Token.Do:
                case Token.For:
                case Token.Each:
                case Token.Next:
                case Token.Is:
                case Token.IsNot:
                case Token.Not:
                case Token.And:
                case Token.Or:
                case Token.Mod:
                case Token.Return:
                case Token.As:
                case Token.Nothing:
                case Token.Void:
                case Token.Integer:
                case Token.Double:
                case Token.Character:
                case Token.Boolean:
                case Token.String:
                case Token.Object:
                case Token.Variant:
                    return true;
            }

            return false;
        }

        #endregion
    }
}