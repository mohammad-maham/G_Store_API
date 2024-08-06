using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using RestSharp;
using System.Net;

namespace GoldStore.BusinessLogics
{
    public class Wallet : IWallet
    {
        private readonly ILogger<Wallet>? _logger;
        private readonly IConfiguration? _config;

        public Wallet()
        {
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public Wallet(ILogger<Wallet> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public bool ExchangeLocalWallet(OrderVM order)
        {
            string host = _config!.GetSection("ProjectUrls")["ApiWallet"]!;

            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/Fund/ExChange");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Headers
                request.AddHeader("content-type", "application/json");

                request.AddJsonBody(new { order });

                // Send SMS
                RestResponse response = client.ExecutePost(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    return response.Content == "true" ? true : false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }
    }
}
