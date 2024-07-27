using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Errors;
using GoldStore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [Route("[action]")]
        public async Task<IActionResult> Buy([FromBody] OrderVM order)
        {
            if (order != null && order.Weight > 0 && order.UserId != 0)
            {
                bool isSuccess = await _shopping.Buy(order);
                string jsonData = JsonConvert.SerializeObject(isSuccess);
                return Ok(new ApiResponse(data: jsonData, statusCode: isSuccess ? 200 : 400));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Sell([FromBody] OrderVM order)
        {
            if (order != null && order.Weight != 0 && order.UserId != 0)
            {
                bool isSuccess = await _shopping.Sell(order);
                string jsonData = JsonConvert.SerializeObject(isSuccess);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> InsertThreshold([FromBody] AmountThreshold threshold)
        {
            if (threshold != null && threshold.SelThreshold != 0 && threshold.BuyThreshold != 0)
            {
                await _shopping.InsertAmountThreshold(threshold);
                return Ok(new ApiResponse());
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateThreshold([FromBody] AmountThreshold threshold)
        {
            if (threshold != null && threshold.SelThreshold != 0 && threshold.BuyThreshold != 0 && threshold.Id != 0)
            {
                await _shopping.UpdateAmountThreshold(threshold);
                return Ok(new ApiResponse());
            }
            return BadRequest(new ApiResponse(404));
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetPrices([FromBody] PriceCalcVM calcVM)
        {
            double price = 0.0;
            if (calcVM != null)
            {
                price = await _shopping.GetPrices(calcVM.GoldCalcType, calcVM.GoldWeight, calcVM.GoldCarat);
                if (price > 0)
                {
                    string jsonData = JsonConvert.SerializeObject(price);
                    return Ok(new ApiResponse(data: jsonData));
                }
            }
            return BadRequest(new ApiResponse(404));
        }
    }
}
