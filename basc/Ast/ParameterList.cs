using System.Collections;
using System.Collections.Generic;

namespace Basic.Ast
{
    public class ParameterList : IEnumerable<Parameter>
    {
        private readonly List<Parameter> parameters;

        public ParameterList()
        {
            parameters = new List<Parameter>();
        }

        public MethodDef Method { get; private set; }

        #region IEnumerable<Parameter> Members

        public IEnumerator<Parameter> GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return parameters.GetEnumerator();
        }

        #endregion

        public void SetMethod(MethodDef method)
        {
            Method = method;
        }

        public void Add(Parameter param)
        {
            parameters.Add(param);
        }

        public void Define()
        {
            foreach (Parameter p in parameters)
            {
                p.Define();
            }
        }

        public void Resolve()
        {
            foreach (Parameter p in parameters)
            {
                p.Resolve();
            }
        }

        public void CreateParameterBuilders()
        {
            foreach (Parameter p in parameters)
            {
                p.DefineParameterBuilder();
            }
        }

        public Parameter Get(string name)
        {
            return parameters.Find(x => x.Name == name);
        }
    }
}