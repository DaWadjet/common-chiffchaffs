using System;
using System.Runtime.Serialization;

namespace CSONGE.Application.Exceptions
{
    public class ApplicationExeption : Exception
    {
        public ApplicationExeption()
        {
        }

        public ApplicationExeption(string message) : base(message)
        {
        }

        public ApplicationExeption(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApplicationExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
