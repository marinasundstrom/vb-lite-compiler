using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class Parameter : IParameterDefinition, IVariable, IIndexableVariable
    {
        private ParameterBuilder ParameterBuilder;

        private ParameterResolveContext resolveContext;

        public Parameter(ParameterList list, string name, FullNamedExpression type, Modifier modifiers)
        {
            ParameterList = list;

            Name = name;
            Type = type;
            Modifiers = modifiers;

            ParameterIndex = list.Count();
        }

        public int ParameterIndex { get; protected set; }

        public Modifier Modifiers { get; private set; }

        public ParameterList ParameterList { get; private set; }

        public MethodDef Method
        {
            get { return ParameterList.Method; }
        }

        #region IIndexableVariable Members

        void IIndexableVariable.EmitLoad(Block block, Expression[] expr)
        {
            if (resolveContext.ParameterType.IsArrayType)
            {
                if (resolveContext.ParameterType.Rank > 1)
                {
                    block.Method.Generator.Emit(OpCodes.Ldarg, ParameterIndex);

                    foreach (Expression e in expr)
                    {
                        e.Emit();
                    }

                    block.Method.Generator.Emit(OpCodes.Ldelem);
                }
                else
                {
                    //Exception
                }
            }
            else
            {
                //Find indexer
            }
        }

        void IIndexableVariable.EmitStore(Block block, Expression[] expr)
        {
            if (resolveContext.ParameterType.IsArrayType)
            {
                if (resolveContext.ParameterType.Rank > 1)
                {
                    block.Method.Generator.Emit(OpCodes.Ldarg, ParameterIndex);

                    foreach (Expression e in expr)
                    {
                        e.Emit();
                    }

                    block.Method.Generator.Emit(OpCodes.Stelem);
                }
                else
                {
                }
            }
            else
            {
                //Find indexer
            }
        }

        void IIndexableVariable.EmitLoad(Block block, Expression expr)
        {
            if (resolveContext.ParameterType.IsArrayType)
            {
                block.Method.Generator.Emit(OpCodes.Ldarg, ParameterIndex);

                expr.Emit();

                block.Method.Generator.Emit(OpCodes.Ldelem);
            }
        }

        void IIndexableVariable.EmitStore(Block block, Expression expr)
        {
            if (resolveContext.ParameterType.IsArrayType)
            {
                block.Method.Generator.Emit(OpCodes.Ldarg, ParameterIndex);

                expr.Emit();

                block.Method.Generator.Emit(OpCodes.Stelem);
            }
        }

        #endregion

        #region IParameterDefinition Members

        public string Name { get; private set; }

        ITypeDefinition IParameterDefinition.ParameterType
        {
            get { return resolveContext.ParameterType; }
        }

        IMethodDefinition IParameterDefinition.Method
        {
            get { return Method; }
        }

        #endregion

        #region IVariable Members

        public FullNamedExpression Type { get; private set; }

        void IVariable.EmitLoad(Block block)
        {
            Method.Generator.Emit(OpCodes.Ldarg, (short) ParameterIndex);
        }

        void IVariable.EmitStore(Block block)
        {
            Method.Generator.Emit(OpCodes.Starg, (short) ParameterIndex);
        }

        bool IVariable.IsIndexable
        {
            get { return resolveContext.ParameterType.IsIndexable; }
        }

        #endregion

        public ParameterBuilder GetParameterBuilder()
        {
            return ParameterBuilder; //this.parameterBuilder;
        }

        public void Define()
        {
            //if (!Method.IsStatic)
            //{
            //    ParameterIndex += 1;
            //}
        }

        public void Resolve()
        {
            resolveContext = new ParameterResolveContext();

            resolveContext.ParameterType = TypeManager.ResolveType(Type);
        }

        public void DefineParameterBuilder()
        {
            if (!Method.IsShared) ParameterIndex++;

            ParameterBuilder = Method.MethodBuilder.DefineParameter(ParameterIndex + 1, ParameterAttributes.None, Name);
        }

        #region Nested type: ParameterResolveContext

        private class ParameterResolveContext
        {
            public ITypeDefinition ParameterType;
            public IPropertyDefinition Property;
        }

        #endregion
    }
}