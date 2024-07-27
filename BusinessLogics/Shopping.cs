using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace GoldStore.BusinessLogics
{
    public class Shopping : IShopping
    {
        private readonly ILogger<Shopping> _logger;
        private readonly GStoreDbContext _store;

        public Shopping(ILogger<Shopping> logger, GStoreDbContext store)
        {
            _logger = logger;
            _store = store;
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
            await Task.Run(() => { });
            return false;
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
    }
}
