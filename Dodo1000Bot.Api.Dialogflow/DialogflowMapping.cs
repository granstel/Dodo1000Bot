using AutoMapper;
using Dodo1000Bot.Api.Dialogflow.Models;
using Dodo1000Bot.Models;

namespace Dodo1000Bot.Api.Dialogflow;

public class DialogflowMapping : Profile
{
    public DialogflowMapping()
    {
        CreateMap<FulfillmentRequest, Request>()
            .ForMember(d => d.ChatHash, m => m.MapFrom((s, d) => s.OriginalDetectIntentRequest?.Payload?.Data?.Chat?.Id))
            .ForMember(d => d.UserHash, m => m.MapFrom((s, d) => s.OriginalDetectIntentRequest?.Payload?.Data?.From?.Id))
            .ForMember(d => d.SessionId, m => m.MapFrom((s, d) => s.OriginalDetectIntentRequest.Payload.Data?.From?.Id))
            .ForMember(d => d.Text, m => m.MapFrom((s, d) => s.QueryResult.Action))
            .ForMember(d => d.Source, m => m.MapFrom(s => s.OriginalDetectIntentRequest.Source))
            .ForMember(d => d.Appeal, m => m.MapFrom(s => Appeal.NoOfficial))
            .ForMember(d => d.HasScreen, m => m.MapFrom(s => true))
            .ForMember(d => d.Language, m => m.Ignore())
            .ForMember(d => d.NewSession, m => m.Ignore());
    }
}