using GoldHelpers.Models;
using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        GoldAPIResult Buy(OrderVM order, string token);
        GoldAPIResult Sell(OrderVM order, string token);
        bool CheckGoldInventory(int weight, int goldType = 1, int goldMaintenanceType = 10);
        AmountThreshold UpdateAmountThreshold(AmountThreshold tresholdVM);
        AmountThreshold InsertAmountThreshold(AmountThreshold amountThreshold);
        bool isExistAmountThreshold(long amountId);
        double GetBasePrices(long amountId, double weight = 0.0);
        double GetAmount(PriceCalcVM priceCalc);
        AmountThreshold GetLastThresholdAmount();
        AmountThreshold GetEntityThresholdAmount(long entityId);
        Amount? GetAmountByEntityId(long entityId);
        Repository ChargeRepository(ChargeRepository chargeStore, string token);
        AmountThreshold ManageSupervisorThresholds(AmountThresholdVM thresholdVM);
        AmountThreshold GetAmountThreshold(long thresholdId);
        RepositoryStatusVM GetRepositoryStatistics(string token);
        string ConvertToPersianDate(DateTime date);
        string GetUserNameById(long userId, string token);
        EntityTypesVM GetEntityTypes();
    }
}