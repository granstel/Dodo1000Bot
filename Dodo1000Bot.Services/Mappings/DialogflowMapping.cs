﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf.WellKnownTypes;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services.Extensions;

namespace Dodo1000Bot.Services.Mappings
{
    public class DialogflowMapping : Profile
    {
        public DialogflowMapping()
        {
            CreateMap<QueryResult, Dialog>()
                .ForMember(d => d.Response, m => m.MapFrom(s => s.FulfillmentText))
                .ForMember(d => d.AllRequiredParamsPresent, m => m.MapFrom(s => s.AllRequiredParamsPresent))
                .ForMember(d => d.Action, m => m.MapFrom(s => s.Action))
                .ForMember(d => d.Parameters, m => m.MapFrom(s => GetParameters(s)))
                .ForMember(d => d.Buttons, m => m.MapFrom(s => GetButtons(s)))
                .ForMember(d => d.Payload, m => m.MapFrom(s => GetPayload(s)))
                .ForMember(d => d.EndConversation, m => m.MapFrom((s, _) =>
                {
                    const string endConversationKey = "end_conversation";

                    var endConversationAtDiagnosticInfo = s.DiagnosticInfo?.Fields?
                        .Where(f => string.Equals(f.Key, endConversationKey, StringComparison.InvariantCultureIgnoreCase))
                        .Select(f => f.Value.BoolValue).FirstOrDefault();

                    var endConversationAtAction = !string.IsNullOrEmpty(s.Action) && string.Equals(s.Action, endConversationKey, StringComparison.InvariantCultureIgnoreCase);

                    return endConversationAtDiagnosticInfo ?? endConversationAtAction;
                }));
        }

        private IDictionary<string, ICollection<string>> GetParameters(QueryResult queryResult)
        {
            var dictionary = new Dictionary<string, ICollection<string>>();

            var fields = queryResult?.Parameters?.Fields;

            if (fields?.Any() != true)
            {
                return dictionary;
            }

            foreach (var field in fields)
            {
                if (field.Value.KindCase == Value.KindOneofCase.StringValue)
                {
                    dictionary.Add(field.Key, new[] { field.Value.StringValue });
                }
                else if (field.Value.KindCase == Value.KindOneofCase.StructValue)
                {
                    var stringValues = fields
                        .Where(valueField => valueField.Value.KindCase == Value.KindOneofCase.StringValue)
                        .Select(valueField => valueField.Value.StringValue)
                        .Where(value => !string.IsNullOrEmpty(value)).ToList();

                    dictionary.Add(field.Key, stringValues);
                }
            }

            return dictionary;
        }

        private Button[] GetButtons(QueryResult s)
        {
            var buttons = new List<Button>();

            var quickReplies = s?.FulfillmentMessages
                ?.Where(IsQuickReplies)
                .SelectMany(m => m.QuickReplies.QuickReplies_.Select(r => new Button
                {
                    Text = r,
                    IsQuickReply = true
                })).Where(r => r != null).ToList() ?? buttons;

            var cards = s?.FulfillmentMessages
                ?.Where(IsCard)
                .SelectMany(m => m.Card.Buttons.Select(b => new Button
                {
                    Text = b.Text,
                    Url = b.Postback
                })).Where(b => b != null).ToList() ?? buttons;

            buttons.AddRange(quickReplies);
            buttons.AddRange(cards);

            return quickReplies.ToArray();
        }

        private bool IsQuickReplies(Intent.Types.Message message)
        {
            return message.MessageCase == Intent.Types.Message.MessageOneofCase.QuickReplies;
        }

        private bool IsCard(Intent.Types.Message message)
        {
            return message.MessageCase == Intent.Types.Message.MessageOneofCase.Card;
        }

        private Payload GetPayload(QueryResult queryResult)
        {
            var payload = queryResult?.FulfillmentMessages?
                .Where(m => m.MessageCase == Intent.Types.Message.MessageOneofCase.Payload)
                .Select(m => m.Payload.ToString().Deserialize<Payload>()).FirstOrDefault();

            return payload;
        }
    }
}
