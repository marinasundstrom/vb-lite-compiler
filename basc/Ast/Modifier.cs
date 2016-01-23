using System;

namespace Basic.Ast
{
    [Flags]
    public enum Modifier
    {
        None = 0x0,
        Private = 0x01,
        Public = 0x02,
        Internal = 0x04,
        Protected = 0x08,
        Shared = 0x10,
        Const = 0x20
    }
}