namespace Basic.Ast
{
    public abstract class AstNode : IAstNode
    {
        #region IAstNode Members

        public SourceLocation StartLoc { get; private set; }

        public SourceLocation EndLoc { get; private set; }

        public SourceSpan SourceSpan
        {
            get { return new SourceSpan(StartLoc, EndLoc); }
        }

        public void SetStartLoc(SourceLocation start)
        {
            StartLoc = start;
        }

        public void SetEndLoc(SourceLocation end)
        {
            EndLoc = end;
            ;
        }

        #endregion
    }
}