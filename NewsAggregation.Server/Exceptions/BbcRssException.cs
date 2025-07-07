using System.Runtime.Serialization;

namespace NewsAggregation.Server.Exceptions
{
    [Serializable]
    public class BbcRssException : Exception
    {
        public BbcRssException() : base() { }
        public BbcRssException(string message) : base(message) { }
        public BbcRssException(string message, Exception innerException) : base(message, innerException) { }
        protected BbcRssException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }

    [Serializable]
    public class BbcRssConfigurationException : BbcRssException
    {
        public BbcRssConfigurationException() : base() { }
        public BbcRssConfigurationException(string message) : base(message) { }
        public BbcRssConfigurationException(string message, Exception innerException) : base(message, innerException) { }
        protected BbcRssConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
} 