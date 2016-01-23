using System.Collections.Generic;

namespace Basic.Ast
{
    public class Namespace
    {
        protected List<Namespace> childNamespaces;

        protected List<TypeDef> types;

        public Namespace(Namespace parent, MemberName name)
        {
            Parent = parent;
            MemberName = name;
            childNamespaces = new List<Namespace>();
            types = new List<TypeDef>();
        }

        public MemberName MemberName { get; private set; }

        public string Name
        {
            get { return MemberName.Name; }
        }

        public Namespace Parent { get; private set; }

        public IEnumerable<Namespace> ChildNamespaces
        {
            get { return childNamespaces; }
        }

        public IEnumerable<TypeDef> Types
        {
            get { return types; }
        }

        public void AddNamespace(Namespace ns)
        {
            childNamespaces.Add(ns);
        }

        public void AddType(TypeDef type)
        {
            types.Add(type);
        }

        public void DefineTypes()
        {
            foreach (TypeDef type in types)
            {
                type.Define();
            }

            foreach (Namespace ns in childNamespaces)
            {
                ns.DefineTypes();
            }
        }

        public void DefineMembers()
        {
            foreach (TypeDef type in types)
            {
                type.DefineMembers();
            }

            foreach (Namespace ns in childNamespaces)
            {
                ns.DefineMembers();
            }
        }

        public void DefineTypeBuilders()
        {
            foreach (TypeDef type in types)
            {
                type.DefineTypeBuilder();
            }

            foreach (Namespace ns in childNamespaces)
            {
                ns.DefineTypeBuilders();
            }
        }

        public void DefineMemberBuilders()
        {
            foreach (TypeDef type in types)
            {
                type.DefineMemberBuilders();
            }

            foreach (Namespace ns in childNamespaces)
            {
                ns.DefineMemberBuilders();
            }
        }

        public void CloseBuilders()
        {
            foreach (TypeDef type in types)
            {
                type.CloseBuilder();
            }

            foreach (Namespace ns in childNamespaces)
            {
                ns.CloseBuilders();
            }
        }

        public void Resolve()
        {
            ResolveTypes();
            ResolveMembers();
        }

        public void ResolveTypes()
        {
            foreach (TypeDef type in types)
            {
                type.Resolve();
            }

            foreach (Namespace nt in childNamespaces)
            {
                nt.ResolveTypes();
            }
        }

        public void ResolveMembers()
        {
            foreach (TypeDef type in types)
            {
                type.ResolveMembers();
            }

            foreach (Namespace nt in childNamespaces)
            {
                nt.ResolveMembers();
            }
        }
    }
}