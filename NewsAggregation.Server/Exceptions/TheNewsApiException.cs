using System.Runtime.Serialization;

namespace NewsAggregation.Server.Exceptions
{
    /// <summary>
    /// Base exception for The News API errors
    /// </summary>
    [Serializable]
    public class TheNewsApiException : Exception
    {
        public TheNewsApiException() : base() { }

        public TheNewsApiException(string message) : base(message) { }

        public TheNewsApiException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected TheNewsApiException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when The News API configuration is invalid
    /// </summary>
    [Serializable]
    public class TheNewsApiConfigurationException : TheNewsApiException
    {
        public TheNewsApiConfigurationException() : base() { }

        public TheNewsApiConfigurationException(string message) : base(message) { }

        public TheNewsApiConfigurationException(string message, Exception innerException) 
            : base(message, innerException) { }

        protected TheNewsApiConfigurationException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }

    /// <summary>
    /// Exception thrown when The News API returns an error response
    /// </summary>
    [Serializable]
    public class TheNewsApiResponseException : TheNewsApiException
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public TheNewsApiResponseException(string message, int statusCode, string errorCode = "") 
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public TheNewsApiResponseException(string message, int statusCode, string errorCode, Exception innerException) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        protected TheNewsApiResponseException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
            StatusCode = info.GetInt32(nameof(StatusCode));
            ErrorCode = info.GetString(nameof(ErrorCode)) ?? string.Empty;
        }

        [Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(StatusCode), StatusCode);
            info.AddValue(nameof(ErrorCode), ErrorCode);
        }
    }
} 