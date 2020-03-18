namespace Nexus.Samples.Sdk.Models.Response
{
    public class DefaultResponseTemplate<T>
    {
        public string Message { get; set; }
        public string[] Errors { get; set; }
        public T Values { get; set; }
        public bool IsSuccess => Errors == null || Errors.Length == 0;
    }
}
