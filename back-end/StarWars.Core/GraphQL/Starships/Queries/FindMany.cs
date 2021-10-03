using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using StarWars.Core.Services;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.Core.GraphQL.StarWars
{
    public partial class StarWarsQueries
    {
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public async Task<IQueryable<Models.Starship>> Starships(
            [Service] IStarshipService starshipService
        )
        {
            return starshipService.GetStarships(size: int.MaxValue);
        }
    }
}