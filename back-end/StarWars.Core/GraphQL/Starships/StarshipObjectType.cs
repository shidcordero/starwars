using HotChocolate.Types;
using StarWarsApiCSharp;
using System.Linq;
using System.Threading.Tasks;

namespace StarWars.Core.GraphQL.StarWars
{
    public class StarWarsObjectType : ObjectType<Models.Starship>
    {
        protected override void Configure(IObjectTypeDescriptor<Models.Starship> descriptor)
        {
            descriptor
                .ImplementsNode()
                .IdField(t => t.Url)
                .ResolveNode((context, url) =>
                {
                    var repository = context.Service<IRepository<Models.Starship>>();
                    return Task.FromResult(repository.GetEntities(size: int.MaxValue).FirstOrDefault(x => x.Url == url));
                });
        }
    }
}