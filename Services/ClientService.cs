using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientService: IClientService
{
    
    private readonly string _connectionString;

    public ClientService(IConfiguration configuration)
    {
       _connectionString = configuration.GetConnectionString("DefaultConnection"); 
    }
    
    public async Task<List<ClientTripDTO>> GetClientTrips(int id)
    {
        List<ClientTripDTO> trips = new List<ClientTripDTO>();

        // Get all the trips client participates in
        string command = @"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, ct.RegisteredAt, ct.PaymentDate 
            FROM Trip t 
            JOIN Client_Trip ct on t.IdTrip = ct.IdTrip 
            WHERE ct.IdClient = @id
               
        ";
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        
        cmd.Parameters.AddWithValue("@id", id);
        
        await con.OpenAsync();

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                trips.Add(new ClientTripDTO()
                {
                   Id = (int) reader["IdTrip"],
                   Name = (string)reader["Name"],
                   Description = (string)reader["Description"],
                   DateFrom = (DateTime) reader["DateFrom"],
                   DateTo = (DateTime) reader["DateTo"],
                   MaxPeople = (int) reader["MaxPeople"],
                   RegisteredAt = (int) reader["RegisteredAt"],
                   PaymentDate =  reader["PaymentDate"] != DBNull.Value ? (int) reader["PaymentDate"] : null
                });
            }
        }

        return trips;
    }

    public async Task<bool> ClientExists(int id)
    {
        // Check if client with specified id exists
        string command = "SELECT COUNT(1) FROM Client WHERE IdClient=@id";
        
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        cmd.Parameters.AddWithValue("@id", id);

        await con.OpenAsync();
        var count =(int) await cmd.ExecuteScalarAsync();

        return count==1;
    }

    public async Task<int> CreateClient(CreateClientDTO clientDTO)
    {
        //  Insert new client and return its id
        string command = @"
            INSERT INTO CLIENT (FirstName, LastName, Email, Telephone, Pesel)
            OUTPUT INSERTED.IdClient
            VALUES (@firstName, @lastName, @email, @telephone, @pesel)
        ";
        
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        
        cmd.Parameters.AddWithValue("@firstName", clientDTO.FirstName);
        cmd.Parameters.AddWithValue("@lastName", clientDTO.LastName);
        cmd.Parameters.AddWithValue("@email", clientDTO.Email);
        cmd.Parameters.AddWithValue("@telephone", clientDTO.Telephone);
        cmd.Parameters.AddWithValue("pesel", clientDTO.Pesel);

        await con.OpenAsync();
        
        int id = (int) await cmd.ExecuteScalarAsync();
        
        return id;
    }


    public async Task<bool> RegisterForTrip(int clientId, int tripId)
    {
        // insert new entry in Client_Trip
        string command = @"
            INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
            VALUES (@idClient, @idTrip, @registeredAt)
        ";
        
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        
        cmd.Parameters.AddWithValue("@idClient", clientId);
        cmd.Parameters.AddWithValue("@idTrip", tripId);
        int date = int.Parse(DateTime.Now.ToString(("yyyyMMdd")));
        cmd.Parameters.AddWithValue("@registeredAt", date);
        
        await con.OpenAsync();
        try
        {
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException exception)
        {
            return false;
        }

        return true;
    }


    public async Task<bool> RegistrationExists(int clientId, int tripId)
    {
        // check if registration exists
        string command = "SELECT COUNT(1) FROM Client_Trip WHERE IdClient=@idClient and IdTrip=@idTrip";
        
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        
        cmd.Parameters.AddWithValue("@idClient", clientId);
        cmd.Parameters.AddWithValue("@idTrip", tripId);

        await con.OpenAsync();
        return 1 == (int)await cmd.ExecuteScalarAsync();
    }

    public async Task<bool> DeregisterFromTheTrip(int clientId, int tripId)
    {
        // Delete registration entry from Client_Trip
        string command = "DELETE FROM Client_Trip WHERE IdClient=@idClient and IdTrip=@idTrip";
        
        await using var con = new SqlConnection(_connectionString);
        await using var cmd = new SqlCommand(command, con);
        
        cmd.Parameters.AddWithValue("@idClient", clientId);
        cmd.Parameters.AddWithValue("@idTrip", tripId);

        await con.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return true;
    }
}