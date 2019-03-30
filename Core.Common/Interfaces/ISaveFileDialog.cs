namespace Core.Common.Interfaces
{
    public interface ISaveFileDialog
    {
        string FileName { get; set; }
        string Filter { get; set; }
        string DefaultExt { get; set; }
        bool? ShowDialog();
    }
}
