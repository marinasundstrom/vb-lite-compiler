namespace Basic.Ast
{
    //Make struct
    public class SourceData
    {
        public SourceData(SourceSpan sourceSpan, SourceFile sourceFile, TokenList tokenList)
        {
            SourceSpan = sourceSpan;
            SourceFile = sourceFile;
            Tokens = tokenList;
        }

        public SourceSpan SourceSpan { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public TokenList Tokens { get; private set; }
    }
}