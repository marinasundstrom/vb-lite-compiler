using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public abstract class Block : Statement
    {
        private readonly LocalScope localScope;
        protected readonly List<Statement> statements;
        protected Label Label_End;
        protected int Label_References;

        public Block(MethodDef method, Block parent)
            : base(method, parent)
        {
            statements = new List<Statement>();

            if (parent == null)
            {
                localScope = new LocalScope(this, null);
            }
            else
            {
                localScope = new LocalScope(this, parent.localScope);
            }
        }

        public bool IsEndLabelReferenced
        {
            get { return Label_References > 0; }
        }

        public LocalScope LocalScope
        {
            get { return localScope; }
        }

        public Block ContainingBlock
        {
            get { return Block; }
        }

        public IEnumerable<Statement> Statements
        {
            get { return statements; }
        }

        public Block RootBlock
        {
            get
            {
                if(ContainingBlock != null)
                {
                    return this.ContainingBlock.RootBlock;
                }

                return this;
            }
        }

        public Label GetEndLabel()
        {
            Label_References++;
            return Label_End;
        }

        public void AddStatement(Statement statement)
        {
            statements.Add(statement);
        }

        public override void CheckAndResolve()
        {
            //Resolve statements.
            foreach (Statement stmt in statements)
            {
                stmt.CheckAndResolve();
            }
        }

        public override void Emit()
        {
            //Define end label.
            Label_End = Method.Generator.DefineLabel();

            Method.Generator.BeginScope();

            //Emit statements.
            foreach (Statement stmt in statements)
            {
                stmt.Emit();
            }

            //Only mark the end with a label if it is referenced. 
            if (Label_References != 0)
            {
                Method.Generator.MarkLabel(Label_End);
            }

            Method.Generator.EndScope();
        }

        public IVariable ResolveVariableName(FullNamedExpression var)
        {
            string str = var.ToString();

            IVariable r = LocalScope.GetVariable(str);

            if (r != null) return r;

            return Method.Parent.Fields.First(x => x.Name == str);
        }
    }
}