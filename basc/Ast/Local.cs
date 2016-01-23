using System;
using System.Linq;
using System.Reflection.Emit;

namespace Basic.Ast
{
    public class Local : IVariable, IIndexableVariable
    {
        public Local(Block block, int index, string name)
        {
            Block = block;
            LocalIndex = index;
            Name = name;
        }

        public Local(Block block, int index, string name, FullNamedExpression type)
            : this(block, index, name)
        {
            Type = type;
        }


        public int LocalIndex { get; private set; }

        public Block Block { get; private set; }

        public bool IsImplicitlyTyped
        {
            get { return Type == null; }
        }

        public bool IsInitialized { get; private set; }

        public bool IsParameter { get; private set; }

        #region IIndexableVariable Members

        public void EmitLoad(Block block, Expression[] expr)
        {
            int index = 0;
            EmitLoad(block);
            block.Method.Generator.Emit(OpCodes.Ldelem, index);
        }

        public void EmitStore(Block block, Expression[] expr)
        {
            int index = 0;
            EmitLoad(block);
            block.Method.Generator.Emit(OpCodes.Stelem, index);
        }


        public void EmitLoad(Block block, Expression expr)
        {
            throw new NotImplementedException();
        }

        public void EmitStore(Block block, Expression expr)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVariable Members

        public string Name { get; private set; }

        public FullNamedExpression Type { get; private set; }

        public void EmitLoad(Block block)
        {
            block.Method.Generator.Emit(OpCodes.Ldloc, LocalIndex);
        }

        public void EmitStore(Block block)
        {
            block.Method.Generator.Emit(OpCodes.Stloc, LocalIndex);
        }

        public bool IsIndexable
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public void SetInitialized(bool value)
        {
            IsInitialized = value;
        }

        public void SetIsParameter(bool value)
        {
            IsParameter = value;
        }

        public void SetType(FullNamedExpression value)
        {
            Type = value;
        }

        /// <summary>
        /// Called after TypeBuilders are created.
        /// </summary>
        public void DeclareLocalBuilder()
        {

            var t = this.Block.Method.Parent.ResolveType(this.Type);

            LocalBuilder local = Block.Method.Generator.DeclareLocal(t.ElementAt(0).GetTypeInfo());
            local.SetLocalSymInfo(Name);
        }
    }
}