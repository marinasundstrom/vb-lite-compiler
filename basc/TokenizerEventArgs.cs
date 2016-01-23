using System;

namespace Basic
{
    public class TokenizerEventArgs : EventArgs
    {
        public TokenizerEventArgs(ITokenizer tokenizer, SourceFile sourceFile)
        {
            Tokenizer = tokenizer;
            SourceFile = sourceFile;
        }

        public ITokenizer Tokenizer { get; private set; }

        public SourceFile SourceFile { get; private set; }
    }
}