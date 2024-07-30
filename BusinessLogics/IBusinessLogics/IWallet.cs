using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IWallet
    {
        Task<bool> ExchangeLocalWalletAsync(OrderVM order);
    }
}
