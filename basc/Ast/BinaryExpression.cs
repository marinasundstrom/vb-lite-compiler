using System;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Statement statement, Operator op, Expression ret, Expression rhs, SourceData sourceData)
            : base(statement, sourceData)
        {
            // TODO: Complete member initialization
            Operator = op;
            Left = ret;
            Right = rhs;

            Left.SetParent(this);
            Right.SetParent(this);
        }

        public Operator Operator { get; private set; }

        public Expression Left { get; private set; }

        public Expression Right { get; private set; }

        public override void Emit()
        {
            ILGenerator generator = Statement.Block.Method.Generator;

            if (Operator == Operator.Assign)
            {
                if (Parent == null)
                {
                    if (this.Operator != Ast.Operator.Assign && this.Statement is ExpressionStatement)
                    {
                        //Surpress and don't emit
                    }
                    else
                    {
                        //Assign expression
                        Right.Emit();

                        var exp = Left as VariableAccess;
                        IVariable target = Block.ResolveVariableName(exp.Member);

                        target.EmitStore(Block);
                    }
                }
                else
                {
                    //Boolean expression
                    Right.Emit();
                    Left.Emit();

                    switch (Operator)
                    {
                            //case Ast.Operator.Assign:
                            //    generator.Emit(OpCodes.Cmp);
                            //    break;

                            //case Ast.Operator.Minus:
                            //    generator.Emit(OpCodes.Sub);
                            //    break;

                            //case Ast.Operator.Star:
                            //    generator.Emit(OpCodes.Mul);
                            //    break;

                            //case Ast.Operator.Slash:
                            //    generator.Emit(OpCodes.Div);
                            //    break;

                        default:
                            throw new Exception();
                    }
                }
            }
            else
            {
                Left.Emit();
                Right.Emit();

                switch (Operator)
                {
                    case Operator.Plus:
                        generator.Emit(OpCodes.Add);
                        break;

                    case Operator.Minus:
                        generator.Emit(OpCodes.Sub);
                        break;

                    case Operator.Star:
                        generator.Emit(OpCodes.Mul);
                        break;

                    case Operator.Slash:
                        generator.Emit(OpCodes.Div);
                        break;

                    case Operator.Equal:
                        generator.Emit(OpCodes.Ceq);
                        break;

                    case Operator.NotEquals:
                        throw new NotImplementedException();
                        generator.Emit(OpCodes.Ceq);
                        break;

                    case Operator.Less:
                        generator.Emit(OpCodes.Clt);
                        break;

                    case Operator.Greater:
                        generator.Emit(OpCodes.Cgt);
                        break;

                    default:
                        throw new Exception();
                }
            }
        }

        public override void CheckAndResolve()
        {
            Left.CheckAndResolve();
            Right.CheckAndResolve();

            if (this.Parent == null && this.Operator != Ast.Operator.Assign && this.Statement is ExpressionStatement)
            {
                Report.Instance.AddWarning("BCxxxxx", "Suppressed useless statement.", this.SourceData.SourceFile, this.SourceData.SourceSpan,
                                           this.SourceData.Tokens, true);
            }          
        }
    }
}