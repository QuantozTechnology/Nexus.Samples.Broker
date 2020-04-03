using System.Collections.Generic;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public Dictionary<string, string> FilteringParameters { get; set; }
        public T[] Records { get; set; }
    }
}
