using System;
using System.IO;

namespace Basic
{
    public class SourceFile : IInputSource
    {
        private readonly string path;

        public SourceFile(string path)
        {
            if (!System.IO.Path.IsPathRooted(path))
                this.path = System.IO.Path.GetFullPath(path);
            else
                this.path = path;
        }

        #region IInputSource Members

        public string Path
        {
            get { return path; }
        }

        public bool Exists
        {
            get { return File.Exists(path); }
        }

        public bool IsValidPath
        {
            get
            {
                try
                {
                    return new FileInfo(path).Exists;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public void Move(string destination)
        {
            File.Move(path, destination);
        }

        public void Delete()
        {
            File.Delete(path);
        }


        public FileStream OpenRead()
        {
            return File.OpenRead(path);
        }

        public StreamReader GetReader()
        {
            return File.OpenText(path);
        }

        #endregion

        public override string ToString()
        {
            return Path;
        }
    }
}