namespace BHHC.UW.PolicyCenter.API.PolicyCenterException
{
    public class PolicyCenterException
    {
    }
    //create another class HandleException
    public class HandledException : Exception
    {
        public HandledException(string message) : base(message)
        {
        }
        public HandledException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public class BusinessLogicException : Exception
        {
            public BusinessLogicException(string message) : base(message)
            {
            }
            public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
        public class WebApiExceptionResponseModel
        {
            public string ExceptionType { get; set; }
            public string ExceptionMessage { get; set; }
        }
    }
}
