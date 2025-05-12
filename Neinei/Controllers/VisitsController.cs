using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neinei.Models;
using Neinei.Services;

namespace Neinei.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitsController : ControllerBase
    {
        private IVisitService _visitService;
        public VisitsController(IVisitService visitService)
        {
            _visitService = visitService;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetVisit(int id)
        {
            var visit = _visitService.GetVisit(id);
            if (visit == null)
            {
                return NotFound();
            }
            return Ok(visit.Result);
        }
        
        [HttpPost]
        public IActionResult CreateVisit([FromBody] VisitDto visitDto)
        {
            if (visitDto == null)
            {
                return BadRequest();
            }
            
            var createdVisit = _visitService.CreateVisit(visitDto);
            if (createdVisit.Result == -1)
                return NotFound("Nie istnieje wizyta o takim numerze");
            if (createdVisit.Result == -2)
                return NotFound("Nie istnieje mechanik lub klient o takim numerze licencji");
            if (createdVisit.Result == -3)
                return NotFound("Nie istnieje usługa o takim numerze");
            if (createdVisit.Result == -4)
                return NotFound("Nie można dodać wizyty");
            if (createdVisit.Result == 1)
                return Created("Created", "Created");
            
            return BadRequest("Nuh uh");
        }
    }
}
