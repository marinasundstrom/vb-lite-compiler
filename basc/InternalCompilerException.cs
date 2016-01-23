using System;

namespace Basic
{
    public class InternalCompilerException : Exception
    {
        public InternalCompilerException(string message)
            : base(message)
        {
        }
    }
}