namespace LTP.DataModels
{
    public class PersonSearchResult : Person
    {
        public string StateCode { get; set; }
    }

    public class ExecuteResult<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}