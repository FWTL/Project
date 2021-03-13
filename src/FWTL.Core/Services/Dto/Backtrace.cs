namespace FWTL.Core.Services.Dto
{
    public class Backtrace
    {
        public string File { get; set; }

        public int Line { get; set; }

        public string Function { get; set; }

        public string Class { get; set; }

        public string Type { get; set; }

        public object Args { get; set; }
    }
}