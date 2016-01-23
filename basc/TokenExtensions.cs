namespace Basic
{
    public static class TokenExtensions
    {
        public static SourceSpan GetSpan(this TokenInfo token, TokenInfo end)
        {
            SourceLocation loc = end.GetSourceLocation();
            return new SourceSpan(token.GetSourceLocation(), new SourceLocation(loc.Ln, loc.Col + end.Value.Length));
        }

        public static SourceSpan Get(this TokenInfo token)
        {
            return token.GetSpan(token);
        }
    }
}