using System.Collections.Generic;

namespace Core.API.DTOs
{
    public class ResponseDownload
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public IDictionary<string, string> UrlAndNames { get; set; }
    }
}
