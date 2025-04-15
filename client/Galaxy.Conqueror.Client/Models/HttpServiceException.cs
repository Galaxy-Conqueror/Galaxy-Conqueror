using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Conqueror.Client.Models
{
    public class HttpServiceException: Exception
    {
        public HttpServiceException()
        {
        }

        public HttpServiceException(string message) : base(message)
        {
        }

        public HttpServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
