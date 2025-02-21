using DB.Application.Features.Institutions.Commands.CreateInstitution;
using DB.Application.Features.Institutions.Queries.GetAllInstitutions;
using DB.Application.Features.Institutions.Queries.GetListOfInstitutions;
using DB.Application.Features.Institutions.Queries.ViewModels;
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

    }
}
