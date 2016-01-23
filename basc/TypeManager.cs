using System;
using System.Collections.Generic;
using System.Reflection;
using Basic.Ast;

namespace Basic
{
    internal class TypeManager
    {
        private static readonly List<ITypeDefinition> types;

        private static readonly List<Assembly> assemblies;

        static TypeManager()
        {
            types = new List<ITypeDefinition>();
            assemblies = new List<Assembly>();

            RuntimeLoader.Load();
        }

        public static void AddAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
        }

        public static bool IsAssembly(Assembly assembly)
        {
            return assemblies.Contains(assembly);
        }

        public static void AddUserDefinedType(TypeDef typeDefinition)
        {
            types.Add(typeDefinition);
        }

        public static ITypeDefinition GetType(string name)
        {
            var defs = new List<ITypeDefinition>();

            ITypeDefinition def = types.Find(x => x.FullName == name);

            if (def != null) defs.Add(def);

            foreach (Assembly a in assemblies)
            {
                try
                {
                    Type t = a.GetType(name);

                    if (t != null)
                    {
                        def = new ImportedType(t);
                        defs.Add(def);
                    }
                }
                catch (Exception)
                {
                }
            }

            if (defs.Count > 1)
                throw new Exception("Ambiguous types.");

            return def;
        }

        public static ITypeDefinition ResolveType(string name)
        {
            if (string.Compare(name, "void", true) == 0)
            {
                return InternalTypeAliases.Void;
            }

            if (string.Compare(name, "integer", true) == 0)
            {
                return InternalTypeAliases.Integer;
            }

            return GetType(name);
        }

        public static ITypeDefinition ResolveType(Namespace ns, string typeName)
        {
            return ResolveType(ns.Name + '.' + typeName);
        }
    }
}