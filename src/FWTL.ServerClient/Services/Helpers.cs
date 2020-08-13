using FWTL.TelegramClient.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FWTL.TelegramClient.Services
{
    public class BaseService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializeOptions;

        public BaseService(HttpClient client)
        {
            _client = client;

            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        protected async Task<TResponse> HandleAsync<TResponse>(string url)
        {
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(response.StatusCode + " " + response.Content);
            }

            ResponseWrapper<TResponse> result;

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                result = await JsonSerializer.DeserializeAsync<ResponseWrapper<TResponse>>(responseStream, _serializeOptions);
            }

            if (!result.IsSuccess)
            {
                throw new TelegramClientException(result.Errors);
            }

            return result.Response;
        }

        protected async Task HandleAsync(string url)
        {
            var response = await _client.GetAsync(url);

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(response.StatusCode + " " + await response.Content.ReadAsStringAsync());
            }

            ResponseWrapper result;
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                result = await JsonSerializer.DeserializeAsync<ResponseWrapper>(responseStream, _serializeOptions);
            }

            if (!result.IsSuccess)
            {
                throw new TelegramClientException(result.Errors);
            }
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