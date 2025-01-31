using GoldHelpers.Helpers;
using GoldHelpers.Models;
using GoldStore.BusinessLogics.IBusinessLogics;

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

        public double GetOnlineGoldPrice()
        {
            double onlinePrice = 0.0;

            try
            {
                GoldAPIResult? result = new GoldAPIResponse(GoldHosts.Gateway, "/api/Prices/GetGoldOnlinePrice", null!).Post();
                onlinePrice = double.Parse(result?.Data ?? "0");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return onlinePrice;
        }
    }
}
