using Core.Common.Items;

namespace Core.Common.Interfaces.Excell
{
    public interface IExcelWriter
    {
        Result<bool> ExportToExcel(string fileDataFileName, CoverSampleResult coverSample);
    }
}
