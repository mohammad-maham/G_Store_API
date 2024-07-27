using GoldStore.BusinessLogics.IBusinessLogics;
using RestSharp;
using System.Net;

namespace GoldStore.BusinessLogics
{
    public class Gateway : IGateway
    {
        public async Task<double> GetOnlineGoldPriceAsync()
        {
            double onlinePrice = 0.0;
            string host = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("GatewayApi")
                .Get<string>()!;

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
                request.AddHeader("cache-control", "no-cache");

                // Send SMS
                RestResponse response = await client.ExecutePostAsync(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                    onlinePrice = double.Parse(response.Content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return onlinePrice;
        }
    }
}
