using System.Collections.Generic;

namespace Wizkisoft.DotNet.IO
{
    public interface IFileCreator
    {
        IFileCreator SetDirectory(string path);

        void CreateFile(string name, IList<string> content);
    }
}
