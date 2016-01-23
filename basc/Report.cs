using System;
using System.Collections;
using System.Collections.Generic;

namespace Basic
{
    public class Report : IEnumerable<ReportItem>
    {
        public Report()
        {
            Items = new List<ReportItem>();

            Instance = this;
        }

        internal static Report Instance { get; private set; }

        public bool HasErrors { get; private set; }

        public bool HasWarnings { get; private set; }
        public List<ReportItem> Items { get; private set; }

        #region IEnumerable<ReportItem> Members

        public IEnumerator<ReportItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        public void AddItem(MessageDescriptor descr, SourceFile sourceFile, SourceSpan sourceSpan,
                            TokenList sourceLine)
        {
            var item = new ReportItem(descr, sourceFile, sourceSpan, sourceLine);
            Items.Add(item);

            //set
        }

        public void AddItem(MessageDescriptor descr, SourceFile sourceFile, SourceSpan sourceSpan,
                            TokenList sourceLine, params string[] args)
        {
            var item = new ReportItem(descr, sourceFile, sourceSpan, sourceLine, args);
            Items.Add(item);

            //set
        }

        public void AddMessage(string errorCode, string description, SourceFile sourceFile, SourceSpan sourceSpan,
                               TokenList sourceLine)
        {
            var descr = new MessageDescriptor
                            {
                                Code = errorCode,
                                Message = description,
                                Type = MessageType.Message,
                                IsVisualizable = false
                            };

            Items.Add(new ReportItem(descr, sourceFile, sourceSpan, sourceLine));
        }

        public void AddWarning(string errorCode, string description, SourceFile sourceFile, SourceSpan sourceSpan,
                               TokenList sourceLine, bool visualize = false)
        {
            var descr = new MessageDescriptor
                            {
                                Code = errorCode,
                                Message = description,
                                Type = MessageType.Warning,
                                IsVisualizable = visualize
                            };

            Items.Add(new ReportItem(descr, sourceFile, sourceSpan, sourceLine));
        }

        public void AddError(string errorCode, string description, SourceFile sourceFile, SourceSpan sourceSpan,
                             TokenList sourceLine)
        {
            var descr = new MessageDescriptor
                            {Code = errorCode, Message = description, Type = MessageType.Error, IsVisualizable = false};

            Items.Add(new ReportItem(descr, sourceFile, sourceSpan, sourceLine));
        }
    }
}