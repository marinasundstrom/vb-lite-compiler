using System;
using System.Collections.Generic;
using Basic.Ast;

namespace Basic
{
    public class NamespaceEntry
    {
        public readonly List<NamespaceEntry> Children;

        public readonly List<FileNamespaceEntry> FileNamespaceEntries;
        public readonly string Name;

        public readonly Namespace Namespace;
        public readonly NamespaceEntry Parent;

        public NamespaceEntry(Namespace ns)
        {
            Name = ns.Name;
            Children = new List<NamespaceEntry>();
            FileNamespaceEntries = new List<FileNamespaceEntry>();

            Namespace = ns;
        }

        public NamespaceEntry(Namespace ns, NamespaceEntry parent)
            : this(ns)
        {
            Parent = parent;
            Parent.Children.Add(this);
        }

        public NamespaceEntry GetNamespace(MemberName name)
        {
            throw new Exception();
        }

        public TypeEntry GetType(MemberName name)
        {
            throw new Exception();
        }

        public TypeEntry[] GetTypes(MemberName name)
        {
            throw new Exception();
        }
    }
}