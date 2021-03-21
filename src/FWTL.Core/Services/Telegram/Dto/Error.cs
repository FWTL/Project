using System.Collections.Generic;

namespace FWTL.Core.Services.Dto
{
    public class Error
    {
        public string Exception { get; set; }

        public string Message { get; set; }

        public string File { get; set; }

        public int Line { get; set; }

        public int Code { get; set; }

        public IEnumerable<Backtrace> Backtrace { get; set; } = new List<Backtrace>();
    }
}