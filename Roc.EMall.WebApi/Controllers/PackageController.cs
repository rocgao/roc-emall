using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roc.EMall.Application;

namespace Roc.EMall.WebApi.Controllers
{
    [ApiController]
    [Route("/api/packages")]
    public class PackageController:ControllerBase
    {
        private readonly IPackAppService _packAppService;

        public PackageController(IPackAppService packAppService)
        {
            _packAppService = packAppService;
        }

        [HttpPost]
        public ValueTask<long> Create([FromQuery]long orderId)
        {
            return _packAppService.CreateAsync(orderId);
        }
    }
}