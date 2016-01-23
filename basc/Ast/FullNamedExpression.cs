namespace Basic.Ast
{
    public abstract class FullNamedExpression : Expression
    {
        public FullNamedExpression(Statement statement, FullNamedExpression left, SourceData sourceData)
            : base(statement, sourceData)
        {
            Left = left;
        }

        public FullNamedExpression Left { get; private set; }

        public bool IsKeyword { get; protected set; }

        public int NumberOfParts
        {
            get
            {
                if (this is GenericNameExpression)
                {
                    return Left.NumberOfParts;
                }

                if (Left != null)
                {
                    return Left.NumberOfParts + 1;
                }

                return 1;
            }
        }

        public bool IsSimpleIdentifier
        {
            get
            {
                if (IsGenericMemberName)
                    return false;

                //return this.Left != null;

                return true;
            }
        }

        public bool IsGenericMemberName
        {
            get { return this is GenericNameExpression; }
        }

        public virtual GenericArguments GetGenericArguments()
        {
            return null;
        }

        public abstract string GetSimpleName();

        public string GetName(bool generic)
        {
            if (!generic)
            {
                return GetSimpleName();
            }

            return Left.GetSimpleName() + GetGenericArguments();
        }

        public override string ToString()
        {
            if (IsGenericMemberName)
                return GetName(true);

            return GetSimpleName();
        }

        public static implicit operator string(FullNamedExpression expr)
        {
            return expr.ToString();
        }
    }
}