using System;

namespace Basic.Ast
{
    public class ErrorStatement : Statement
    {
        public ErrorStatement(Block block)
            : base(block)
        {
        }

        public override void Emit()
        {
            throw new NotImplementedException();
        }

        public override void CheckAndResolve()
        {
            throw new NotImplementedException();
        }
    }
}