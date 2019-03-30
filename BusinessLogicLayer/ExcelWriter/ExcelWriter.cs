using System;
using System.Data;
using System.Linq;
using Core.Common.Items;
using ClosedXML.Excel;
using Core.Common.Interfaces;
using Core.Common.Interfaces.Excell;

namespace BusinessLogicLayer.ExcelWriter
{
    public class ExcelWriter : IExcelWriter
    {
        private const string MacierzPredykcjiTestowe = "Macierz Predykcji testowe";
        private const string MacierzPredykcjiTreningowe = "Macierz Predykcji treningowe";
        private const string Obiekty = "Obiekty";
        private const string NieZapisanoPliku = "Nie zapisano pliku";
        private const string Illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
        private const string NewSfdDefaultExt = ".xlsx";
        private const string XlsDocumentsXlsxXlsx = "XLS Documents (.xlsx)|*.xlsx";
        private IPredictionMatrixWriter _predictionMatrixWriter { get; set; }
        private IAttributeWriter _attributeWriter { get; set; }
        private ISaveFileDialog _newSFD { get; set; }
        public IDataObjectsConverter _dataObjectsConverter { get; }
        public ExcelWriter(
            IPredictionMatrixWriter predictionMatrixWriter,
            IAttributeWriter attributeWriter,
            ISaveFileDialog sfd,
            IDataObjectsConverter dataObjectsConverter)
        {
            _predictionMatrixWriter = predictionMatrixWriter;
            _attributeWriter = attributeWriter;
            _newSFD = sfd;
            _dataObjectsConverter = dataObjectsConverter;
        }
        public Result<bool> ExportToExcel(string fileDataFileName, CoverSampleResult coverSample)
        {
            var coverMatrix = coverSample.CoverResult;

            var suffix = $"_SLOW_{coverSample.SLOW}_SHIGH_{coverSample.SHIGH}_LOW_{coverMatrix.DataMatrix.LOW}_HIGH_{coverMatrix.DataMatrix.HIGH}_STEP_{coverSample.STEP}_METHOD_{coverSample.SelecteMethod.Replace(' ', '_').ToUpper()}_PARAM_{coverSample.SelecteMethodParam}_GRADE_{coverSample.CoverResult.Grade}";

            var sfdResult = SaveFileDialog(fileDataFileName, suffix);

            string fileName;
            if (sfdResult.Value == true)
            {
                fileName = _newSFD.FileName;
            }
            else
            {
                return new Result<bool>(NieZapisanoPliku);
            }

            var wbResult = CreateXlWorkbook();
            if (wbResult.HasErrors())
            {
                return new Result<bool>(wbResult.Error);
            }
            var wb = wbResult.Value;


            var datatable =
                _dataObjectsConverter.ConvertToDataTable(coverMatrix.DataObjects.Concat(coverMatrix.TestObjects).ToArray());
            var result = AddDataTableToExcell(datatable, wb, Obiekty);
            if (result.HasErrors())
            {
                return result;
            }

            result = AddRaportToExcel(coverSample, wb);
            if (result.HasErrors())
            {
                return result;
            }

            result = AddDataTableToExcell(coverMatrix.DataMatrix.DataTable, wb, MacierzPredykcjiTreningowe);
            if (result.HasErrors())
            {
                return result;
            }

            result = _attributeWriter.AddAttributes(wb, coverMatrix);
            if (result.HasErrors())
            {
                return result;
            }

            result = AddDataTableToExcell(coverMatrix.TestMatrix.DataTable, wb, MacierzPredykcjiTestowe);
            if (result.HasErrors())
            {
                return result;
            }

            result = SaveAndOpenFile(wb, fileName);
            if (result.HasErrors())
            {
                return result;
            }

            return new Result<bool>(true);
        }

        private static Result<XLWorkbook> CreateXlWorkbook()
        {
            try
            {
                XLWorkbook wb = new XLWorkbook();
                return new Result<XLWorkbook>(wb);
            }
            catch (Exception e)
            {
                return new Result<XLWorkbook>(e);
            }
        }

        private static Result<bool> SaveAndOpenFile(XLWorkbook wb, string fileName)
        {
            try
            {
                wb.SaveAs(fileName);
                System.Diagnostics.Process.Start(fileName);
            }
            catch (Exception e)
            {
                return new Result<bool>(e);
            }
            return new Result<bool>(true);
        }

        private Result<bool> AddDataTableToExcell(DataTable dataTable, XLWorkbook wb, string title)
        {
            var result = _predictionMatrixWriter.AddMatrix(wb, dataTable, title);
            return result;
        }

        private Result<bool> AddRaportToExcel(CoverSampleResult coverMatrix, XLWorkbook wb)
        {
            Result<bool> result;
            result = _attributeWriter.AddRaport(wb, coverMatrix);
            return result;
        }
        private Result<bool?> SaveFileDialog(string fileDataFileName, string suffix)
        {
            try
            {
                _newSFD.FileName = fileDataFileName + suffix;
                _newSFD.FileName = _newSFD.FileName.Replace('.', '_').ToUpper().Replace('-', '_').Replace(',', '_').Replace('.', '_')
                    .Replace('%', '_').Replace('-', '_');
                string invalid = _newSFD.FileName;

                string _Illegal;

                foreach (char c in invalid)
                {
                    _Illegal = Illegal.Replace(c.ToString(), "");
                }

                _newSFD.FileName = invalid;
                _newSFD.DefaultExt = NewSfdDefaultExt;
                _newSFD.Filter = XlsDocumentsXlsxXlsx;
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
