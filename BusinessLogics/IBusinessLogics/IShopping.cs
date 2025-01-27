using GoldStore.Errors;
using GoldStore.Models;
using static GoldStore.Models.Enums;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        ApiResponse Buy(OrderVM order, string token);
        ApiResponse Sell(OrderVM order, string token);
        bool CheckGoldInventory(int weight, int goldType = 1, int goldMaintenanceType = 10);
        AmountThreshold UpdateAmountThreshold(AmountThreshold tresholdVM);
        AmountThreshold InsertAmountThreshold(AmountThreshold amountThreshold);
        bool isExistAmountThreshold(long amountId);
        double GetBasePrices(EntityTypes entity,double weight = 0.0);
        double GetPrices(PriceCalcVM priceCalc);
        AmountThreshold GetLastThresholdAmount();
        AmountThreshold GetEntityThresholdAmount(EntityTypes entity);
        Repository ChargeRepository(ChargeRepository chargeStore, string token);
        AmountThreshold ManageSupervisorThresholds(AmountThresholdVM thresholdVM);
        AmountThreshold GetAmountThreshold(long thresholdId);
        RepositoryStatusVM GetRepositoryStatistics(string token);
        string ConvertToPersianDate(DateTime date);
        string GetUserNameById(long userId, string token);
        EntityTypesVM GetEntityTypes();
    }
}