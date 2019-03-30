using System;
using System.Linq;
using Core.Common.Helpers;
using Core.Common.Interfaces;
using Core.Common.Items;

namespace BusinessLogicLayer.Reader
{
    public class FileReaderProvider : IFileReaderProvider
    {
        private readonly IFileChecker _fileChecker;
        private readonly IFileReader _fileReader;
        private readonly IOpenFileDialog _openFileDialog;
        private readonly IDataReader _dataReader;
        private readonly IAttributeColumnConverter _attributeColumnConverter;
        private readonly ITrainingObjectsConverter _trainingObjectsConverter;
        public FileReaderProvider
            (IOpenFileDialog openFileDialog,
            IFileChecker fileChecker,
            IFileReader fileReader,
            IDataReader dataReader,
            IAttributeColumnConverter attributeColumnConverter,
            ITrainingObjectsConverter trainingObjectsConverter)
        {
            _openFileDialog = openFileDialog;
            _fileChecker = fileChecker;
            _fileReader = fileReader;
            _dataReader = dataReader;
            _attributeColumnConverter = attributeColumnConverter;
            _trainingObjectsConverter = trainingObjectsConverter;
        }

        private bool IsFileHasAppriopriateExtension(string path, string dataFilter)
        {
            var isFileHasAppriopriateExtension = _fileChecker.IsFileHasAppriopriateExtension(path, dataFilter);
            return isFileHasAppriopriateExtension;

        }
        private Result<InputData> GetDataModel(string[] rawData)
        {
            var dataModel = _dataReader.GetDataModel(rawData);

            if (dataModel.HasErrors())
            {
                dataModel.GetError();
            }
            return dataModel;

        }

        private DataObject[] ConvertRows2DataObjects(
            AttributeDescription[] attributes,
            string[] observations, 
            long startId = 0)
        {
            return _trainingObjectsConverter.ConvertRows2DataObjects(attributes, observations, startId);

        }
        private Result<bool> OpenFileDialog(string fileFilter)
        {
            _openFileDialog.Filter = fileFilter;
            bool? isOkClicked = _openFileDialog.ShowDialog();
            return new Result<bool>(isOkClicked != null && isOkClicked.Value);

        }
        private Result<FileData> FillRawData(FileData fileData)
        {
                var joinedSring = string.Empty;
                if (fileData.RawData != null)
                    joinedSring = fileData.RawData.Aggregate(joinedSring,
                        (current, nextRow) => current + nextRow + Environment.NewLine);

                var newLineSignLength = Environment.NewLine.Length;
                joinedSring = joinedSring.Remove(joinedSring.Length - newLineSignLength, newLineSignLength);
                fileData.JoinedSring = joinedSring;
                return new Result<FileData>(fileData);
        }

        public Result<FileData> ReadFile(string fileFilter)
        {
            var result = new FileData();
            try
            {
                var opf = OpenFileDialog(fileFilter);

                if (opf.Value != true)
                {
                    return new Result<FileData>((FileData) null);
                }

                var isFileExsist = _fileChecker.IsFileExsist(_openFileDialog.FileName); 
                var isFileHasAppriopriateExtension = IsFileHasAppriopriateExtension(_openFileDialog.FileName, fileFilter);
                result.FileName = _openFileDialog.SafeFileName;
                result.RawData = _fileReader.GetFileContent(_openFileDialog.FileName);
                var dataModel = GetDataModel(result.RawData);
                result.Observations = dataModel.Value.Rows;
                result.DataDescription = dataModel.Value.Columns;
                result.Attributes = _attributeColumnConverter.ConvertColumns2Attributes(result.DataDescription).ToArray();  
                var length = result.Observations.Length;
                var dataObject = result.Observations.Take(length * 2 / 3).ToArray();
                var testObjects = result.Observations.Except(dataObject).ToArray();
                DataObjectHelper.SchemeObject = null;
                result.DataObjects = ConvertRows2DataObjects(result.Attributes, dataObject);
                result.TestObjects = ConvertRows2DataObjects(result.Attributes, testObjects, result.DataObjects.Max(x => x.Id +1));
                return FillRawData(result);
            }
            catch (Exception e)
            {
                return new Result<FileData>(e);
            }
        }

    }
}
