using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GoldStore.BusinessLogics
{
    public class Shopping : IShopping
    {
        private readonly ILogger<Shopping>? _logger;
        private readonly GStoreDbContext _store;
        private readonly IGateway _gateway;

        public Shopping()
        {
            _store = new GStoreDbContext();
            _gateway = new Gateway();
        }

        public Shopping(ILogger<Shopping> logger, GStoreDbContext store, IGateway gateway)
        {
            _logger = logger;
            _store = store;
            _gateway = gateway;
        }

        public async Task<bool> Buy(int weight, long userId)
        {
            bool result = false;
            GoldRepository? repository = new();
            using GStoreDbContext? store = _store;
            TransactionOptions scopeOption = new()
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using TransactionScope scope = new(TransactionScopeOption.RequiresNew, scopeOption, TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (await CheckGoldInventory(weight))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight > weight);
                    if (repository != null && repository.Id != 0)
                    {
                        // STEP 1:
                        // Sayid Method
                        // STEP 2:
                        // Gold Repository Transaction
                        // STEP 3:
                        repository!.Weight -= weight;
                        store.GoldRepositories.Update(repository);
                        await store.SaveChangesAsync();
                        result = true;
                    }
                }
                result = false;
                scope.Complete();
                return result;
            }
            catch (Exception)
            {
                scope.Complete();
                return result;
            }
        }

        public async Task<bool> CheckGoldInventory(int weight, int goldType = 1) => await _store.GoldRepositories.AnyAsync(x => x.Weight < weight && x.GoldType == goldType);

        public async Task<double> GetBasePrices(double weight = 0.0)
        {
            double onlinePrice = await _gateway.GetOnlineGoldPriceAsync();
            return onlinePrice * weight;
        }

        public async Task InsertAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.SelThreshold != 0 && amountThreshold.BuyThreshold != 0)
            {
                bool isExist = await isExistAmountThreshold(amountThreshold.Id);
                if (!isExist)
                {
                    await _store.AmountThresholds.AddAsync(amountThreshold);
                    await _store.SaveChangesAsync();
                }
            }
        }

        public async Task<bool> isExistAmountThreshold(long amountId) => await _store.AmountThresholds.AnyAsync(x => x.Id == amountId || x.Status == 1);

        public async Task<bool> Sell(int weight, long userId)
        {
            bool result = false;
            GoldRepository? repository = new();
            using GStoreDbContext? store = _store;
            TransactionOptions scopeOption = new()
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using TransactionScope scope = new(TransactionScopeOption.RequiresNew, scopeOption, TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (await CheckGoldInventory(weight))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight > weight);
                    if (repository != null && repository.Id != 0)
                    {
                        // STEP 1:
                        // Sayid Method
                        // STEP 2:
                        // Gold Repository Transaction
                        // STEP 3:
                        repository!.Weight += weight;
                        store.GoldRepositories.Update(repository);
                        await store.SaveChangesAsync();
                        result = true;
                    }
                }
                result = false;
                scope.Complete();
                return result;
            }
            catch (Exception)
            {
                scope.Complete();
                return result;
            }
        }

        public async Task UpdateAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.Id != 0)
            {
                bool isExist = await isExistAmountThreshold(amountThreshold.Id);
                if (isExist)
                {
                    _store.AmountThresholds.Update(amountThreshold);
                    await _store.SaveChangesAsync();
                }
            }
        }

        public async Task<double> GetPrices(CalcTypes calcTypes, double weight = 0.0, double carat = 750.0)
        {
            double res = 0.0;
            double basePrice = await GetBasePrices(weight);
            AmountThreshold? threshold = await GetLastThresholdAmount();
            switch (calcTypes)
            {
                case CalcTypes.none:
                    res = basePrice * carat;
                    break;
                case CalcTypes.buy:
                    res = ThresholdsSault(threshold.BuyThreshold, basePrice, calcTypes) * carat;
                    break;
                case CalcTypes.sell:
                    res = ThresholdsSault(threshold.SelThreshold, basePrice, calcTypes) * carat;
                    break;
                default:
                    break;
            }
            return res;
        }

        private double ThresholdsSault(double thresholdValue, double basePrice, CalcTypes calcType)
        {
            double result = 0.0;
            switch (calcType)
            {
                case CalcTypes.none:
                    if (thresholdValue < 1)
                    {
                        // Percentage
                    }
                    else
                    {
                        // Price
                        result = thresholdValue * basePrice;
                    }
                    break;
                case CalcTypes.buy:
                    if (thresholdValue < 1)
                    {
                        // Percentage
                    }
                    else
                    {
                        // Price
                        result = thresholdValue * basePrice;
                    }
                    break;
                case CalcTypes.sell:
                    if (thresholdValue < 1)
                    {
                        // Percentage
                    }
                    else
                    {
                        // Price
                        result = thresholdValue * basePrice;
                    }
                    break;
                default:
                    break;
            }

            return result;
        }

        public async Task<AmountThreshold> GetLastThresholdAmount() => await _store.AmountThresholds.FirstOrDefaultAsync(x => x.Status == 1 && x.BuyThreshold != 0 && x.SelThreshold != 0);

    }
}
