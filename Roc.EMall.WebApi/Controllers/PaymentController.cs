using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roc.EMall.Application;

namespace Roc.EMall.WebApi.Controllers
{
    [ApiController]
    [Route("/api/payments")]
    public class PaymentController:ControllerBase
    {
        private readonly ICompletePaymentAppService _completePaymentAppService;

        public PaymentController(ICompletePaymentAppService completePaymentAppService)
        {
            _completePaymentAppService = completePaymentAppService;
        }

        [HttpPost("{transactionId}")]
        public async ValueTask PayAsync(long transactionId)
        {
            await _completePaymentAppService.CompleteAsync(transactionId);
        }
    }
}