using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roc.EMall.Application;

namespace Roc.EMall.WebApi.Controllers
{
    [ApiController]
    [Route("/api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly ISubmitOrderAppService _submitOrderAppService;

        public OrderController(ISubmitOrderAppService submitOrderAppService)
        {
            _submitOrderAppService = submitOrderAppService;
        }
        
        /// <summary>
        /// 新增订单
        /// </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async ValueTask<IActionResult> Post([FromBody]ISubmitOrderAppService.Dto dto)
        {
            var orderId=await _submitOrderAppService.SubmitAsync(dto);
            return Ok(new
            {
                Code = "success",
                Data = orderId,
            });
        }
    }
}