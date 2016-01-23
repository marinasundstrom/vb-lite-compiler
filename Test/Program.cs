using System;
using System.IO;
using System.Reflection;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Build source file
            Basic.Program.Main("Tests/helloworld.vb");

            var assembly = Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, "helloworld.exe"));
            var module = assembly.GetType("ConsoleApplication1.Module1");
            var foo = module.GetMethod("Foo");

            //Run code
            int i = (int)foo.Invoke(null, new object[] { 2 });
            Console.WriteLine(i);
        }
    }
}