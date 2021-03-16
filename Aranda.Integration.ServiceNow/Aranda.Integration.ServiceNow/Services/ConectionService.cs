using Aranda.Integration.ServiceNow.Interface;
using Aranda.Integration.ServiceNow.Utils;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Aranda.Integration.ServiceNow.Services
{
    internal class ConectionService : IConectionService
    {
        private IRestResponse response;
        private IRestClient restClient;
        private IRestRequest restRequest;


        public async Task<T> PostAsync<T>(string Token, string restUrl, object data) where T : new()
        {
            Initialization(restUrl, Token, Constants.Authorization);

            restRequest.AddJsonBody(data);

            response = await restClient.ExecutePostAsync(restRequest);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                throw new CustomException(string.IsNullOrWhiteSpace(response.Content) ? Constants.ErrorServer : response.Content);
            }
        }

        public async Task<T> GetAsync<T>(string Token, string restUrl, string nameHeader, object data) where T : new()
        {
            nameHeader = string.IsNullOrWhiteSpace(nameHeader) ? Constants.Authorization : nameHeader;

            var request = WebRequest.Create(restUrl);

            request.ContentType = Constants.FormatTypeJson;
            request.Method = Method.GET.ToString();
            request.Headers[nameHeader] = Token;

            var type = request.GetType();
            var currentMethod = type.GetProperty(Constants.CurrentMethod, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(request);

            var methodType = currentMethod.GetType();
            methodType.GetField(Constants.ContentBodyNotAllowed, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(currentMethod, false);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(JsonConvert.SerializeObject(data));
            }

            HttpWebResponse responseWebRequest = await request.GetResponseAsync() as HttpWebResponse;

            if (responseWebRequest.StatusCode == HttpStatusCode.OK || responseWebRequest.StatusCode == HttpStatusCode.Created)
            {
                using (Stream dataStream = responseWebRequest.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }
            }
            else
            {
                using (Stream dataStream = responseWebRequest.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    throw new CustomException(string.IsNullOrWhiteSpace(reader.ReadToEnd()) ? Constants.ErrorServer : reader.ReadToEnd());
                }
            }
        }

        public async Task<T> GetAsync<T>(string Token, string restUrl) where T : new()
        {
            Initialization(restUrl, Token, Constants.Authorization);

            response = await restClient.ExecuteGetAsync<T>(restRequest, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                throw new CustomException(string.IsNullOrWhiteSpace(response.Content) ? Constants.ErrorServer : response.Content);
            }
        }

        public async Task PostAsync(string Token, string restUrl, object data)
        {
            Initialization(restUrl, Token, Constants.Authorization);

            restRequest.AddJsonBody(data);

            response = await restClient.ExecutePostAsync(restRequest);

            if (response.StatusCode >= HttpStatusCode.Ambiguous)
            {
                throw new CustomException(string.IsNullOrWhiteSpace(response.Content) ? Constants.ErrorServer : response.Content);
            }
        }
        private void Initialization(string restUrl, string Token, string nameHeader)
        {
            restClient = new RestClient();
            restRequest = new RestRequest();

            restClient.ThrowOnAnyError = true;
            restClient.FailOnDeserializationError = true;
            restClient.ThrowOnDeserializationError = true;

            restRequest.Resource = restUrl;
            restRequest.AddHeader(Constants.ContentType, Constants.FormatTypeJson);

            if (!string.IsNullOrWhiteSpace(Token))
            {
                restRequest.AddHeader(nameHeader, Token);
            }
        }

        public async Task<T> PutAsync<T>(string Token, string restUrl, object data) where T : new()
        {
            Initialization(restUrl, Token, Constants.Authorization);

            restRequest.AddJsonBody(data);

            response = await restClient.ExecuteAsync(restRequest, Method.PUT);

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                return JsonConvert.DeserializeObject<T>(response.Content);
            }
            else
            {
                throw new CustomException(string.IsNullOrWhiteSpace(response.Content) ? Constants.ErrorServer : response.Content);
            }
        }
    }
}
