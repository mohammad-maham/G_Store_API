using GoldHelpers.Helpers;
using GoldStore.BusinessLogics.IBusinessLogics;
using GoldStore.Errors;
using GoldStore.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GoldStore.Controllers
{
    [GoldAuthorize]
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
        public IActionResult Buy([FromBody] OrderVM order)
        {
            if (order != null && order.Weight > 0 && order.UserId != 0)
            {
                ApiResponse response = _shopping.Buy(order);
                return Ok(response);
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Sell([FromBody] OrderVM order)
        {
            if (order != null && order.Weight != 0 && order.UserId != 0)
            {
                ApiResponse response = _shopping.Sell(order);
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
        public IActionResult GetPrices([FromBody] PriceCalcVM calcVM)
        {
            double price = 0.0;
            if (calcVM != null && calcVM.GoldWeight > 0)
            {
                calcVM.GoldCarat = calcVM.GoldCarat == 0 ? 750 : calcVM.GoldCarat;
                price = _shopping.GetPrices((CalcTypes)calcVM.GoldCalcType, calcVM.GoldWeight, calcVM.GoldCarat);
                if (price > 0)
                {
                    return Ok(new ApiResponse(data: price.ToString("N0")));
                }
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult ChargeStore([FromBody] ChargeStore chargeStore)
        {
            if (chargeStore != null && chargeStore.Weight > 0)
            {
                GoldRepository? goldRepository = _shopping.ChargeGoldRepository(chargeStore);
                string jsonData = JsonConvert.SerializeObject(goldRepository);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult InsertThreshold([FromBody] AmountThresholdVM threshold)
        {
            if (threshold != null && threshold.RegUserId != 0)
            {
                _shopping.InsertSupervisorThresholds(threshold);
                return Ok(new ApiResponse());
            }
            return BadRequest(new ApiResponse(404));
        }

        [HttpPost]
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
