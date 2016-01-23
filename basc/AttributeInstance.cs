using System.Collections.Generic;
using Basic.Ast;

namespace Basic
{
    public class AttributeInstance
    {
        private readonly object obj;

        public AttributeInstance(ITypeDefinition type)
        {
            Parameters = new Dictionary<string, Expression>();
        }

        public AttributeInstance(object obj)
        {
            this.obj = obj;
            AttributeType = TypeManager.ResolveType(obj.GetType().FullName);
        }

        public ITypeDefinition AttributeType { get; private set; }

        public bool IsCompiled
        {
            get { return obj != null; }
        }

        public Dictionary<string, Expression> Parameters { get; private set; }

        public object GetObject()
        {
            return obj;
        }
    }
}