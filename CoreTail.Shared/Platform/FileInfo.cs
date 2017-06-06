namespace CoreTail.Shared.Platform
{
    // used in WPF and Avalonia (not in UWP)
    public class FileInfo : IFileInfo
    {
        public FileInfo(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}
