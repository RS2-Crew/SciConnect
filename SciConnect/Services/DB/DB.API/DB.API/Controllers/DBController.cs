using DB.Application.Features.Institutions.Commands.CreateInstitution;
using DB.Application.Features.Institutions.Commands.DeleteInstitution;
using DB.Application.Features.Institutions.Queries.GetAllInstitutions;
using DB.Application.Features.Institutions.Queries.GetListOfInstitutions;
using DB.Application.Features.Institutions.Queries.ViewModels;
using DB.Application.Features.Instruments.Commands.CreateInstrument;
using DB.Application.Features.Instruments.Commands.DeleteInstrument;
using DB.Application.Features.Instruments.Queries.GetAllInstruments;
using DB.Application.Features.Instruments.Queries.GetListOfInstruments;
using DB.Application.Features.Instruments.Queries.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DB.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DBController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DBController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("institutions/{name}")]
        [ProducesResponseType(typeof(IEnumerable<InstitutionViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionViewModel>>> GetInstitutionByName(string name)
        {
            var query = new GetListOfInstitutionsQuery(name);
            var orders = await _mediator.Send(query);

            return Ok(orders);
        }

        [HttpPost("institutions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateInstitution(CreateInstitutionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("institutions")]
        [ProducesResponseType(typeof(IEnumerable<InstitutionViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionViewModel>>> GetAllInstitutions()
        {
            var query = new GetAllInstitutionsQuery();
            var institutions = await _mediator.Send(query);

            return Ok(institutions);
        }

        [HttpDelete("institutions/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInstitution(string name)
        {
            var command = new DeleteInstitutionCommand(name);
            var result = await _mediator.Send(command);

            if (result == Unit.Value)
            {
                return NoContent();  // Status 204 - uspešno obrisano, ali nema sadržaja u odgovoru
            }

            return NotFound();  // Status 404 - ako nije pronađena institucija
        }

        [HttpGet("instruments/{name}")]
        [ProducesResponseType(typeof(IEnumerable<InstrumentViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstrumentViewModel>>> GetInstrumentByName(string name)
        {
            var query = new GetListOfInstrumentsQuery(name);

            var instruments = await _mediator.Send(query);

            return Ok(instruments);
        }

        [HttpGet("instruments")]
        [ProducesResponseType(typeof(IEnumerable<InstrumentViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionViewModel>>> GetAllInstruments()
        {
            var query = new GetAllInstrumentsQuery();
            var institutions = await _mediator.Send(query);

            return Ok(institutions);
        }

        [HttpPost("instruments")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateInstrument(CreateInstrumentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("instruments/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInstrument(string name)
        {
            var command = new DeleteInstrumentCommand(name);
            try
            {
                var result = await _mediator.Send(command);
                if (result == Unit.Value)
                {
                    return NoContent(); // 204 - uspešno obrisano
                }
                return NotFound(); // fallback, mada neće se ovde stići
            }
            catch (ArgumentException)
            {
                return NotFound(); // 404 - instrument nije pronađen
            }
        }

    }
}
