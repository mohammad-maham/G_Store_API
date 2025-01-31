using GoldHelpers.Helpers;
using GoldHelpers.Models;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using Newtonsoft.Json;

namespace GoldStore.BusinessLogics
{
    public class Accounting : IAccounting
    {
        private readonly ILogger<Accounting>? _logger;
        private readonly IConfiguration? _config;

        public Accounting()
        {
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public Accounting(ILogger<Accounting> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public UserInfoVM GetUserInfo(long userId, string token)
        {
            UserInfoVM userInfo = new();
            try
            {
                GoldAPIResult? result = new GoldAPIResponse(GoldHosts.Accounting, "/api/User/GetUserInfo", new { Id = userId }, authorization: token).Post();

                if (result != null && !string.IsNullOrEmpty(result.Data))
                {
                    userInfo = JsonConvert.DeserializeObject<UserInfoVM>(result.Data) ?? new UserInfoVM();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return userInfo;
        }

        public string GetUserNameById(long userId, string token)
        {
            string username = string.Empty;

            UserInfoVM userInfo = GetUserInfo(userId, token);

            if (userInfo != null)
            {
                username = $"{userInfo.FirstName} {userInfo.LastName}";
            }

            return username;
        }

        public UserInfoVM ParseUserInfo(string userAdditionalData)
        {
            UserInfoVM userInfo = new();
            if (!string.IsNullOrEmpty(userAdditionalData))
            {
                userInfo = JsonConvert.DeserializeObject<UserInfoVM>(userAdditionalData) ?? new UserInfoVM();
            }
            return userInfo;
        }
    }
}
