using GoldHelpers.Helpers;
using GoldHelpers.Models;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using GoldStore.Models.Gateway;
using Newtonsoft.Json;

namespace GoldStore.BusinessLogics
{
    public class Gateway : IGateway
    {
        private readonly GStoreDbContext _store;
        private readonly ILogger<Gateway>? _logger;
        private readonly IConfiguration? _config;

        public Gateway()
        {
            _store = new GStoreDbContext();
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public Gateway(ILogger<Gateway> logger, IConfiguration config, GStoreDbContext store)
        {
            _store = store;
            _logger = logger;
            _config = config;
        }

        public double GetOnlineAmounts(long amountId)
        {
            double onlinePrice = 0.0;

            try
            {
                GoldAPIResult? result = new GoldAPIResponse(GoldHosts.Gateway, "/api/Prices/GetOnlineAmountWithDetail", new { amountId }).Post();
                if (result != null && !string.IsNullOrEmpty(result.Data))
                {
                    GoldOnlineAmountApi? onlineAmount = JsonConvert.DeserializeObject<GoldOnlineAmountApi>(result.Data);

                    if (onlineAmount != null)
                    {
                        switch (amountId)
                        {
                            case 11:
                                // Gold
                                Geram18? goldAmount = onlineAmount?.geram18;
                                onlinePrice = goldAmount?.value ?? 0;
                                if (goldAmount != null)
                                {
                                    _store.GoldPrices.Add(new GoldPrice()
                                    {
                                        Value = goldAmount.value ?? 0,
                                        RegDate = DateTime.Now,
                                        Timestamp = long.Parse(goldAmount.timestamp),
                                        OrginalValue = goldAmount.orginalValue ?? 0,
                                    });
                                    _store.SaveChanges();
                                }
                                break;
                            case 12:
                                // Silver
                                Geram18? silverAmount = onlineAmount?.geram18;
                                onlinePrice = /*silverAmount?.value ??*/ 0;
                                /*if (silverAmount != null)
                                {
                                    _store.SilverPrices.Add(new SilverPrice()
                                    {
                                        Value = silverAmount.value ?? 0,
                                        RegDate = DateTime.Now,
                                        Timestamp = long.Parse(silverAmount.timestamp),
                                        OrginalValue = silverAmount.orginalValue ?? 0,
                                    });
                                    _store.SaveChanges();
                                }*/
                                break;
                            case 13:
                                // USD
                                Dolar? usdAmount = onlineAmount?.dolar;
                                onlinePrice = usdAmount?.value ?? 0;
                                if (usdAmount != null)
                                {
                                    _store.DollarPrices.Add(new DollarPrice()
                                    {
                                        Value = usdAmount.value ?? 0,
                                        RegDate = DateTime.Now,
                                        Timestamp = long.Parse(usdAmount.timestamp),
                                        OrginalValue = decimal.Parse(usdAmount.orginalValue ?? "0"),
                                    });
                                    _store.SaveChanges();
                                }
                                break;
                            case 14:
                                // USDT
                                Geram18? usdtAmount = onlineAmount?.geram18;
                                onlinePrice = /*usdtAmount?.value ??*/ 0;
                                /*if (usdtAmount != null)
                                {
                                    _store.TeterPrices.Add(new TeterPrice()
                                    {
                                        Value = usdtAmount.value ?? 0,
                                        RegDate = DateTime.Now,
                                        Timestamp = long.Parse(usdtAmount.timestamp),
                                        OrginalValue = usdtAmount.orginalValue ?? 0,
                                    });
                                    _store.SaveChanges();
                                }*/
                                break;
                            default:
                                break;
                        }
                    }
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
