namespace Basic.Ast
{
    public class ImportDirective
    {
        public ImportDirective(MemberName ns)
        {
            Namespace = ns;
        }

        public MemberName Namespace { get; private set; }
    }
}