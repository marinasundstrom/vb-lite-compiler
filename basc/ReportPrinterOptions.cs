using System;

namespace Basic
{
    /// <summary>
    /// ReportPrinter options.
    /// </summary>
    [Flags]
    public enum ReportPrinterOptions
    {
        /// <summary>
        /// Default configuration. Shows prioritized messages and visualizes code-lines.
        /// </summary>
        Default = ShowPrioritizedMessages | Visualize,

        /// <summary>
        /// Shows only the most prioritized message in a statement.
        /// </summary>
        ShowPrioritizedMessages = 1,

        /// <summary>
        /// Visualizes code-lines.
        /// </summary>
        Visualize = 2,

        /// <summary>
        /// Colorizes the visualized code-lines. (Requires the code-line visualization to be switched on).
        /// </summary>
        Colorize = 4
    }
}