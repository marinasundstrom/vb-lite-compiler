using System.Linq;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement(Block block)
            : base(block)
        {
        }

        public Expression Expression { get; private set; }

        public void SetExpression(Expression expr)
        {
            Expression = expr;
        }

        public override void Emit()
        {
            ILGenerator generator = Block.Method.Generator;

            //if (this.Expression != null)
            //{
            //    this.Expression.Emit(Ast.Expression.EvaluationResult.ExpressionOrConstant);

            //    if (this.Expression.resolveContext.ResultType == Expression.EvaluationResult.Constant)
            //    {
            //        generator.Emit(OpCodes.Ldc_I4, (int)this.Expression.resolveContext.ResultValue);
            //    }
            //}

            if (Expression != null)
            {
                Expression.Emit();
                //generator.Emit(OpCodes.Ldc_I4, (int)this.Expression.resolveContext.ResultValue);
            }

            if (Block.Method.Body == Block
                && Block.Statements.ElementAt(Block.Statements.Count() - 1) == this)
            {
                //generator.Emit(OpCodes.Ret);
            }
            else
            {
                Label endLbl = this.Block.RootBlock.GetEndLabel();
                generator.Emit(OpCodes.Br, endLbl);

                Block.Method.InnerReturnJump = true;
            }
        }

        public override void CheckAndResolve()
        {
            Expression.CheckAndResolve();
        }
    }
}