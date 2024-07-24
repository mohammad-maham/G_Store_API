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
                repository = store.GoldRepositories.FirstOrDefault(r => r.Id == 1000000000);
                if (repository != null && repository.Weight > 0)
                {
                    repository!.Weight -= weight;
                    store.GoldRepositories.Update(repository);
                    await store.SaveChangesAsync();
                    result = true;
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

        public async Task<bool> CheckGoldInventory(int weight, int goldType = 1)
        {
            return await _store.GoldRepositories.AnyAsync(x => x.Weight < weight && x.GoldType == goldType);
        }

        public async Task<bool> Sell(int weight, long userId)
        {
            await Task.Run(() => { });
            return false;
        }
    }
}
