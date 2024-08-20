using GoldStore.Models;

namespace GoldStore.BusinessLogics.IBusinessLogics
{
    public interface IAccounting
    {
        UserInfoVM GetUserInfo(long userId, string token);
        string GetUserNameById(long userId, string token);
        UserInfoVM ParseUserInfo(string userAdditionalData);
    }
}
