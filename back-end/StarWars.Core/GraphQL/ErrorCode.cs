using System;
using System.Net;
using System.Reflection;

namespace StarWars.Core.GraphQL
{
    public enum ErrorCode
    {
        [ErrorCodeAttribute(HttpStatusCode.Unauthorized)] Unauthorised,
        [ErrorCodeAttribute(HttpStatusCode.Conflict)] DataExists,
        [ErrorCodeAttribute(HttpStatusCode.BadRequest)] DataRequired,
        [ErrorCodeAttribute(HttpStatusCode.NotFound)] NotFound,
        [ErrorCodeAttribute(HttpStatusCode.BadRequest)] BadInput,
    }

    [AttributeUsage(System.AttributeTargets.Field)]
    internal sealed class ErrorCodeAttribute : Attribute
    {
        public HttpStatusCode Code { get; private set; }

        internal ErrorCodeAttribute(HttpStatusCode code)
        {
            this.Code = code;
        }
    }

    public static class ErrorCodes
    {
        private static ErrorCodeAttribute GetAttribute(ErrorCode type)
        {
            return (ErrorCodeAttribute)Attribute.GetCustomAttribute(ForValue(type), typeof(ErrorCodeAttribute));
        }

        private static MemberInfo ForValue(ErrorCode type)
        {
            return typeof(ErrorCode).GetField(Enum.GetName(typeof(ErrorCode), type));
        }

        public static HttpStatusCode GetHttpStatusCode(this ErrorCode type)
        {
            ErrorCodeAttribute attr = GetAttribute(type);
            return attr.Code;
        }
    }
}