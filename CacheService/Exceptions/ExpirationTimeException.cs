using System;

namespace CacheService.Exceptions
{
    public class ExpirationTimeException : Exception
    {
        public ExpirationTimeException() : base() { }

        public ExpirationTimeException(string message) : base(message) { }

        public ExpirationTimeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
