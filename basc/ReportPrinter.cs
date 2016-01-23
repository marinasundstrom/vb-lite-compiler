using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Basic
{
    /// <summary>
    /// A Report printer.
    /// </summary>
    public class ReportPrinter
    {
        private ReportColorScheme colorScheme;

        private bool colorize = true;
        private bool showOneErrorPerStmt = true;
        private bool visualize = true;

        /// <summary>
        /// Initializes an instance of the ReportPrinter class.
        /// </summary>
        /// <param name="writer">The TextWriter to write to.</param>
        /// <param name="report">The Report to print.</param>
        public ReportPrinter(TextWriter writer, Report report)
        {
            Writer = writer;
            Report = report;

            Console.SetOut(writer);
        }

        /// <summary>
        /// Gets the underlying TextWriter.
        /// </summary>
        public TextWriter Writer { get; private set; }

        /// <summary>
        /// Gets the Report to print.
        /// </summary>
        public Report Report { get; private set; }

        /// <summary>
        /// Prints the Report.
        /// </summary>
        /// <param name="visualize">Activate code-line visualization if possible.</param>
        /// <param name="colorize">Use colorization of code. Requires code-line visualization to be activated.</param>
        /// <param name="showOneErrorPerStmt">Show only the most prioritized error of each line.</param>
        public void Print(bool visualize = true, bool colorize = false, bool showOneErrorPerStmt = true)
        {
            Print(visualize, colorize, showOneErrorPerStmt);
        }

        /// <summary>
        /// Prints the Report.
        /// </summary>
        /// <param name="colorScheme">The color scheme used.</param>
        /// <param name="visualize">Activate code-line visualization if possible.</param>
        /// <param name="colorize">Use colorization of code. Requires code-line visualization to be activated.</param>
        /// <param name="showOneErrorPerStmt">Show only the most prioritized error of each line.</param>
        public void Print(ReportColorScheme colorScheme, bool visualize = true, bool colorize = false,
                          bool showOneErrorPerStmt = true)
        {
            this.colorScheme = colorScheme;

            this.visualize = visualize;
            this.showOneErrorPerStmt = showOneErrorPerStmt;

            if (!visualize && colorize)
                throw new Exception("Code-line visualization must be active in order to print colorized code-lines.");

            this.colorize = colorize;

            if (colorize)
            {
                Console.BackgroundColor = this.colorScheme.BackgroundColor;
                Console.ForegroundColor = this.colorScheme.ForegroundColor;
            }


            IEnumerable<ReportItem> items = Report.Items;

            foreach (ReportItem x in items)
            {
                if (showOneErrorPerStmt)
                {
                    IEnumerable<ReportItem> z = Report.Items.Where(n => n.SourceSpan.Start.Ln == x.SourceSpan.Start.Ln);
                    IEnumerable<ReportItem> m = z.OrderBy(y => y.MessageDescriptor.Priority);

                    if (m.Count() > 1)
                    {
                        m = m.Intersect(new[] {m.ElementAt(0)});
                    }

                    if (m.ElementAt(0) == x)
                    {
                        PrintReportItem(x);
                    }
                }
                else
                {
                    PrintReportItem(x);
                }
            }
        }

        private void PrintReportItem(ReportItem item)
        {
            PrintMessage(item);

            if (item.MessageDescriptor.IsVisualizable && visualize)
            {
                VisualizeMessage(item);
            }
        }

        private void VisualizeMessage(ReportItem item)
        {
            Console.WriteLine();

            foreach (TokenInfo token in item.SourceLine)
            {
                if (token.Is(Token.EOL))
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.SetCursorPosition(token.GetSourceLocation().Col, Console.CursorTop++);

                    if (colorize)
                    {
                        if (BasicParser.Helpers.IsReservedWord(token))
                            Console.ForegroundColor = colorScheme.ForegorundColor_Keyword;
                        else
                            Console.ForegroundColor = colorScheme.ForegroundColor; //ConsoleColor.Gray;
                    }

                    Console.Write("{0} ", token.Value);
                }
            }

            Console.SetCursorPosition(item.SourceSpan.Start.Col, Console.CursorTop + 1);

            if (colorize) Console.ForegroundColor = colorScheme.ForegroundColor_ErrorUnderline;

            for (int i = item.SourceSpan.Start.Col; i < item.SourceSpan.End.Col; i++)
            {
                Console.Write('~');
            }

            Console.WriteLine();

            if (colorize)
            {
                Console.ForegroundColor = colorScheme.ForegroundColor; // ConsoleColor.Gray;
            }
        }

        private static void PrintMessage(ReportItem item)
        {
            Console.WriteLine(string.Format("{0}({1}) : error {2}: {3}", item.SourceFile, item.SourceSpan.Start.Ln,
                                            item.MessageDescriptor.Code,
                                            item.Message));
        }
    }
}