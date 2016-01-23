namespace Basic
{
    /// <summary>
    /// Represents a report item.
    /// </summary>
    public class ReportItem
    {
        /// <summary>
        /// Initializes an instance of the ReportItem class.
        /// </summary>
        /// <param name="descriptor">A message descriptor.</param>
        /// <param name="sourceFile">A source file.</param>
        /// <param name="sourceSpan">A source span.</param>
        /// <param name="sourceLine">A source line.</param>
        /// <param name="args">Some arguments required by the message descriptor.</param>
        public ReportItem(MessageDescriptor descriptor, SourceFile sourceFile, SourceSpan sourceSpan,
                          TokenList sourceLine, params string[] args)
        {
            MessageDescriptor = descriptor;
            SourceFile = sourceFile;
            SourceSpan = sourceSpan;
            SourceLine = sourceLine;
            Arguments = args;
        }

        /// <summary>
        /// Gets the MessageDescriptor.
        /// </summary>
        public MessageDescriptor MessageDescriptor { get; private set; }

        /// <summary>
        /// Gets the SourceFile.
        /// </summary>
        public SourceFile SourceFile { get; set; }

        /// <summary>
        /// Gets the SourceSpan.
        /// </summary>
        public SourceSpan SourceSpan { get; private set; }

        /// <summary>
        /// Gets the SourceLine.
        /// </summary>
        public TokenList SourceLine { get; private set; }

        /// <summary>
        /// Gets the arguments required by the MessageDescriptor.
        /// </summary>
        public string[] Arguments { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get
            {
                if (MessageDescriptor.NumberOfArguments > 0)
                {
                    return string.Format(MessageDescriptor.Message, Arguments);
                }

                return string.Format(MessageDescriptor.Message);
            }
        }

        /// <summary>
        /// Gets the ReportItem as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Message;
        }
    }
}