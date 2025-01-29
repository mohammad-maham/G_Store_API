using GoldHelpers.Helpers;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Errors;
using GoldStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GoldStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ILogger<ShoppingController> _logger;
        private readonly IShopping _shopping;

        public ShoppingController(ILogger<ShoppingController> logger, IShopping shopping)
        {
            _logger = logger;
            _shopping = shopping;
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult GetRepositoryStatistics()
        {
            StringValues headerValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);
            if (headerValue != null && headerValue.Parameter != null)
            {
                string token = headerValue.Parameter;
                RepositoryStatusVM? statusVM = _shopping.GetRepositoryStatistics(token);
                if (statusVM != null)
                {
                    string jsonData = JsonConvert.SerializeObject(statusVM);
                    return Ok(new ApiResponse(data: jsonData));
                }
            }
            return BadRequest(new ApiResponse(401));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult GetEntityTypes()
        {
            EntityTypesVM? goldTypes = _shopping.GetEntityTypes();
            if (goldTypes != null)
            {
                string jsonData = JsonConvert.SerializeObject(goldTypes);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult Buy([FromBody] OrderVM order)
        {
            StringValues headerValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            if (order != null && order.Weight > 0 && order.UserId != 0 && headerValue != null && headerValue.Parameter != null)
            {
                string token = headerValue.Parameter;
                ApiResponse response = _shopping.Buy(order, token);
                return Ok(response);
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult Sell([FromBody] OrderVM order)
        {
            StringValues headerValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            if (order != null && order.Weight != 0 && order.UserId != 0 && headerValue != null && headerValue.Parameter != null)
            {
                string token = headerValue.Parameter;
                ApiResponse response = _shopping.Sell(order, token);
                return Ok(response);
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult GetPrices([FromBody] PriceCalcVM calcVM)
        {
            double price = 0.0;

            if (calcVM != null && calcVM.Weight > 0 && calcVM.EntityId > 0)
            {
                calcVM.Carat = ((calcVM.EntityId == Enums.EntityTypes.PhysicallyGold || calcVM.EntityId == Enums.EntityTypes.VirtualyGold) && calcVM.Carat == 0) ? 750 : calcVM.Carat;

                price = _shopping.GetPrices(calcVM);

                if (price > 0)
                {
                    return Ok(new ApiResponse(data: price.ToString("N0")));
                }
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult ChargeRepository([FromBody] ChargeRepository chargeStore)
        {
            StringValues headerValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            if (chargeStore != null && chargeStore.Weight > 0 && headerValue != null && headerValue.Parameter != null)
            {
                string token = headerValue.Parameter;
                Repository? goldRepository = _shopping.ChargeRepository(chargeStore, token);
                string jsonData = JsonConvert.SerializeObject(goldRepository);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult ManageThresholds([FromBody] AmountThresholdVM threshold)
        {
            if (threshold != null && threshold.RegUserId != 0)
            {
                AmountThreshold amountThreshold = _shopping.ManageSupervisorThresholds(threshold);
                string jsonData = JsonConvert.SerializeObject(amountThreshold);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [GoldAuthorize]
        [Route("[action]")]
        public IActionResult GetThreshold([FromBody] AmountThresholdVM threshold)
        {
            if (threshold != null && threshold.Id != 0)
            {
                AmountThreshold? amountThreshold = _shopping.GetAmountThreshold(threshold.Id);
                string jsonData = JsonConvert.SerializeObject(amountThreshold);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }
    }
}
