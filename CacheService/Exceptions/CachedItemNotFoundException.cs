using System;

namespace CacheService.Exceptions
{
    public class CachedItemNotFoundException : Exception
    {
        public CachedItemNotFoundException() : base() { }

        public CachedItemNotFoundException(string message) : base(message) { }

        public CachedItemNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
