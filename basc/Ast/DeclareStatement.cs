using System.Reflection.Emit;
using System.Linq;

namespace Basic.Ast
{
    public class DeclareStatement : Statement
    {
        private DeclareStatementResolveContext resolveContext;

        public DeclareStatement(Block block)
            : base(block)
        {
        }

        public SimpleName Name { get; private set; }
        public FullNamedExpression Type { get; private set; }
        public Expression InitializationExpression { get; private set; }

        public override void CheckAndResolve()
        {
            resolveContext = new DeclareStatementResolveContext();

            LocalScope localScope = Block.LocalScope;

            if (localScope.LocalExists(Name.Value))
            {
                Report.Instance.AddItem(VBErrors.LocalVariable_variablename_IsAlreadyDeclaredInTheCurrentBlock,
                                        Name.SourceData.SourceFile, Name.SourceData.SourceSpan, Name.SourceData.Tokens,
                                        Name);
            }
            else
            {
                if (Type != null)
                {
                    var l = this.Method.Parent.ResolveType(Type);

                    if(l.Count() > 0)
                    {
                        //Ambiguos types
                    }

                    //ITypeDefinition type = TypeManager.ResolveType(Type);

                    ITypeDefinition type = l.ElementAt(0);

                    if (type == null)
                    {
                        // Error: Type was not found.

                        //Report.Instance.AddMessage("*", "Type was nor", this.Block.Method.FileNamespaceEntry.File.File, SourceData.Tokens);
                    }
                    else
                    {
                        resolveContext.Type = type;
                    }
                }

                resolveContext.Local = localScope.DeclareLocal(Name, Type);

                if (InitializationExpression != null && Type != null)
                {
                    //Analyze initialization expression.
                    InitializationExpression.CheckAndResolve();

                    //ITypeDefinition assignType = BasicParser.SemanticAnalyzer.AnalyzeExpression(this.Block.Method, this.InitializationExpression, true);

                    //BasicParser.SemanticAnalyzer.AnalyzeAssignment(resolveContext.Type, assignType);
                    //this.resolveContext.Type
                }
            }
        }

        public override void Emit()
        {
            ILGenerator generator = Block.Method.Generator;

            resolveContext.Local.DeclareLocalBuilder();

            if (InitializationExpression != null)
            {
                if (InitializationExpression.resolveContext.ResultType == Expression.EvaluationResult.Constant)
                {
                    generator.Emit(OpCodes.Ldc_I4, (int) InitializationExpression.resolveContext.ResultValue);
                }
                else
                {
                    InitializationExpression.Emit();
                }

                resolveContext.Local.EmitStore(Block);
            }
        }

        public void SetInitializationExpression(Expression initializationExpression)
        {
            InitializationExpression = initializationExpression;
        }

        public void SetName(SimpleName name)
        {
            Name = name;
        }

        public void SetType(FullNamedExpression type)
        {
            Type = type;
        }

        #region Nested type: DeclareStatementResolveContext

        private class DeclareStatementResolveContext
        {
            public Local Local;
            public ITypeDefinition Type;
            public bool VariableExists;
        }

        #endregion
    }
}