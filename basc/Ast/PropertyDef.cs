using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class PropertyDef : MemberDef, IPropertyDefinition
    {
        private readonly ParameterList parameters;

        private PropertyResolveContext resolveContext;

        protected PropertyDef(ClassStructOrModuleDef parent, string name, FullNamedExpression propertyType,
                              ParameterList parameters, bool isIndexer, Modifier modifiers)
            : base(parent, name, modifiers)
        {
            PropertyType = propertyType;
            IsIndexedProperty = isIndexer;

            this.parameters = parameters;
        }

        #region IPropertyDefinition Members

        public FullNamedExpression PropertyType { get; private set; }

        public bool IsAssignable
        {
            get { return Assigner != null; }
        }

        public bool IsIndexedProperty { get; private set; }

        public IParameterDefinition[] GetParameters()
        {
            if (IsAssignable && Assigner != null)
                return Assigner.GetParameters();

            return null;
        }

        public IMethodDefinition Accessor { get; private set; }

        public IMethodDefinition Assigner { get; private set; }

        public PropertyInfo GetPropertyInfo()
        {
            return GetMemberInfo() as PropertyBuilder;
        }

        FullNamedExpression IVariable.Type
        {
            get { return PropertyType; }
        }

        bool IVariable.IsIndexable
        {
            get { return IsIndexedProperty; }
        }

        void IVariable.EmitLoad(Block block)
        {
            block.Method.Generator.Emit(getOpCode(), Accessor.GetMethodInfo());
        }

        void IVariable.EmitStore(Block block)
        {
            block.Method.Generator.Emit(getOpCode(), Assigner.GetMethodInfo());
        }

        void IIndexableVariable.EmitLoad(Block block, Expression[] expr)
        {
            foreach (Expression e in expr)
            {
                e.Emit();
            }

            block.Method.Generator.Emit(getOpCode(), Accessor.GetMethodInfo());
        }

        void IIndexableVariable.EmitStore(Block block, Expression[] expr)
        {
            foreach (Expression e in expr)
            {
                e.Emit();
            }

            block.Method.Generator.Emit(getOpCode(), Assigner.GetMethodInfo());
        }

        void IIndexableVariable.EmitLoad(Block block, Expression expr)
        {
            expr.Emit();
            block.Method.Generator.Emit(getOpCode(), Accessor.GetMethodInfo());
        }

        void IIndexableVariable.EmitStore(Block block, Expression expr)
        {
            expr.Emit();
            block.Method.Generator.Emit(getOpCode(), Assigner.GetMethodInfo());
        }

        #endregion

        public static PropertyDef CreateProperty(ClassStructOrModuleDef parent, string name,
                                                 FullNamedExpression propertyType, Parameter parameter,
                                                 Modifier modifiers)
        {
            var plist = new ParameterList();
            plist.Add(parameter);

            return new PropertyDef(parent, name, propertyType, plist, false, modifiers);
        }

        public static PropertyDef CreateIndexedProperty(ClassStructOrModuleDef parent, string name,
                                                        FullNamedExpression propertyType, ParameterList parameters,
                                                        Modifier modifiers)
        {
            return new PropertyDef(parent, name, propertyType, parameters, true, modifiers);
        }

        public IMethodDefinition CreateAndGetAssigner(Modifier modifiers)
        {
            if (Assigner == null)
            {
                Assigner = new MethodDef((ClassStructOrModuleDef) Parent, "set_" + Name,
                                         new SimpleName(null, "Void", null), parameters, modifiers);
                ((ClassStructOrModuleDef) Parent).AddMethod((MethodDef) Assigner);
            }

            return Assigner;
        }

        public IMethodDefinition CreateAndGetAccessor(Modifier modifiers)
        {
            if (Accessor == null)
            {
                Accessor = new MethodDef((ClassStructOrModuleDef) Parent, "get_" + Name, PropertyType, parameters,
                                         modifiers);
                ((ClassStructOrModuleDef) Parent).AddMethod((MethodDef) Accessor);
            }

            return Accessor;
        }

        public override void Resolve()
        {
            resolveContext = new PropertyResolveContext();

            resolveContext.PropertyType = TypeManager.ResolveType(PropertyType);
        }

        private OpCode getOpCode()
        {
            return OpCodes.Callvirt;
        }

        #region Nested type: PropertyResolveContext

        private class PropertyResolveContext
        {
            public ITypeDefinition PropertyType;
        }

        #endregion
    }
}