namespace Basic.Ast
{
    public interface IAstNode
    {
        SourceLocation StartLoc { get; }

        SourceLocation EndLoc { get; }

        SourceSpan SourceSpan { get; }


        void SetStartLoc(SourceLocation start);

        void SetEndLoc(SourceLocation end);
    }
}