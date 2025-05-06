using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    Task<List<TripDTO>> GetTrips();
    
    Task<bool> TripExists(int id);

    Task<int> TripPeopleCount(int id);
    Task<int> GetTripMaxPeople(int id);
}