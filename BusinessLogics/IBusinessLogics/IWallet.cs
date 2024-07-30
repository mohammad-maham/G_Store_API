using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IWallet
    {
        Task ExchangeLocalWalletAsync(OrderVM order);
    }
}
