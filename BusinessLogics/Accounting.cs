using GoldHelpers.Models;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

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
            string host = _config!.GetSection("ProjectUrls")["ApiAccounting"]!;
            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/User/GetUserInfo");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Headers
                request.AddHeader("content-type", "application/json");
                request.AddHeader("Authorization", $"Bearer {token}");

                request.AddJsonBody(new { Id = userId });

                // Send Request
                RestResponse response = client.ExecutePost(request);

                if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(response.Content))
                {
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content) ?? new ApiResponse();
                    if (apiResponse != null && !string.IsNullOrEmpty(apiResponse.Data))
                    {
                        userInfo = JsonConvert.DeserializeObject<UserInfoVM>(apiResponse.Data) ?? new UserInfoVM();
                    }
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
                username = $"{userInfo.FirstName} {userInfo.LastName}";

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
