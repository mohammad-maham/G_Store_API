using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Helpers;
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

        public async Task<bool> Buy(OrderVM order)
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
                if (await CheckGoldInventory(order.Weight))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight > order.Weight);
                    if (repository != null && repository.Id != 0)
                    {
                        GoldRepositoryTransaction repositoryTransaction = new();
                        int repoWeight = repository!.Weight;
                        repository!.Weight -= order.Weight;
                        // STEP 1:
                        // Wallet Exchange Transaction

                        // STEP 2:
                        repositoryTransaction.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(store, "seq_goldrepositorytransactions");
                        repositoryTransaction.Weight = order.Weight;
                        repositoryTransaction.RegDate = DateTime.Now;
                        repositoryTransaction.RegUserId = order.UserId;
                        repositoryTransaction.GoldRepositoryId = repository.Id;
                        repositoryTransaction.LastGoldValue = repoWeight;
                        repositoryTransaction.NewGoldValue = repository.Weight;
                        repositoryTransaction.Status = 0;
                        repositoryTransaction.TransactionMode = 2; // Online
                        repositoryTransaction.TransactionType = 2; // Buy
                        await store.GoldRepositoryTransactions.AddAsync(repositoryTransaction);

                        // STEP 3:
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

        public async Task<bool> Sell(OrderVM order)
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
                if (await CheckGoldInventory(order.Weight))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight <= order.Weight);
                    if (repository != null && repository.Id != 0)
                    {
                        GoldRepositoryTransaction repositoryTransaction = new();
                        int repoWeight = repository!.Weight;
                        repository!.Weight += order.Weight;
                        // STEP 1:
                        // Wallet Exchange Transaction

                        // STEP 2:
                        repositoryTransaction.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(store, "seq_goldrepositorytransactions");
                        repositoryTransaction.Weight = order.Weight;
                        repositoryTransaction.RegDate = DateTime.Now;
                        repositoryTransaction.RegUserId = order.UserId;
                        repositoryTransaction.GoldRepositoryId = repository.Id;
                        repositoryTransaction.LastGoldValue = repoWeight;
                        repositoryTransaction.NewGoldValue = repository.Weight;
                        repositoryTransaction.Status = 0;
                        repositoryTransaction.TransactionMode = 2; // Online
                        repositoryTransaction.TransactionType = 1; // Sell
                        await store.GoldRepositoryTransactions.AddAsync(repositoryTransaction);

                        // STEP 3:
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

        public async Task<GoldRepository> ChargeGoldRepository(ChargeStore chargeStore)
        {

            GoldRepository? repo = await _store
                .GoldRepositories
                .FirstOrDefaultAsync(x => x.Carat == chargeStore.Carat && x.Status == 1 && x.GoldType == chargeStore.GoldType);

            if (repo != null)
            {
                repo.Weight = chargeStore.Weight;
                repo.RegDate = DateTime.Now;
                repo.Carat = chargeStore.Carat;
                repo.Status = chargeStore.Status;
                repo.CaratologyInfo = chargeStore.CaratologyInfo;
                repo.EntityType = chargeStore.EntityType;
                repo.RegUserId = chargeStore.RegUserId;
                repo.GoldType = chargeStore.GoldType;
                _store.GoldRepositories.Update(repo);
                await _store.SaveChangesAsync();
            }
            else
            {
                repo!.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_store, "seq_goldrepository");
                repo.Weight = chargeStore.Weight;
                repo.RegDate = DateTime.Now;
                repo.Carat = chargeStore.Carat;
                repo.Status = chargeStore.Status;
                repo.CaratologyInfo = chargeStore.CaratologyInfo;
                repo.EntityType = chargeStore.EntityType;
                repo.RegUserId = chargeStore.RegUserId;
                repo.GoldType = chargeStore.GoldType;
                await _store.GoldRepositories.AddAsync(repo);
                await _store.SaveChangesAsync();
            }
            return repo;
        }

        public async Task InsertSupervisorThresholds(AmountThresholdVM thresholdVM)
        {
            AmountThreshold? threshold = new();
            double onlinePrice = await _gateway.GetOnlineGoldPriceAsync();
            thresholdVM.CurrentPrice = thresholdVM.IsOnlinePrice == 1 ? onlinePrice : thresholdVM.CurrentPrice;

            if (thresholdVM.Id != 0)
            {
                threshold = await _store.AmountThresholds
                    .FirstOrDefaultAsync(x => x.Id == thresholdVM.Id && x.Status == 1 && x.RegUserId != 0);

                if (thresholdVM != null)
                {
                    threshold!.ExpireEffectDate = thresholdVM.ExpireEffectDate;
                    threshold.CurrentPrice = thresholdVM.CurrentPrice;
                    threshold.IsOnlinePrice = thresholdVM.IsOnlinePrice;
                    threshold.Status = thresholdVM.Status;
                    threshold.BuyThreshold = thresholdVM.BuyThreshold;
                    threshold.SelThreshold = thresholdVM.SelThreshold;
                    threshold.RegUserId = thresholdVM.RegUserId;
                    _store.AmountThresholds.Update(threshold);
                    await _store.SaveChangesAsync();
                }
            }
            else
            {
                threshold = await _store.AmountThresholds.FirstOrDefaultAsync();
                if (threshold != null && threshold.Id != 0)
                {
                    threshold.ExpireEffectDate = thresholdVM.ExpireEffectDate;
                    threshold.CurrentPrice = thresholdVM.CurrentPrice;
                    threshold.IsOnlinePrice = thresholdVM.IsOnlinePrice;
                    threshold.Status = thresholdVM.Status;
                    threshold.BuyThreshold = thresholdVM.BuyThreshold;
                    threshold.SelThreshold = thresholdVM.SelThreshold;
                    threshold.RegUserId = thresholdVM.RegUserId;
                    _store.AmountThresholds.Update(threshold);
                    await _store.SaveChangesAsync();
                }
                else
                {
                    threshold!.ExpireEffectDate = thresholdVM.ExpireEffectDate;
                    threshold.CurrentPrice = thresholdVM.CurrentPrice;
                    threshold.IsOnlinePrice = thresholdVM.IsOnlinePrice;
                    threshold.Status = thresholdVM.Status;
                    threshold.BuyThreshold = thresholdVM.BuyThreshold;
                    threshold.SelThreshold = thresholdVM.SelThreshold;
                    threshold.RegUserId = thresholdVM.RegUserId;
                    threshold.RegDate = DateTime.Now;
                    await _store.AmountThresholds.AddAsync(threshold);
                    await _store.SaveChangesAsync();
                }
            }
        }

        public async Task<AmountThreshold> GetAmountThreshold(long thresholdId)
        {
            AmountThreshold? amountThreshold = new();
            amountThreshold = await _store.AmountThresholds.FirstOrDefaultAsync(x => x.Id == thresholdId && x.Status == 1);
            if (amountThreshold == null || amountThreshold.Id == 0)
                amountThreshold = await _store.AmountThresholds.FirstOrDefaultAsync(x => x.RegUserId == 0 && x.Status == 1);
            return amountThreshold;
        }
    }
}
