using System;

namespace Basic.Ast
{
    public class Expression : AstNode
    {
        #region EvaluationResult enum

        [Flags]
        public enum EvaluationResult
        {
            Nothing,
            Expression,
            Constant,
            ExpressionOrConstant = Expression | Constant
        }

        #endregion

        public ExpressionResolveContext resolveContext;

        public Expression(Statement statement, SourceData sourceData)
        {
            Statement = statement;
            SourceData = sourceData;

            resolveContext = new ExpressionResolveContext();
        }

        public Statement Statement { get; private set; }

        public Block Block
        {
            get { return Statement.Block; }
        }

        public MethodDef Method
        {
            get { return Statement.Block.Method; }
        }

        public SourceData SourceData { get; private set; }

        public Expression Parent { get; private set; }

        public void SetParent(Expression expr)
        {
            Parent = expr;
        }

        public virtual void CheckAndResolve()
        {
        }

        public virtual void Emit()
        {
        }

        #region Nested type: ExpressionResolveContext

        public struct ExpressionResolveContext
        {
            public EvaluationResult ResultType;
            public double ResultValue;
            public bool Simplify;
            public ITypeDefinition Type;
        }

        #endregion
    }
}