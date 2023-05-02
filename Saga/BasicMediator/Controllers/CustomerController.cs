using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Contracts;

namespace Saga.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController :
        ControllerBase
    {


        public CustomerController()
        {
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromServices] IPublishEndpoint client, Guid id, string customerNumber)
        {
            await client.Publish<CustomerAccountClosed>(new
            (
                 id,
                 customerNumber
            ));

            return Accepted();
        }
    }
}
