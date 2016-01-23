using System.Collections.Generic;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class MethodCallOrArrayDereference : Expression
    {
        private MethOrArrDerfResolverContext resolveContext2;

        public MethodCallOrArrayDereference(Statement statement, FullNamedExpression member, Expression[] args,
                                            SourceData sourceData)
            : base(statement, sourceData)
        {
            // TODO: Complete member initialization
            Member = member;
            Arguments = args;
        }

        public FullNamedExpression Member { get; private set; }

        public Expression[] Arguments { get; private set; }

        public override void CheckAndResolve()
        {
            //Check fullnamed expression.
            //Determine the type type of the expression: array dereference or method call.
            //Is name even a proper name?

            //Check the arguments possed

            resolveContext2 = new MethOrArrDerfResolverContext();

            var parameterTypes = new List<ITypeDefinition>();

            foreach (Expression arg in Arguments)
            {
                arg.CheckAndResolve();
                parameterTypes.Add(arg.resolveContext.Type);
            }

            resolveContext2.Method = Method.ResolveMethodName(Member, parameterTypes.ToArray());
        }

        public override void Emit()
        {
            foreach (Expression arg in Arguments)
            {
                arg.Emit();
            }

            Statement.Block.Method.Generator.Emit(OpCodes.Call, resolveContext2.Method.GetMethodInfo());
        }

        #region Nested type: MethOrArrDerfResolverContext

        private class MethOrArrDerfResolverContext
        {
            public IMethodDefinition Method;
        }

        #endregion
    }
}