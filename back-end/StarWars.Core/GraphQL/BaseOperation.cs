using HotChocolate;
using HotChocolate.Execution;
using System;

namespace StarWars.Core.GraphQL
{
    public class BaseOperation
    {
        protected QueryException CreateError(ErrorCode code, string message)
        {
            return new QueryException(
                ErrorBuilder.New()
                    .SetMessage(message)
                    .SetCode(Enum.GetName(typeof(ErrorCode), code))
                    .Build()
            );
        }
    }
}