namespace Basic
{
    public static class StringResources
    {
        public static readonly string IdentifierExpected = "Identifier expected.";

        public static readonly string RightParenthesisExpected = "\')\' expected.";

        public static readonly string Sub_Unexpected = "\"Sub {0}\" unexpected.";

        public static readonly string StatementInMethodBodyEosAssumed =
            "Statement cannot appear within a method body. End of statement assumed.";

        public static readonly string EndOfStatementExpected = "End of statement expected.";

        public static readonly string SyntaxError = "Syntax error.";

        public static readonly string StatementCannotAppearWithinAMethodBodyEndOfMethodAssumedError =
            "Statement cannot appear within a method body. End of method assumed.";

        public static readonly string ModuleStatementMustEndWithAMatchingEndModule =
            "\'Module\' statement must end with a matching 'End Module'.";

        public static readonly string ClassStatementMustEndWithAMatchingEndClass =
            "\'Class\' statement must end with a matching 'End Class'.";

        public static readonly string FunctionStatementMustEndWithAMatchingEndFunction =
            "\'Function\' statement must end with a matching 'End Function'.";

        public static readonly string SubStatementMustEndWithAMatchingEndSub =
            "\'Sub\' statement must end with a matching 'End Sub'.";

        public static readonly string EndSubExpected = "\'End Sub\' expected.";

        public static readonly string EndFunctionExpected = "\'End Function\' expected.";

        public static readonly string ExpressionExpected = "Expression expected.";

        public static readonly string BracketedIdentifierIsMissingClosingRightSquareBracket =
            "Bracketed identifier is missing closing ']'.";

        public static readonly string CommaOrValidExpressionContinuationExpected =
            "Comma, ')', or a valid expression continuation expected.";

        public static readonly string LocalVariable_variablename_IsAlreadyDeclaredInTheCurrentBlock =
            "Local variable '{0}' is already declared in the current block.";

        public static readonly string
            CannotReferToAnInstanceMemberOfAClassFromWithiASharedMethodOrSharedMemberInitializerWithoutAnExplicitInstanceOfTheClass
                =
                "Cannot refer to an instance member of a class from within a shared method or shared member initializer without an explicit instance of the class.";
    }
}