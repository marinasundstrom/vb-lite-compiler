namespace Basic.Ast
{
    public class GlobalRootNamespace : RootNamespace
    {
        protected GlobalRootNamespace(string name)
            : base(null, new MemberName(name))
        {
        }

        public GlobalRootNamespace()
            : this(string.Empty)
        {
        }
    }
}