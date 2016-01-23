namespace Basic
{
    public enum Token
    {
        //Symbolic tokens
        UNSPECIFIED,
        BAD_TOKEN,

        //Sequence tokens
        EOF,
        EOL,

        SPACE,

        //Value tokens
        Identifier,
        ReservedWord,
        Number,

        //Operators
        Plus,
        Minus,
        Star,
        Slash,
        Backslash,
        Percent,
        Ampersand,
        Equality,
        Caret,

        //Separators
        Period,
        Comma,
        Colon,

        //Quotes
        SingleQuote,
        DoubleQuote,

        //Delimiter tokens
        LeftParenthesis,
        RightParenthesis,
        LeftBrace,
        RightBrace,
        LeftAngleBracket,
        RightAngleBracket,
        LeftSquareBracket,
        RightSquareBracket,

        Inequality,

        GreaterEqual,
        SmallerEqual,

        //Keyword tokens
        Imports,
        Namespace,
        Module,
        Sub,
        Function,
        End,
        Public,
        Private,
        Internal,
        Shared,
        Static,
        Const,
        Me,
        My,
        True,
        False,
        Dim,
        ByVal,
        ByRef,
        Call,
        Handles,
        If,
        Then,
        Else,
        ElseIf,
        While,
        When,
        Do,
        For,
        Each,
        Next,
        Is,
        IsNot,
        Not,
        And,
        AndAlso,
        Or,
        OrElse,
        Xor,
        Mod,
        GoTo,
        Return,
        As,
        Nothing,
        Void,
        Integer,
        Double,
        Character,
        Boolean,
        String,
        Object,
        Variant
    }
}