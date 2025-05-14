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
using DB.Application.Features.Analysis.Queries.GetMicroorganismWithAnalyses;
using DB.Application.Features.Analysis.Queries.GetMicroorganismWithInstitutions;
using DB.Application.Features.Employees.Queries.GetEmployeeWithInstitution;
using DB.Application.Features.Employees.Queries.GetEmployeeWithKeywords;
using DB.Application.Features.Instruments.Queries.GetInstitutionsByInstrument;
using DB.Application.Features.Institutions.Commands.AddInstrument;
using DB.Application.Features.Keywords.Queries.GetEmployeesByKeyword;
using Microsoft.AspNetCore.Authorization;


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
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<InstitutionViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionViewModel>>> GetInstitutionByName(string name)
        {
            var query = new GetListOfInstitutionsQuery(name);
            var orders = await _mediator.Send(query);
            return Ok(orders);
        }

        [HttpPost("institutions")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateInstitution(CreateInstitutionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("institutions")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<InstitutionViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstitutionViewModel>>> GetAllInstitutions()
        {
            var query = new GetAllInstitutionsQuery();
            var institutions = await _mediator.Send(query);
            return Ok(institutions);
        }

        [HttpDelete("institutions/{name}")]
        [Authorize(Policy = "WriteAccess")]
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
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionWithInstruments(string institutionName)
        {
            var query = new GetInstrumentsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
       

        [HttpGet("institution/with-employees/{institutionName}")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionWithEmployees(string institutionName)
        {
            var query = new GetEmployeesByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-analyses/{institutionName}")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionWithAnalyses(string institutionName)
        {
            var query = new GetAnalysesByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-keywords/{institutionName}")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionWithKeywords(string institutionName)
        {
            var query = new GetKeywordsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("institution/with-microorganisms/{institutionName}")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionWithMicroorganisms(string institutionName)
        {
            var query = new GetMicroorganismsByInstitutionQuery(institutionName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("institutions/{institutionId}/instruments/{instrumentId}")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddInstrumentToInstitution(int institutionId, int instrumentId)
        {
            var command = new AddInstrumentToInstitutionCommand(institutionId, instrumentId);
            await _mediator.Send(command);

            return Ok();
        }


        // ---------- INSTRUMENTS ----------
        [HttpGet("instruments/{name}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<InstrumentViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstrumentViewModel>>> GetInstrumentByName(string name)
        {
            var query = new GetListOfInstrumentsQuery(name);
            var instruments = await _mediator.Send(query);
            return Ok(instruments);
        }

        [HttpGet("instruments")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<InstrumentViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstrumentViewModel>>> GetAllInstruments()
        {
            var query = new GetAllInstrumentsQuery();
            var instruments = await _mediator.Send(query);
            return Ok(instruments);
        }

        [HttpPost("instruments")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateInstrument(CreateInstrumentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("instruments/{name}")]
        [Authorize(Policy = "WriteAccess")]
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

        [HttpGet("instrument/with-institutions/{name}")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetInstitutionsByInstrumentName(string name)
        {
            var query = new GetInstitutionsByInstrumentQuery(name);
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        // ---------- MICROORGANISMS ----------
        [HttpGet("microorganisms/{name}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<MicroorganismViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MicroorganismViewModel>>> GetMicroorganismByName(string name)
        {
            var query = new GetListOfMicroorganismsQuery(name);
            var microorganisms = await _mediator.Send(query);
            return Ok(microorganisms);
        }

        [HttpGet("microorganisms")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<MicroorganismViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MicroorganismViewModel>>> GetAllMicroorganisms()
        {
            var query = new GetAllMicroorganismsQuery();
            var microorganisms = await _mediator.Send(query);
            return Ok(microorganisms);
        }

        [HttpPost("microorganisms")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateMicroorganism(CreateMicroorganismCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("microorganisms/{name}")]
        [Authorize(Policy = "WriteAccess")]
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
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<KeywordViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KeywordViewModel>>> GetKeywordByName(string name)
        {
            var query = new GetListOfKeywordsQuery(name);
            var keywords = await _mediator.Send(query);
            return Ok(keywords);
        }

        [HttpGet("keywords")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<KeywordViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<KeywordViewModel>>> GetAllKeywords()
        {
            var query = new GetAllKeywordsQuery();
            var keywords = await _mediator.Send(query);
            return Ok(keywords);
        }

        [HttpPost("keywords")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateKeyword(CreateKeywordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("keywords/{name}")]
        [Authorize(Policy = "WriteAccess")]
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

        [HttpGet("keywords/{keywordName}/employees")]
        [Authorize(Policy = "ReadAccess")]
        public async Task<IActionResult> GetEmployeesByKeyword(string keywordName)
        {
            var query = new GetEmployeesByKeywordQuery(keywordName);
            var result = await _mediator.Send(query);

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        // ---------- EMPLOYEES ----------
        [HttpGet("employees/{firstName}/{lastName}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeViewModel>>> GetEmployeeByName(string firstName, string lastName)
        {
            var query = new GetListOfEmployeesQuery(firstName, lastName);
            var employees = await _mediator.Send(query);
            return Ok(employees);
        }

        [HttpGet("employees")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<EmployeeViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeViewModel>>> GetAllEmployees()
        {
            var query = new GetAllEmployeesQuery();
            var employees = await _mediator.Send(query);
            return Ok(employees);
        }

        [HttpPost("employees")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateEmployee(CreateEmployeeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("employees/{id}")]
        [Authorize(Policy = "WriteAccess")]
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
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<AnalysisViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnalysisViewModel>>> GetAllAnalyses()
        {
            var query = new GetAllAnalysesQuery();
            var analyses = await _mediator.Send(query);
            return Ok(analyses);
        }
        [HttpGet("analyses/{name}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(IEnumerable<AnalysisViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AnalysisViewModel>>> GetAnalysisByName(string name)
        {
            var query = new GetListOfAnalysisQuery(name);
            var analyses = await _mediator.Send(query);
            return Ok(analyses);
        }


        [HttpPost("analyses")]
        [Authorize(Policy = "WriteAccess")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CreateAnalysis(CreateAnalysisCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpDelete("analyses/{name}")]
        [Authorize(Policy = "WriteAccess")]
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
        [Authorize(Policy = "ReadAccess")]
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
        [Authorize(Policy = "ReadAccess")]
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

        [HttpGet("microorganism/with-analysis/{microorganismName}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(MicroorganismViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMicroorganismWithAnalysis(string microorganismName)
        {
            var query = new GetAnalysisByMicroorganismQuery(microorganismName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("microorganism/with-institution/{microorganismName}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(MicroorganismViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMicroorganismWithInstituions(string microorganismName)
        {
            var query = new GetInstitutionByMicroorganismQuery(microorganismName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("employee/with-institution/{employeeFirstName}/{employeeLastName}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeWithInstituions(string employeeFirstName, string employeeLastName)
        {
            var query = new GetInstitutionByEmployeeQuery(employeeFirstName, employeeLastName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("employee/with-keywords/{employeeFirstName}/{employeeLastName}")]
        [Authorize(Policy = "ReadAccess")]
        [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeWithKeywords(string employeeFirstName, string employeeLastName)
        {
            var query = new GetKeywordsByEmployeeQuery(employeeFirstName, employeeLastName);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound();

            return Ok(result);
        }






    }

}
