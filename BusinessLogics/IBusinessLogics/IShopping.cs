using GoldStore.Errors;
using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        Task<ApiResponse> Buy(OrderVM order);
        Task<ApiResponse> Sell(OrderVM order);
        Task<bool> CheckGoldInventory(int weight, int goldType = 1);
        Task UpdateAmountThreshold(AmountThreshold tresholdVM);
        Task InsertAmountThreshold(AmountThreshold amountThreshold);
        Task<bool> isExistAmountThreshold(long amountId);
        Task<double> GetBasePrices(double weight = 0.0);
        Task<double> GetPrices(CalcTypes calcTypes, double weight = 0.0, double carat = 750.0);
        Task<AmountThreshold> GetLastThresholdAmount();
        Task<GoldRepository> ChargeGoldRepository(ChargeStore chargeStore);
        Task InsertSupervisorThresholds(AmountThresholdVM thresholdVM);
        Task<AmountThreshold> GetAmountThreshold(long thresholdId);
    }
}