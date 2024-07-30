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
                ApiResponse response = await _shopping.Buy(order);
                return Ok(response);
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Sell([FromBody] OrderVM order)
        {
            if (order != null && order.Weight != 0 && order.UserId != 0)
            {
                ApiResponse response = await _shopping.Sell(order);
                return Ok(response);
            }
            return BadRequest(new ApiResponse(404));
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> InsertThreshold([FromBody] AmountThreshold threshold)
        //{
        //    if (threshold != null && threshold.SelThreshold != 0 && threshold.BuyThreshold != 0)
        //    {
        //        await _shopping.InsertAmountThreshold(threshold);
        //        return Ok(new ApiResponse());
        //    }
        //    return BadRequest(new ApiResponse(404));
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> UpdateThreshold([FromBody] AmountThreshold threshold)
        //{
        //    if (threshold != null && threshold.SelThreshold != 0 && threshold.BuyThreshold != 0 && threshold.Id != 0)
        //    {
        //        await _shopping.UpdateAmountThreshold(threshold);
        //        return Ok(new ApiResponse());
        //    }
        //    return BadRequest(new ApiResponse(404));
        //}


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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChargeStore([FromBody] ChargeStore chargeStore)
        {
            if (chargeStore != null && chargeStore.Weight > 0)
            {
                GoldRepository? goldRepository = await _shopping.ChargeGoldRepository(chargeStore);
                string jsonData = JsonConvert.SerializeObject(goldRepository);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> InsertThreshold([FromBody] AmountThresholdVM threshold)
        {
            if (threshold != null && threshold.RegUserId != 0)
            {
                await _shopping.InsertSupervisorThresholds(threshold);
                return Ok(new ApiResponse());
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetThreshold([FromBody] AmountThresholdVM threshold)
        {
            if (threshold != null && threshold.Id != 0)
            {
                AmountThreshold? amountThreshold = await _shopping.GetAmountThreshold(threshold.Id);
                string jsonData = JsonConvert.SerializeObject(amountThreshold);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }
    }
}
