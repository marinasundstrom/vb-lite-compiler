using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Basic.Ast;

namespace Basic
{
    /// <summary>
    /// Represents the module of a assembly.
    /// </summary>
    public class AssemblyModule
    {
        public AssemblyModule()
        {
            RootNamespace = new GlobalRootNamespace();
            RootNamespaceEntry = new NamespaceEntry(RootNamespace);
        }

        /// <summary>
        /// Gets the root namespace.
        /// </summary>
        public GlobalRootNamespace RootNamespace { get; private set; }

        /// <summary>
        /// Gets the root namespace entry.
        /// </summary>
        public NamespaceEntry RootNamespaceEntry { get; private set; }

        /// <summary>
        /// Gets the ModuleBuilder.
        /// </summary>
        public ModuleBuilder ModuleBuilder { get; private set; }

        /// <summary>
        /// Gets the AssemblyBuilder.
        /// </summary>
        public AssemblyBuilder AssemblyBuilder { get; private set; }

        /// <summary>
        /// Initializes the module before build.
        /// </summary>
        /// <param name="assembly"></param>
        public void InitializeForBuild(ModuleBuilder module)
        {
            AssemblyBuilder = (AssemblyBuilder) module.Assembly;
            ModuleBuilder = module;
        }

        /// <summary>
        /// Gets all defined types.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TypeDef> GetTypes()
        {
            return getTypes(RootNamespace);
        }

        /// <summary>
        /// Gets a specific type by name. Either simple name or fullname.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TypeDef GetType(string name)
        {
            return GetTypes().First(x => x.Name == name || x.FullName == name);
        }

        public void DefineTypes()
        {
            RootNamespace.DefineTypes();
        }

        public void DefineTypeBuilders()
        {
            RootNamespace.DefineTypeBuilders();
        }

        public void DefineMemberBuilders()
        {
            RootNamespace.DefineMemberBuilders();
        }

        public void Resolve()
        {
            RootNamespace.Resolve();
        }

        public void ResolveTypes()
        {
            RootNamespace.ResolveTypes();
        }

        public void ResolveMembers()
        {
            RootNamespace.ResolveMembers();
        }

        //public void ResolveNestedTypes()
        //{
        //    this.RootNamespace.ResolveNestedTypes();
        //}

        private IEnumerable<TypeDef> getTypes(Namespace ns)
        {
            return ns.Types.Union(ns.ChildNamespaces.SelectMany(x => getTypes(x)));
        }

        public void CloseBuilders()
        {
            RootNamespace.CloseBuilders();
        }

        public void DefineMembers()
        {
            RootNamespace.DefineMembers();
        }
    }
}