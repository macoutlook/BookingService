using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoMapper;
using Core.Application.Contract;
using Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Adapters;
using Service.Dto;
using Service.Validators;

namespace Service.Controllers;

[ApiController]
[Route("[controller]/api/availability")]
[Authorize(AuthenticationSchemes = "Basic")]
public class Controller(
    ILogger<Controller> logger,
    ISlotService slotService,
    IMapper mapper,
    ScheduleAdapter adapter,
    ScheduleStartDateValidator validator)
    : ControllerBase
{
    [HttpPost]
    [Route("/TakeSlot")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<string>> TakeSlot([FromBody] [Required] AppointmentDto appointmentDto,
        CancellationToken cancellationToken = default)
    {
        var appointmentId = await slotService.TakeSlotAsync(mapper.Map<Appointment>(appointmentDto), cancellationToken);

        logger.LogInformation("Appointment with id={appointmentId} was created.", appointmentId);

        return Created();
    }

    [HttpGet]
    [Route("GetWeeklyAvailability")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<ScheduletDto>> GetAvailability([Required] string date,
        CancellationToken cancellationToken = default)
    {
        var validationResult = validator.ValidateScheduleDate(date);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var schedule = await slotService.GetAvailabilityAsync(date, cancellationToken);
        if (schedule == null) return NotFound();

        var scheduleDto = adapter.Adapt(schedule);

        return Ok(scheduleDto);
    }
}