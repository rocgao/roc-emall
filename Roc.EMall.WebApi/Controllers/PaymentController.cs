using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Roc.EMall.Application;

namespace Roc.EMall.WebApi.Controllers
{
    [ApiController]
    [Route("/api/payments")]
    public class PaymentController:ControllerBase
    {
        private readonly IPayAppService _payAppService;

        public PaymentController(IPayAppService payAppService)
        {
            _payAppService = payAppService;
        }

        [HttpPost("{transactionId}")]
        public async ValueTask PayAsync(long transactionId)
        {
            await _payAppService.PayAsync(transactionId);
        }
    }
}