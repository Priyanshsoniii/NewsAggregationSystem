using System.Runtime.Serialization;

namespace NewsAggregation.Server.Exceptions
{
    [Serializable]
    public class NewsApiException : Exception
    {
        public NewsApiException() : base() { }
        public NewsApiException(string message) : base(message) { }
        public NewsApiException(string message, Exception innerException) : base(message, innerException) { }
        protected NewsApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class NewsApiConfigurationException : NewsApiException
    {
        public NewsApiConfigurationException() : base() { }
        public NewsApiConfigurationException(string message) : base(message) { }
        public NewsApiConfigurationException(string message, Exception innerException) : base(message, innerException) { }
        protected NewsApiConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
} 