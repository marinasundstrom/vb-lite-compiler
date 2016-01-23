namespace Basic.Ast
{
    public class SubroutineDef : MethodDef
    {
        public SubroutineDef(ClassStructOrModuleDef parent, string name, ParameterList param, Modifier modifiers)
            : base(parent, name, new SimpleName(null, "Void", null), param, modifiers)
        {
        }

        public SubroutineDef(ClassStructOrModuleDef parent, string name, ParameterList param, Modifier modifiers,
                             MemberName _event)
            : this(parent, name, param, modifiers)
        {
            Event = _event;
        }

        public MemberName Event { get; private set; }

        public bool IsEventHandler
        {
            get { return Event != null; }
        }

        public override void Define()
        {
            base.Define();
        }

        public override void Emit()
        {
            base.Emit();
        }

        public override void Resolve()
        {
            base.Resolve();
        }
    }
}