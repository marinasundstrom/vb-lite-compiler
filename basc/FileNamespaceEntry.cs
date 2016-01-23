using System.Collections.Generic;
using Basic.Ast;

namespace Basic
{
    public class FileNamespaceEntry
    {
        private static readonly FullNamedExpression StandardNamespace = new SimpleName(null, "Basic", null);
        public readonly Dictionary<string, MemberName> Aliases;
        public readonly List<FileNamespaceEntry> Children;
        public readonly FileEntry File;

        public readonly List<MemberName> Imports;

        public readonly NamespaceEntry NamespaceEntry;

        public readonly FileNamespaceEntry Parent;

        public readonly List<TypeEntry> Types;

        private FileNamespaceEntry(FileEntry file)
        {
            File = file;

            Types = new List<TypeEntry>();

            Imports = new List<MemberName>();
            Aliases = new Dictionary<string, MemberName>();

            //Imports.Add(StandardNamespace);
        }

        private FileNamespaceEntry(NamespaceEntry namespaceEntry, FileEntry file)
            : this(file)
        {
            NamespaceEntry = namespaceEntry;
            NamespaceEntry.FileNamespaceEntries.Add(this);
        }

        public FileNamespaceEntry(NamespaceEntry namespaceEntry, FileNamespaceEntry parent, string name, FileEntry file)
            : this(namespaceEntry, file)
        {
            Children = new List<FileNamespaceEntry>();

            if (parent != null)
            {
                Parent = parent;
                Parent.Children.Add(this);
            }
        }
    }
}