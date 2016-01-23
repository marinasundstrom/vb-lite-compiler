using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Basic.Ast
{
    public class WhileStatement: Block
    {
        public WhileStatement(Block block)
            : base(block.Method, block)
        {
        }

        public Expression Condition { get; private set; }

        public Block Do { get { return this; } }

        public override void Emit()
        {
            //Define end label.
            Label_End = Method.Generator.DefineLabel();

            Label label_start = Method.Generator.DefineLabel();

            Method.Generator.BeginScope();

            Method.Generator.MarkLabel(label_start);

            Condition.Emit();

            Method.Generator.Emit(OpCodes.Brfalse_S, this.GetEndLabel());

            //Emit statements.
            foreach (Statement stmt in statements)
            {
                stmt.Emit();
            }

            Method.Generator.Emit(OpCodes.Br, label_start);

            Method.Generator.EndScope();

            Method.Generator.MarkLabel(Label_End);
        }

        public override void CheckAndResolve()
        {
            Condition.CheckAndResolve();

            base.CheckAndResolve();
        }

        public void SetCondition(Expression expr)
        {
            this.Condition = expr;
        }
    }
}
