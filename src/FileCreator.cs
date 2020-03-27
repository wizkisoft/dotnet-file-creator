using System;
using System.Collections.Generic;
using Wizkisoft.DotNet.Extension;
using Wizkisoft.DotNet.Wrapper;

namespace Wizkisoft.DotNet.IO
{
    public class FileCreator : IFileCreator
    {
        public FileCreator(IDirectory directory, Func<string, IStreamWriter> streamWriter)
        {
            _directory = directory;
            _newStreamWriter = streamWriter;
        }

        public IFileCreator SetDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException("You must provide a path");

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("You must provide a path");

            _directory.CreateDirectory(_path = path);
            return this;
        }

        public void CreateFile(string name, IList<string> content)
        {
            var filePath = !string.IsNullOrWhiteSpace(_path) ? string.Concat(_path, "/", name) : name;

            using var writer = _newStreamWriter(filePath);
            content.ForEach(line => writer.WriteLine(line));
        }

        private string _path;

        private readonly IDirectory _directory;

        private readonly Func<string, IStreamWriter> _newStreamWriter;
    }
}
