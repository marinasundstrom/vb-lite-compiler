namespace Basic.Ast
{
    public class VariableAccess : Expression
    {
        private VariableAccessResolveContext ResolveContext;

        public VariableAccess(Statement statement, FullNamedExpression member, SourceData sourceData)
            : base(statement, sourceData)
        {
            Member = member;
        }

        public FullNamedExpression Member { get; private set; }

        public override void CheckAndResolve()
        {
            ResolveContext = new VariableAccessResolveContext();

            ResolveContext.Member = Block.ResolveVariableName(Member);

            //Determine the type: 1. local? 2. field? 3. class or other member

            //Is it even valid? Can you access the members.

            if (Method.IsShared && ResolveContext.Member is FieldDef && !(ResolveContext.Member as FieldDef).IsShared)
            {
                Report.Instance.AddItem(
                    VBErrors.
                        CannotReferToAnInstanceMemberOfAClassFromWithiASharedMethodOrSharedMemberInitializerWithoutAnExplicitInstanceOfTheClass,
                    Method.FileNamespaceEntry.File.File, Member.SourceData.SourceSpan, Member.SourceData.Tokens);
            }
        }

        public override void Emit()
        {
            //if(
            // If name is dotted then it is declare outside method as an object.
            //else
            {
                ResolveContext.Member.EmitLoad(Block);

                //Local e = Statement.Block.LocalScope.Get(Member);

                //if (e != null)
                //{


                //    if (e.IsParameter)
                //    {
                //        Parameter p = Statement.Block.Method.Parameters.Get(e.Name);
                //        Statement.Block.Method.Generator.Emit(OpCodes.Ldarg, e.LocalIndex);
                //    }
                //    else
                //    {
                //        Statement.Block.Method.Generator.Emit(OpCodes.Ldloc,
                //                                              e.LocalIndex - Statement.Block.Method.Parameters.Count());
                //    }
                //}
                //else
                //{
                //    //Report.Instance.
                //}
            }


            base.Emit();
        }

        #region Nested type: VariableAccessResolveContext

        private class VariableAccessResolveContext
        {
            public IVariable Member;
        }

        #endregion
    }
}