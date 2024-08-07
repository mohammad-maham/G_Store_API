using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IWallet
    {
        bool ExchangeLocalWallet(WalletTransactionVM wallet);
    }
}
