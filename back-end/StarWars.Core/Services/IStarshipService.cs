using System.Linq;

namespace StarWars.Core.Services
{
    public interface IStarshipService
    {
        IQueryable<Models.Starship> GetStarships(int size);
    }
}