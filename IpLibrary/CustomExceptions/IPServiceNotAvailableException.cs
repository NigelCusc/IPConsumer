using System;

namespace IPLibrary.CustomExceptions
{
    public class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException(string message) : base(message)
        {

        }
    }
}
