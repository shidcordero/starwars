using HotChocolate;
using HotChocolate.Types.Descriptors;
using System.Globalization;

namespace StarWars.Core.GraphQL
{
    public class PlatformNamingConventions : DefaultNamingConventions
    {
        public override NameString GetEnumValueName(object value)
        {
            var input = value.ToString();
            if (input == null)
            {
                return base.GetEnumValueName(value);
            }

            return input.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}