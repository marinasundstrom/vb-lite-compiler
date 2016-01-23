using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Basic
{
    public class SourceUnit : IEnumerable<SourceFile>
    {
        private readonly List<SourceFile> files;

        public SourceUnit()
        {
            files = new List<SourceFile>();
        }

        public IEnumerable<SourceFile> Files
        {
            get { return files; }
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public SourceFile this[int index]
        {
            get { return files.ElementAt(index); }
        }

        #region IEnumerable<SourceFile> Members

        public IEnumerator<SourceFile> GetEnumerator()
        {
            return files.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return files.GetEnumerator();
        }

        #endregion

        public void Add(SourceFile sourceFile)
        {
            files.Add(sourceFile);
        }

        public void Add(string sourcePath)
        {
            files.Add(new SourceFile(sourcePath));
        }

        public void Remove(SourceFile sourceFile)
        {
            files.Remove(sourceFile);
        }

        public bool Check()
        {
            return files.All(file => file.Exists);
        }
    }
}