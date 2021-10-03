using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;
using System;
using System.Linq;
using System.Net;

namespace StarWars.Core.GraphQL
{
    public class GraphqlHttpResultSerializer : DefaultHttpResultSerializer
    {
        public override HttpStatusCode GetStatusCode(IExecutionResult result)
        {
            if (result is IQueryResult queryResult && queryResult.Errors?.Count > 0)
            {
                var error = queryResult.Errors.FirstOrDefault();
                if (error.Code == null)
                {
                    return base.GetStatusCode(result);
                }

                if (error.Code == "AUTH_NOT_AUTHENTICATED")
                {
                    return HttpStatusCode.Unauthorized;
                }

                try
                {
                    Enum.TryParse(queryResult.Errors[0].Code, true, out ErrorCode code);
                    return code.GetHttpStatusCode();
                }
                catch (ArgumentException)
                {
                    return base.GetStatusCode(result);
                }
            }

            return base.GetStatusCode(result);
        }
    }
}