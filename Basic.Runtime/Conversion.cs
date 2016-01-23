using System;

namespace Basic
{
    public static class Conversion
    {
        public static string CString(object o)
        {
            return Convert.ToString(o);
        }

        public static char CChar(object o)
        {
            return Convert.ToChar(o);
        }

        public static int CInt(object o)
        {
            return Convert.ToInt32(o);
        }

        public static bool CBool(object o)
        {
            return Convert.ToBoolean(o);
        }

        public static T CType<T>(object from, T to)
        {
            return (T) from;
        }
    }
}