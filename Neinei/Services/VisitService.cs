using Microsoft.Data.SqlClient;
using Neinei.Models;

namespace Neinei.Services;

public class VisitService : IVisitService
{
    private readonly IConfiguration _configuration;
    public VisitService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<VisitReturnDto> GetVisit(int id)
    {
        string command = "SELECT date, client_id, mechanic_id FROM Visit where visit_id = @VisitId";
        
        VisitReturnDto visitReturnDto = new VisitReturnDto();
        int clientId = 0;
        int mechanicId = 0;
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@VisitId", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    visitReturnDto.Date = reader.GetDateTime(0);
                    clientId = reader.GetInt32(1);
                    mechanicId = reader.GetInt32(2);
                }
            }
        }
        
        command = "Select first_name, last_name, date_of_birth from Client where client_id = @ClientId";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@ClientId", clientId);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    visitReturnDto.Client = new ClientDto()
                    {
                        firstName = reader.GetString(0),
                        lastName = reader.GetString(1),
                        dateOfBirth = reader.GetDateTime(2),
                    };
                }
            }
        }
        
        command = "Select licence_number from Mechanic where mechanic_id = @MechanicId";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@MechanicId", mechanicId);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    visitReturnDto.Mechanic = new MechanicDto()
                    {
                        mechanicId = mechanicId,
                        licenceNumber = reader.GetString(0)
                    };
                }
            }
        }
        
        command = "Select s.name, ass.service_fee from Visit_Service ass join Service s on s.service_id = ass.service_id where ass.visit_id = @VisitId";
        
        List<ServiceDto> visitServices = new List<ServiceDto>();
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@VisitId", id);

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    visitServices.Add(new ServiceDto
                    {
                        ServiceName = reader.GetString(0),
                        serviceFee = reader.GetDecimal(1),
                    });
                }
            }
        }
        
        visitReturnDto.Services = visitServices;

        return visitReturnDto;
    }

    public async Task<int> CreateVisit(VisitDto visitDto)
    {
        if (!DoesVisitExist(visitDto.VisitId).Result)
            return -1;
        
        string command = "SELECT 1 FROM Client WHERE client_id = @ClientId";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@ClientId", visitDto.ClientId);

            conn.Open();
            var visit = cmd.ExecuteScalar();
            if (visit is null)
                return -2;
        }
        
        command = "SELECT 1 FROM Mechanic WHERE licence_number = @LicenceNumber";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@LicenceNumber", visitDto.MechanicLicenceNumber);

            conn.Open();
            var visit = cmd.ExecuteScalar();
            if (visit is null)
                return -2;
        }
        
        command = "Select name from Service";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            conn.Open();
            var count = 0;
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                    if (visitDto.Services.Select(x => x.ServiceName).Contains(reader.GetString(0)))
                        count++;
                
            }
            if (count != visitDto.Services.Count)
                return -3;
        }
        
        command = "Insert into Visit (visit_id, date, client_id, mechanic_id) values (@VisitId, @Date, @ClientId, (SELECT mechanic_id FROM Mechanic WHERE licence_number = @LicenceNumber))";
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@VisitId", visitDto.VisitId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            cmd.Parameters.AddWithValue("@ClientId", visitDto.ClientId);
            cmd.Parameters.AddWithValue("@LicenceNumber", visitDto.MechanicLicenceNumber);

            conn.Open();
            var id = cmd.ExecuteScalar();
            if (id is not null)
                return -4;
        }

        return 1;
    }

    public async Task<bool> DoesVisitExist(int id)
    {
        string command = "SELECT 1 FROM Visit WHERE visit_id = @VisitId";
        
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("Default")))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("@VisitId", id);

            conn.Open();
            var visit = cmd.ExecuteScalar();
            if (visit is not null)
                return false;
            return true;
        }

    }
}