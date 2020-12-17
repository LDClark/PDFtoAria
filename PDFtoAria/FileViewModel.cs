using System;

namespace PDFtoAria
{
    public class FileViewModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsSelected { get; set; }
    }
}
