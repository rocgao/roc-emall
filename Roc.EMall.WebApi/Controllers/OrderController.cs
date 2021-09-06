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
        private readonly IPayOrderAppService _payOrderAppService;

        public OrderController(ISubmitOrderAppService submitOrderAppService,IPayOrderAppService payOrderAppService)
        {
            _submitOrderAppService = submitOrderAppService;
            _payOrderAppService = payOrderAppService;
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

        [HttpGet("{orderId}/payment")]
        public async ValueTask<IActionResult> Pay([FromRoute] long orderId)
        {
            var transactionId = await _payOrderAppService.PayAsync(orderId);
            return Ok(new
            {
                Code = "success",
                Data = transactionId,
            });
        }

        // [HttpPut("{orderId}/status")]
        // public ValueTask<IActionResult> ChangeStatus([FromRoute] long orderId, [FromQuery] string targetStatus)
        // {
        //     throw New
        // }
        
    }
}