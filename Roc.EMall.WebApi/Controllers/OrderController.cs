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
        private readonly IInitiatePaymentAppService _initiatePaymentAppService;
        private readonly ICancelOrderAppService _cancelOrderAppService;

        public OrderController(ISubmitOrderAppService submitOrderAppService,IInitiatePaymentAppService initiatePaymentAppService,ICancelOrderAppService cancelOrderAppService)
        {
            _submitOrderAppService = submitOrderAppService;
            _initiatePaymentAppService = initiatePaymentAppService;
            _cancelOrderAppService = cancelOrderAppService;
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
        public async ValueTask<IActionResult> InitiatePayment([FromRoute] long orderId)
        {
            var transactionId = await _initiatePaymentAppService.InitiateAsync(orderId);
            return Ok(new
            {
                Code = "success",
                Data = transactionId,
            });
        }

        [HttpDelete("{orderId}")]
        public async ValueTask<IActionResult> Cancel([FromRoute] long orderId)
        {
            await _cancelOrderAppService.CancelAsync(orderId);
            return Ok(new
            {
                Code = "success",
            });
        }
        
    }
}