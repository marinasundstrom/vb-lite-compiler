using System;
using System.Collections.Generic;
using System.IO;

//using Basic.Ast;

namespace Basic
{
    public class Program
    {
        public static void Main(params string[] args)
        {
            //tokenizerTest();

            //parserTest();

            Run(args);
        }

        private static void Run(params string[] args)
        {
            header();

            var context = new CompilerContext();

            try
            {
                foreach (string arg in args)
                {
                    context.SourceUnit.Add(arg);
                }

                var compiler = new Compiler();
                compiler.Build(context, Path.GetFileNameWithoutExtension(args[0]), false);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}\n", e);
            }
            finally
            {
                var printer = new ReportPrinter(Console.Out, context.Report);
                printer.Print(ReportColorScheme.Default, colorize: true, visualize: true, showOneErrorPerStmt: true);
            }
        }

        private static void parserTest()
        {
            header();
            
            var cu = new SourceUnit();
            cu.Add("Tests/helloworld.vb");

            var context = new CompilerContext();

            try
            {
                var parser = new BasicParser(context);
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}\n", e);
            }
            finally
            {
                printErrors(context.Report);
            }
        }

        private static void header()
        {
            Console.WriteLine("Visual Basic compiler version 1.0.100");
            Console.WriteLine("Copyright (c) Robert Sundström. All rights reserved.\n");
        }

        private static void tokenizerTest()
        {
            var list = new List<TokenInfo>();

            var report = new Report();
            var tokenizer = new Tokenizer("Tests/helloworld.vb", report);

            TokenInfo token;

            while (!tokenizer.IsEOF)
            {
                token = tokenizer.GetNextToken();

                list.Add(token);

                Console.WriteLine("{0,-20} {1, -30} {2}, {3}", token.GetString(), token.Token, token.Ln, token.Col);
            }

            token = tokenizer.PeekToken();
            list.Add(token);

            Console.WriteLine(token);

            printErrors(report);
        }

        private static void printErrors(Report report)
        {
            var printer = new ReportPrinter(Console.Out, report);
            printer.Print(ReportColorScheme.Default, colorize: true);
        }
    }
}