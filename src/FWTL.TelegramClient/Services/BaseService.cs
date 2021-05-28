using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FWTL.Core.Services.Telegram.Dto;
using FWTL.TelegramClient.Converters;
using Microsoft.AspNetCore.WebUtilities;

namespace FWTL.TelegramClient.Services
{
    public class BaseService
    {
        private readonly HttpClient _client;

        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new GetDialogsConverter(), new DateConverterFactory() }
        };

        public BaseService(HttpClient client)
        {
            _client = client;
        }

        protected Task<ResponseWrapper<TResponse>> HandleAsync<TResponse>(string url)
        {
            return HandleAsync<TResponse>(url, new Dictionary<string, string>());
        }

        protected async Task<ResponseWrapper<TResponse>> HandleAsync<TResponse>(string url, IDictionary<string, string> query)
        {
            url = QueryHelpers.AddQueryString(url, query);
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                return new ResponseWrapper<TResponse>()
                {
                    IsSuccess = false,
                    Errors = new[]
                    {
                        new Error() {Message = response.StatusCode + " " + await response.Content.ReadAsStringAsync()}
                    }
                };
            }

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ResponseWrapper<TResponse>>(responseStream, SerializeOptions);
        }

        protected Task<ResponseWrapper> HandleAsync(string url)
        {
            return HandleAsync(url, new Dictionary<string, string>());
        }

        protected async Task<ResponseWrapper> HandleAsync(string url, IDictionary<string, string> query)
        {
            url = QueryHelpers.AddQueryString(url, query);
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                return new ResponseWrapper()
                {
                    IsSuccess = false,
                    Errors = new[]
                        {new Error() {Message = response.StatusCode + " " + await response.Content.ReadAsStringAsync()}}
                };
            }

            await using Stream responseStream = await response.Content.ReadAsStreamAsync();
            var wrapper = await JsonSerializer.DeserializeAsync<ResponseWrapper>(responseStream, SerializeOptions);

            if (wrapper.Errors.Any(x => x.Message == "Session not found"))
            {
                wrapper.NotFound = true;
            }

            return wrapper;
        }
    }
}