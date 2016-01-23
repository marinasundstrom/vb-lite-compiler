namespace Basic.Ast
{
    public class ClassDef : ClassOrStructDef
    {
        public ClassDef(ClassOrStructDef parent, string name, MemberName baseClass, MemberName[] inherits,
                        Modifier modifiers)
            : base(parent, name, baseClass, inherits, modifiers)
        {
        }

        public ClassDef(Namespace ns, string name, MemberName baseClass, MemberName[] inherits, Modifier modifiers)
            : base(ns, name, baseClass, inherits, modifiers)
        {
        }

        public override void Define()
        {
            base.Define();
        }
    }
}