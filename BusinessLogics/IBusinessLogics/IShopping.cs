using GoldStore.Errors;
using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        ApiResponse Buy(OrderVM order);
        ApiResponse Sell(OrderVM order);
        bool CheckGoldInventory(int weight, int goldType = 1);
        void UpdateAmountThreshold(AmountThreshold tresholdVM);
        void InsertAmountThreshold(AmountThreshold amountThreshold);
        bool isExistAmountThreshold(long amountId);
        double GetBasePrices(double weight = 0.0);
        double GetPrices(CalcTypes calcTypes, double weight = 0.0, double carat = 750.0);
        AmountThreshold GetLastThresholdAmount();
        GoldRepository ChargeGoldRepository(ChargeStore chargeStore);
        void InsertSupervisorThresholds(AmountThresholdVM thresholdVM);
        AmountThreshold GetAmountThreshold(long thresholdId);
    }
}