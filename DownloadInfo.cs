namespace PowerShellStart
{
    class DownloadInfo
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public string CheckOutState { get; set; }
        public bool IsCheckOut { get; set; }
        public string CheckOutPC { get; set; }
        public string EditedBy { get; set; }
        public string ErrorState { get; set; }
        public int ExitCode { get; set; }
    }
}
