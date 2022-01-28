namespace PowerShellStart
{
    class DownloadInfo
    {
        public bool Success { get; set; }
        public string Filename { get; set; }
        public string FullFilename { get; set; }
        public bool IsCheckOut { get; set; }
        public string CheckOutBy { get; set; }
        public int ExitCode { get; set; }
    }
}
