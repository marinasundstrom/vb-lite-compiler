using System.IO;

namespace Basic
{
    public interface ITokenizer
    {
        int Col { get; }
        bool IgnoreCase { get; set; }
        bool IgnoreEOLs { get; set; }
        bool IgnoreSpaces { get; set; }
        bool IsEOF { get; }
        int Ln { get; }
        SourceFile SourceFile { get; }
        Report Report { get; }
        TextReader TextReader { get; }
        TokenInfo GetNextToken();
        SourceLocation GetSourceLocation();
        TokenInfo PeekToken();
    }
}