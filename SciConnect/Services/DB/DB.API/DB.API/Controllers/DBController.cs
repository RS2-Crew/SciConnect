using DB.Application.Features.Analyses.Commands.CreateAnalysis;
using DB.Application.Features.Analyses.Commands.DeleteAnalysis;
using DB.Application.Features.Analyses.Queries.GetAllAnalyses;
using DB.Application.Features.Analysis.Queries.GetListOfAnalysis;
using DB.Application.Features.Analysis.Queries.ViewModels;
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
using DB.Application.Features.Keywords.Commands.CreateKeyword;
using DB.Application.Features.Keywords.Commands.DeleteKeyword;
using DB.Application.Features.Keywords.Queries.GetAllKeywords;
using DB.Application.Features.Keywords.Queries.GetListOfKeywords;
using DB.Application.Features.Keywords.Queries.ViewModels;
using DB.Application.Features.Microorganisms.Commands.CreateMicroorganism;
using DB.Application.Features.Microorganisms.Commands.DeleteMicroorganism;
using DB.Application.Features.Microorganisms.Queries;
using DB.Application.Features.Microorganisms.Queries.GetAllMicroorganisms;
using DB.Application.Features.Microorganisms.Queries.ViewModels;
using DB.Application.Features.Employees.Commands.CreateEmployee;
using DB.Application.Features.Employees.Commands.DeleteEmployee;
using DB.Application.Features.Employees.Queries.GetAllEmployees;
using DB.Application.Features.Employees.Queries.GetListOfEmployees;
using DB.Application.Features.Employees.Queries.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithInstruments;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithEmployees;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithAnalyses;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithKeywords;
using DB.Application.Features.Institutions.Queries.GetInstitutionWithMicroorganisms;
using DB.Application.Features.Analysis.Queries.GetAnalysisWithMicroorganism;
using DB.Application.Features.Analysis.Queries.GetAnalysisWithInstitution;

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

        // ---------- INSTITUTIONS ----------
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
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("institution/with-instruments/{institutionName}")]
        public async Task<IActionResult> GetInstitutionWithInstruments(string institutionName)
        {
            var query = new GetInstrumentsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-employees/{institutionName}")]
        public async Task<IActionResult> GetInstitutionWithEmployees(string institutionName)
        {
            var query = new GetEmployeesByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-analyses/{institutionName}")]
        public async Task<IActionResult> GetInstitutionWithAnalyses(string institutionName)
        {
            var query = new GetAnalysesByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-keywords/{institutionName}")]
        public async Task<IActionResult> GetInstitutionWithKeywords(string institutionName)
        {
            var query = new GetKeywordsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-microorganisms/{institutionName}")]
        public async Task<IActionResult> GetInstitutionWithMicroorganisms(string institutionName)
        {
            var query = new GetMicroorganismsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


        // ---------- INSTRUMENTS ----------
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
        public async Task<ActionResult<IEnumerable<InstrumentViewModel>>> GetAllInstruments()
        {
            var query = new GetAllInstrumentsQuery();
            var instruments = await _mediator.Send(query);
            return Ok(instruments);
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
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // ---------- MICROORGANISMS ----------
        [HttpGet("microorganisms/{name}")]
        [ProducesResponseType(typeof(IEnumerable<MicroorganismViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MicroorganismViewModel>>> GetMicroorganismByName(string name)
        {
            var query = new GetListOfMicroorganismsQuery(name);
            var microorganisms = await _mediator.Send(query);
            return Ok(microorganisms);
        }

        [HttpGet("microorganisms")]
        [ProducesResponseType(typeof(IEnumerable<MicroorganismViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MicroorganismViewModel>>> GetAllMicroorganisms()
        {
            var query = new GetAllMicroorganismsQuery();
            var microorganisms = await _mediator.Send(query);
            return Ok(microorganisms);
        }

        [HttpPost("microorganisms")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateMicroorganism(CreateMicroorganismCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("microorganisms/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMicroorgnaism(string name)
        {
            var command = new DeleteMicroorganismCommand(name);
            try
            {
                var result = await _mediator.Send(command);
                if (result == Unit.Value)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // ---------- KEYWORDS ----------
        [HttpGet("keywords/{name}")]
        [ProducesResponseType(typeof(IEnumerable<KeywordViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KeywordViewModel>>> GetKeywordByName(string name)
        {
            var query = new GetListOfKeywordsQuery(name);
            var keywords = await _mediator.Send(query);
            return Ok(keywords);
        }

        [HttpGet("keywords")]
        [ProducesResponseType(typeof(IEnumerable<KeywordViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KeywordViewModel>>> GetAllKeywords()
        {
            var query = new GetAllKeywordsQuery();
            var keywords = await _mediator.Send(query);
            return Ok(keywords);
        }

        [HttpPost("keywords")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateKeyword(CreateKeywordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("keywords/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteKeyword(string name)
        {
            var command = new DeleteKeywordCommand(name);
            try
            {
                var result = await _mediator.Send(command);
                if (result == Unit.Value)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        // ---------- EMPLOYEES ----------
        [HttpGet("employees/{firstName}/{lastName}")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeViewModel>>> GetEmployeeByName(string firstName, string lastName)
        {
            var query = new GetListOfEmployeesQuery(firstName, lastName);
            var employees = await _mediator.Send(query);
            return Ok(employees);
        }

        [HttpGet("employees")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeViewModel>>> GetAllEmployees()
        {
            var query = new GetAllEmployeesQuery();
            var employees = await _mediator.Send(query);
            return Ok(employees);
        }

        [HttpPost("employees")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateEmployee(CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("employees/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var command = new DeleteEmployeeCommand(id);
            try
            {
                var result = await _mediator.Send(command);
                if (result == Unit.Value)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }
    

    
        [HttpGet("analyses")]
        [ProducesResponseType(typeof(IEnumerable<AnalysisViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnalysisViewModel>>> GetAllAnalyses()
        {
            var query = new GetAllAnalysesQuery();
            var analyses = await _mediator.Send(query);
            return Ok(analyses);
        }
        [HttpGet("analyses/{name}")]
        [ProducesResponseType(typeof(IEnumerable<AnalysisViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnalysisViewModel>>> GetAnalysisByName(string name)
        {
            var query = new GetListOfAnalysisQuery(name);
            var analyses = await _mediator.Send(query);
            return Ok(analyses);
        }


        [HttpPost("analyses")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateAnalysis(CreateAnalysisCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpDelete("analyses/{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAnalysis(string name)
        {
            var command = new DeleteAnalysisCommand(name);
            try
            {
                var result = await _mediator.Send(command);
                if (result == Unit.Value)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpGet("analysis/with-microorganisms/{analysisName}")]
        [ProducesResponseType(typeof(AnalysisViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnalysisWithMicroorganisms(string analysisName)
        {
            var query = new GetMicroorganismsByAnalysisQuery(analysisName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("analysis/with-institutions/{analysisName}")]
        [ProducesResponseType(typeof(AnalysisViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAnalysisWithInstitutions(string analysisName)
        {
            var query = new GetInstitutionsByAnalysisQuery(analysisName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }


    }

}
