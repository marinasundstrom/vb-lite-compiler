using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Basic
{
    /// <summary>
    /// A tokenizer.
    /// </summary>
    public class Tokenizer : ITokenizer
    {
        private readonly SourceFile sourceFile;
        private readonly TextReader textReader;

        private State currentState;
        private State lookaheadState;

        private string[] reservedWords;

        public Tokenizer(string path, Report report)
            : this(new SourceFile(path), report)
        {
        }

        public Tokenizer(SourceFile sourceFile, Report report)
        {
            this.sourceFile = sourceFile;
            Report = report;

            Col = 1;
            Ln = 1;

            textReader = sourceFile.GetReader();

            Initialize();
        }

        private void Initialize()
        {
            reservedWords = TokenStringConstants.ReservedWords;

            IgnoreEOLs = false;
            IgnoreCase = true;
            IgnoreSpaces = true;
        }

        #region Public properties

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int Ln { get; private set; }

        /// <summary>
        /// Gets the column number.
        /// </summary>
        public int Col { get; private set; }

        /// <summary>
        /// Gets the collection of encountered errors or warnings.
        /// </summary>
        public Report Report { get; private set; }

        /// <summary>
        /// Gets and sets the value that indicates whether EOLs should be ignored or not.
        /// </summary>
        public bool IgnoreEOLs { get; set; }

        /// <summary>
        /// Gets and sets the value that indicates whether case should be ignored or not.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets and sets the value that indicates whether spaces should be ignored or not.
        /// </summary>
        public bool IgnoreSpaces { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the end of the file has been reached or not.
        /// </summary>
        public bool IsEOF
        {
            get { return isEos(); }
        }

        /// Gets the object representing the source file.
        /// </summary>
        public SourceFile SourceFile
        {
            get { return sourceFile; }
        }

        /// <summary>
        /// Gets the TextReader assigned to this instance.
        /// </summary>
        public TextReader TextReader
        {
            get { return textReader; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns>The next token in the stream.</returns>
        public TokenInfo GetNextToken()
        {
            return nextToken();
        }

        /// <summary>
        /// Returns the next token without moving the pointer.
        /// </summary>
        /// <returns>The next token in the stream.</returns>
        public TokenInfo PeekToken()
        {
            return peekToken();
        }

        public SourceLocation GetSourceLocation()
        {
            if (lookaheadState == null)
                PeekToken();

            return lookaheadState.Token.GetSourceLocation();
        }

        #endregion

        #region Character flow mechanics

        private char peekChar()
        {
            return (char) textReader.Peek();
        }

        private char readChar()
        {
            var ch = (char) textReader.Read();

            Col++;

            if (ch == '\n')
            {
                Col = 1;
                Ln++;
            }

            return ch;
        }

        private char readChar2()
        {
            var ch = (char) textReader.Read();
            return ch;
        }

        private bool isEos()
        {
            return textReader.Peek() == -1;
        }

        #endregion

        #region Token flow mechanics

        private TokenInfo nextToken()
        {
            if (lookaheadState != null)
            {
                currentState = lookaheadState;
                lookaheadState = null;
            }
            else
            {
                currentState = getNextState();
            }

            return currentState.Token;
        }

        private State getNextState()
        {
            var state = new State
                            {
                                Token = scan(),
                                Col = Col,
                                Ln = Ln
                            };

            return state;
        }

        private TokenInfo peekToken()
        {
            if (lookaheadState == null)
            {
                lookaheadState = getNextState();
            }

            return lookaheadState.Token;
        }

        #endregion

        #region Token scanner

        /// <summary>
        /// Scans for the next token in the stream.
        /// </summary>
        /// <returns>A token. If it is end-of-stream then it is an EOF token.</returns>
        private TokenInfo scan()
        {
            var sb = new StringBuilder();

            int ln, col; //The start location of the token

            //bool isBad = false; //Is it a bad token?

            char ch; //First lookahead variable
            //char? ch2 = null; //Second lookahead variable (temporary)

            ln = 1;
            col = 1;

            while (!isEos())
            {
                ln = Ln;
                col = Col;

                ch = peekChar();

                if (char.IsLetter(ch) || ch == '_')
                {
                    //An identifier token.

                    while (!isEos() && (char.IsLetterOrDigit(ch) || ch == '_'))
                    {
                        readChar();

                        sb.Append(ch);

                        ch = peekChar();
                    }

                    //string rw = getReservedWord(sb.ToString());

                    //if (rw != null)
                    //{
                    //    return reservedWord(rw, sourceFile, ln, col);
                    //}

                    string rw = sb.ToString();

                    if (isReservedWord(rw))
                    {
                        return reservedWord(rw, sourceFile, ln, col);
                    }

                    return identifier(sb, sourceFile, ln, col);
                }
                else if (char.IsNumber(ch))
                {
                    #region Old code

                    ////A number token.

                    //bool hasReadPeriod = false;
                    //bool isReal = false;

                    //while (!this.isEos() && (char.IsDigit(ch) || ch == '.'))
                    //{
                    //    this.readChar();

                    //    if (hasReadPeriod)
                    //    {
                    //        if (char.IsDigit((char)ch))
                    //        {
                    //            hasReadPeriod = false;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (ch2 != null)
                    //        {
                    //            if (char.IsDigit((char)ch2) && ch == '.')
                    //            {
                    //                hasReadPeriod = true;
                    //                isReal = true;

                    //                ch2 = this.peekChar();

                    //                if (!char.IsDigit((char)ch2))
                    //                {
                    //                    this.Report.ReportError("Expected digit.", this.sourceFile, this.GetSourceLocation(), null);
                    //                    isBad = true;
                    //                }
                    //            }
                    //        }

                    //    }

                    //    sb.Append(ch);

                    //    ch2 = ch;
                    //    ch = this.peekChar();
                    //}

                    //if (isBad)
                    //{
                    //    //It is a bad token.
                    //    return badToken(sb.ToString(), ln, col);
                    //}

                    //if (isReal)
                    //{
                    //    return realNumber(sb, ln, col);
                    //}

                    #endregion

                    while (char.IsDigit(ch))
                    {
                        readChar();

                        sb.Append(ch);

                        ch = peekChar();
                    }

                    return number(sb, sourceFile, ln, col);

                    #region Old code

                    // return integerNumber(sb, this.sourceFile, ln, col);

                    #endregion
                }
                else
                {
                    //Symbol tokens.
                    TokenInfo token;

                    switch (ch)
                    {
                        case '+':
                            token = new TokenInfo(Token.Plus, TokenStringConstants.Plus, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '-':
                            token = new TokenInfo(Token.Minus, TokenStringConstants.Minus, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '*':
                            token = new TokenInfo(Token.Star, TokenStringConstants.Star, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '/':
                            token = new TokenInfo(Token.Slash, TokenStringConstants.Slash, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '\\':
                            token = new TokenInfo(Token.Backslash, TokenStringConstants.Backslash, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '%':
                            token = new TokenInfo(Token.Percent, TokenStringConstants.Percent, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '&':
                            token = new TokenInfo(Token.Ampersand, TokenStringConstants.Ampersand, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '=':
                            token = new TokenInfo(Token.Equality, TokenStringConstants.Equality, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '^':
                            token = new TokenInfo(Token.Caret, TokenStringConstants.Caret, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '\'':
                            token = new TokenInfo(Token.SingleQuote, TokenStringConstants.SingleQuote, sourceFile, ln,
                                                  col);
                            readChar();
                            return token;

                        case '\"':
                            token = new TokenInfo(Token.DoubleQuote, TokenStringConstants.DoubleQuote, sourceFile, ln,
                                                  col);
                            readChar();
                            return token;

                        case '.':
                            token = new TokenInfo(Token.Period, TokenStringConstants.Period, sourceFile, ln, col);
                            readChar();
                            return token;

                        case ',':
                            token = new TokenInfo(Token.Comma, TokenStringConstants.Comma, sourceFile, ln, col);
                            readChar();
                            return token;

                        case ':':
                            token = new TokenInfo(Token.Colon, TokenStringConstants.Colon, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '(':
                            token = new TokenInfo(Token.LeftParenthesis, TokenStringConstants.LeftParenthesis,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case ')':
                            token = new TokenInfo(Token.RightParenthesis, TokenStringConstants.RightParenthesis,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case '{':
                            token = new TokenInfo(Token.LeftBrace, TokenStringConstants.LeftBrace, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '}':
                            token = new TokenInfo(Token.RightBrace, TokenStringConstants.RightBrace, sourceFile, ln, col);
                            readChar();
                            return token;

                        case '[':
                            token = new TokenInfo(Token.LeftSquareBracket, TokenStringConstants.LeftSquareBracket,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case ']':
                            token = new TokenInfo(Token.RightSquareBracket, TokenStringConstants.RightSquareBracket,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case '<':
                            token = new TokenInfo(Token.LeftAngleBracket, TokenStringConstants.LeftAngleBracket,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case '>':
                            token = new TokenInfo(Token.RightAngleBracket, TokenStringConstants.RightAngleBracket,
                                                  sourceFile, ln, col);
                            readChar();
                            return token;

                        case ' ':
                            readChar();
                            if (!IgnoreSpaces)
                            {
                                token = space(sourceFile, ln, col);
                                return token;
                            }
                            continue;

                        case '\t':
                            readChar2();
                            continue;

                        case '\r':
                            readChar2();
                            continue;

                        case '\n':
                            readChar();
                            if (!IgnoreEOLs)
                            {
                                token = eol(sourceFile, ln, col);
                                return token;
                            }
                            continue;
                    }

                    //An unspecified token
                    readChar();
                    return new TokenInfo(Token.UNSPECIFIED, ch.ToString(), sourceFile, ln, col);
                }
            }

            return eof(sourceFile, ln, col);
        }

        private bool isReservedWord(string p)
        {
            return reservedWords.Any(n => string.Equals(n, p, StringComparison.OrdinalIgnoreCase));
        }

        private string getReservedWord(string p)
        {
            string word;

            for (int i = 0; i < reservedWords.Length; i++)
            {
                word = reservedWords[i];

                if (string.Compare(p, word, IgnoreCase) == 0)
                {
                    return word;
                }
            }

            return null;
        }

        #endregion

        #region Token factory methods

        private static TokenInfo identifier(StringBuilder sb, SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.Identifier, sb.ToString(), sourceFile, ln, col);
        }

        private static TokenInfo reservedWord(string value, SourceFile sourceFile, int ln, int col)
        {
            var t = (Token) Enum.Parse(typeof (Token), value, true);
            return new TokenInfo(t, value, sourceFile, ln, col);
        }

        private static TokenInfo number(StringBuilder sb, SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.Number, sb.ToString(), sourceFile, ln, col);
        }

        private static TokenInfo eol(SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.EOL, TokenStringConstants.EOL, sourceFile, ln, col);
        }

        private static TokenInfo eof(SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.EOF, TokenStringConstants.EOF, sourceFile, ln, col);
        }

        private static TokenInfo space(SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.SPACE, TokenStringConstants.SPACE, sourceFile, ln, col);
        }

        private static TokenInfo badToken(string value, SourceFile sourceFile, int ln, int col)
        {
            return new TokenInfo(Token.BAD_TOKEN, value, sourceFile, ln, col);
        }

        #region Old code

        //private TokenInfo reservedWord(string value, int ln, int col)
        //{
        //    return new TokenInfo(Token.ReservedWord value, ln, col);
        //}

        #endregion

        #endregion

        #region Nested type: State

        /// <summary>
        /// Represents a tokenizer state.
        /// </summary>
        private class State
        {
            /// <summary>
            /// Gets the line number.
            /// </summary>
            public int Ln { get; set; }

            /// <summary>
            /// Gets the column number.
            /// </summary>
            public int Col { get; set; }

            /// <summary>
            /// Gets the last token that was read.
            /// </summary>
            public TokenInfo Token { get; set; }
        }

        #endregion
    }
}