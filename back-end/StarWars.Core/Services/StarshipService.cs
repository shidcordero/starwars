using StarWarsApiCSharp;
using System;
using System.Globalization;
using System.Linq;

namespace StarWars.Core.Services
{
    public class StarshipService : IStarshipService
    {
        private readonly IRepository<Models.Starship> _starshipRepository;
        private readonly IRepository<Person> _personRepository;

        public StarshipService(IRepository<Models.Starship> starshipRepository, IRepository<Person> personRepository)
        {
            _starshipRepository = starshipRepository;
            _personRepository = personRepository;
        }

        public IQueryable<Models.Starship> GetStarships(int size)
        {
            var starships = _starshipRepository.GetEntities(size: size).ToList();

            // this will be slow but its the limitation of the API
            starships.ForEach(s =>
            {
                if (s.Pilots.Any())
                {
                    foreach (var pilot in s.Pilots)
                    {
                        var person = _personRepository.GetById(GetId(pilot));

                        s.PilotList.Add(person);
                    }
                }
            });

            return starships.AsQueryable();
        }

        private int GetId(string url)
        {
            var secondSlash = url.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
            var firstSlash = url.LastIndexOf("/", secondSlash - 1, StringComparison.OrdinalIgnoreCase);
            var lengthOfSubstring = (secondSlash - firstSlash) - 1;
            string stringId = url.Substring(firstSlash + 1, lengthOfSubstring);

            int result = int.Parse(stringId, CultureInfo.InvariantCulture);
            return result;
        }
    }
}