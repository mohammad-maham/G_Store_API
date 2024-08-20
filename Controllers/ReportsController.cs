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
    [Route("api/[controller]")]
    [GoldAuthorize]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly IReports _reports;

        public ReportsController(IReports reports, ILogger<ReportsController> logger)
        {
            _reports = reports;
            _logger = logger;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult GetGoldRepositoryReport(GoldRepositoryReportFilterVM filterVM)
        {
            StringValues headerValues = HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            if (filterVM != null && headerValue != null && headerValue.Parameter != null)
            {
                string token = headerValue.Parameter;
                List<GoldRepositoryReportFilterDataVM> report = _reports.GoldRepositoryReport(filterVM, token);
                string jsonData = JsonConvert.SerializeObject(report);
                return Ok(new ApiResponse(data: jsonData));
            }
            return BadRequest(new ApiResponse(404));
        }
    }
}
