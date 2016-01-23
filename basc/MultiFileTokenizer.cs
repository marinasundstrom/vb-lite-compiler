using System;
using System.IO;

namespace Basic
{
    public class MultiFileTokenizer : IMultiFileTokenizer
    {
        private readonly SourceUnit compilationUnit;
        private readonly Report report;
        private int fileIndex;
        private bool started;
        private ITokenizer tokenizer;

        public MultiFileTokenizer(SourceFile[] files, Report report)
            : this(makeCU(files), report)
        {
        }

        public MultiFileTokenizer(SourceUnit compilationUnit, Report report)
        {
            fileIndex = 0;
            this.compilationUnit = compilationUnit;
            this.report = report;

            Initialize();
        }

        public ITokenizer UnderlyingTokenizer
        {
            get { return tokenizer; }
            private set { tokenizer = value; }
        }

        #region IMultiFileTokenizer Members

        public event EventHandler<TokenizerEventArgs> FileChanged;

        public int Col
        {
            get { return tokenizer.Col; }
        }

        public TokenInfo GetNextToken()
        {
            tryChangeFile();

            return tokenizer.GetNextToken();
        }

        public bool NextFile()
        {
            return tryChangeFile();
        }

        public SourceLocation GetSourceLocation()
        {
            return tokenizer.GetSourceLocation();
        }

        public bool IgnoreCase
        {
            get { return tokenizer.IgnoreCase; }

            set { tokenizer.IgnoreCase = value; }
        }

        public bool IgnoreEOLs
        {
            get { return tokenizer.IgnoreEOLs; }

            set { tokenizer.IgnoreEOLs = value; }
        }

        public bool IgnoreSpaces
        {
            get { return tokenizer.IgnoreSpaces; }

            set { tokenizer.IgnoreSpaces = value; }
        }

        public bool IsEOF
        {
            get { return tokenizer.IsEOF; }
        }

        public int Ln
        {
            get { return tokenizer.Ln; }
        }

        public TokenInfo PeekToken()
        {
            tryChangeFile();

            return tokenizer.PeekToken();
        }

        public SourceFile SourceFile { get; private set; }

        public Report Report
        {
            get { return report; }
        }

        public TextReader TextReader
        {
            get { throw new Exception(""); }
        }

        public SourceUnit CompilationUnit
        {
            get { return compilationUnit; }
        }

        #endregion

        private void Initialize()
        {
            SourceFile = compilationUnit[fileIndex];
            UnderlyingTokenizer = new Tokenizer(SourceFile, Report);

            FileChanged += MultiFileTokenizer_FileChanged;
        }

        private void MultiFileTokenizer_FileChanged(object sender, TokenizerEventArgs e)
        {
        }

        private static SourceUnit makeCU(SourceFile[] files)
        {
            var unit = new SourceUnit();

            foreach (SourceFile file in files)
            {
                unit.Add(file);
            }

            return unit;
        }

        private bool tryChangeFile()
        {
            if (!started)
            {
                FileChanged(this, new TokenizerEventArgs(UnderlyingTokenizer, SourceFile));
                started = true;

                return true;
            }
            else
            {
                if (tokenizer.IsEOF)
                {
                    fileIndex++;

                    if (fileIndex < compilationUnit.Count)
                    {
                        SourceFile = compilationUnit[fileIndex];
                        UnderlyingTokenizer = new Tokenizer(SourceFile, Report);

                        FileChanged(this, new TokenizerEventArgs(UnderlyingTokenizer, SourceFile));

                        return true;
                    }
                }
            }

            return false;
        }
    }
}