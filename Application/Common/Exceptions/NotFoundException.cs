using System;
using System.Net;
using System.Runtime.Serialization;

namespace Application.Common.Exceptions
{

    [Serializable]
    public sealed class NotFoundException : Exception
    {
        public NotFoundException()
        {

        }

        public NotFoundException(string name, string fieldName, object key)
        {
            throw new ResponseException(HttpStatusCode.NotFound, $"{fieldName} > ({key}) was not found in [{name}] entity.");
        }

        private NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }

    }
}
