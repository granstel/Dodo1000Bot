using AutoMapper;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Services.Mappings
{
    public class InternalMapping : Profile
    {
        public InternalMapping()
        {
            CreateMap<Request, Response>()
                .ForMember(d => d.ChatHash, m => m.MapFrom(s => s.ChatHash))
                .ForMember(d => d.UserHash, m => m.MapFrom(s => s.UserHash))
                .ForMember(d => d.Text, m => m.Ignore())
                .ForMember(d => d.AlternativeText, m => m.Ignore())
                .ForMember(d => d.Finished, m => m.Ignore())
                .ForMember(d => d.Buttons, m => m.Ignore());
        }
    }
}
