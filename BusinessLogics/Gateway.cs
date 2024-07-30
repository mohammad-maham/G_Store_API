using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Errors;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace GoldStore.BusinessLogics
{
    public class Gateway : IGateway
    {
        private readonly ILogger<Gateway>? _logger;
        private readonly IConfiguration? _config;

        public Gateway()
        {
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public Gateway(ILogger<Gateway> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<double> GetOnlineGoldPriceAsync()
        {
            double onlinePrice = 0.0;
            string host = _config!.GetSection("ProjectUrls")["ApiGateway"]!;

            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/Prices/GetGoldOnlinePrice");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Headers
                request.AddHeader("content-type", "application/json");

                // Send SMS
                RestResponse response = await client.ExecutePostAsync(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content)!;
                    onlinePrice = double.Parse(apiResponse.Data ?? "0");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return onlinePrice;
        }
    }
}
