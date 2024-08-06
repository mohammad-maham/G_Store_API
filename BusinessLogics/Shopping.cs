using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Errors;
using GoldStore.Helpers;
using GoldStore.Models;
using System.Transactions;

namespace GoldStore.BusinessLogics
{
    public class Shopping : IShopping
    {
        private readonly IGateway _gateway;
        private readonly IWallet _wallet;
        private readonly GStoreDbContext _store;
        private readonly ILogger<Shopping>? _logger;

        public Shopping()
        {
            _store = new GStoreDbContext();
            _gateway = new Gateway();
            _wallet = new Wallet();
        }

        public Shopping(ILogger<Shopping> logger, GStoreDbContext store, IGateway gateway, IWallet wallet)
        {
            _logger = logger;
            _store = store;
            _gateway = gateway;
            _wallet = wallet;
        }

        public ApiResponse Buy(OrderVM order)
        {
            ApiResponse response = new();
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
                if (CheckGoldInventory(order.Weight, order.GoldType))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight > order.Weight);
                    if (repository != null && repository.Id != 0)
                    {
                        GoldRepositoryTransaction repositoryTransaction = new();
                        int repoWeight = repository!.Weight;
                        repository!.Weight -= order.Weight;
                        // STEP 1:
                        double baseOnlinePrice = GetBasePrices(order.Weight);
                        double orderPrice = GetPrices(CalcTypes.buy, order.Weight, order.Carat);
                        if (orderPrice == order.CurrentCalculatedPrice)
                        {
                            // STEP 1:
                            order.SourceAmount = orderPrice;
                            bool isExchanged = _wallet.ExchangeLocalWallet(order);

                            if (isExchanged)
                            {
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
                                store.GoldRepositoryTransactions.Add(repositoryTransaction);

                                // STEP 3:
                                store.GoldRepositories.Update(repository);
                                store.SaveChanges();
                                response = new ApiResponse();
                            }
                            else
                            {
                                response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "خطای تراکنش کیف پول" };
                            }
                        }
                        else
                        {
                            response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "قیمت انتخاب شده با قیمت بروز مغایرت دارد" };
                        }
                    }
                }
                else
                {
                    response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "موجودی انبار کافی نمی باشد" };
                }
                scope.Complete();
            }
            catch (Exception ex)
            {
                scope.Complete();
                response = new ApiResponse() { StatusCode = 400, Data = "false", Message = ex.Message };
            }
            return response;
        }

        public bool CheckGoldInventory(int weight, int goldType = 1)
        {
            return _store.GoldRepositories.Any(x => x.Weight > weight && x.GoldType == goldType);
        }

        public double GetBasePrices(double weight = 0.0)
        {
            double onlinePrice = _gateway.GetOnlineGoldPrice();
            return onlinePrice * weight;
        }

        public void InsertAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.SelThreshold != 0 && amountThreshold.BuyThreshold != 0)
            {
                bool isExist = isExistAmountThreshold(amountThreshold.Id);
                if (!isExist)
                {
                    _store.AmountThresholds.Add(amountThreshold);
                    _store.SaveChanges();
                }
            }
        }

        public bool isExistAmountThreshold(long amountId)
        {
            return _store.AmountThresholds.Any(x => x.Id == amountId || x.Status == 1);
        }

        public ApiResponse Sell(OrderVM order)
        {
            ApiResponse response = new();
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
                if (CheckGoldInventory(order.Weight))
                {
                    repository = store.GoldRepositories.FirstOrDefault(r => r.Weight <= order.Weight);
                    if (repository != null && repository.Id != 0)
                    {
                        GoldRepositoryTransaction repositoryTransaction = new();
                        int repoWeight = repository!.Weight;
                        repository!.Weight += order.Weight;
                        double baseOnlinePrice = GetBasePrices(order.Weight);
                        double orderPrice = GetPrices(CalcTypes.sell, order.Weight, order.Carat);
                        if (orderPrice == order.CurrentCalculatedPrice)
                        {
                            // STEP 1:
                            order.DestinationAmount = orderPrice;
                            bool isExchanged = _wallet.ExchangeLocalWallet(order);

                            if (isExchanged)
                            {
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
                                store.GoldRepositoryTransactions.Add(repositoryTransaction);

                                // STEP 3:
                                store.GoldRepositories.Update(repository);
                                store.SaveChanges();
                                response = new ApiResponse();
                            }
                            else
                            {
                                response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "خطای تراکنش کیف پول" };
                            }
                        }
                        else
                        {
                            response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "قیمت انتخاب شده با قیمت بروز مغایرت دارد" };
                        }
                    }
                }
                else
                {
                    response = new ApiResponse() { StatusCode = 400, Data = "false", Message = "موجودی انبار کافی نمی باشد" };
                }
                scope.Complete();
            }
            catch (Exception ex)
            {
                scope.Complete();
                response = new ApiResponse() { StatusCode = 400, Data = "false", Message = ex.Message };
            }
            return response;
        }

        public void UpdateAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.Id != 0)
            {
                bool isExist = isExistAmountThreshold(amountThreshold.Id);
                if (isExist)
                {
                    _store.AmountThresholds.Update(amountThreshold);
                    _store.SaveChanges();
                }
            }
        }

        public double GetPrices(CalcTypes calcTypes, double weight = 0.0, double carat = 750)
        {
            double res = 0.0;
            double basePrice = GetBasePrices(weight);
            AmountThreshold? threshold = GetLastThresholdAmount();
            res = calcTypes != CalcTypes.none && threshold != null
                ? calcTypes switch
                {
                    CalcTypes.buy => ThresholdsSault(threshold.BuyThreshold, basePrice) * carat / 750,
                    CalcTypes.sell => ThresholdsSault(threshold.SelThreshold, basePrice) * carat / 750,
                    _ => basePrice * carat,
                }
                : basePrice * carat / 750;
            return res;
        }

        private double ThresholdsSault(double thresholdValue, double basePrice)
        {
            double result = 0.0;
            if (thresholdValue < 1)
            {
                // Percentage
                result = basePrice + (basePrice * (thresholdValue / 100));
            }
            else
            {
                // Price
                result = thresholdValue + basePrice;
            }
            return result;
        }

        public AmountThreshold GetLastThresholdAmount()
        {
            return _store.AmountThresholds.FirstOrDefault(x => x.Status == 1 && x.BuyThreshold != 0 && x.SelThreshold != 0);
        }

        public GoldRepository ChargeGoldRepository(ChargeStore chargeStore)
        {

            GoldRepository? repo = _store
                .GoldRepositories
                .FirstOrDefault(x => x.Carat == chargeStore.Carat && x.Status == 1 && x.GoldType == chargeStore.GoldType);

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
                _store.SaveChanges();
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
                _store.GoldRepositories.Add(repo);
                _store.SaveChanges();
            }
            return repo;
        }

        public void InsertSupervisorThresholds(AmountThresholdVM thresholdVM)
        {
            AmountThreshold? threshold = new();
            double onlinePrice = _gateway.GetOnlineGoldPrice();
            thresholdVM.CurrentPrice = thresholdVM.IsOnlinePrice == 1 ? onlinePrice : thresholdVM.CurrentPrice;

            if (thresholdVM.Id != 0)
            {
                threshold = _store.AmountThresholds
                    .FirstOrDefault(x => x.Id == thresholdVM.Id && x.Status == 1 && x.RegUserId != 0);

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
                    _store.SaveChanges();
                }
            }
            else
            {
                threshold = _store.AmountThresholds.FirstOrDefault();
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
                    _store.SaveChanges();
                }
                else
                {
                    threshold = new();
                    threshold!.ExpireEffectDate = thresholdVM.ExpireEffectDate;
                    threshold.CurrentPrice = thresholdVM.CurrentPrice;
                    threshold.IsOnlinePrice = thresholdVM.IsOnlinePrice;
                    threshold.Status = thresholdVM.Status;
                    threshold.BuyThreshold = thresholdVM.BuyThreshold;
                    threshold.SelThreshold = thresholdVM.SelThreshold;
                    threshold.RegUserId = thresholdVM.RegUserId;
                    threshold.RegDate = DateTime.Now;
                    _store.AmountThresholds.Add(threshold);
                    _store.SaveChanges();
                }
            }
        }

        public AmountThreshold GetAmountThreshold(long thresholdId)
        {
            AmountThreshold? amountThreshold = new();
            amountThreshold = _store.AmountThresholds.FirstOrDefault(x => x.Id == thresholdId && x.Status == 1);
            if (amountThreshold == null || amountThreshold.Id == 0)
            {
                amountThreshold = _store.AmountThresholds.FirstOrDefault(x => x.RegUserId == 0 && x.Status == 1);
            }

            return amountThreshold;
        }
    }
}
