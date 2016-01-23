using System;

namespace Basic
{
    public class ReportColorScheme
    {
        public static ReportColorScheme LightAndBlue = new ReportColorScheme
                                                           {
                                                               BackgroundColor = ConsoleColor.Gray,
                                                               ForegroundColor = ConsoleColor.Black,
                                                               ForegorundColor_Keyword = ConsoleColor.Blue,
                                                               ForegroundColor_ErrorUnderline = ConsoleColor.Red
                                                           };

        public static ReportColorScheme LightAndRed = new ReportColorScheme
                                                          {
                                                              BackgroundColor = ConsoleColor.Gray,
                                                              ForegroundColor = ConsoleColor.Black,
                                                              ForegorundColor_Keyword = ConsoleColor.Red,
                                                              ForegroundColor_ErrorUnderline = ConsoleColor.DarkRed
                                                          };

        public static ReportColorScheme Default = new ReportColorScheme
                                                      {
                                                          BackgroundColor = ConsoleColor.Black,
                                                          ForegroundColor = ConsoleColor.Gray,
                                                          ForegorundColor_Keyword = ConsoleColor.Blue,
                                                          ForegroundColor_ErrorUnderline = ConsoleColor.Red
                                                      };

        public ConsoleColor BackgroundColor;
        public ConsoleColor ForegorundColor_Keyword;
        public ConsoleColor ForegroundColor;
        public ConsoleColor ForegroundColor_ErrorUnderline;

        protected ReportColorScheme()
        {
        }
    }
}