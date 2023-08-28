using AutoMapper;
using Dodo1000Bot.Models;
using Telegram.Bot.Types;

namespace Dodo1000Bot.Messengers.Telegram
{
    public class TelegramMapping : Profile
    {
        public TelegramMapping()
        {
            CreateMap<Update, Request>()
            .ForMember(d => d.ChatHash, m => m.MapFrom((s, _) => (s.Message?.Chat?.Id).GetValueOrDefault()))
            .ForMember(d => d.UserHash, m => m.MapFrom((s, _) => (s.Message?.From?.Id).GetValueOrDefault()))
            .ForMember(d => d.SessionId, m => m.MapFrom((s, _) => (s.Message?.From?.Id).GetValueOrDefault()))
            .ForMember(d => d.Text, m => m.MapFrom((s, _) => s.Message?.Text))
            .ForMember(d => d.Source, m => m.MapFrom(s => Source.Telegram))
            .ForMember(d => d.Appeal, m => m.MapFrom(s => Appeal.NoOfficial))
            .ForMember(d => d.HasScreen, m => m.MapFrom(s => true))
            .ForMember(d => d.Language, m => m.Ignore())
            .ForMember(d => d.NewSession, m => m.Ignore())
            .ForMember(d => d.Action, m => m.Ignore());

            CreateMap<MessageEntity, FormattingEntity>()
                .ForMember(d => d.Offset, m => m.MapFrom((s, _) => s.Offset))
                .ForMember(d => d.Length, m => m.MapFrom((s, _) => s.Length))
                .ForMember(d => d.Type, m => m.MapFrom((s, _) => s.Type));
        }
    }
}
