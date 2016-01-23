namespace Basic
{
    internal static class RuntimeLoader
    {
        static RuntimeLoader()
        {
            TypeManager.AddAssembly(typeof (Conversion).Assembly);
        }

        public static void Load()
        {
        }
    }
}