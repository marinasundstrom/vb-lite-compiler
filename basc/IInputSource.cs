using System.IO;

namespace Basic
{
    public interface IInputSource
    {
        bool Exists { get; }
        bool IsValidPath { get; }
        string Path { get; }
        void Delete();
        StreamReader GetReader();
        void Move(string destination);
        FileStream OpenRead();
    }
}