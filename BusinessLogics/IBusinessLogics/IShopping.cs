using GoldStore.Errors;
using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        ApiResponse Buy(OrderVM order, string token);
        ApiResponse Sell(OrderVM order, string token);
        bool CheckGoldInventory(int weight, int goldType = 1, int goldMaintenanceType = 10);
        void UpdateAmountThreshold(AmountThreshold tresholdVM);
        void InsertAmountThreshold(AmountThreshold amountThreshold);
        bool isExistAmountThreshold(long amountId);
        double GetBasePrices(double weight = 0.0);
        double GetPrices(CalcTypes calcTypes, double weight = 0.0, double carat = 750.0);
        AmountThreshold GetLastThresholdAmount();
        GoldRepository ChargeGoldRepository(ChargeStore chargeStore, string token);
        void InsertSupervisorThresholds(AmountThresholdVM thresholdVM);
        AmountThreshold GetAmountThreshold(long thresholdId);
        GoldRepositoryStatusVM GetGoldRepositoryStatistics(string token);
        string ConvertToPersianDate(DateTime date);
        string GetUserNameById(long userId, string token);
        GoldTypesVM GetGoldTypes();
    }
}