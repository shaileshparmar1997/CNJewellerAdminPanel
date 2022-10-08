using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;
using System.Text;

namespace CNJewellerAdmin.Helper.DataAccess
{
    public static class DataHelper<TOut> where TOut : class, new()
    {
        //private static string BaseUrl1 = @"https://ikarttest.azurewebsites.net/api/v1/";
        private static string BaseUrl1 = @"https://localhost:7151/api/v1/";
        public async static Task<Response<TOut>> Execute(string route, OperationTypes type, string authToken, object payload = null)
        {
            Response<TOut> response = new Response<TOut>() { Data = default(TOut), Result = new Result() };
            try
            {
                TOut data = default(TOut);
                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri(BaseUrl1)
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                // client.DefaultRequestHeaders.Add("Authorization", authToken);
                client.DefaultRequestHeaders.Add("crossDomain", "true");

                HttpResponseMessage httpResponse = null;
                if (type == OperationTypes.GET)
                {
                    httpResponse = await client.GetAsync(route);
                }
                else if (type == OperationTypes.POST)
                {
                    var jsonobj = JsonConvert.SerializeObject(payload);
                    HttpContent content = new StringContent(jsonobj, Encoding.UTF8, "application/json");
                    httpResponse = await client.PostAsync(route, content);
                }
                else if (type == OperationTypes.DELETE)
                {
                    var jsonobj = JsonConvert.SerializeObject(payload);
                    HttpContent content = new StringContent(jsonobj, Encoding.UTF8, "application/json");
                    httpResponse = await client.DeleteAsync(route);
                }
                else if (type == OperationTypes.PUT)
                {
                    var jsonobj = JsonConvert.SerializeObject(payload);
                    HttpContent content = new StringContent(jsonobj, Encoding.UTF8, "application/json");
                    httpResponse = await client.PutAsync(route, content);
                }
                else
                {
                    response.Result.Flag = false;
                    response.Result.Message = "Request type not supported";
                }

                var result = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode)
                {
                    data = JsonConvert.DeserializeObject<TOut>(result, new IsoDateTimeConverter());
                    response.Result.Flag = true;
                    response.Result.Message = string.Empty;
                }
                else
                {
                    var resultObj = JsonConvert.DeserializeObject<Result>(result, new IsoDateTimeConverter());
                    response.Result.Flag = false;
                    response.Result.Message = resultObj.Message;
                }
                response.Data = data;
            }
            catch (Exception ex)
            {
                response.Result.Flag = false;
                response.Result.Message = ex.Message;
            }
            return response;
        }

        public async static Task<Response<TOut>> AuthenticateUser(string route, object payload)
        {
            Response<TOut> response = new Response<TOut>();
            response.Result = new Result();

            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(BaseUrl1)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("crossDomain", "true");

            try
            {
                var jsonobj = JsonConvert.SerializeObject(payload);
                HttpContent content = new StringContent(jsonobj, Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = await client.PostAsync(route, content);
                var strResponse = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode)
                {
                    TOut authResponse = JsonConvert.DeserializeObject<TOut>(strResponse);

                    response.Result.Flag = true;
                    response.Result.Message = "Success";
                    response.Data = authResponse;
                }
                else
                {
                    response.Result.Flag = false;
                    response.Result.Message = "Failed to authenticate user. Either user name or password is not correct.";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Result.Flag = false;
                response.Result.Message = ex.Message;
                response.Data = null;
            }

            return response;
        }

        public class Response<T> where T : class
        {
            [JsonProperty("result")]
            public Result Result { get; set; }

            [JsonProperty("data")]
            public T Data { get; set; }
        }

        public class Result
        {
            [JsonProperty("flag")]
            public bool Flag { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }

}
