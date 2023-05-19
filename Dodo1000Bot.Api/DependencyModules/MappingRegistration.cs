using System.Collections.Generic;
using AutoMapper;
using Dodo1000Bot.Services.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Dodo1000Bot.Api.DependencyModules
{
    internal static class MappingRegistration
    {
        internal static void AddMapping(this IServiceCollection services, IEnumerable<string> names)
        {
            services.AddSingleton<IMapper>(p => new Mapper(new MapperConfiguration(c =>
            {
                c.AddMaps(names);

                c.AddProfile<InternalMapping>();
                c.AddProfile<DialogflowMapping>();
            })));
        }
    }
}
