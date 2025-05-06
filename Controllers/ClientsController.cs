using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        IClientService _clientService;
        private ITripsService _tripsService;

        public ClientsController(IClientService clientService, ITripsService tripsService)
        {
            _clientService = clientService;
            _tripsService = tripsService;
        }
        
        [HttpGet("{id}/trips")]
        public async Task<IActionResult> GetClientTrips(int id)
        {

            if (! await _clientService.ClientExists(id))
            {
                return NotFound();
            }
            
            var trips = await _clientService.GetClientTrips(id);

            if (trips.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(trips);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewClient([FromBody] CreateClientDTO clientDTO)
        {
            if (!clientDTO.IsValid())
            {
                return BadRequest();
            }
            
            int id = await _clientService.CreateClient(clientDTO);
            
            return Created($"/api/clients/{id}", clientDTO);
        }

        [HttpPut("{id}/trips/{tripId}")]
        public async Task<IActionResult> RegisterClientForTheTrip(int id, int tripId)
        {

            if (!(await _clientService.ClientExists(id) && await _tripsService.TripExists(tripId)))
            {
                return NotFound();
            }

            if (await _tripsService.TripPeopleCount(tripId) >= await _tripsService.GetTripMaxPeople(tripId))
            {
                return Conflict("This trip is fully occupied");
            }

            bool result = await _clientService.RegisterForTrip(id, tripId);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Already registered");
            }
            
        }

        [HttpDelete("{id}/trips/{tripId}")]
        public async Task<IActionResult> DeregisterFromTheTrip(int id, int tripId)
        {
            if (!await _clientService.RegistrationExists(id, tripId))
            {
                return NotFound();
            }

            await _clientService.DeregisterFromTheTrip(id, tripId);
            return NoContent();
        }
        
    }
}
