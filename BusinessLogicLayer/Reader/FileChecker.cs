using System.IO;
using System.Linq;
using Core.Common.Interfaces;

namespace BusinessLogicLayer.Reader
{
    public class FileChecker : IFileChecker
    {
        private const char Separator = '.';

        public bool IsFileExsist(string path)
        {
            return File.Exists(path);
        }

        public bool IsFileHasAppriopriateExtension(string path, string dataFilter)
        {
            return dataFilter.Contains(path.Split(Separator).Last());
        }
    }
}
