using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Slim.Exceptions
{
    public class DependencyInjectionException : Exception
    {
        public DependencyInjectionException()
        {
        }

        public DependencyInjectionException(string message) : base(message)
        {
        }

        public DependencyInjectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DependencyInjectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
