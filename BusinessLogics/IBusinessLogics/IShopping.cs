namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IShopping
    {
        Task<bool> Buy(int weight, long userId);
        Task<bool> Sell(int weight, long userId);
        Task<bool> CheckGoldInventory(int weight, int goldType = 1);
    }
}
