namespace Basic.Ast
{
    public class RootNamespace : Namespace
    {
        public RootNamespace(Namespace parent, string name)
            : this(
                parent,
                (parent is GlobalRootNamespace
                     ? new MemberName(name)
                     : new MemberName(parent.MemberName, Separators.Dot, name)))
        {
            parent.AddNamespace(this);
        }

        protected RootNamespace(Namespace parent, MemberName name)
            : base(parent, name)
        {
        }
    }
}