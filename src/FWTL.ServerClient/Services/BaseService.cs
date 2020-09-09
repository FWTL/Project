using FWTL.TelegramClient.Converters;
using FWTL.TelegramClient.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

        protected Task<TResponse> HandleAsync<TResponse>(string url)
        {
            return HandleAsync<TResponse>(url, new Dictionary<string, string>());
        }

        protected async Task<TResponse> HandleAsync<TResponse>(string url, IDictionary<string, string> query)
        {
            url = QueryHelpers.AddQueryString(url, query);
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(response.StatusCode + " " + await response.Content.ReadAsStringAsync());
            }

            ResponseWrapper<TResponse> result;

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                result = await JsonSerializer.DeserializeAsync<ResponseWrapper<TResponse>>(responseStream, SerializeOptions);
            }

            HandleResponse(result);

            return result.Response;
        }

        protected Task HandleAsync(string url)
        {
            return HandleAsync(url, new Dictionary<string, string>());
        }

        protected async Task HandleAsync(string url, IDictionary<string, string> query)
        {
            url = QueryHelpers.AddQueryString(url, query);
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(response.StatusCode + " " + await response.Content.ReadAsStringAsync());
            }

            ResponseWrapper result;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                result = await JsonSerializer.DeserializeAsync<ResponseWrapper>(responseStream, SerializeOptions);
            }

            HandleResponse(result);
        }

        private void HandleResponse(ResponseWrapper result)
        {
            if (result.IsSuccess)
            {
                return;
            }

            if (result.Errors.All(e => e.Message.Contains("No sessions available") || e.Message.Contains("Session not found")))
            {
                throw new TelegramSessionNotFoundException();
            }

            throw new TelegramClientException(result.Errors);
        }

        //public static string ToPascalCase(this string source)
        //{
        //    var list = source.ToList();
        //    list[0] = char.ToUpper(list[0]);
        //    for (int i = 1; i < list.Count; i++)
        //    {
        //        if (list[i] == '.')
        //        {
        //            list[i + 1] = char.ToUpper(list[i + 1]);
        //            list.RemoveAt(i);
        //            i++;
        //        }
        //    }

        //    return string.Concat(list);
        //}
    }
}