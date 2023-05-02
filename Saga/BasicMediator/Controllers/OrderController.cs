using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Components.Consumers;
using Saga.Contracts;

namespace Saga.Api.Controllers
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

        [HttpGet]
        public async Task<IActionResult> Get(Guid id, [FromServices] IRequestClient<CheckOrder> client)
        {
            var (status, notFound) = await client.GetResponse<OrderStatus, OrderNotFound>(new CheckOrder(id));
            if (status.IsCompletedSuccessfully)
                return Ok((await status).Message);
            return NotFound(id);
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
            var endpoint = await client.GetSendEndpoint(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
            var endpoint2 = await client.GetSendEndpoint(new Uri("exchange:submit-order"));
            await endpoint.Send<SubmitOrder>(submitOrder);
            return Accepted();

        }

        [HttpPatch]
        public async Task<IActionResult> Patch([FromServices] IPublishEndpoint client, [FromBody] SubmitOrder submitOrder)
        {
            await client.Publish<SubmitOrder>(submitOrder);
            return Accepted();

        }
        [HttpOptions]
        public async Task<IActionResult> Options([FromServices] IPublishEndpoint client, [FromBody] ExceptionEvent exceptionEvent)
        {
            await client.Publish<ExceptionEvent>(exceptionEvent);
            return Accepted();

        }
        [HttpPost]
        [Route("OrderAccepted")]
        public async Task<IActionResult> OrderAccepted([FromServices] IPublishEndpoint client, [FromBody] OrderAccepted orderAccepted)
        {
            await client.Publish<OrderAccepted>(orderAccepted);
            return Accepted();

        }
    }
}