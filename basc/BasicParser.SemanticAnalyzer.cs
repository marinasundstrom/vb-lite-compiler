using Basic.Ast;

namespace Basic
{
    partial class BasicParser
    {
        #region Nested type: SemanticAnalyzer

        internal class SemanticAnalyzer
        {
            public static ITypeDefinition AnalyzeExpression(MethodDef method, Expression expression,
                                                            bool shallReturnResult)
            {
                return null;
            }

            public static void AnalyzeAssignment(ITypeDefinition toType, ITypeDefinition assignType)
            {
                assignType.IsAssignableFrom(toType);
            }
        }

        #endregion
    }
}