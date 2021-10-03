using FluentChoco;
using StarWars.Core;
using StarWars.Core.GraphQL;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace StarWars.API
{
    public partial class Startup
    {
        private static void ConfigureHotChocolate(IServiceCollection services)
        {
            if (services != null)
            {
                var graphql = services
                    .AddHttpResultSerializer<GraphqlHttpResultSerializer>()
                    .AddGraphQLServer()
                    .AddAuthorization()
                    .UseFluentValidation();

                var core = Assembly.Load("StarWars.Core");
                var mutations = core
                    .GetTypes()
                    .Where(type =>
                        type.GetCustomAttributes()
                            .Any(attribute => attribute is ExtendObjectTypeAttribute extention && extention.Name == "Mutation")
                    )
                    .ToList();

                var queries = core
                    .GetTypes()
                    .Where(type =>
                        type.GetCustomAttributes()
                            .Any(attribute => attribute is ExtendObjectTypeAttribute extention && extention.Name == "Query")
                    )
                    .ToList();

                var subscriptions = core
                    .GetTypes()
                    .Where(type =>
                        type.GetCustomAttributes()
                            .Any(attribute => attribute is ExtendObjectTypeAttribute extention && extention.Name == "Subscription")
                    )
                    .ToList();

                var objectTypes = core
                    .GetTypes()
                    .Where(type =>
                        type.IsSubclassOf(typeof(ObjectType))
                    )
                    .ToList();

                if (mutations.Any())
                {
                    graphql.AddMutationType(d => d.Name("Mutation"));
                    foreach (var mutation in mutations)
                    {
                        graphql.AddTypeExtension(mutation);
                    }
                }

                if (queries.Any())
                {
                    graphql.AddQueryType(d => d.Name("Query"));
                    foreach (var query in queries)
                    {
                        graphql.AddTypeExtension(query);
                    }
                }

                if (subscriptions.Any())
                {
                    graphql.AddSubscriptionType(d => d.Name("Subscription"));
                    foreach (var subscription in subscriptions)
                    {
                        graphql.AddTypeExtension(subscription);
                    }
                }

                if (objectTypes.Any())
                {
                    foreach (var objectType in objectTypes)
                    {
                        graphql.AddType(objectType);
                    }
                }

                graphql
                    .AddProjections()
                    .AddSpatialTypes()
                    .AddSpatialProjections()
                    .AddFiltering()
                    .AddSorting()
                    .SetPagingOptions(
                        new PagingOptions()
                        {
                            MaxPageSize = StaticData.Settings.MaxPageSize,
                            DefaultPageSize = StaticData.Settings.DefaultPageSize,
                            IncludeTotalCount = true
                        }
                    )
                    .ConfigureSchema(s => { })
                    .EnableRelaySupport();
            }
        }
    }
}