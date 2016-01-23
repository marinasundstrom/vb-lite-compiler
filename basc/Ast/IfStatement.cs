using System.Reflection.Emit;

namespace Basic.Ast
{
    public class IfStatement : Block
    {
        public IfStatement(Block block)
            : base(block.Method, block)
        {
        }

        public Expression Condition { get; private set; }

        public Block Then { get { return this; } }

        public Statement Else { get; private set; }

        public override void Emit()
        {
            //Define end label.
            Label_End = Method.Generator.DefineLabel();

            Method.Generator.BeginScope();

            Condition.Emit();

            Method.Generator.Emit(OpCodes.Brfalse_S, this.GetEndLabel());

            //Emit statements.
            foreach (Statement stmt in statements)
            {
                stmt.Emit();
            }

            Method.Generator.EndScope();

            Method.Generator.MarkLabel(Label_End);

            if (Else != null)
            {
                Else.Emit();
            }
        }

        public override void CheckAndResolve()
        {
            Condition.CheckAndResolve();

            base.CheckAndResolve();
            
            if(Else != null) Else.Emit();
        }

        public void SetElse(Ast.Statement statement)
        {
            this.Else = statement;
        }

        public void SetCondition(Expression expr)
        {
            this.Condition = expr;
        }
    }
}