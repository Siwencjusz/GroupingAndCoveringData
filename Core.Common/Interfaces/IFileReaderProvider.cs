using Core.Common.Items;

namespace Core.Common.Interfaces
{
      public interface IFileReaderProvider
    {
        Result<FileData> ReadFile(string fileFilter);
    }
}
