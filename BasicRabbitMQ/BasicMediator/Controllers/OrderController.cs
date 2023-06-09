using BasicRabbitMQ.Components.Consumers;
using BasicRabbitMQ.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace BasicRabbitMQ.Api.Controllers
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

            var (accept, reject) = await client.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(submitOrder);
            if (accept.IsCompletedSuccessfully)
                return Ok((await accept).Message);
            return BadRequest((await reject).Message);
        }
        [HttpPut]
        public async Task<IActionResult> Put([FromServices] ISendEndpointProvider client, [FromBody] SubmitOrder submitOrder)
        {
            var endpoint =await client.GetSendEndpoint(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
            var endpoint2 =await client.GetSendEndpoint(new Uri("exchange:submit-order"));
            await endpoint.Send<SubmitOrder>(submitOrder);
            return Accepted();

        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromServices] IPublishEndpoint client, [FromBody] SubmitOrder submitOrder)
        {
            await client.Publish<SubmitOrder>(submitOrder);
            return Accepted();

        }
    }
}