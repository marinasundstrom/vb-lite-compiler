namespace Basic
{
    public class SourceSpan
    {
        public SourceSpan(SourceLocation start, SourceLocation end)
        {
            Start = start;
            End = end;
        }

        public SourceLocation Start { get; private set; }
        public SourceLocation End { get; private set; }
    }
}