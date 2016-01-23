using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class MethodDef : MemberDef, IMethodDefinition
    {
        public ILGenerator Generator;

        public bool InnerReturnJump;

        protected MethodResolveContext ResolveContext;

        public MethodDef(ClassStructOrModuleDef parent, string name, FullNamedExpression returnType, ParameterList param,
                         Modifier modifiers)
            : base(parent, new MemberName(parent.MemberName, Separators.DoubleColon, name), modifiers)
        {
            ReturnType = returnType;
            Parameters = param;

            parent.AddMethod(this);
        }

        public FullNamedExpression ReturnType { get; private set; }

        public ParameterList Parameters { get; private set; }

        public Block Body { get; private set; }

        public virtual bool IsEventHandler
        {
            get { return false; }
        }

        public bool IsTypeInitializer { get; protected set; }

        public bool IsConstructor { get; protected set; }

        public bool IsEntryPoint
        {
            get { return RootContext.EntryPoint == this; }
        }

        #region IMethodDefinition Members

        public IParameterDefinition[] GetParameters()
        {
            return Parameters.ToArray();
        }

        MethodInfo IMethodDefinition.GetMethodInfo()
        {
            return MethodBuilder;
        }

        #endregion

        public void SetBody(Block block)
        {
            Body = block;
        }

        public override void Define()
        {
            if (Parent != null)
            {
                if (Parent is ModuleDef)
                {
                    Modifiers = Modifiers | Modifier.Shared;
                }
            }

            //Check if it is a Main-method.
            if (Name == "Main" && IsShared)
            {
                RootContext.SetEntryPoint(this);
            }

            Parameters.Define();

            base.Define();
        }

        public void DefineParameterBuilders()
        {
            foreach (Parameter p in Parameters)
            {
                p.DefineParameterBuilder();
            }
        }

        private Type GetReturnType()
        {
            return ResolveContext.ReturnType.GetTypeInfo();
        }

        protected Type[] GetArgumentTypes()
        {
            if (Parameters.Count() == 0) return null;

            return Parameters.Select(x => ((IParameterDefinition) x).ParameterType.GetTypeInfo()).ToArray();
        }

        public virtual MethodBuilder DefineMethodBuilder()
        {
            MethodBuilder = Parent.TypeBuilder.DefineMethod(
                Name,
                GetMethodModifiers(),
                CallingConventions.Standard,
                GetReturnType(),
                GetArgumentTypes());

            return MethodBuilder;
        }

        protected MethodAttributes GetMethodModifiers()
        {
            MethodAttributes attr = MethodAttributes.Private;

            if (IsEntryPoint)
            {
                attr = attr | MethodAttributes.HideBySig;
            }

            if (IsPublic)
            {
                attr = attr ^ MethodAttributes.Private;
                attr = attr | MethodAttributes.Public;
            }

            if (IsShared)
            {
                attr = attr | MethodAttributes.Static;
            }

            if (IsAssembly)
            {
                attr = attr | MethodAttributes.Assembly;
            }

            return attr;
        }

        public override void Resolve()
        {
            ResolveParameters();

            ResolveContext.ReturnType = TypeManager.ResolveType(ReturnType);

            ResolveContext.ParameterTypes = new List<ITypeDefinition>();

            foreach (Parameter pt in Parameters)
            {
                var pt2 = (IParameterDefinition) pt;
                ResolveContext.ParameterTypes.Add(pt2.ParameterType);
            }

            Body.CheckAndResolve();

            base.Resolve();
        }

        public void ResolveParameters()
        {
            Parameters.Resolve();
        }

        public override void Emit()
        {
            emit();
        }

        private void emit()
        {
            Generator = MethodBuilder.GetILGenerator();

            Body.Emit();

            if (Body.Statements.Count() == 0)
            {
                Generator.Emit(OpCodes.Nop);
            }

            Generator.Emit(OpCodes.Ret);
        }

        public IMethodDefinition ResolveMethodName(FullNamedExpression fullNamedExpression,
                                                   params ITypeDefinition[] parameterTypes)
        {
            if (fullNamedExpression.NumberOfParts == 1)
            {
                string val = fullNamedExpression.GetName(false);
                //var t = TypeManager.ResolveType(fullNamedExpression);
                IMethodDefinition[] methods = Parent.GetMethods();

                IEnumerable<IMethodDefinition> r = methods.Where(x => x.Name == val);

                if (r.Count() > 1)
                {
                    //Ambiguous 

                    return null;
                }

                //r = r.Where(x => x.GetParameters()

                //if(r.Count() == 0)
                //{

                //}

                return r.ElementAt(0);
            }

            return null;
        }

        #region Nested type: MethodResolveContext

        public struct MethodResolveContext
        {
            public List<ITypeDefinition> ParameterTypes;
            public ITypeDefinition ReturnType;
        }

        #endregion
    }
}