using System.Collections.Generic;
using System.Reflection;

namespace Basic
{
    public class ImportedMethod : ImportedMember, IMethodDefinition
    {
        private List<IParameterDefinition> parameters;

        protected ImportedMethod(MethodInfo method)
            : base(method, null)
        {
        }

        public ImportedMethod(MethodInfo method, ImportedType parent)
            : base(method, parent)
        {
        }

        #region IMethodDefinition Members

        public MethodInfo GetMethodInfo()
        {
            return (MethodInfo) member;
        }

        public IParameterDefinition[] GetParameters()
        {
            if (parameters == null)
            {
                parameters = new List<IParameterDefinition>();

                foreach (ParameterInfo param in ((MethodInfo) member).GetParameters())
                {
                    IParameterDefinition p = new ImportedMethodParameter(this, param);
                    parameters.Add(p);
                }
            }

            return parameters.ToArray();
        }

        public override bool IsShared
        {
            get { return GetMethodInfo().IsStatic; }
        }

        public override bool IsPrivate
        {
            get { return GetMethodInfo().IsPrivate; }
        }

        public override bool IsPublic
        {
            get { return GetMethodInfo().IsPublic; }
        }

        public override bool IsFamily
        {
            get { return GetMethodInfo().IsFamily; }
        }

        public override bool IsAssembly
        {
            get { return GetMethodInfo().IsAssembly; }
        }

        #endregion
    }
}