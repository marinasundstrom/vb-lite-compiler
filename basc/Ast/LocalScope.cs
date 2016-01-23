using System.Collections.Generic;
using System.Linq;

namespace Basic.Ast
{
    public class LocalScope
    {
        public LocalScope(Block block, LocalScope parent)
        {
            Block = block;

            Parent = parent;

            Parameters = new List<Parameter>();
            Locals = new List<Local>();

            if (parent == null)
            {
                if (Method.Parameters.Count() != 0)
                {
                    Parameters.AddRange(Method.Parameters);
                }
            }
        }

        public MethodDef Method
        {
            get { return Block.Method; }
        }

        public Block Block { get; private set; }

        public LocalScope Parent { get; private set; }

        public List<Parameter> Parameters { get; private set; }

        public List<Local> Locals { get; private set; }

        public Local DeclareLocal(string name, FullNamedExpression type)
        {
            var entry = new Local(Block, Locals.Count, name, type);
            Locals.Add(entry);

            return entry;
        }

        public Local DeclareLocal(string name)
        {
            var entry = new Local(Block, Locals.Count, name);
            Locals.Add(entry);

            return entry;
        }

        public bool LocalExists(string name)
        {
            bool k = Parameters.Any(x => x.Name == name);

            if (k) return true;

            k = Locals.Any(x => x.Name == name);

            if (k) return true;

            if (Parent != null)
            {
                k = Parent.LocalExists(name);
            }

            return k;
        }

        public IVariable GetVariable(string name)
        {
            IVariable v = Parameters.Find(x => x.Name == name);

            if (v != null) return v;

            v = Locals.Find(x => x.Name == name);

            if (Parent != null)
            {
                v = Parent.GetVariable(name);
            }

            return v;
        }
    }
}