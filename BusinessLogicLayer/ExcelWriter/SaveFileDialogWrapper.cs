using Core.Common.Interfaces;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace BusinessLogicLayer.ExcelWriter
{
    public class SaveFileDialogWrapper : ISaveFileDialog
    {
        private readonly SaveFileDialog _sfd;
        public SaveFileDialogWrapper()
        {
            _sfd = new SaveFileDialog();
        }
        public string FileName
        {
            get { return _sfd.FileName;}
            set                                     
                {
                    if (value != null) _sfd.FileName = value;
                }
            }

        public string Filter
        {
            get { return _sfd.Filter; }
            set
            {
                if (value != null) _sfd.Filter = value;
            }
        }

        public string DefaultExt
        {
            get { return _sfd.DefaultExt; }
            set
            {
                if (value != null) _sfd.DefaultExt = value;
            }
        }

        public bool? ShowDialog()
        {
            return _sfd.ShowDialog();
        }
    }
}
