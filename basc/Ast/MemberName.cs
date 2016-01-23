namespace Basic.Ast
{
    public class MemberName
    {
        public MemberName(string name)
        {
            Name = name;
        }

        public MemberName(MemberName left, Separators separator, string name)
            : this(name)
        {
            Left = left;
            Separator = separator;
        }

        public MemberName Left { get; private set; }

        public Separators Separator { get; private set; }

        public string Name { get; private set; }

        public string BaseName
        {
            get
            {
                if (Left == null)
                {
                    return Name;
                }

                return Left.BaseName + s(Separator) + Name;
            }
        }

        private string s(Separators sep)
        {
            switch (sep)
            {
                case Separators.Dot:
                    return ".";
                case Separators.DoubleColon:
                    return "::";
                case Separators.Slash:
                    return "/";
            }

            return string.Empty;
        }


        public static MemberName operator +(MemberName op1, MemberName op2)
        {
            return op2.Left = op1;
        }

        public override string ToString()
        {
            return BaseName;
        }
    }
}