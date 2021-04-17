using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.CustomExceptions
{
    public class DuplicateIPAddressException : Exception
    {
        public DuplicateIPAddressException(string message) : base(message)
        {

        }
    }
}
