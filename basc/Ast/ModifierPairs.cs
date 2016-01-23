namespace Basic.Ast
{
    public static class ModifierPairs
    {
        public static readonly Modifier[] AccessModifiers = {Modifier.Private, Modifier.Public, Modifier.Internal};
        public static readonly Modifier[] InheritanceModifiers = {Modifier.Shared};
        public static readonly Modifier[] QualifierModifiers = {Modifier.Const};
    }
}