using System;
using System.Reflection;

namespace Basic
{
    public class ImportedMethodParameter : IParameterDefinition
    {
        private readonly ParameterInfo parameter;

        public ImportedMethodParameter(IMethodDefinition method, ParameterInfo parameter)
        {
            Method = method;
            this.parameter = parameter;
        }

        #region IParameterDefinition Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeDefinition ParameterType
        {
            get { throw new NotImplementedException(); }
        }

        public IMethodDefinition Method { get; private set; }

        #endregion

        public ParameterInfo GetParameterInfo()
        {
            return parameter;
        }
    }
}