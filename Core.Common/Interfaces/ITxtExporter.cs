using Core.Common.Items;
using Core.Common.Items.MatrixFeatures;

namespace Core.Common.Interfaces
{
    public interface ITxtExporter
    {
        Result<bool> ExportToTxt(Matrix coverResultDataObjects, string suffix);
    }
}
