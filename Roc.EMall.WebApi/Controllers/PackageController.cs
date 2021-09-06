using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roc.EMall.Application;

namespace Roc.EMall.WebApi.Controllers
{
    [ApiController]
    [Route("/api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackAppService _packAppService;
        private readonly IDeliverPackageAppService _deliverPackageAppService;

        public PackageController(IPackAppService packAppService,IDeliverPackageAppService deliverPackageAppService)
        {
            _packAppService = packAppService;
            _deliverPackageAppService = deliverPackageAppService;
        }

        [HttpPost]
        public async ValueTask<IActionResult> Create([FromQuery] long orderId)
        {
            var packageId = await _packAppService.CreateAsync(orderId);
            return Ok(new
            {
                Code = "success",
                Data = packageId,
            });
        }

        public record DeliverRequest(string ExpressNo);
        
        [HttpPut("{packageId}/delivering")]
        public async ValueTask<IActionResult> Deliver([FromRoute] long packageId,[FromBody]DeliverRequest model)
        {
            await _deliverPackageAppService.DeliverAsync(packageId, model.ExpressNo);
            return Ok(new
            {
                Code = "success",
            });
        }
    }
}