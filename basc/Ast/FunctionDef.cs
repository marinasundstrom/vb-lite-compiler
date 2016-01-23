namespace Basic.Ast
{
    public class FunctionDef : MethodDef
    {
        public FunctionDef(ClassStructOrModuleDef parent, string name, FullNamedExpression returnType,
                           ParameterList param, Modifier modifiers)
            : base(parent, name, returnType, param, modifiers)
        {
        }

        public override void Define()
        {
            base.Define();
        }
    }
}