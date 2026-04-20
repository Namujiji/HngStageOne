using Cortex.Mediator;
using HngStageOne.Application.Features.Genderize.GetGenderByName;
using HngStageOne.Application.Features.Profiles.CreateProfile;
using HngStageOne.Application.Features.Profiles.Responses;
using HngStageOne.Controllers.Profiles.Requests;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HngStageOne.Controllers.Profiles;

[Route("api/[controller]")]
[ApiController]
public class ProfilesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DuplicateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Create(CreateProfile createProfile, CancellationToken cancellationToken)
    {
        var result = await mediator.SendCommandAsync(new CreateProfileByNameCommand(createProfile.Name), cancellationToken);

        try
        {
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(Create), result.Data);
            }

            if (result.Error!.Status == "502")
            {
                return StatusCode(StatusCodes.Status502BadGateway, result.Error);
            }

            return BadRequest(result.Error);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
        }
    }

    //[HttpGet("{id:guid}")]
    //[Produces("application/json")]
    //[ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Create(Guid id, CancellationToken cancellationToken)
    //{
    //    var result = await mediator.SendCommandAsync(new GetProfileByIdCommand(id), cancellationToken);

    //    try
    //    {
    //        if (result.Succeeded)
    //        {
    //            return CreatedAtAction(nameof(Create), result.Data);
    //        }

    //        if (result.Error!.Status == "502")
    //        {
    //            return StatusCode(StatusCodes.Status502BadGateway, result.Data);
    //        }

    //        return BadRequest(result.Data);
    //    }
    //    catch (Exception)
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError, result.Data);
    //    }
    //}
}
