using System;

namespace Basic
{
    public struct SourceLocation : IComparable<SourceLocation>
    {
        private readonly int col;
        private readonly int ln;

        public SourceLocation(int ln, int col)
        {
            this.ln = ln;
            this.col = col;
        }

        public int Ln
        {
            get { return ln; }
        }

        public int Col
        {
            get { return col; }
        }

        #region IComparable<SourceLocation> Members

        public int CompareTo(SourceLocation other)
        {
            if (this == other)
            {
                return 0;
            }

            return this > other ? 1 : -1;
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, {1}", ln, col);
        }

        public static bool operator ==(SourceLocation l1, SourceLocation l2)
        {
            return (l1.Ln == l2.Ln) && (l1.Col == l2.Col);
        }

        public static bool operator !=(SourceLocation l1, SourceLocation l2)
        {
            return (l1 != l2);
        }

        public static bool operator <(SourceLocation l1, SourceLocation l2)
        {
            return (l1.Ln < l2.Ln) || (l1.Col < l2.Col);
        }

        public static bool operator >(SourceLocation l1, SourceLocation l2)
        {
            return (l1.Ln > l2.Ln) || (l1.Col > l2.Col);
        }
    }
}