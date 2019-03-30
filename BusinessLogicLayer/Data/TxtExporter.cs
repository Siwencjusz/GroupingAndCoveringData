using System;
using System.Data;
using System.Linq;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Data
{
    public class TxtExporter: ITxtExporter
    {
        private ISaveFileDialog _newSFD { get; set; }
        public IDataObjectsConverter _dataObjectsConverter { get; }
        public TxtExporter(
            ISaveFileDialog sfd,
            IDataObjectsConverter dataObjectsConverter)
        {
            _newSFD = sfd;
            _dataObjectsConverter = dataObjectsConverter;
        }
        public Result<bool> ExportToTxt(Core.Common.Items.MatrixFeatures.Matrix coverResultDataObjects,
            string suffix)
        {
            var sfdResult = SaveFileDialog(suffix);
            string fileName;
            if (sfdResult.Value == true)
            {
                fileName = _newSFD.FileName;
            }
            else
            {
                return new Result<bool>("Nie zapisano pliku");
            }

            WriteToFile(coverResultDataObjects, fileName);
            return new Result<bool>(true);
        }

        private void WriteToFile(Core.Common.Items.MatrixFeatures.Matrix matrix, string fileName)
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(fileName))
            {
                foreach (DataRow line in matrix.DataTable.Rows)
                {
                    var array = line.ItemArray.ToList();
                    array.RemoveAt(0);
                    foreach (var attribute in line.ItemArray)
                    {                     
                            file.Write(attribute + "\t");                                              
                    }
                    file.WriteLine();
                }
            }

        }

        private Result<bool?> SaveFileDialog(string suffix)
        {
            try
            {
                _newSFD.FileName = DateTime.Now.ToFileTimeUtc() + suffix;
                _newSFD.DefaultExt = ".txt";
                _newSFD.Filter = "Txt (.txt)|*.txt";
                var sfdResult = _newSFD.ShowDialog();
                return new Result<bool?>(sfdResult);
            }
            catch (Exception e)
            {
                return new Result<bool?>(e);
            }

        }
    }
}
