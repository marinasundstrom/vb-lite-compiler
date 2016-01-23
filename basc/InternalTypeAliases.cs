using System;
using System.Reflection;

namespace Basic
{
    internal static class InternalTypeAliases
    {
        private static ITypeDefinition _integer;
        private static ITypeDefinition _void;
        private static ITypeDefinition _boolean;
        private static ITypeDefinition _string;

        public static ITypeDefinition Integer
        {
            get
            {
                if (_integer == null)
                {
                    _integer = new ImportedType(typeof (int));
                }

                return _integer;
            }
        }

        public static ITypeDefinition Boolean
        {
            get
            {
                if (_boolean == null)
                {
                    _boolean = new ImportedType(typeof(bool));
                }

                return _boolean;
            }
        }

        public static ITypeDefinition String
        {
            get
            {
                if (_string == null)
                {
                    _string = new ImportedType(typeof(string));
                }

                return _string;
            }
        }

        public static ITypeDefinition Void
        {
            get
            {
                if (_void == null)
                {
                    _void = new ImportedType(typeof (void));
                }

                return _void;
            }
        }

        public static ITypeDefinition GetTypeByAlias(string name, bool ignoreCase = false)
        {
            foreach (var propertyInfo in typeof(InternalTypeAliases).GetProperties())
            {
                if (string.Compare(propertyInfo.Name, name, ignoreCase) == 0)
                {
                    return (ITypeDefinition)propertyInfo.GetValue(null, null);
                }
            }

            return null;
        }
    }
}