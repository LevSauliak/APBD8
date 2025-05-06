using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString;

    public TripsService(IConfiguration configuration)
    {
       _connectionString = configuration.GetConnectionString("DefaultConnection"); 
    }

    public async Task<List<TripDTO>> GetTrips()
    {
        var tripsDict = new Dictionary<int, TripDTO>();

        // query to get info about each trip along with countries
        string command = @"
            SELECT 
                t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                c.IdCountry, c.Name AS CountryName
            FROM Trip t
            LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            LEFT JOIN Country c ON ct.IdCountry = c.IdCountry
            ORDER BY t.IdTrip;";
        

        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));

                    if (!tripsDict.TryGetValue(tripId, out var tripDto))
                    {
                        tripDto = new TripDTO
                        {
                            Id = tripId,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            DateFrom = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            DateTo = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountryDTO>()
                        };
                        tripsDict[tripId] = tripDto;
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("IdCountry")))
                    {
                        var country = new CountryDTO
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("IdCountry")),
                            Name = reader.GetString(reader.GetOrdinal("CountryName"))
                        };
                        tripDto.Countries.Add(country);
                    }
                }
            }
        }

        return tripsDict.Values.ToList();
    }

    public async Task<int> GetTripMaxPeople(int id)
    {
        // Get how many people is allowed for a trip
        string command = "SELECT MaxPeople FROM Trip WHERE IdTrip = @id";
        
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand(command, con);
        
        com.Parameters.AddWithValue("@Id", id);
        
        await con.OpenAsync();
        
        int maxPeople = (int)await com.ExecuteScalarAsync();
        return maxPeople;
    }

    public async Task<bool> TripExists(int id)
    {
        // Check if trip exists
        string command = @"SELECT COUNT(1) FROM Trip t WHERE t.IdTrip = @Id";
        
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand(command, con);
        
        com.Parameters.AddWithValue("@Id", id);
        
        await con.OpenAsync();
        int re = (int)await com.ExecuteScalarAsync();

        return re == 1;
    }

    public async Task<int> TripPeopleCount(int id)
    {
        // How many people registered for the trip
        string command = @"SELECT COUNT(*) FROM Client_Trip WHERE IdTrip=@Id";
        
        await using var con = new SqlConnection(_connectionString);
        await using var com = new SqlCommand(command, con);
        
        com.Parameters.AddWithValue("@Id", id);
        
        await con.OpenAsync();
        int count = (int)await com.ExecuteScalarAsync();

        return count;
    }
}