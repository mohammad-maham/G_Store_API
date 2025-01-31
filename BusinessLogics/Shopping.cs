using GoldHelpers.Middleware;
using GoldHelpers.Models;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Helpers;
using GoldStore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Transactions;
using static GoldStore.Models.Enums;

namespace GoldStore.BusinessLogics
{
    public class Shopping : IShopping
    {
        private readonly IGateway _gateway;
        private readonly IWallet _wallet;
        private readonly IAccounting _accounting;
        private readonly GStoreDbContext _store;
        private readonly ILogger<Shopping>? _logger;

        public Shopping()
        {
            _store = new GStoreDbContext();
            _gateway = new Gateway();
            _wallet = new Wallet();
            _accounting = new Accounting();
        }

        public Shopping(ILogger<Shopping> logger, GStoreDbContext store, IGateway gateway, IWallet wallet, IAccounting accounting)
        {
            _logger = logger;
            _store = store;
            _gateway = gateway;
            _wallet = wallet;
            _accounting = accounting;
        }

        public GoldAPIResult Buy(OrderVM order, string token)
        {
            long repositoryTransactionId = 0;
            GoldAPIResult response = new();
            Repository? ownerRepository = new();
            Repository? bondedRepository = new();
            using GStoreDbContext? store = _store;
            TransactionOptions scopeOption = new()
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using TransactionScope scope = new(TransactionScopeOption.RequiresNew, scopeOption, TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (CheckGoldInventory(order.Weight, (int)order.EntityId, 10))
                {
                    ownerRepository = store.Repositories.FirstOrDefault(r => r.Entity == (int)order.EntityId && r.Value > order.Weight && r.MaintenanceTypeId == 10);
                    bondedRepository = store.Repositories.FirstOrDefault(r => r.Entity == (int)order.EntityId && r.MaintenanceTypeId == 11);

                    if (ownerRepository != null && ownerRepository.Id != 0)
                    {
                        DateTime now = DateTime.Now;
                        RepositoryTransaction repositoryTransaction = new();

                        decimal repoWeight = ownerRepository!.Value;
                        ownerRepository!.Value -= order.Weight;
                        ownerRepository.RegUserId = order.UserId;
                        bondedRepository!.Value += order.Weight;
                        bondedRepository.RegUserId = order.UserId;
                        ownerRepository.RegDate = now;
                        bondedRepository.RegDate = now;

                        double baseOnlinePrice = GetBasePrices(order.EntityId, order.Weight);

                        PriceCalcVM calcVM = new PriceCalcVM()
                        {
                            CalcType = (int)CalcTypes.buy,
                            Weight = order.Weight,
                            Carat = order.Carat,
                            EntityId = order.EntityId
                        };

                        double orderPrice = GetPrices(calcVM);

                        if (orderPrice == order.CurrentCalculatedPrice && order.SourceWalletCurrency != 0 && order.DestinationWalletCurrency != 0)
                        {
                            // STEP 1:
                            WalletTransactionVM wallet = new()
                            {
                                SourceAmount = orderPrice,
                                DestinationAmout = order.DestinationAmount,
                                SourceWalletCurrency = order.SourceWalletCurrency,
                                DestinationWalletCurrency = order.DestinationWalletCurrency,
                                SourceAddress = order.SourceAddress,
                                DestinationAddress = order.DestinationAddress,
                                WalletId = order.WalleId,
                                RegUserId = order.UserId
                            };

                            // Perform Wallet Exchange
                            bool isExchanged = _wallet.ExchangeLocalWallet(wallet);
                            UserInfoVM userInfoVM = _accounting.GetUserInfo(order.UserId, token);

                            if (isExchanged)
                            {
                                // STEP 2:
                                repositoryTransactionId = DataBaseHelper.GetPostgreSQLSequenceNextVal(store, "seq_goldrepositorytransactions");
                                repositoryTransaction.Id = repositoryTransactionId;
                                repositoryTransaction.Value = order.Weight;
                                repositoryTransaction.RegDate = DateTime.Now;
                                repositoryTransaction.RegUserId = order.UserId;
                                repositoryTransaction.RepositoryId = ownerRepository.Id;
                                repositoryTransaction.LastValue = repoWeight;
                                repositoryTransaction.NewValue = ownerRepository.Value;
                                repositoryTransaction.Status = 0;
                                repositoryTransaction.TransactionMode = 2; // Online
                                repositoryTransaction.TransactionType = 2; // Buy in TransactionType table
                                repositoryTransaction.WalletInfo = JsonConvert.SerializeObject(wallet);
                                repositoryTransaction.UserAdditionalData = JsonConvert.SerializeObject(userInfoVM);
                                store.RepositoryTransactions.Add(repositoryTransaction);

                                ownerRepository.TransactionId = repositoryTransactionId;
                                bondedRepository.TransactionId = repositoryTransactionId;

                                // STEP 3:
                                store.Repositories.Update(ownerRepository);
                                store.Repositories.Update(bondedRepository);
                                store.SaveChanges();
                                response = new GoldAPIResult(data: repositoryTransactionId.ToString());
                            }
                            else
                            {
                                response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "خطای تراکنش کیف پول" };
                            }
                        }
                        else
                        {
                            response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "قیمت انتخاب شده با قیمت بروز مغایرت دارد" };
                        }
                    }
                }
                else
                {
                    response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "موجودی انبار کافی نمی باشد" };
                }
                scope.Complete();
            }
            catch (Exception ex)
            {
                scope.Complete();
                response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = ex.Message };
            }
            return response;
        }

        public bool CheckGoldInventory(int weight, int goldType, int goldMaintenanceType = 10)
        {
            return _store.Repositories
                .Any(x =>
                x.Value >= weight &&
                x.Entity == goldType &&
                x.MaintenanceTypeId == goldMaintenanceType);
        }

        public double GetBasePrices(EntityTypes entity, double weight = 0.0)
        {
            double onlinePrice = _gateway.GetOnlineGoldPrice();
            return onlinePrice * weight;
        }

        public AmountThreshold InsertAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.SelThreshold != 0 && amountThreshold.BuyThreshold != 0)
            {
                bool isExist = isExistAmountThreshold(amountThreshold.Id);
                if (!isExist)
                {
                    _store.AmountThresholds.Add(amountThreshold);
                    _store.SaveChanges();
                    return amountThreshold;
                }
            }
            return new AmountThreshold();
        }

        public bool isExistAmountThreshold(long amountId)
        {
            return _store.AmountThresholds.Any(x => x.Id == amountId || x.Status == 1);
        }

        public GoldAPIResult Sell(OrderVM order, string token)
        {
            long repositoryTransactionId = 0;
            GoldAPIResult response = new();
            Repository? ownerRepository = new();
            Repository? bondedRepository = new();
            using GStoreDbContext? store = _store;
            TransactionOptions scopeOption = new()
            {
                IsolationLevel = IsolationLevel.Serializable,
                Timeout = TransactionManager.DefaultTimeout
            };
            using TransactionScope scope = new(TransactionScopeOption.RequiresNew, scopeOption, TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                if (CheckGoldInventory(order.Weight, (int)order.EntityId, 11))
                {
                    ownerRepository = store.Repositories.FirstOrDefault(x => x.Entity == (int)order.EntityId && x.MaintenanceTypeId == 10);
                    bondedRepository = store.Repositories.FirstOrDefault(x => x.MaintenanceTypeId == 11);

                    if (ownerRepository != null && ownerRepository.Id != 0)
                    {
                        DateTime now = DateTime.Now;
                        RepositoryTransaction repositoryTransaction = new();

                        decimal repoWeight = ownerRepository!.Value;
                        ownerRepository!.Value += order.Weight;
                        ownerRepository.RegUserId = order.UserId;
                        bondedRepository!.Value -= order.Weight;
                        bondedRepository.RegUserId = order.UserId;
                        ownerRepository.RegDate = now;
                        bondedRepository.RegDate = now;

                        double baseOnlinePrice = GetBasePrices(order.EntityId, order.Weight);
                        PriceCalcVM calcVM = new PriceCalcVM()
                        {
                            CalcType = (int)CalcTypes.sell,
                            Weight = order.Weight,
                            Carat = order.Carat,
                            EntityId = order.EntityId
                        };

                        double orderPrice = GetPrices(calcVM);

                        if (orderPrice == order.CurrentCalculatedPrice && order.SourceWalletCurrency != 0 && order.DestinationWalletCurrency != 0)
                        {
                            // STEP 1:
                            WalletTransactionVM wallet = new()
                            {
                                SourceAmount = order.SourceAmount,
                                DestinationAmout = orderPrice,
                                SourceWalletCurrency = order.SourceWalletCurrency,
                                DestinationWalletCurrency = order.DestinationWalletCurrency,
                                SourceAddress = order.SourceAddress,
                                DestinationAddress = order.DestinationAddress,
                                WalletId = order.WalleId,
                                RegUserId = order.UserId
                            };

                            // Perform Wallet Exchange
                            bool isExchanged = _wallet.ExchangeLocalWallet(wallet);
                            UserInfoVM userInfoVM = _accounting.GetUserInfo(order.UserId, token);

                            if (isExchanged)
                            {
                                // STEP 2:
                                repositoryTransactionId = DataBaseHelper.GetPostgreSQLSequenceNextVal(store, "seq_goldrepositorytransactions");
                                repositoryTransaction.Id = repositoryTransactionId;
                                repositoryTransaction.Value = order.Weight;
                                repositoryTransaction.RegDate = DateTime.Now;
                                repositoryTransaction.RegUserId = order.UserId;
                                repositoryTransaction.RepositoryId = ownerRepository.Id;
                                repositoryTransaction.LastValue = repoWeight;
                                repositoryTransaction.NewValue = ownerRepository.Value;
                                repositoryTransaction.Status = 0;
                                repositoryTransaction.TransactionMode = 2; // Online
                                repositoryTransaction.TransactionType = 1; // Sell in TransactionType table
                                repositoryTransaction.WalletInfo = JsonConvert.SerializeObject(wallet);
                                repositoryTransaction.UserAdditionalData = JsonConvert.SerializeObject(userInfoVM);
                                store.RepositoryTransactions.Add(repositoryTransaction);

                                ownerRepository.TransactionId = repositoryTransactionId;
                                bondedRepository.TransactionId = repositoryTransactionId;

                                // STEP 3:
                                store.Repositories.Update(ownerRepository);
                                store.Repositories.Update(bondedRepository);
                                store.SaveChanges();
                                response = new GoldAPIResult(data: repositoryTransactionId.ToString());
                            }
                            else
                            {
                                response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "خطای تراکنش کیف پول" };
                            }
                        }
                        else
                        {
                            response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "قیمت انتخاب شده با قیمت بروز مغایرت دارد" };
                        }
                    }
                }
                else
                {
                    response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = "موجودی انبار کافی نمی باشد" };
                }
                scope.Complete();
            }
            catch (Exception ex)
            {
                scope.Complete();
                response = new GoldAPIResult() { StatusCode = 400, Data = "false", Message = ex.Message };
            }
            return response;
        }

        public AmountThreshold UpdateAmountThreshold(AmountThreshold amountThreshold)
        {
            if (amountThreshold != null && amountThreshold.Id != 0)
            {
                bool isExist = isExistAmountThreshold(amountThreshold.Id);
                if (isExist)
                {
                    _store.AmountThresholds.Update(amountThreshold);
                    _store.SaveChanges();
                    return amountThreshold;
                }
            }
            return new AmountThreshold();
        }

        public double GetPrices(PriceCalcVM priceCalc)
        {
            double res = 0.0;
            double basePrice = 0.0;
            bool isGoldProduct = (priceCalc.EntityId == EntityTypes.PhysicallyGold || priceCalc.EntityId == EntityTypes.VirtualyGold);

            try
            {
                AmountThreshold? threshold = GetEntityThresholdAmount(priceCalc.EntityId);

                if (threshold != null && threshold.IsOnlinePrice == 0)
                {
                    if (threshold.ExpireEffectDate < DateTime.Now)
                    {
                        basePrice = GetBasePrices(priceCalc.EntityId, priceCalc.Weight);
                        threshold.ExpireEffectDate = DateTime.Now.AddMinutes(10);
                        threshold.CurrentPrice = basePrice;
                        threshold.RegUserId = 1;
                        _store.AmountThresholds.Entry(threshold).State = EntityState.Modified;
                        _store.SaveChanges();
                    }
                    else
                    {
                        basePrice = (threshold.CurrentPrice * priceCalc.Weight) ?? 0.0;
                    }
                }
                if (isGoldProduct && priceCalc.Carat.HasValue && priceCalc.Carat.Value > 0)
                {
                    if ((CalcTypes)priceCalc.CalcType != CalcTypes.none && threshold != null)
                    {
                        switch ((CalcTypes)priceCalc.CalcType)
                        {
                            case CalcTypes.none:
                                res = basePrice * priceCalc.Carat.Value;
                                break;
                            case CalcTypes.buy:
                                res = ThresholdsSault(threshold.BuyThreshold, basePrice) * priceCalc.Carat.Value / 750;
                                break;
                            case CalcTypes.sell:
                                res = ThresholdsSault(threshold.SelThreshold, basePrice) * priceCalc.Carat.Value / 750;
                                break;
                            case CalcTypes.threshold:
                                res = threshold.CurrentPrice ?? 0.0;
                                break;

                        }
                    }
                    else
                    {
                        res = (double)(basePrice * priceCalc.Carat.Value / 750);
                    }
                }
                else
                {
                    if ((CalcTypes)priceCalc.CalcType != CalcTypes.none && threshold != null)
                    {
                        switch ((CalcTypes)priceCalc.CalcType)
                        {
                            case CalcTypes.none:
                                res = basePrice;
                                break;
                            case CalcTypes.buy:
                                res = ThresholdsSault(threshold.BuyThreshold, basePrice);
                                break;
                            case CalcTypes.sell:
                                res = ThresholdsSault(threshold.SelThreshold, basePrice);
                                break;
                            case CalcTypes.threshold:
                                res = threshold.CurrentPrice ?? 0.0;
                                break;

                        }
                    }
                    else
                    {
                        res = (double)(basePrice);
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }

            return res;
        }

        private double ThresholdsSault(double thresholdValue, double basePrice)
        {
            double result = 0.0;
            if (thresholdValue < 1)
            {
                // Percentage
                result = basePrice + (basePrice * thresholdValue);
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

        public Repository ChargeRepository(ChargeRepository chargeStore, string token)
        {
            Repository? repo = new();
            RepositoryTransaction repositoryTransaction = new();

            repo = _store.Repositories.FirstOrDefault(x => x.Status == 1 && x.Entity == chargeStore.EntityType && x.MaintenanceTypeId == chargeStore.MaintenanceType) ?? new Repository();

            if (repo != null && repo.Id != 0)
            {
                decimal weight = repo.Value;
                if (chargeStore.Decharge == 0)
                {
                    repo.Value += chargeStore.Weight;
                }
                else
                {
                    repo.Value -= chargeStore.Weight;
                }

                repo.RegDate = DateTime.Now;
                repo.RegUserId = chargeStore.RegUserId;

                UserInfoVM userInfoVM = _accounting.GetUserInfo(chargeStore.RegUserId, token);

                if (userInfoVM != null && userInfoVM.UserId != 0)
                {
                    long repositoryTransactionId = DataBaseHelper.GetPostgreSQLSequenceNextVal(_store, "seq_goldrepositorytransactions");
                    repositoryTransaction.Id = repositoryTransactionId;
                    repositoryTransaction.Value = chargeStore.Weight;
                    repositoryTransaction.RegDate = DateTime.Now;
                    repositoryTransaction.RegUserId = chargeStore.RegUserId;
                    repositoryTransaction.RepositoryId = repo.Id;
                    repositoryTransaction.LastValue = weight;
                    repositoryTransaction.NewValue = repo.Value;
                    repositoryTransaction.Status = 0;
                    repositoryTransaction.TransactionMode = 2; // Online
                    repositoryTransaction.TransactionType = chargeStore.Decharge == 0 ? 3 : 4; // chargeStore.Decharge == 0 ? Increase: Decrease;
                    repositoryTransaction.UserAdditionalData = JsonConvert.SerializeObject(userInfoVM);

                    repo.TransactionId = repositoryTransactionId;

                    _store.RepositoryTransactions.Add(repositoryTransaction);
                    _store.Repositories.Update(repo);
                    _store.SaveChanges();
                }
            }
            else
            {
                UserInfoVM userInfoVM = _accounting.GetUserInfo(chargeStore.RegUserId, token);

                if (userInfoVM != null && userInfoVM.UserId != 0)
                {
                    decimal weight = chargeStore.Weight;
                    repo!.Id = DataBaseHelper.GetPostgreSQLSequenceNextVal(_store, "seq_goldrepository");
                    repo.Value = weight;
                    repo.RegDate = DateTime.Now;
                    repo.Status = chargeStore.Status;
                    repo.Entity = (int)chargeStore.EntityType;
                    repo.RegUserId = chargeStore.RegUserId;
                    repo.MaintenanceTypeId = 10;

                    long repositoryTransactionId = DataBaseHelper.GetPostgreSQLSequenceNextVal(_store, "seq_goldrepositorytransactions");
                    repositoryTransaction.Id = repositoryTransactionId;
                    repositoryTransaction.Value = weight;
                    repositoryTransaction.RegDate = DateTime.Now;
                    repositoryTransaction.RegUserId = repo.RegUserId;
                    repositoryTransaction.RepositoryId = repo.Id;
                    repositoryTransaction.LastValue = weight;
                    repositoryTransaction.NewValue = weight;
                    repositoryTransaction.Status = 0;
                    repositoryTransaction.TransactionMode = 2; // Online
                    repositoryTransaction.TransactionType = chargeStore.Decharge == 0 ? 3 : 4; // chargeStore.Decharge == 0 ? Increase: Decrease;
                    repositoryTransaction.UserAdditionalData = JsonConvert.SerializeObject(userInfoVM);

                    repo!.TransactionId = repositoryTransactionId;

                    _store.RepositoryTransactions.Add(repositoryTransaction);
                    _store.Repositories.Add(repo);
                    _store.SaveChanges();
                }
            }
            return repo;
        }

        public AmountThreshold ManageSupervisorThresholds(AmountThresholdVM thresholdVM)
        {
            AmountThreshold? threshold = new();
            double onlinePrice = 0;

            switch (thresholdVM.EntityId)
            {
                case (int)EntityTypes.PhysicallyGold:
                case (int)EntityTypes.VirtualyGold:
                    onlinePrice = _gateway.GetOnlineGoldPrice();
                    break;
                default:
                    break;
            }

            thresholdVM.CurrentPrice = thresholdVM.IsOnlinePrice == 1 ? onlinePrice : thresholdVM.CurrentPrice;

            threshold = _store.AmountThresholds.FirstOrDefault(x => x.Status == 1 && x.RegUserId != 0);

            if (threshold != null && threshold.Id != 0)
            {
                if (thresholdVM != null && thresholdVM.BuyThreshold != 0 && thresholdVM.SelThreshold != 0)
                {
                    threshold!.ExpireEffectDate = thresholdVM.ExpireEffectDate;
                    threshold.CurrentPrice = thresholdVM.CurrentPrice;
                    threshold.IsOnlinePrice = thresholdVM.IsOnlinePrice;
                    threshold.Status = thresholdVM.Status;
                    threshold.BuyThreshold = thresholdVM.BuyThreshold;
                    threshold.SelThreshold = thresholdVM.SelThreshold;
                    threshold.RegUserId = thresholdVM.RegUserId;
                    threshold.EntityId = thresholdVM.EntityId;
                    _store.AmountThresholds.Update(threshold);
                    _store.SaveChanges();
                }
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
                threshold.EntityId = thresholdVM.EntityId;
                _store.AmountThresholds.Add(threshold);
                _store.SaveChanges();
            }
            return threshold;
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

        public RepositoryStatusVM GetRepositoryStatistics(string token)
        {
            decimal totalWeights = 0.0M;
            RepositoryStatusVM statusVM = new();
            List<RepositoryVM>? lstRepos = _store.Repositories
                .Select(x => new RepositoryVM()
                {
                    Weight = x.Value,
                    EntityTypeId = x.Entity,
                    LastUpdateGregDate = x.RegDate,
                    LastUpdateUserId = x.RegUserId,
                })
                .ToList();

            foreach (RepositoryVM item in lstRepos)
            {
                totalWeights += item.Weight;
                item.LastUpdatePersianDate = ConvertToPersianDate(item.LastUpdateGregDate!.Value);
                item.LastUpdateUser = GetUserNameById(item.LastUpdateUserId, token);
            }

            statusVM.RepositoryVM = lstRepos;
            statusVM.TotalWeight = totalWeights;
            return statusVM;
        }

        public string ConvertToPersianDate(DateTime date)
        {
            string persianDateString = date.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("fa-IR"));
            return persianDateString;
        }

        public string GetUserNameById(long userId, string token)
        {
            string username = string.Empty;
            username = _accounting.GetUserNameById(userId, token);
            return username;
        }

        public EntityTypesVM GetEntityTypes()
        {
            EntityTypesVM goldTypesVM = new();
            List<Entity>? goldTypes = _store.Entities.Where(x => x.Status == 1).ToList();
            goldTypesVM.EntityTypes = goldTypes;
            return goldTypesVM;
        }

        public AmountThreshold GetEntityThresholdAmount(EntityTypes entity)
        {
            AmountThreshold? threshold = _store.AmountThresholds
                .Where(x => x.Status == 1 && x.BuyThreshold > 0 && x.SelThreshold > 0 && x.EntityId == (long)entity)
                .FirstOrDefault();

            return threshold;
        }
    }
}
