using System.Collections.Generic;

namespace CoreTail.Shared
{
    public class OpenFileDialogSettings
    {
        public string Title { get; set; }

        public string InitialDirectory { get; set; }

        public List<FileDialogFilter> Filters { get; set; }

        public string InitialFileName { get; set; }

        public bool AllowMultiple { get; set; }
    }
}
