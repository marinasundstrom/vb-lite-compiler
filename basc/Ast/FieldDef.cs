using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class FieldDef : MemberDef, IVariable, IIndexableVariable
    {
        private FieldResolveContext resolveContext;

        public FieldDef(TypeDef parent, string name, FullNamedExpression type, Modifier modifiers)
            : base(parent, new MemberName(parent.MemberName, Separators.DoubleColon, name), modifiers)
        {
            FieldType = type;
        }

        protected FieldDef(Namespace ns, string name, Modifier modifiers)
            : base(
                null,
                (ns is GlobalRootNamespace
                     ? new MemberName(name)
                     : new MemberName(ns.MemberName, Separators.DoubleColon, name)), modifiers)
        {
        }

        public FullNamedExpression FieldType { get; private set; }

        public Expression InitializationExpression { get; private set; }

        public bool IsConstant
        {
            get { return (Modifiers & Modifier.Const) == Modifier.Const; }
        }

        #region IIndexableVariable Members

        void IIndexableVariable.EmitLoad(Block block, Expression[] expr)
        {
            throw new NotSupportedException();
        }

        void IIndexableVariable.EmitStore(Block block, Expression[] expr)
        {
            throw new NotSupportedException();
        }

        void IIndexableVariable.EmitLoad(Block block, Expression expr)
        {
            throw new NotSupportedException();
        }

        void IIndexableVariable.EmitStore(Block block, Expression expr)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IVariable Members

        public FullNamedExpression Type
        {
            get { throw new NotImplementedException(); }
        }

        public void EmitLoad(Block block)
        {
            if (IsShared)
            {
                block.Method.Generator.Emit(OpCodes.Ldsfld, FieldBuilder);
            }
            else
            {
                block.Method.Generator.Emit(OpCodes.Ldfld, FieldBuilder);
            }
        }

        public void EmitStore(Block block)
        {
            if (IsShared)
            {
                block.Method.Generator.Emit(OpCodes.Stsfld, FieldBuilder);
            }
            else
            {
                block.Method.Generator.Emit(OpCodes.Stfld, FieldBuilder);
            }
        }

        public bool IsIndexable
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public FieldBuilder DefineFieldBuilder()
        {
            Emit();
            return FieldBuilder;
        }

        public void SetInitalizationExpression(Expression expr)
        {
            InitializationExpression = expr;
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

            base.Define();
        }

        public override void Resolve()
        {
            resolveContext = new FieldResolveContext();
            resolveContext.Type = TypeManager.ResolveType(FieldType);

            base.Resolve();
        }

        private new void Emit()
        {
            FieldBuilder = Parent.TypeBuilder.DefineField(Name, resolveContext.Type.GetTypeInfo(), getFieldModifiers());

            if (IsConstant)
            {
                if (InitializationExpression == null)
                {
                    //Throw error
                }
                else
                {
                    object value = evaluateConstantExpression(InitializationExpression);
                    FieldBuilder.SetConstant(value);
                }
            }
            else
            {
                if (InitializationExpression != null)
                {
                    //Emitted automatically
                }
            }
        }

        private object evaluateConstantExpression(Expression expression)
        {
            if (expression is IntegerConstant)
            {
                return ((IntegerConstant) expression).nr;
            }
            else
            {
                throw new Exception();
                //Throw error
            }
        }

        private FieldAttributes getFieldModifiers()
        {
            FieldAttributes attr = FieldAttributes.Private;

            if (IsPublic)
            {
                attr = attr ^ FieldAttributes.Private;
                attr = attr | FieldAttributes.Public;
            }
            else
            {
                if (IsAssembly)
                {
                    attr = attr ^ FieldAttributes.Private;
                    attr = attr | FieldAttributes.Assembly;
                }
            }

            if (IsConstant)
            {
                attr = attr | FieldAttributes.Static | FieldAttributes.Literal;
            }
            else
            {
                if (IsShared)
                {
                    attr = attr | FieldAttributes.Static;
                }
            }

            return attr;
        }


        public void EmitLoad(Block block, Expression[] expr)
        {
            EmitLoad(block);

            foreach (Expression e in expr)
            {
                e.Emit();
            }
            block.Method.Generator.Emit(OpCodes.Ldelem);
            //Multi-dimensional arrays?
        }

        public void EmitStore(Block block, Expression[] expr, Expression assignExpr)
        {
            EmitLoad(block);

            foreach (Expression e in expr)
            {
                e.Emit();
            }
            assignExpr.Emit();
            block.Method.Generator.Emit(OpCodes.Stelem);
        }

        #region Nested type: FieldResolveContext

        private class FieldResolveContext
        {
            public ITypeDefinition Type;
        }

        #endregion
    }
}