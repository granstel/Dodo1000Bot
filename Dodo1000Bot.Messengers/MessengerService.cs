﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dodo1000Bot.Models;
using Dodo1000Bot.Services;
using Microsoft.Extensions.Logging;

namespace Dodo1000Bot.Messengers
{
    public abstract class MessengerService<TInput, TOutput> : IMessengerService<TInput, TOutput>
    {
        protected readonly ILogger Log;

        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;

        protected MessengerService(ILogger log, IConversationService conversationService, IMapper mapper)
        {
            Log = log;
            _conversationService = conversationService;
            _mapper = mapper;
        }

        protected virtual Request Before(TInput input)
        {
            var request = _mapper.Map<Request>(input);

            return request;
        }

        public virtual async Task<TOutput> ProcessIncomingAsync(TInput input, CancellationToken cancellationToken)
        {
            Response response;

            try
            {
                var request = Before(input);

                response = await ProcessCommand(request, cancellationToken);

                if (response == null)
                {
                    response = await _conversationService.GetResponseAsync(request, cancellationToken);
                }

                _mapper.Map(request, response);
            }
            catch (Exception e)
            {
                Log.LogError(e, "Error while processing incoming message");

                response = new Response
                {
                    Text = "Прости, у меня какие-то проблемы... Давай попробуем ещё раз"
                };
            }

            var output = await AfterAsync(input, response);

            return output;
        }

        protected virtual Task<Response> ProcessCommand(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult<Response>(null);
        }

        protected virtual async Task<TOutput> AfterAsync(TInput input, Response response)
        {
            return await Task.Run(() =>
            {
                var output = _mapper.Map<TOutput>(response);

                return output;
            });
        }

        public virtual Task<bool> SetWebhookAsync(string url, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> DeleteWebhookAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
