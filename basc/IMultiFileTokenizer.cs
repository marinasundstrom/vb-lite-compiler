using System;

namespace Basic
{
    public interface IMultiFileTokenizer : ITokenizer
    {
        SourceUnit CompilationUnit { get; }
        event EventHandler<TokenizerEventArgs> FileChanged;

        bool NextFile();
    }
}