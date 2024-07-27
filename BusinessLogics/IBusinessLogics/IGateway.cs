namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IGateway
    {
        Task<double> GetOnlineGoldPriceAsync();
    }
}
