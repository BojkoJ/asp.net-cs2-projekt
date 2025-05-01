using System;

namespace BOJ0043_Web.Infrastructure
{
    /// <summary>
    /// Custom attribute for specifying request/response schema type for API documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiSchemaAttribute : Attribute
    {
        public Type RequestType { get; }
        public Type ResponseType { get; }

        public ApiSchemaAttribute(Type responseType)
        {
            ResponseType = responseType;
        }

        public ApiSchemaAttribute(Type requestType, Type responseType)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }
    }
}
