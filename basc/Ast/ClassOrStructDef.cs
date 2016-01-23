namespace Basic.Ast
{
    public abstract class ClassOrStructDef : ClassStructOrModuleDef
    {
        public ClassOrStructDef(ClassOrStructDef parent, string name, MemberName baseClass, MemberName[] inherits,
                                Modifier modifiers)
            : base(parent, name, baseClass, inherits, modifiers)
        {
            parent.AddNestedType(this);
        }

        public ClassOrStructDef(Namespace ns, string name, MemberName baseClass, MemberName[] inherits,
                                Modifier modifiers)
            : base(ns, name, baseClass, inherits, modifiers)
        {
        }

        public void AddNestedType(TypeDef type)
        {
            nestedTypes.Add(type);
        }

        public override void Emit()
        {
            base.Emit();
        }
    }
}