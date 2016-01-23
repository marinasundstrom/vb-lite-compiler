using System.Reflection.Emit;

namespace Basic.Ast
{
    public class IntegerConstant : Expression
    {
        public int nr;

        public IntegerConstant(Statement statement, int nr, SourceData sourceData)
            : base(statement, sourceData)
        {
            // TODO: Complete member initialization
            this.nr = nr;
        }

        public override void CheckAndResolve()
        {
            //this.resolveContext.ResultType = EvaluationResult.Constant;
            //this.resolveContext.ResultValue = this.nr;
        }

        public override void Emit()
        {
            //if (type == EvaluationResult.Expression)
            //{

            //}
            Statement.Block.Method.Generator.Emit(OpCodes.Ldc_I4, nr);
        }
    }
}