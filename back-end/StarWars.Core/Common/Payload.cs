using System.Collections.Generic;

namespace StarWars.Core.Common
{
    public abstract class Payload
    {
        protected Payload(IReadOnlyList<UserError> errors = null)
        {
            Errors = errors;
        }

        public IReadOnlyList<UserError> Errors { get; }
    }
}