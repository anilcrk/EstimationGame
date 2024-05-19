namespace EstimationGame.Result
{
    public class ServiceResult
    {
        public ServiceResult()
        {
        }

        public ServiceResult(bool status, string explanation)
        {
            Status = status;
            Explanation = explanation;
        }

        public bool Status { get; set; }
        public string Explanation { get; set; }
    }

    public class ServiceResultExt<T> : ServiceResult
    {
        public T ResultObject { get; set; }
    }
}
