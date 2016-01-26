Error Reporting
==

If and when a problem has been encountered, either with syntax or samantics, an item will be added to the Report together with information about its location in source. It might also store the source span between which the message was thrown.

If it is a syntax error, the parsed line will be stored in order for the ReportPrinter to recreate the line for viewing.

The ReportPrinter can print errors with colorized code-highlights.