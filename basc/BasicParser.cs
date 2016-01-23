using System;
using System.Collections.Generic;
using System.Linq;
using Basic.Ast;

namespace Basic
{
    /// <summary>
    /// Visual Basic parser.
    /// </summary>
    public partial class BasicParser
    {
        //Main
        private readonly CompilerContext context;
        private Block block;
        private FileEntry fileEntry;
        private FileNamespaceEntry fileNamespaceEntry;
        private bool hasParsed;
        private MethodDef method;
        private Stack<Modifier> modifierStack;
        private NamespaceEntry namespaceEntry;
        private Namespace ns;
        private Stack<Operator> operatorStack;
        private Statement statement;
        private TokenList statementSymbols;
        private Queue<TokenInfo> tokenQueue;
        private IMultiFileTokenizer tokenizer;
        private TypeDef type;
        private TypeEntry typeEntry;

        private bool isComparisonExpression;

        /// <summary>
        /// Initializes an instance of the BasicParser class.
        /// </summary>
        /// <param name="context">A CompilerContext.</param>
        public BasicParser(CompilerContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets the CompilerContext.
        /// </summary>
        public CompilerContext Context
        {
            get { return context; }
        }

        /// <summary>
        /// Gets the Report.
        /// </summary>
        public Report Report
        {
            get { return context.Report; }
        }

        /// <summary>
        /// Gets the SourceFile.
        /// </summary>
        public SourceFile SourceFile
        {
            get { return tokenizer.SourceFile; }
        }

        /// <summary>
        /// Gets the Tokenizer.
        /// </summary>
        public ITokenizer Tokenizer
        {
            get { return tokenizer; }
        }

        /// <summary>
        /// Gets a value indicating whether the parser has parsed, or not.
        /// </summary>
        public bool HasParsed
        {
            get { return hasParsed; }
        }

        /// <summary>
        /// Gets the Abstract Syntax Tree (AST)
        /// </summary>
        public GlobalRootNamespace GlobalNamespace { get; private set; }

        /// <summary>
        /// Get the structure of the compilation in a file-oriented hierarchy.
        /// </summary>
        public NamespaceEntry NamespaceEntryRoot { get; private set; }


        private void t_FileChanged(object sender, TokenizerEventArgs e)
        {
            changeFile();
        }

        private void changeFile()
        {
            fileEntry = new FileEntry(SourceFile);
            fileNamespaceEntry = new FileNamespaceEntry(NamespaceEntryRoot, null, string.Empty, fileEntry);
        }

        /// <summary>
        /// Gets the next Token in the stream embedded in a TokenInfo.
        /// </summary>
        /// <returns>A TokenInfo representing a Token.</returns>
        public TokenInfo GetNextToken()
        {
            TokenInfo t = Tokenizer.GetNextToken();

            if (!t.Is(Token.EOL) && !t.Is(Token.EOL))
            {
                statementSymbols.Add(t);
            }

            return t;
        }

        private Block CloseBlock()
        {
            //if (this.block.Parent == null)
            //    throw new InternalCompilerException("Block stack is empty.");

            Block block = this.block;
            this.block = this.block.ContainingBlock;

            return block;
        }

        private TypeDef CloseType()
        {
            this.type.SetFileNamespaceEntry(fileNamespaceEntry);

            if (typeEntry.Parent == null)
            {
                typeEntry = null;
            }
            else
            {
                typeEntry = typeEntry.Parent;
            }

            TypeDef type = this.type;
            this.type = this.type.Parent;

            return type;
        }

        private Namespace CloseNamespace()
        {
            //if (fileNamespaceEntry.Parent == null)
            //    throw new InternalCompilerException("");

            if (namespaceEntry.Parent == null)
            {
                namespaceEntry = null;
            }
            else
            {
                namespaceEntry = namespaceEntry.Parent;
            }

            Namespace ns = this.ns;
            this.ns = this.ns.Parent;

            return ns;
        }

        /// <summary>
        /// Parse the current SourceUnit given when the Parser was initialized.
        /// </summary>
        public void Parse()
        {
            initialize();

            bool invalidInputPaths = checkInputPaths();

            if (invalidInputPaths)
            {
                parse();

                RootContext.Module.Resolve();
            }
            else
            {
                //this.Report.AddMessage("Succ", null, default(SourceLocation), null);
            }
        }

        private void parse()
        {
            tokenizer = new MultiFileTokenizer(context.SourceUnit, context.Report);
            tokenizer.FileChanged += t_FileChanged;

            //Parse files
            ParseGlobalMember();

            RootContext.Module.DefineTypes();
            RootContext.Module.DefineMembers();

            hasParsed = true;
        }

        private void initialize()
        {
            //Stacks
            operatorStack = new Stack<Operator>();
            modifierStack = new Stack<Modifier>();
            tokenQueue = new Queue<TokenInfo>();

            //Tree
            GlobalNamespace = RootContext.Module.RootNamespace;
            NamespaceEntryRoot = RootContext.Module.RootNamespaceEntry;

            //ParserContext
            ns = GlobalNamespace;
            namespaceEntry = NamespaceEntryRoot;

            //Statement symbols
            statementSymbols = new TokenList();
        }

        private void ParseGlobalMember()
        {
            int numberOfDecl;
            bool flag;
            TokenInfo token;

            numberOfDecl = 0;
            flag = true;

            token = GetNextToken();

            while (flag)
            {
                tokenQueue.Enqueue(token);

                switch (token.Token)
                {
                    case Token.Imports:
                        ParseImportDirective();
                        break;

                    case Token.Private:
                    case Token.Public:
                    case Token.Internal:
                    case Token.Shared:
                    case Token.Static:
                    case Token.Const:
                        PrepareModifiers();
                        break;

                    case Token.Module:
                        ParseModule();
                        break;

                    case Token.Namespace:
                        ParseNamespace();
                        break;

                    case Token.SingleQuote:
                        TokenList list;
                        ParseComment(out list);
                        break;

                    case Token.EOL:
                        tokenQueue.Clear();
                        goto end; //break;

                    case Token.EOF:
                        if (numberOfDecl == 0)
                            Report.AddWarning("?", "File is empty.", token.SourceFile, default(SourceSpan), null);
                        flag = false;
                        break;

                    default:
                        if (modifierStack.Count > 0) Unexpected(Report, token);
                        Report.AddError("?", "Namespace, module or type declaration expected.", token.SourceFile,
                                        new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                        break;
                }

                numberOfDecl++;

                end:
                token = GetNextToken();
            }
        }

        private void ParseImportDirective()
        {
            ImportDirective item = ParseImportStmt();

            if (ns.Types.Count() > 0)
            {
            }

            fileNamespaceEntry.Imports.Add(item.Namespace);
        }

        private void Unexpected(Report Report, TokenInfo token)
        {
            throw new NotImplementedException();
        }

        private TokenInfo ParseComment(out TokenList tokens)
        {
            resetStmtTokens();

            tokens = statementSymbols;

            Token token = tokenQueue.Dequeue();

            TokenInfo token2 = EatToEOL();

            resetStmtTokens();

            return token2;
        }


        private TokenInfo ParseComment2()
        {
            TokenInfo token2 = EatToEOL();

            return token2;
        }

        private ImportDirective ParseImportStmt()
        {
            TokenInfo token;
            MemberName name;

            token = tokenQueue.Dequeue();

            Assure_NoModifiers();

            MemberName expr = ParseDottedName();

            if (EatOptional(Token.Equality))
            {
                name = ParseMemberName();
            }

            ExpectEndOfStatementOrEat();

            return new ImportDirective(expr);
        }

        private MemberName ParseDottedName()
        {
            TokenInfo token;
            MemberName nameExpr = null;

            token = Tokenizer.PeekToken();

            while (Helpers.IsIdentifier(token) || Helpers.IsReservedWord(token))
            {
                token = GetNextToken();

                if (nameExpr == null)
                {
                    nameExpr = new MemberName(token.Value);
                }
                else
                {
                    nameExpr = new MemberName(nameExpr, Separators.Dot, token.Value);
                }

                token = Tokenizer.PeekToken();

                if (token.Is(Token.Period))
                {
                    GetNextToken();
                }
                else
                {
                    break;
                }
            }

            return nameExpr;
        }

        private void Assure_NoModifiers()
        {
            if (modifierStack.Count != 0)
            {
                TokenInfo token = tokenQueue.Peek();

                while (modifierStack.Count > 0)
                {
                    token = tokenQueue.Dequeue();
                    modifierStack.Pop();
                }

                Report.AddError("?", "Unexpected modifiers.", Tokenizer.SourceFile,
                                new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
            }
        }

        private void PrepareModifiers()
        {
            Modifier modifier;

            foreach (TokenInfo t in tokenQueue)
            {
                modifier = GetModifier(t);
                modifierStack.Push(modifier);
            }
        }

        private Modifier GetModifier(TokenInfo token)
        {
            return (Modifier) Enum.Parse(typeof (Modifier), token.Value);
        }

        private void ParseNamespace()
        {
            TokenInfo token, token2;

            token = default(TokenInfo);
            token2 = default(TokenInfo);

            bool flag = false;
            Modifier modifiers;

            token = tokenQueue.Dequeue();

            modifiers = ParseModuleModifiers();

            flag = Expect(Token.Identifier, ref token2);

            ns = new RootNamespace(ns, token2.Value);

            OpenNamespace(ns);

            //TokenInfo token = tokenQueue.Dequeue();
            //Report.AddError("?", "Namespaces are not yet supported by the compiler.", Tokenizer.SourceFile,
            //                new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);

            TokenList symbols = statementSymbols;

            ExpectEndOfStatementOrEat();

            flag = ParseNamespaceMembers();

            if (!flag)
            {
                Report.AddItem(VBErrors.ModuleStatementMustEndWithAMatchingEndModule, SourceFile, token.GetSpan(token2),
                               symbols);
            }

            CloseNamespace();
        }

        private bool ParseNamespaceMembers()
        {
            bool flag;
            TokenInfo token;

            flag = true;

            while (flag)
            {
                token = GetNextToken();

                if (token.Is(Token.EOF))
                    return false;

                tokenQueue.Enqueue(token);

                switch (token.Token)
                {
                    case Token.Imports:
                        ParseImportDirective();
                        break;

                    case Token.Private:
                    case Token.Public:
                    case Token.Internal:
                    case Token.Shared:
                    case Token.Static:
                    case Token.Const:
                        PrepareModifiers();
                        break;

                    case Token.Module:
                        ParseModule();
                        break;

                    case Token.Namespace:
                        Report.AddError("?", "Namespaces cannot be nested in one another.", token.SourceFile,
                                        new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                        break;

                    case Token.End:
                        flag = !ParseEndStmt(Token.Namespace);
                        break;

                    case Token.SingleQuote:
                        TokenList list;
                        ParseComment(out list);
                        break;

                    case Token.EOL:
                        tokenQueue.Dequeue();
                        break;

                    default:
                        if (modifierStack.Count > 0)
                        {
                            Unexpected(Report, token);
                            Report.AddError("?", "Function, subroutine, nested module or type declaration expected.",
                                            Tokenizer.SourceFile,
                                            new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                        }
                        break;
                }
            }

            return true;
        }

        private void OpenNamespace(Namespace ns)
        {
            NamespaceEntry entry = namespaceEntry.Children.Find(x => x.Name == ns.Name);

            if (entry == null)
            {
                namespaceEntry = new NamespaceEntry(ns, namespaceEntry);
            }
            else
            {
                namespaceEntry = entry;
            }

            fileNamespaceEntry = new FileNamespaceEntry(namespaceEntry, fileNamespaceEntry, string.Empty, fileEntry);
        }

        private void ParseModule()
        {
            TokenInfo token, token2;

            token = default(TokenInfo);
            token2 = default(TokenInfo);

            bool flag = false;
            Modifier modifiers;

            token = tokenQueue.Dequeue();

            modifiers = ParseModuleModifiers();

            flag = Expect(Token.Identifier, ref token2);

            type = new ModuleDef(ns, token2, modifiers);

            OpenModule();

            TokenList symbols = statementSymbols;

            ExpectEndOfStatementOrEat();

            flag = ParseModuleMembers();

            if (!flag)
            {
                Report.AddItem(VBErrors.ModuleStatementMustEndWithAMatchingEndModule, SourceFile, token.GetSpan(token2),
                               symbols);
            }

            CloseType();
        }

        private void OpenModule()
        {
            if (typeEntry == null)
            {
                typeEntry = new TypeEntry(type, fileNamespaceEntry);
            }
            else
            {
                typeEntry = new TypeEntry(type, typeEntry);
            }
        }

        private Modifier ParseModuleModifiers()
        {
            for (int i = 0; i < modifierStack.Count; i++)
            {
                tokenQueue.Dequeue();
            }

            Modifier[] m = popModifiers();

            Modifier modifiers = Modifier.None;

            foreach (Modifier i in m)
            {
                if (modifiers == 0)
                {
                    modifiers = i;
                }
                else
                {
                    modifiers = modifiers | i;
                }
            }

            return modifiers;
        }

        private bool ParseModuleMembers()
        {
            bool flag;
            TokenInfo token;

            flag = true;

            while (flag)
            {
                token = GetNextToken();

                if (token.Is(Token.EOL))
                    continue;

                if (token.Is(Token.EOF))
                    return false;

                tokenQueue.Enqueue(token);

                switch (token.Token)
                {
                    case Token.Imports:
                        ParseImportStmt();
                        break;

                    case Token.Private:
                    case Token.Public:
                    case Token.Internal:
                    case Token.Shared:
                    case Token.Static:
                    case Token.Const:
                        PrepareModifiers();
                        break;

                        //case Token.Module:
                        //    ParseModule();
                        //    break;

                    case Token.Sub:
                        ParseSubroutine();
                        break;

                    case Token.Function:
                        ParseFunction();
                        break;

                    case Token.Namespace:
                        Report.AddError("?", "Namespaces cannot be nested in types.", token.SourceFile,
                                        new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                        break;

                    case Token.End:
                        flag = !ParseEndStmt(Token.Module);
                        break;

                    case Token.SingleQuote:
                        TokenList list;
                        ParseComment(out list);
                        break;

                    case Token.EOL:
                        tokenQueue.Dequeue();
                        break;

                    default:
                        if (token.Is(Token.Identifier) || token.Is(Token.LeftSquareBracket))
                        {
                            ParseField();
                        }
                        else
                        {
                            if (modifierStack.Count > 0)
                            {
                                Unexpected(Report, token);
                                Report.AddError("?", "Function, subroutine, nested module or type declaration expected.",
                                                Tokenizer.SourceFile,
                                                new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                            }
                        }
                        break;
                }
            }

            return true;
        }

        private void ParseField()
        {
            #region Old

            //bool b = Expect(Token.LeftSquareBracket);

            //TokenInfo token = Tokenizer.PeekToken();

            //if (token.Is(Token.Identifier) || token.Is(Token.ReservedWord))
            //{

            //}

            //if (Expect(Token.As))
            //{
            //    token = Tokenizer.PeekToken();

            //    if (Expect(Token.Identifier))
            //    {

            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{

            //}

            //if (Expect(Token.Equality))
            //{
            //    token = Tokenizer.PeekToken();

            //    if (Expect(Token.Identifier))
            //    {

            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{

            //}

            #endregion

            TokenInfo token;

            Modifier modifiers;
            SimpleName name;
            FullNamedExpression type;
            Expression expr;

            modifiers = ParseMethodModifiers();

            token = tokenQueue.Dequeue();

            name = null;
            type = null;
            expr = null;


            if (token.Is(Token.Identifier))
            {
                name = new SimpleName(statement, token.Value,
                                      new SourceData(token.GetSpan(token), SourceFile, statementSymbols));

                if (EatOptional(Token.As))
                {
                    type = ParseFullName();
                }

                TokenInfo token3 = Tokenizer.PeekToken();

                if (EatOptional(Token.Equality))
                {
                    EatOptional(Token.EOL);

                    //Set parse context to initializer
                    MethodDef initializer;

                    if ((modifiers & Modifier.Shared) == Modifier.Shared)
                    {
                        initializer = (MethodDef) this.type.GetTypeInitializer();
                    }
                    else
                    {
                        initializer = (MethodDef) this.type.GetDefaultConstructor();
                    }

                    //Initalize context
                    method = initializer;
                    block = method.Body;
                    var assignStmt = new ExpressionStatement(initializer.Body);
                    statement = assignStmt;

                    //Parse initialization expression
                    expr = ParseExpression();

                    //Build initialization expression statement
                    assignStmt.SetExpression(
                        new BinaryExpression(assignStmt, Operator.Assign,
                                             new VariableAccess(assignStmt,
                                                                new SimpleName(assignStmt, name, null), null),
                                             expr, null));

                    //Add statement
                    initializer.Body.AddStatement(assignStmt);

                    //Reset context to null
                    method = null;
                    block = null;
                    statement = null;

                    if (expr == null)
                    {
                        Report.AddItem(VBErrors.ExpressionExpected, SourceFile,
                                       token3.GetSpan(token3), statementSymbols);
                    }
                }

                TokenInfo token4 = Tokenizer.PeekToken();

                if (token4.Is(Token.EOL))
                {
                    //GetNextToken(); //*
                    resetStmtTokens();
                }
                else
                {
                    ExpectEndOfStatementOrEat();
                }
            }
            else
            {
                GetNextToken();
                TokenInfo token2 = Tokenizer.PeekToken();
                Report.AddItem(VBErrors.IdentifierExpected, SourceFile,
                               new SourceSpan(token.GetSourceLocation(), token2.GetSourceLocation()), statementSymbols);
                EatToEOL();
            }

            var fieldDef = new FieldDef(this.type, name, type, modifiers);
            fieldDef.SetInitalizationExpression(expr);

            ((ClassStructOrModuleDef) this.type).AddField(fieldDef);
        }

        private bool ParseEndStmt(Token expectedToken)
        {
            TokenInfo token;
            TokenInfo token2;

            bool flag = false;

            var allowedTokens = new[] { Token.Namespace, Token.Module, Token.Sub, Token.Function, Token.If };

            token = tokenQueue.Dequeue();
            token2 = Tokenizer.PeekToken();

            if (allowedTokens.Contains(token2.Token))
            {
                GetNextToken();

                if (token2.Token == expectedToken)
                {
                    flag = true;
                }
                else
                {
                    if (expectedToken == Token.Sub && token2.Is(Token.Module))
                    {
                        Report.AddItem(VBErrors.StatementCannotAppearWithinAMethodBodyEndOfMethodAssumedError,
                                       token.SourceFile, token.GetSpan(token2), statementSymbols);
                    }
                    else
                    {
                        string message = string.Format("\"End {0}\" was not expected", token2.Value);
                        Report.AddWarning("?", message, token.SourceFile,
                                          token.GetSpan(token2), statementSymbols);
                    }

                    resetStmtTokens();
                }
            }
            //else
            //{
            //    Report.Error("?", "Invalid statement.", token.SourceFile, token.GetSourceLocation(), null);
            //}

            ExpectEndOfStatementOrEat();
            return flag;
        }

        private void ParseFunction()
        {
            TokenInfo token = tokenQueue.Dequeue();

            Modifier modifiers;

            MemberName name;
            ParameterList paramList;
            FullNamedExpression returnType;

            bool flag;

            modifiers = ParseMethodModifiers();

            name = ParseMemberName();
            paramList = ParseMethodParameters();

            Expect(Token.As);
            returnType = ParseFullName();

            TokenInfo token2 = statementSymbols.ElementAt(statementSymbols.Count() - 1);

            TokenList symbols = statementSymbols;

            ExpectEndOfStatementOrEat();

            method = new FunctionDef((ClassStructOrModuleDef) type, name.BaseName, returnType, paramList, modifiers);

            flag = ParseFunctionBlock();

            if (!flag)
            {
                Report.AddItem(VBErrors.EndFunctionExpected, SourceFile, token.GetSpan(token2),
                               symbols);
            }

            paramList.SetMethod(method);
            method.SetBody(CloseBlock());
        }

        private Modifier ParseMethodModifiers()
        {
            for (int i = 0; i < modifierStack.Count; i++)
            {
                tokenQueue.Dequeue();
            }

            return makeModifier(popModifiers());
        }

        private Modifier makeModifier(Modifier[] modifiers)
        {
            Modifier modifier = Modifier.None;

            for (int i = 0; i < modifiers.Length; i++)
            {
                modifier = modifier | modifiers[i];
            }

            return modifier;
        }

        private Modifier[] popModifiers()
        {
            Modifier[] a = modifierStack.ToArray();
            modifierStack.Clear();
            return a;
        }

        private void ParseSubroutine()
        {
            Modifier modifiers;

            MemberName name;
            ParameterList paramList;

            TokenInfo token = tokenQueue.Dequeue();

            bool flag;

            modifiers = ParseMethodModifiers();

            name = ParseMemberName();
            paramList = ParseMethodParameters();

            TokenInfo token2 = Tokenizer.PeekToken();

            if (token.Is(Token.Handles))
            {
                FullNamedExpression expr = ParseTypeName();

                if (expr == null)
                {
                    Report.AddError("?", "Identifier expected.", token.SourceFile,
                                    new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                }
            }

            TokenInfo token3 = statementSymbols.ElementAt(statementSymbols.Count() - 1);

            TokenList symbols = statementSymbols;

            ExpectEndOfStatementOrEat();

            method = new SubroutineDef((ClassStructOrModuleDef) type, name.BaseName, paramList, modifiers);

            flag = ParseSubroutineBlock();

            if (!flag)
            {
                Report.AddItem(VBErrors.EndSubExpected, SourceFile, token.GetSpan(token3), symbols);
            }

            paramList.SetMethod(method);
            method.SetBody(CloseBlock());
        }

        private FullNamedExpression ParseTypeName()
        {
            throw new NotImplementedException();
        }

        private bool ExpectEndOfStatementOrEat()
        {
            // Expect an end of line. Report error if not found and eat the rest until the end of line or end of file.

            TokenInfo token;
            bool flag = true;

            token = Tokenizer.PeekToken();

            switch (token.Token)
            {
                case Token.EOL:
                case Token.EOF:
                    token = GetNextToken();
                    break;

                case Token.SingleQuote:
                    tokenQueue.Enqueue(token);

                    TokenList list;
                    ParseComment(out list);

                    break;

                default:
                    flag = false;
                    break;
            }

            if (!flag)
            {
                TokenInfo token2 = EatToEOL();

                Report.AddItem(VBErrors.EndOfStatementExpected, token.SourceFile,
                               new SourceSpan(token.GetSourceLocation(), token2.GetSourceLocation()), statementSymbols);
            }

            resetStmtTokens();

            return flag;
        }

        private TokenInfo EatToEOL()
        {
            // Eat to end of line or in worst case end of file.

            TokenInfo token;

            while (true)
            {
                token = Tokenizer.PeekToken();

                if (token.Is(Token.EOL) || token.Is(Token.EOF))
                    return token;

                GetNextToken();
            }
        }

        private void resetStmtTokens()
        {
            statementSymbols = new TokenList();
        }

        private bool ParseSubroutineBlock()
        {
            return ParseBlock(Token.Sub);
        }

        private bool ParseFunctionBlock()
        {
            return ParseBlock(Token.Function);
        }

        private bool ParseBlock(Token end)
        {
            TokenInfo token;
            Statement statement = null;
            bool flag = true;

            block = new ScopeBlock(method, block);

            while (flag)
            {
                token = Tokenizer.PeekToken();

                if (token.Is(Token.EOF))
                {
                    GetNextToken();
                    return false;
                }

                tokenQueue.Enqueue(token);

                if (token.Is(Token.EOL))
                {
                    tokenQueue.Dequeue();
                    GetNextToken();
                    continue;
                }

                switch (token.Token)
                {
                    case Token.End:
                        GetNextToken();

                        flag = !ParseEndStmt(end);
                        if (flag)
                            continue;
                        else
                            goto end;

                        //case Token.EOL:
                        //    tokenQueue.Dequeue();
                        //    GetNextToken();
                        //    continue;

                    case Token.SingleQuote:
                        TokenList list;
                        ParseComment(out list);
                        //CommentStatement stmt = new CommentStatement(block);
                        //stmt.SetTokenList(list);
                        //statement = stmt;
                        //break;
                        continue;

                    case Token.DoubleQuote:
                        SourceLocation slocEnd = skipCharStringLiteral();
                        var span = new SourceSpan(token.GetSourceLocation(), slocEnd);
                        Report.AddItem(VBErrors.SyntaxError, SourceFile, span, statementSymbols);
                        EatToEOL();
                        resetStmtTokens();
                        //statement = null;
                        continue; //break;

                    default:
                        tokenQueue.Dequeue();
                        statement = ParseStatement();
                        resetStmtTokens();
                        break;
                }

                if (statement == null)
                {
                    statement = new ErrorStatement(block);
                }

                // Set source location
                SourceLocation loc = token.GetSourceLocation();
                //statement.SetLoc(loc);

                // Add statement to block collection
                block.AddStatement(statement);
            }

            end:

            return true;
        }

        private SourceLocation skipCharStringLiteral()
        {
            Expression expr = ParseCharStringLiteral(false);
            return expr.EndLoc;
        }

        /// <summary>
        /// Parse the next-coming statement.
        /// </summary>
        /// <returns></returns>
        public Statement ParseStatement()
        {
            Expression expr;
            bool flag;
            TokenInfo token;

            statement = null;
            flag = true;

            while (flag)
            {
                token = Tokenizer.PeekToken();

                if (token.Is(Token.EOF))
                    break;

                tokenQueue.Enqueue(token);

                switch (token.Token)
                {
                    case Token.Dim:
                        GetNextToken();
                        statement = ParseDeclareStmt();
                        break;

                    case Token.If:
                        GetNextToken();
                        statement = ParseIfStmt();
                        break;

                    case Token.While:
                        GetNextToken();
                        statement = ParseWhileStmt();
                        break;

                    case Token.For:
                        GetNextToken();
                        FeatureNotImplemented(Report, "For statement", token);
                        goto default;

                    case Token.Return:
                        GetNextToken();
                        statement = ParseReturnStatement();
                        EatToEOL();
                        break;

                    case Token.Sub:
                    case Token.Function:
                        GetNextToken();
                        Report.AddItem(VBErrors.StatementCannotAppearWithinAMethodBodyEndOfMethodAssumedError,
                                       SourceFile, new SourceSpan(token.GetSourceLocation(), default(SourceLocation)),
                                       null);
                        EatToEOL();
                        break;

                    case Token.EOL:
                        tokenQueue.Dequeue();
                        GetNextToken();
                        continue;

                    default:
                        tokenQueue.Dequeue();
                        if (token.Is(Token.LeftParenthesis)
                            || Helpers.IsNumber(token)
                            || Helpers.IsIdentifier(token)
                            || Helpers.IsReservedWord(token))
                        {
                            var ret = new ExpressionStatement(block);
                            statement = ret;
                            expr = ParseExpression();
                            ExpectEndOfStatementOrEat();
                            ret.SetExpression(expr);
                            return ret;
                        }

                        GetNextToken();
                        Report.AddItem(VBErrors.SyntaxError, SourceFile,
                                       token.GetSpan(token),
                                       statementSymbols);
                        EatToEOL();
                        break;
                }

                break;
            }

            return statement;
        }

        private Statement ParseReturnStatement()
        {
            tokenQueue.Dequeue();

            var stmt = new ReturnStatement(block);
            statement = stmt;
            Expression expr = ParseExpression();
            stmt.SetExpression(expr);
            return stmt;
        }

        private void FeatureNotImplemented(Report Report, string p, TokenInfo token)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Parse a REPL-statement.
        ///// </summary>
        ///// <returns></returns>
        //public Statement ParseReplStatement()
        //{
        //    TokenInfo token;
        //    Statement statement = null;

        //    token = Tokenizer.PeekToken();

        //    if (token.Is(Token.EOF))
        //    {
        //        GetNextToken();
        //    }
        //    else
        //    {
        //        tokenQueue.Enqueue(token);

        //        switch (token.Token)
        //        {
        //            //case Token.Sub:
        //            //    GetNextToken();
        //            //    return new MethodDefStatement(ParseSubroutine(), new SourceData(sourceFile );

        //            //case Token.Function:
        //            //    GetNextToken();
        //            //    return new MethodDefStatement(ParseFunction());

        //            case Token.EOL:
        //                tokenQueue.Dequeue();
        //                GetNextToken();
        //                break;

        //            default:
        //                tokenQueue.Dequeue();
        //                statement = ParseStatement();
        //                break;
        //        }
        //    }

        //    if (statement == null)
        //    {
        //        statement = new ErrorStatement(null);
        //    }

        //    SourceLocation loc = token.GetSourceLocation();
        //    //statement.SetLoc(loc);

        //    return statement;
        //}

        //private TopLevelBlock ParseBlock(Token end)
        //{
        //    TopLevelBlock block;
        //    Statement statement;
        //    bool flag;
        //    TokenInfo token;

        //    block = new TopLevelBlock();
        //    statement = null;
        //    flag = true;

        //    while (flag)
        //    {
        //        token = this.GetNextToken();

        //        // Has the end of file been reached?
        //        if (token.Is(Token.EOF))
        //            break;

        //        //Push first token on the token stack
        //        this.tokenStack.Enqueue(token);

        //        switch (token.Token)
        //        {
        //                // Declaration statement
        //            case Token.Dim:
        //                statement = this.state_DeclareStmt();
        //                break;

        //                // If statement
        //            case Token.If:
        //                statement = this.ParseIfStmt();
        //                break;

        //                // For statement
        //            case Token.For:
        //                this.Report.FeatureNotImplemented("For statement", token);
        //                goto default;

        //                // Return statement
        //            case Token.Return:
        //                this.Report.FeatureNotImplemented("Return statement", token);
        //                goto default;

        //                // Could be an en of this block
        //            case Token.End:
        //                flag = !this.ParseEndStmt(end);
        //                if (flag) 
        //                    continue; 
        //                else 
        //                    break;

        //                // Empty line
        //            case Token.EOL:
        //                this.tokenStack.Dequeue();
        //                continue;

        //                // Error: Invalid statement
        //            default:
        //                this.tokenStack.Dequeue();
        //                this.Report.Warning("Invalid statement.", token.SourceFile, token.GetSourceLocation(), null);
        //                ExpectOrEatToEOL();
        //                break;
        //        }

        //        // If faulty statement was found and null was returned then emit error statement
        //        if (statement == null) statement = new ErrorStatement();

        //        // Set source location
        //        SourceLocation loc = token.GetSourceLocation();
        //        statement.SetLoc(loc);       

        //        // Add statement to block collection
        //        block.AddStatement(statement);
        //    }

        //    return block;
        //}

        private Statement ParseIfStmt()
        {
            tokenQueue.Dequeue();

            Expression expr;

            Statement inlineStmt;
            TokenInfo token;

            bool flag = true;
            bool inElse;
            
            IfStatement ifStatement = new IfStatement(block);
            statement = ifStatement;

            expr = ParseCompExpr();
            ifStatement.SetCondition(expr);

            Expect(Token.Then);

            ExpectEndOfStatementOrEat();
            
            block = ifStatement;

            //flag = ParseBlock(Token.If);

            while(flag)
            {
                token = this.Tokenizer.PeekToken();

                switch (token.Token)
                {
                    case Token.EOL:
                        GetNextToken();
                        break;

                    case Token.EOF:
                        GetNextToken();
                        flag = false;
                        break;

                    case Token.End:
                        GetNextToken();
                        token = this.Tokenizer.PeekToken();
                        if (token.Is(Token.If))
                        {
                            GetNextToken();
                            flag = false;
                        }
                        else
                        {
                            //Unexpected
                        }
                        break;
                     
                    case Token.Else:
                        GetNextToken();
                        EatOptional(Token.EOL);
                        token = this.Tokenizer.PeekToken();
                        if (token.Is(Token.If))
                        {
                            var tmp1 = ParseIfStmt();
                            ifStatement.SetElse(tmp1);

                            statement = ifStatement;
                        }
                        else
                        {
                            ParseBlock(Token.If);
                            var tmp1 = CloseBlock();
                            ifStatement.SetElse(tmp1);

                            statement = ifStatement;
                        }
                        break;

                    default:
                        inlineStmt = ParseStatement();
                        ifStatement.AddStatement(inlineStmt);

                        statement = ifStatement;
                        break;
                }
            }

            block = ifStatement.ContainingBlock;

            return ifStatement;
        }

        private Statement ParseWhileStmt()
        {
            tokenQueue.Dequeue();

            Expression expr;

            Statement inlineStmt;
            TokenInfo token;

            bool flag = true;
            bool inElse;

            WhileStatement whileStatement = new WhileStatement(block);
            statement = whileStatement;

            expr = ParseCompExpr();
            whileStatement.SetCondition(expr);

            ExpectEndOfStatementOrEat();

            block = whileStatement;

            while (flag)
            {
                token = this.Tokenizer.PeekToken();

                switch (token.Token)
                {
                    case Token.EOL:
                        GetNextToken();
                        break;

                    case Token.EOF:
                        GetNextToken();
                        flag = false;
                        break;

                    case Token.End:
                        GetNextToken();
                        token = this.Tokenizer.PeekToken();
                        if (token.Is(Token.While))
                        {
                            GetNextToken();
                            flag = false;
                        }
                        else
                        {
                            //Unexpected
                        }
                        break;
                
                    default:
                        inlineStmt = ParseStatement();
                        whileStatement.AddStatement(inlineStmt);

                        statement = whileStatement;
                        break;
                }
            }

            block = whileStatement.ContainingBlock;

            return whileStatement;
        }

        private Expression ParseCompExpr()
        {
            isComparisonExpression = true;

            Expression ret = ParseExpression();

            TokenInfo token;

            while (true)
            {
                Operator op;

                token = Tokenizer.PeekToken();

                switch (token.Token)
                {
                    case Token.Equality:
                        GetNextToken();
                        op = Operator.Equal;
                        break;

                    case Token.Inequality:
                        GetNextToken();
                        op = Operator.NotEquals;
                        break;

                    case Token.Is:
                        GetNextToken();
                        op = Operator.Is;
                        break;

                    case Token.IsNot:
                        GetNextToken();
                        op = Operator.IsNot;
                        break;

                    default:
                        isComparisonExpression = false;
                        return ret;
                }
                Expression rhs = ParseCompExpr();

                var sourceData =
                    new SourceData(new SourceSpan(ret.SourceData.SourceSpan.Start, rhs.SourceData.SourceSpan.End),
                                   SourceFile, statementSymbols);
                var be = new BinaryExpression(statement, op, ret, rhs, sourceData);
                //be.SetLoc(ret.Start, GetEnd());
                ret = be;
            }
        }

        private Expression ParseExpression()
        {
            return ParseExpr(0);
        }

        private Expression ParseExpr(int precedence)
        {
            Expression ret = ParseFactor();

            while (true)
            {
                TokenInfo t = Tokenizer.PeekToken();

                if (!Helpers.IsBinaryOperator(t)) return ret;

                Operator op = GetBinaryOperator(t);
                int prec = GetOperatorPrecedence(op);

                if (prec >= precedence)
                {
                    GetNextToken();

                    t = Tokenizer.PeekToken();

                    if (EatOptional(Token.EOL))
                    {
                        statementSymbols.Add(t);
                    }

                    Expression right = ParseExpr(prec + 1);

                    //SourceLocation start = ret.Location.Start;
                    var sourceData =
                        new SourceData(
                            new SourceSpan(ret.SourceData.SourceSpan.Start, right.SourceData.SourceSpan.End), SourceFile,
                            statementSymbols);
                    ret = new BinaryExpression(statement, op, ret, right, sourceData);
                    //ret.SetLoc(start, this.Tokenizer.GetSourceLocation());
                }
                else
                {
                    return ret;
                }
            }
        }

        private Expression ParseFactor()
        {
            //SourceLocation start = _lookahead.Span.Start;

            Expression ret, expr;

            TokenInfo token = Tokenizer.PeekToken();

            switch (token.Token)
            {
                case Token.Plus:
                    expr = ParseFactor();
                    ret = new UnaryExpression(statement, Operator.Positive, expr,
                                              new SourceData(
                                                  new SourceSpan(token.GetSourceLocation(),
                                                                 expr.SourceData.SourceSpan.End), SourceFile,
                                                  statementSymbols));
                    break;

                case Token.Minus:
                    expr = ParseFactor();
                    ret = new UnaryExpression(statement, Operator.Negate, expr,
                                              new SourceData(
                                                  new SourceSpan(token.GetSourceLocation(),
                                                                 expr.SourceData.SourceSpan.End), SourceFile,
                                                  statementSymbols));
                    break;

                    //case Token.Twiddle:
                    //    this.GetNextToken();
                    //    ret = new UnaryExpression(PythonOperator.Invert, ParseFactor());
                    //    break;

                default:
                    return ParsePower();
            }

            return ret;
        }

        private Expression ParsePower()
        {
            Expression ret = ParsePrimary();
            //ret = AddTrailers(ret);
            if (EatOptional(Token.Caret))
            {
                Expression right = ParseFactor();
                var sourceData =
                    new SourceData(new SourceSpan(ret.SourceData.SourceSpan.Start, right.SourceData.SourceSpan.End),
                                   SourceFile, statementSymbols);
                ret = new BinaryExpression(statement, Operator.Power, ret, right, sourceData);
            }
            return ret;
        }

        private Expression ParsePrimary()
        {
            TokenInfo tokenInfo = Tokenizer.PeekToken();

            Expression ret;

            switch (tokenInfo.Token)
            {
                case Token.LeftParenthesis:
                    return ParseParenthesisExpr();

                case Token.LeftSquareBracket:
                case Token.Identifier:
                    ret = ParseMemberOrMethodAccess();
                    return ret;

                case Token.Number:
                    return ParseNumberConstant();

                case Token.DoubleQuote:
                    return ParseCharStringLiteral();

                case Token.SingleQuote:
                    GetNextToken();
                    ParseComment2();
                    return null;

                    //default:

                    //    //Report.Add(VBErrors.ExpressionExpected, SourceFile, tokenInfo.GetSpan(tokenInfo), statementSymbols);
                    //    //ret = new ErrorExpression();
                    //    return null;
            }

            //if (VBTokens.IsReservedWord(tokenInfo))
            //{
            //    return this.ParseMethodAccess();
            //}

            return null;
        }

        private Expression ParseCharStringLiteral(bool throwExpectedEndError = true)
        {
            SourceLocation srcStartLoc;

            var tokens = new List<TokenInfo>();

            TokenInfo token = GetNextToken();
            TokenInfo startToken = token;

            srcStartLoc = token.GetSourceLocation();

            Tokenizer.IgnoreSpaces = false;

            token = Tokenizer.PeekToken();

            TokenInfo token2 = default(TokenInfo);

            while (charStringCond(token))
            {
                tokens.Add(token);

                if (charStringCond(token))
                {
                    GetNextToken();
                }
                else
                {
                    break;
                }

                token2 = token;
                token = Tokenizer.PeekToken();
            }

            Tokenizer.IgnoreSpaces = true;

            SourceLocation srcEndLoc = token.GetSourceLocation();

            token = Tokenizer.PeekToken();

            if (token.Is(Token.DoubleQuote))
            {
                GetNextToken();
                srcEndLoc = new SourceLocation(token.Ln + token.Value.Length, token.Col + token.Value.Length);
            }

            //if (throwExpectedEndError)
            //{
            //    EatOptional(Token.DoubleQuote, "End of string literal expected.");
            //}

            Expression expr = new CharStringLiteral(statement, tokens,
                                                    new SourceData(startToken.GetSpan(token), SourceFile,
                                                                   statementSymbols));
            expr.SetStartLoc(srcStartLoc);
            expr.SetEndLoc(srcEndLoc);

            return expr;
        }

        private bool charStringCond(TokenInfo token)
        {
            return !token.Is(Token.DoubleQuote) && !(token.Is(Token.EOL) || token.Is(Token.EOF));
        }

        private Expression ParseMemberOrMethodAccess()
        {
            TokenInfo token = Tokenizer.PeekToken();

            TokenInfo token2;

            FullNamedExpression name = ParseFullName();

            if (EatOptional(Token.LeftParenthesis))
            {
                Expression[] args = ParseArguments();

                token2 = GetNextToken();

                SourceSpan span = token.GetSpan(token2);

                var sourceData = new SourceData(span, SourceFile, statementSymbols);

                return new MethodCallOrArrayDereference(statement, name, args, sourceData);
            }

            token2 = Tokenizer.PeekToken();

            return new VariableAccess(statement, name,
                                      new SourceData(token.GetSpan(token2), SourceFile, statementSymbols));
        }

        private FullNamedExpression ParseFullName()
        {
            bool b = EatOptional(Token.LeftSquareBracket);

            TokenInfo token;
            FullNamedExpression name = null;
            token = Tokenizer.PeekToken();

            while (true)
            {
                token = Tokenizer.PeekToken();

                switch (token.Token)
                {
                    case Token.Identifier:
                        name = new SimpleName(statement, name, token.Value,
                                              new SourceData(token.GetSpan(token), SourceFile, statementSymbols));
                        GetNextToken();
                        //if(token.Is(Token.LeftParenthesis))
                        //{
                        //    GetNextToken();
                        //    GenericArguments args = ParseFullNameGenericArgs();
                        //    SimpleName n = name as SimpleName;
                        //    name = new GenericNameExpression(statement, n, args, new SourceData(token.GetSpan(token), this.SourceFile, statementSymbols));
                        //    goto end;
                        //}
                        goto end2;
                }

                if (Helpers.IsReservedWord(token))
                {
                    name = new SimpleName(statement, token.Value, b,
                                          new SourceData(token.GetSpan(token), SourceFile, statementSymbols));
                    GetNextToken();

                    if (b && !EatOptional(Token.RightSquareBracket))
                    {
                        Report.AddItem(VBErrors.BracketedIdentifierIsMissingClosingRightSquareBracket, SourceFile,
                                       token.GetSpan(token), statementSymbols);
                        EatToEOL();
                        goto end;
                    }
                }

                end2:

                token = tokenizer.PeekToken();

                if (token.Is(Token.Period))
                {
                    GetNextToken();
                }
                else
                {
                    goto end;
                }
            }

            end:
            return name;
        }

        private GenericArguments ParseFullNameGenericArgs()
        {
            //if (EatOptional(Token.LeftAngleBracket))
            //{
            //    while (true)
            //    {
            //        TokenInfo token = Tokenizer.PeekToken();

            //        switch(token.Token)
            //        {
            //            c
            //        }

            //        EatOptional(Token.RightAngleBracket);
            //    }
            //}

            var args = new GenericArguments();

            bool end = false;
            bool expectArg = true;

            TokenInfo token = Tokenizer.PeekToken();

            if (token.Is(Token.RightParenthesis))
            {
                GetNextToken();
                goto end;
            }

            while (true)
            {
                if (expectArg)
                {
                    Expression expr = ParseExpression();

                    if (expr != null)
                        expectArg = false;
                    else
                        goto error;

                    token = tokenizer.PeekToken();
                    end = token.Is(Token.RightParenthesis);

                    if (end)
                        goto end;
                }
                else
                {
                    bool ateComma = EatOptional(Token.Comma);

                    if (ateComma)
                        expectArg = true;
                    else
                        goto error;

                    token = tokenizer.PeekToken();
                    end = token.Is(Token.RightParenthesis);

                    if (end)
                        goto end;
                }

                token = Tokenizer.PeekToken();
            }

            error:
            if (expectArg) token = GetLastToken();

            Report.AddItem(VBErrors.CommaOrValidExpressionContinuationExpected, SourceFile, token.GetSpan(token),
                           statementSymbols);
            EatToEOL();

            end:
            //token = Tokenizer.PeekToken();

            //if (!end && token.Is(Token.EOL))
            //{
            //    Report.Add(VBErrors.EndOfStatementExpected, SourceFile, token.GetSpan(token), statementSymbols);
            //}

            //EatToEOL();

            return args;
        }

        private Expression[] ParseArguments()
        {
            #region Old

            //List<Expression> exprs = new List<Expression>();

            //TokenInfo token = Tokenizer.PeekToken();

            //bool expectArg = true;

            //while (true)
            //{
            //    if (token.Is(Token.Comma))
            //    {
            //        GetNextToken();
            //        expectArg = true;
            //    }
            //    else
            //    {
            //        if (expectArg)
            //        {
            //            Expression expr = ParseExpr(0);

            //            if (expr == null)
            //            {
            //                break;
            //            }

            //            exprs.Add(expr);
            //            expectArg = false;
            //        }
            //        else
            //        {
            //            if (EatOptional(Token.RightParenthesis))
            //                break;

            //            Report.Add(VBErrors.CommaOrValidExpressionContinuationExpected, SourceFile, token.GetSpan(token), statementSymbols);

            //            token = Tokenizer.PeekToken();

            //            while (!token.Is(Token.RightParenthesis))
            //            {
            //                if (token.Is(Token.EOL))
            //                    break;

            //                GetNextToken();
            //                token = Tokenizer.PeekToken();
            //            }

            //            break;
            //        }
            //    }

            //    token = Tokenizer.PeekToken();
            //}

            //EatToEOL();

            #endregion

            var exprs = new List<Expression>();

            bool end = false;
            bool expectArg = true;

            TokenInfo token = Tokenizer.PeekToken();

            if (token.Is(Token.RightParenthesis))
            {
                GetNextToken();
                goto end;
            }

            while (true)
            {
                if (expectArg)
                {
                    Expression expr = ParseExpression();
                    exprs.Add(expr);

                    if (expr != null)
                        expectArg = false;
                    else
                        goto error;

                    token = tokenizer.PeekToken();
                    end = token.Is(Token.RightParenthesis);

                    if (end)
                        goto end;
                }
                else
                {
                    bool ateComma = EatOptional(Token.Comma);

                    if (ateComma)
                        expectArg = true;
                    else
                        goto error;

                    token = tokenizer.PeekToken();
                    end = token.Is(Token.RightParenthesis);

                    if (end)
                        goto end;
                }

                token = Tokenizer.PeekToken();
            }

            error:
            if (expectArg) token = GetLastToken();

            Report.AddItem(VBErrors.CommaOrValidExpressionContinuationExpected, SourceFile, token.GetSpan(token),
                           statementSymbols);
            EatToEOL();

            end:
            //token = Tokenizer.PeekToken();

            //if (!end && token.Is(Token.EOL))
            //{
            //    Report.Add(VBErrors.EndOfStatementExpected, SourceFile, token.GetSpan(token), statementSymbols);
            //}

            //EatToEOL();

            return exprs.ToArray();
        }

        private TokenInfo GetLastToken()
        {
            return statementSymbols.ElementAt(statementSymbols.Count() - 1);
        }

        private Expression ParseParenthesisExpr()
        {
            GetNextToken();

            Expression expr = ParseExpr(0);
            Expect(Token.RightParenthesis);
            return expr;
        }

        private Expression ParseNumberConstant()
        {
            TokenInfo tokenInfo = default(TokenInfo);
            int nr = 0;

            if (Expect(Token.Number, ref tokenInfo))
            {
                nr = tokenInfo.GetAsInt32();
            }

            return new IntegerConstant(statement, nr,
                                       new SourceData(tokenInfo.GetSpan(tokenInfo), SourceFile, statementSymbols));
        }

        private int GetOperatorPrecedence(Operator op)
        {
            switch (op)
            {
                case Operator.Period:
                    return 7;

                case Operator.Star:
                case Operator.Slash:
                    return 6;

                case Operator.Plus:
                case Operator.Minus:
                    return 5;

                case Operator.Less:
                case Operator.Greater:
                    return 4;

                case Operator.Equal:
                case Operator.NotEquals:
                    return 3;

                case Operator.And:
                    return 2;

                case Operator.Or:
                    return 1;

                case Operator.Assign:
                    //case Operator.AssignsAdd
                    //case Operator.AssignSub:
                    return 0;

                    //case Operator.Period:
                    //    return 0;

                    //case Operator.Star:
                    //case Operator.Slash:
                    //    return 1;

                    //case Operator.Plus:
                    //case Operator.Minus:
                    //    return 2;

                    //case Operator.Equal:
                    //case Operator.NotEquals:
                    //    return 3;

                    //case Operator.And:
                    //    return 4;

                    //case Operator.Or:
                    //    return 5;

                    //case Operator.Assign:
                    //    //case Operator.AssignsAdd
                    //    //case Operator.AssignSub:
                    //    return 6;
            }

            throw new InternalCompilerException("");
        }

        private Operator GetBinaryOperator(Token token)
        {
            switch (token)
            {
                case Token.Plus:
                    return Operator.Plus;

                case Token.Minus:
                    return Operator.Minus;

                case Token.Star:
                    return Operator.Star;

                case Token.Slash:
                    return Operator.Slash;

                case Token.Equality:
                    if (isComparisonExpression)
                        return Operator.Equal;
                    return Operator.Assign;

                case Token.LeftAngleBracket:
                    return Operator.Less;

                case Token.RightAngleBracket:
                    return Operator.Greater;

                case Token.Period:
                    return Operator.Period;
            }

            throw new InternalCompilerException("");
        }

        private Statement ParseDeclareStmt()
        {
            TokenInfo token;

            SimpleName name;
            FullNamedExpression type;
            Expression expr;

            tokenQueue.Dequeue();

            token = Tokenizer.PeekToken();

            name = null;
            type = null;
            expr = null;

            var stmt = new DeclareStatement(block);
            statement = stmt;

            if (EatOptional(Token.Identifier))
            {
                name = new SimpleName(statement, token.Value,
                                      new SourceData(token.GetSpan(token), SourceFile, statementSymbols));

                if (EatOptional(Token.As))
                {
                    type = ParseFullName();
                }

                TokenInfo token3 = Tokenizer.PeekToken();

                if (EatOptional(Token.Equality))
                {
                    EatOptional(Token.EOL);

                    expr = ParseExpression();

                    if (expr == null)
                    {
                        Report.AddItem(VBErrors.ExpressionExpected, SourceFile,
                                       token3.GetSpan(token3), statementSymbols);
                    }
                }

                TokenInfo token4 = Tokenizer.PeekToken();

                if (token4.Is(Token.EOL))
                {
                    //GetNextToken(); //*
                    resetStmtTokens();
                }
                else
                {
                    ExpectEndOfStatementOrEat();
                }
            }
            else
            {
                GetNextToken();
                TokenInfo token2 = Tokenizer.PeekToken();
                Report.AddItem(VBErrors.IdentifierExpected, SourceFile,
                               new SourceSpan(token.GetSourceLocation(), token2.GetSourceLocation()), statementSymbols);
                EatToEOL();
            }

            stmt.SetName(name);
            stmt.SetType(type);
            stmt.SetInitializationExpression(expr);

            return stmt;
        }

        private ParameterList ParseMethodParameters()
        {
            bool flag;
            TokenInfo token;
            ParameterList paramList;
            Parameter param;

            bool expectParam = true;

            flag = true;
            paramList = new ParameterList();

            Expect(Token.LeftParenthesis);

            while (flag)
            {
                token = GetNextToken();

                tokenQueue.Enqueue(token);

                switch (token.Token)
                {
                    case Token.ByRef:
                    case Token.ByVal:
                        ParseMethodParameterModifier();
                        break;

                    case Token.Identifier:
                        if (!expectParam)
                        {
                        }
                        param = ParseMethodParameter(paramList);
                        paramList.Add(param);
                        expectParam = false;
                        break;

                    case Token.Comma:
                        if (expectParam)
                        {
                        }
                        tokenQueue.Dequeue();
                        expectParam = true;
                        break;

                    case Token.EOL:
                        tokenQueue.Dequeue();
                        Report.AddItem(VBErrors.RighParenthesisExpected, token.SourceFile,
                                       new SourceSpan(token.GetSourceLocation(), default(SourceLocation)), null);
                        goto case Token.RightParenthesis;

                    case Token.RightParenthesis:
                        tokenQueue.Dequeue();
                        flag = false;
                        break;

                    default:

                        break;
                }
            }

            return paramList;
        }

        private void ParseMethodParameterModifier()
        {
            tokenQueue.Dequeue();
            //TODO: Push to modifier stack.
        }

        private Parameter ParseMethodParameter(ParameterList plist)
        {
            TokenInfo token;
            Modifier[] modifiers;
            FullNamedExpression type;

            token = tokenQueue.Dequeue();

            modifiers = popModifiers();

            Expect(Token.As);

            type = ParseFullName();

            return new Parameter(plist, token.Value, type, default(Modifier));
        }

        private MemberName ParseMemberName()
        {
            TokenInfo token;

            MemberName nameExpr = null;

            token = Tokenizer.PeekToken();

            while (true)
            {
                if (!Helpers.IsIdentifierOrReservedWord(token))
                {
                    Expected(Report, Token.Identifier, ref token);
                    break;
                }

                token = GetNextToken();

                if (nameExpr == null)
                {
                    nameExpr = new MemberName(token.Value);
                }
                else
                {
                    nameExpr = new MemberName(nameExpr, Separators.Dot, token.Value);
                }

                token = Tokenizer.PeekToken();

                if (token.Is(Token.Period))
                {
                    GetNextToken();
                }
                else
                {
                    break;
                }
            }

            return nameExpr;
        }

        private void Expected(Report Report, Token token, ref TokenInfo token_2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Conditional eat. Does not throw error.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool EatOptional(Token token)
        {
            TokenInfo tokenKind = Tokenizer.PeekToken();

            if (tokenKind.Is(token))
            {
                GetNextToken();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Conditional eat that throws custom message. By default a warning.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool EatOptional(Token token, string message, MessageType type = MessageType.Warning)
        {
            TokenInfo tokenInfo = Tokenizer.PeekToken();

            if (tokenInfo.Is(token))
            {
                GetNextToken();
                return true;
            }

            switch (type)
            {
                case MessageType.Error:
                    Report.AddError("?", message, tokenInfo.SourceFile,
                                    new SourceSpan(tokenInfo.GetSourceLocation(), default(SourceLocation)), null);
                    break;

                case MessageType.Message:
                    Report.AddMessage("?", message, tokenInfo.SourceFile,
                                      new SourceSpan(tokenInfo.GetSourceLocation(), default(SourceLocation)), null);
                    break;

                case MessageType.Warning:
                    Report.AddWarning("?", message, tokenInfo.SourceFile,
                                      new SourceSpan(tokenInfo.GetSourceLocation(), default(SourceLocation)), null);
                    break;
            }
            return false;
        }

        private bool Expect(Token token)
        {
            TokenInfo tokenInfo = default(TokenInfo);
            return Expect(token, ref tokenInfo);
        }

        private bool Expect(Token token, string message)
        {
            TokenInfo tokenInfo = default(TokenInfo);
            return Expect(token, ref tokenInfo, message);
        }

        private bool Expect(Token token, ref TokenInfo tokenInfo)
        {
            tokenInfo = GetNextToken();

            if (tokenInfo.Is(token))
            {
                return true;
            }

            Expected(Report, token, ref tokenInfo);
            return false;
        }

        private bool Expect(Token token, ref TokenInfo tokenInfo, string message)
        {
            tokenInfo = GetNextToken();

            if (tokenInfo.Is(token))
            {
                return true;
            }

            Report.AddError("?", message, tokenInfo.SourceFile,
                            new SourceSpan(tokenInfo.GetSourceLocation(), default(SourceLocation)),
                            null);

            return false;
        }

        private bool ExpectIdentifier(ref TokenInfo tokenInfo)
        {
            tokenInfo = GetNextToken();

            if (tokenInfo.Is(Token.Identifier))
            {
                return true;
            }

            Report.AddItem(VBErrors.IdentifierExpected, SourceFile,
                           new SourceSpan(tokenInfo.GetSourceLocation(), default(SourceLocation)),
                           null);

            return false;
        }

        private TokenInfo GetNextTokenAndPush()
        {
            TokenInfo token = GetNextToken();
            tokenQueue.Enqueue(token);
            return token;
        }

        private bool checkInputPaths()
        {
            bool flag = true;

            foreach (SourceFile file in context.SourceUnit)
            {
                if (!file.Exists)
                {
                    string message = "The file \"{0}\" could not be found.";

                    if (file.IsValidPath)
                    {
                        message += " The path is not valid.";
                    }

                    Report.AddError("?", string.Format(message, file.Path), null, default(SourceSpan), null);

                    flag = false;
                }
            }

            return flag;
        }
    }
}