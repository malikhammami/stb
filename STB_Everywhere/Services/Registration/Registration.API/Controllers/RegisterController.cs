using MediatR;
using Microsoft.AspNetCore.Mvc;
using Registration.Application.Features.Commands;
using Registration.Application.Features.Commands.VerifEmail;
using Registration.Application.Features.Commands.VerifPhone;

namespace Registration.API.Controllers
{


    [ApiController]
    [Route("api/v1/[controller]")]
    public class RegisterController : ControllerBase
    {

        private readonly IMediator _mediator;

        public RegisterController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        [HttpPost]
        public async Task<ActionResult> Register([FromBody] ClientRegistrationCommand command)
        {
            try
            {
                
                var result = await _mediator.Send(command);
                return Ok(result);

            } catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred: {ex.Message}");
            
            }
        }


        [HttpPatch("validateEmail/{clientId}")]
        public async Task<ActionResult> ValidateEmail(Guid clientId, [FromBody] VerifyEmailCommand command)
        {

            try
            {

                var result = await _mediator.Send(command);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        
        }

        [HttpPatch("validatePhoneNumber/{clientId}")]
        public async Task<ActionResult> ValidatePhoneNumber(Guid clientId, [FromBody] VerifyPhoneNumberCommand command)
        {

            try
            {

                var result = await _mediator.Send(command);
                return Ok(result);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }


    }
}
