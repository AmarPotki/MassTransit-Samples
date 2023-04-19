using BasicMediator.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BasicMediator.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {


        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IRequestClient<SubmitOrder> client, [FromBody] SubmitOrder submitOrder)
        {
            
            var (accept,reject) = await client.GetResponse<OrderSubmissionAccepted,OrderSubmissionRejected>(submitOrder);
            if(accept.IsCompletedSuccessfully)
            return Ok((await accept).Message);
            return BadRequest((await reject).Message);
        }
    }
}