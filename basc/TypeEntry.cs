using System.Collections.Generic;
using Basic.Ast;

namespace Basic
{
    public class TypeEntry
    {
        public readonly FileNamespaceEntry Namespace;
        public readonly List<TypeEntry> Nested;

        public readonly TypeEntry Parent;

        public readonly TypeDef Type;

        public TypeEntry(TypeDef type, FileNamespaceEntry ns)
        {
            Type = type;
            Namespace = ns;

            Namespace.Types.Add(this);

            Nested = new List<TypeEntry>();
        }

        public TypeEntry(TypeDef type, TypeEntry parent)
        {
            Type = type;
            Namespace = parent.Namespace;

            Namespace.Types.Add(this);

            Nested = new List<TypeEntry>();
            Parent.Nested.Add(this);
        }
    }
}