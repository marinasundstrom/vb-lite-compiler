namespace Basic.Ast
{
    public class ModuleDef : ClassStructOrModuleDef
    {
        public ModuleDef(Namespace ns, string name, Modifier modifiers)
            : base(ns, name, null, null, modifiers)
        {
        }

        public override void Define()
        {
            Modifiers = Modifiers | Modifier.Shared;

            base.Define();
        }
    }
}