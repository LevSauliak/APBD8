using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    Task<List<ClientTripDTO>> GetClientTrips(int id);
    
    Task<bool> ClientExists(int id);
    
    Task<int> CreateClient(CreateClientDTO clientDTO);

    Task<bool> RegisterForTrip(int clientId, int tripId);

    Task<bool> RegistrationExists(int clientId, int tripId);

    Task<bool> DeregisterFromTheTrip(int clientId, int tripId);
}